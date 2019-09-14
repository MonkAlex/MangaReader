// hardcodet.net NotifyIcon for WPF
// Copyright (c) 2009 - 2013 Philipp Sumi
// Contact and Information: http://www.hardcodet.net
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the Code Project Open License (CPOL);
// either version 1.0 of the License, or (at your option) any later
// version.
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//
// THIS COPYRIGHT NOTICE MAY NOT BE REMOVED FROM THIS FILE


using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.VisualTree;
using MangaReader.Avalonia.Platform.Win.Interop;
using Brushes = Avalonia.Media.Brushes;

namespace MangaReader.Avalonia.Platform.Win
{
  /// <summary>
  /// A WPF proxy to for a taskbar icon (NotifyIcon) that sits in the system's
  /// taskbar notification area ("system tray").
  /// </summary>
  public class TaskBarIcon : IDisposable
  {
    private readonly object lockObject = new object();

    #region Members

    /// <summary>
    /// Represents the current icon data.
    /// </summary>
    private NotifyIconData iconData;

    /// <summary>
    /// Receives messages from the taskbar icon.
    /// </summary>
    private readonly WindowMessageSink messageSink;

    /// <summary>
    /// An action that is being invoked if the
    /// <see cref="singleClickTimer"/> fires.
    /// </summary>
    private Action singleClickTimerAction;

    /// <summary>
    /// A timer that is used to differentiate between single
    /// and double clicks.
    /// </summary>
    private readonly Timer singleClickTimer;

    /// <summary>
    /// The time we should wait for a double click.
    /// </summary>
    private int DoubleClickWaitTime => WinApi.GetDoubleClickTime();

    /// <summary>
    /// Indicates whether the taskbar icon has been created or not.
    /// </summary>
    public bool IsTaskbarIconCreated { get; private set; }

    /// <summary>
    /// Indicates whether custom tooltips are supported, which depends
    /// on the OS. Windows Vista or higher is required in order to
    /// support this feature.
    /// </summary>
    public bool SupportsCustomToolTips => messageSink.Version == NotifyIconVersion.Vista;

    public Icon Icon { get; }

    public event EventHandler<MouseEvent> MouseEventHandler;

    #endregion

    #region Construction

    /// <summary>
    /// Initializes the taskbar icon and registers a message listener
    /// in order to receive events from the taskbar area.
    /// </summary>
    public TaskBarIcon(Icon icon)
    {
      Icon = icon;

      // using dummy sink in design mode
      messageSink = new WindowMessageSink(NotifyIconVersion.Vista);

      // init icon data structure
      iconData = NotifyIconData.CreateDefault(messageSink.MessageWindowHandle);
      iconData.IconHandle = Icon?.Handle ?? IntPtr.Zero;
      iconData.ToolTipText = nameof(MangaReader);

      // create the taskbar icon
      CreateTaskbarIcon();

      // register event listeners
      messageSink.MouseEventReceived += OnMouseEvent;
      messageSink.TaskbarCreated += OnTaskbarCreated;

      // init single click / balloon timers
      singleClickTimer = new Timer(DoSingleClickAction);

      // register listener in order to get notified when the application closes
      if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
      {
        lifetime.Exit += OnExit;
      }
    }

    #endregion
    
    #region Process Incoming Mouse Events

    /// <summary>
    /// Processes mouse events, which are bubbled
    /// through the class' routed events, trigger
    /// certain actions (e.g. show a popup), or
    /// both.
    /// </summary>
    /// <param name="me">Event flag.</param>
    private void OnMouseEvent(MouseEvent me)
    {
      if (IsDisposed)
        return;

      MouseEventHandler?.Invoke(this, me);
      switch (me)
      {
        case MouseEvent.MouseMove:
          // immediately return - there's nothing left to evaluate
          return;
        case MouseEvent.IconRightMouseDown:
          break;
        case MouseEvent.IconLeftMouseDown:
          break;
        case MouseEvent.IconRightMouseUp:
          break;
        case MouseEvent.IconLeftMouseUp:
          break;
        case MouseEvent.IconMiddleMouseDown:
          break;
        case MouseEvent.IconMiddleMouseUp:
          break;
        case MouseEvent.IconDoubleClick:
          // cancel single click timer
          singleClickTimer.Change(Timeout.Infinite, Timeout.Infinite);
          // bubble event
          break;
        case MouseEvent.BalloonToolTipClicked:
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof(me), "Missing handler for mouse event flag: " + me);
      }


      // get mouse coordinates
      var cursorPosition = new Interop.Point();
      if (messageSink.Version == NotifyIconVersion.Vista)
      {
        // physical cursor position is supported for Vista and above
        WinApi.GetPhysicalCursorPos(ref cursorPosition);
      }
      else
      {
        WinApi.GetCursorPos(ref cursorPosition);
      }

      cursorPosition = TrayInfo.GetDeviceCoordinates(cursorPosition);

      bool isLeftClickCommandInvoked = false;

      // show context menu, if requested
      if (IsMatch(me, PopupActivationMode.RightClick))
      {
        if (me == MouseEvent.IconLeftMouseUp)
        {
          // show context menu once we are sure it's not a double click
          singleClickTimerAction = () =>
          {
            ShowContextMenu(cursorPosition);
          };
          singleClickTimer.Change(DoubleClickWaitTime, Timeout.Infinite);
          isLeftClickCommandInvoked = true;
        }
        else
        {
          // show context menu immediately
          ShowContextMenu(cursorPosition);
        }
      }

      // make sure the left click command is invoked on mouse clicks
      if (me == MouseEvent.IconLeftMouseUp && !isLeftClickCommandInvoked)
      {
        // show context menu once we are sure it's not a double click
        singleClickTimer.Change(DoubleClickWaitTime, Timeout.Infinite);
      }
    }

    #endregion

    #region Context Menu

    /// <summary>
    /// Displays the <see cref="ContextMenu"/> if it was set.
    /// </summary>
    private void ShowContextMenu(Interop.Point cursorPosition)
    {
      //if (IsDisposed)
      //  return;

      //if (ContextMenu == null)
      //{
      //  return;
      //}

      //// use absolute positioning. We need to set the coordinates, or a delayed opening
      //// (e.g. when left-clicked) opens the context menu at the wrong place if the mouse
      //// is moved!
      //ContextMenu.Open();
      
      //var handle = messageSink.MessageWindowHandle;

      //// activate the context menu or the message window to track deactivation - otherwise, the context menu
      //// does not close if the user clicks somewhere else. With the message window
      //// fallback, the context menu can't receive keyboard events - should not happen though
      //WinApi.SetForegroundWindow(handle);
    }

    #endregion

    #region Balloon Tips

    /// <summary>
    /// Displays a balloon tip with the specified title,
    /// text, and icon in the taskbar for the specified time period.
    /// </summary>
    /// <param name="title">The title to display on the balloon tip.</param>
    /// <param name="message">The text to display on the balloon tip.</param>
    /// <param name="symbol">A symbol that indicates the severity.</param>
    public void ShowBalloonTip(string title, string message, BalloonFlags symbol)
    {
      lock (lockObject)
      {
        ShowBalloonTip(title, message, symbol, IntPtr.Zero);
      }
    }

    /// <summary>
    /// Displays a balloon tip with the specified title,
    /// text, and a custom icon in the taskbar for the specified time period.
    /// </summary>
    /// <param name="title">The title to display on the balloon tip.</param>
    /// <param name="message">The text to display on the balloon tip.</param>
    /// <param name="customIcon">A custom icon.</param>
    /// <param name="largeIcon">True to allow large icons (Windows Vista and later).</param>
    /// <exception cref="ArgumentNullException">If <paramref name="customIcon"/>
    /// is a null reference.</exception>
    public void ShowBalloonTip(string title, string message, Icon customIcon, bool largeIcon = false)
    {
      if (customIcon == null)
        throw new ArgumentNullException(nameof(customIcon));

      lock (lockObject)
      {
        var flags = BalloonFlags.User;

        if (largeIcon)
        {
          // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
          flags |= BalloonFlags.LargeIcon;
        }

        ShowBalloonTip(title, message, flags, customIcon.Handle);
      }
    }


    /// <summary>
    /// Invokes <see cref="WinApi.Shell_NotifyIcon"/> in order to display
    /// a given balloon ToolTip.
    /// </summary>
    /// <param name="title">The title to display on the balloon tip.</param>
    /// <param name="message">The text to display on the balloon tip.</param>
    /// <param name="flags">Indicates what icon to use.</param>
    /// <param name="balloonIconHandle">A handle to a custom icon, if any, or
    /// <see cref="IntPtr.Zero"/>.</param>
    private void ShowBalloonTip(string title, string message, BalloonFlags flags, IntPtr balloonIconHandle)
    {
      EnsureNotDisposed();

      iconData.BalloonText = message ?? string.Empty;
      iconData.BalloonTitle = title ?? string.Empty;

      iconData.BalloonFlags = flags;
      iconData.CustomBalloonIconHandle = balloonIconHandle;
      WriteIconData(ref iconData, NotifyCommand.Modify, IconDataMembers.Info | IconDataMembers.Icon);
    }


    /// <summary>
    /// Hides a balloon ToolTip, if any is displayed.
    /// </summary>
    public void HideBalloonTip()
    {
      EnsureNotDisposed();

      // reset balloon by just setting the info to an empty string
      iconData.BalloonText = iconData.BalloonTitle = string.Empty;
      WriteIconData(ref iconData, NotifyCommand.Modify, IconDataMembers.Info);
    }

    #endregion

    #region Single Click Timer event

    /// <summary>
    /// Performs a delayed action if the user requested an action
    /// based on a single click of the left mouse.<br/>
    /// This method is invoked by the <see cref="singleClickTimer"/>.
    /// </summary>
    private void DoSingleClickAction(object state)
    {
      if (IsDisposed)
        return;

      // run action
      Action action = singleClickTimerAction;
      if (action != null)
      {
        // cleanup action
        singleClickTimerAction = null;

        // switch to UI thread
        action();
      }
    }

    #endregion

    #region Set Version (API)

    /// <summary>
    /// Sets the version flag for the <see cref="iconData"/>.
    /// </summary>
    private void SetVersion()
    {
      iconData.VersionOrTimeout = (uint)NotifyIconVersion.Vista;
      bool status = WinApi.Shell_NotifyIcon(NotifyCommand.SetVersion, ref iconData);

      if (!status)
      {
        iconData.VersionOrTimeout = (uint)NotifyIconVersion.Win2000;
        status = WriteIconData(ref iconData, NotifyCommand.SetVersion);
      }

      if (!status)
      {
        iconData.VersionOrTimeout = (uint)NotifyIconVersion.Win95;
        status = WriteIconData(ref iconData, NotifyCommand.SetVersion);
      }

      if (!status)
      {
        Debug.Fail("Could not set version");
      }
    }

    #endregion

    #region Create / Remove Taskbar Icon

    /// <summary>
    /// Recreates the taskbar icon if the whole taskbar was
    /// recreated (e.g. because Explorer was shut down).
    /// </summary>
    private void OnTaskbarCreated()
    {
      IsTaskbarIconCreated = false;
      CreateTaskbarIcon();
    }


    /// <summary>
    /// Creates the taskbar icon. This message is invoked during initialization,
    /// if the taskbar is restarted, and whenever the icon is displayed.
    /// </summary>
    private void CreateTaskbarIcon()
    {
      lock (lockObject)
      {
        if (IsTaskbarIconCreated)
        {
          return;
        }

        const IconDataMembers members = IconDataMembers.Message | IconDataMembers.Icon | IconDataMembers.Tip;

        //write initial configuration
        var status = WriteIconData(ref iconData, NotifyCommand.Add, members);
        if (!status)
        {
          // couldn't create the icon - we can assume this is because explorer is not running (yet!)
          // -> try a bit later again rather than throwing an exception. Typically, if the windows
          // shell is being loaded later, this method is being re-invoked from OnTaskbarCreated
          // (we could also retry after a delay, but that's currently YAGNI)
          return;
        }

        //set to most recent version
        SetVersion();
        messageSink.Version = (NotifyIconVersion)iconData.VersionOrTimeout;

        IsTaskbarIconCreated = true;
      }
    }

    /// <summary>
    /// Closes the taskbar icon if required.
    /// </summary>
    private void RemoveTaskbarIcon()
    {
      lock (lockObject)
      {
        // make sure we didn't schedule a creation

        if (!IsTaskbarIconCreated)
        {
          return;
        }

        WriteIconData(ref iconData, NotifyCommand.Delete, IconDataMembers.Message);
        IsTaskbarIconCreated = false;
      }
    }

    #endregion

    #region Dispose / Exit

    /// <summary>
    /// Set to true as soon as <c>Dispose</c> has been invoked.
    /// </summary>
    public bool IsDisposed { get; private set; }


    /// <summary>
    /// Checks if the object has been disposed and
    /// raises a <see cref="ObjectDisposedException"/> in case
    /// the <see cref="IsDisposed"/> flag is true.
    /// </summary>
    private void EnsureNotDisposed()
    {
      if (IsDisposed)
        throw new ObjectDisposedException(GetType().FullName);
    }


    /// <summary>
    /// Disposes the class if the application exits.
    /// </summary>
    private void OnExit(object sender, EventArgs e)
    {
      Dispose();
    }


    /// <summary>
    /// This destructor will run only if the <see cref="Dispose()"/>
    /// method does not get called. This gives this base class the
    /// opportunity to finalize.
    /// <para>
    /// Important: Do not provide destructor in types derived from this class.
    /// </para>
    /// </summary>
    ~TaskBarIcon()
    {
      Dispose(false);
    }


    /// <summary>
    /// Disposes the object.
    /// </summary>
    /// <remarks>This method is not virtual by design. Derived classes
    /// should override <see cref="Dispose(bool)"/>.
    /// </remarks>
    public void Dispose()
    {
      Dispose(true);

      // This object will be cleaned up by the Dispose method.
      // Therefore, you should call GC.SuppressFinalize to
      // take this object off the finalization queue 
      // and prevent finalization code for this object
      // from executing a second time.
      GC.SuppressFinalize(this);
    }


    /// <summary>
    /// Closes the tray and releases all resources.
    /// </summary>
    /// <summary>
    /// <c>Dispose(bool disposing)</c> executes in two distinct scenarios.
    /// If disposing equals <c>true</c>, the method has been called directly
    /// or indirectly by a user's code. Managed and unmanaged resources
    /// can be disposed.
    /// </summary>
    /// <param name="disposing">If disposing equals <c>false</c>, the method
    /// has been called by the runtime from inside the finalizer and you
    /// should not reference other objects. Only unmanaged resources can
    /// be disposed.</param>
    /// <remarks>Check the <see cref="IsDisposed"/> property to determine whether
    /// the method has already been called.</remarks>
    private void Dispose(bool disposing)
    {
      // don't do anything if the component is already disposed
      if (IsDisposed || !disposing)
        return;

      lock (lockObject)
      {
        IsDisposed = true;

        // de-register application event listener
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
        {
          lifetime.Exit -= OnExit;
        }

        // stop timers
        singleClickTimer.Dispose();

        // dispose message sink
        messageSink.Dispose();

        // remove icon
        RemoveTaskbarIcon();
      }
    }

    #endregion

    #region WriteIconData

    /// <summary>
    /// Updates the taskbar icons with data provided by a given
    /// <see cref="NotifyIconData"/> instance.
    /// </summary>
    /// <param name="data">Configuration settings for the NotifyIcon.</param>
    /// <param name="command">Operation on the icon (e.g. delete the icon).</param>
    /// <returns>True if the data was successfully written.</returns>
    /// <remarks>See Shell_NotifyIcon documentation on MSDN for details.</remarks>
    private bool WriteIconData(ref NotifyIconData data, NotifyCommand command)
    {
      return WriteIconData(ref data, command, data.ValidMembers);
    }


    /// <summary>
    /// Updates the taskbar icons with data provided by a given
    /// <see cref="NotifyIconData"/> instance.
    /// </summary>
    /// <param name="data">Configuration settings for the NotifyIcon.</param>
    /// <param name="command">Operation on the icon (e.g. delete the icon).</param>
    /// <param name="flags">Defines which members of the <paramref name="data"/>
    /// structure are set.</param>
    /// <returns>True if the data was successfully written.</returns>
    /// <remarks>See Shell_NotifyIcon documentation on MSDN for details.</remarks>
    private bool WriteIconData(ref NotifyIconData data, NotifyCommand command, IconDataMembers flags)
    {
      data.ValidMembers |= flags;
      lock (lockObject)
      {
        return WinApi.Shell_NotifyIcon(command, ref data);
      }
    }

    #endregion

    /// <summary>
    /// Checks if a given <see cref="PopupActivationMode"/> is a match for
    /// an effectively pressed mouse button.
    /// </summary>
    public static bool IsMatch(MouseEvent me, PopupActivationMode activationMode)
    {
      switch (activationMode)
      {
        case PopupActivationMode.LeftClick:
          return me == MouseEvent.IconLeftMouseUp;
        case PopupActivationMode.RightClick:
          return me == MouseEvent.IconRightMouseUp;
        case PopupActivationMode.LeftOrRightClick:
          return Is(me, MouseEvent.IconLeftMouseUp, MouseEvent.IconRightMouseUp);
        case PopupActivationMode.LeftOrDoubleClick:
          return Is(me, MouseEvent.IconLeftMouseUp, MouseEvent.IconDoubleClick);
        case PopupActivationMode.DoubleClick:
          return Is(me, MouseEvent.IconDoubleClick);
        case PopupActivationMode.MiddleClick:
          return me == MouseEvent.IconMiddleMouseUp;
        case PopupActivationMode.All:
          //return true for everything except mouse movements
          return me != MouseEvent.MouseMove;
        default:
          throw new ArgumentOutOfRangeException("activationMode");
      }
    }

    /// <summary>
    /// Checks a list of candidates for equality to a given
    /// reference value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value">The evaluated value.</param>
    /// <param name="candidates">A liste of possible values that are
    /// regarded valid.</param>
    /// <returns>True if one of the submitted <paramref name="candidates"/>
    /// matches the evaluated value. If the <paramref name="candidates"/>
    /// parameter itself is null, too, the method returns false as well,
    /// which allows to check with null values, too.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="candidates"/>
    /// is a null reference.</exception>
    public static bool Is<T>(T value, params T[] candidates)
    {
      if (candidates == null)
        return false;

      foreach (var t in candidates)
      {
        if (value.Equals(t))
          return true;
      }

      return false;
    }

    /// <summary>
    /// Reads a given image resource into a WinForms icon.
    /// </summary>
    /// <param name="imageSource">Image source pointing to
    /// an icon file (*.ico).</param>
    /// <returns>An icon object that can be used with the
    /// taskbar area.</returns>
    public static Icon ToIcon(string imageSource)
    {
      if (imageSource == null)
        return null;

      var executingAssembly = System.Reflection.Assembly.GetExecutingAssembly();
      if (executingAssembly.GetManifestResourceNames().Contains(imageSource))
      {
        var stream = executingAssembly.GetManifestResourceStream(imageSource);
        return new Icon(stream);
      }

      return null;
    }
  }
}
