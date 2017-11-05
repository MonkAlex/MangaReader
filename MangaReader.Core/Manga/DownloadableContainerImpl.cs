using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

namespace MangaReader.Core.Manga
{
  public abstract class DownloadableContainerImpl<T> : Entity.Entity, IDownloadableContainer<T> where T : IDownloadable
  {
    private ICollection<T> container;

    /// <summary>
    /// Статус загрузки.
    /// </summary>
    public virtual bool IsDownloaded
    {
      get { return InDownloading != null && InDownloading.Any() && InDownloading.All(v => v.IsDownloaded); }
    }

    /// <summary>
    /// Процент загрузки манги.
    /// </summary>
    public virtual double Downloaded
    {
      get { return (InDownloading != null && InDownloading.Any()) ? InDownloading.Average(ch => ch.Downloaded) : 0; }
      set { }
    }

    public Uri Uri { get; set; }

    public virtual string Folder { get; protected set; }

    public DateTime? DownloadedAt { get; set; }

    public event EventHandler<IManga> DownloadProgressChanged;

    public abstract Task Download(string folder = null);

    public ICollection<T> Container
    {
      get { return container; }
      set { Init(value); }
    }

    IEnumerable<T> IDownloadableContainer<T>.Container { get { return Container; } }

    public abstract IEnumerable<T> InDownloading { get; protected set; }

    private void ContainerOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
    {
      if (args.Action == NotifyCollectionChangedAction.Remove ||
          args.Action == NotifyCollectionChangedAction.Replace ||
          args.Action == NotifyCollectionChangedAction.Move)
      {
        if (args.OldItems != null)
          foreach (var item in args.OldItems.OfType<T>())
          {
            item.DownloadProgressChanged -= OnDownloadProgressChanged;
          }
      }

      if (args.Action == NotifyCollectionChangedAction.Add ||
          args.Action == NotifyCollectionChangedAction.Replace ||
          args.Action == NotifyCollectionChangedAction.Move ||
          args.Action == NotifyCollectionChangedAction.Reset)
      {
        if (args.NewItems != null)
          foreach (var item in args.NewItems.OfType<T>())
          {
            item.DownloadProgressChanged += OnDownloadProgressChanged;
          }
      }
    }

    private void OnDownloadProgressChanged(object sender, IManga manga)
    {
      DownloadProgressChanged?.Invoke(this, manga);
    }

    protected virtual void Init(ICollection<T> baseItems)
    {
      var nccea = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, 
        (baseItems ?? Enumerable.Empty<T>()).ToList(), (container ?? Enumerable.Empty<T>()).ToList());
      if (baseItems is INotifyCollectionChanged collectionChanged)
      {
        (container as INotifyCollectionChanged).CollectionChanged -= ContainerOnCollectionChanged;
        collectionChanged.CollectionChanged += ContainerOnCollectionChanged;
        ContainerOnCollectionChanged(this, nccea);
        container = (ICollection<T>)collectionChanged;
        return;
      }

      if (container != null)
      {
        container.Clear();
        (container as INotifyCollectionChanged).CollectionChanged -= ContainerOnCollectionChanged;
      }

      var observableCollection = baseItems != null ? new ObservableCollection<T>(baseItems) : new ObservableCollection<T>();
      observableCollection.CollectionChanged += ContainerOnCollectionChanged;
      ContainerOnCollectionChanged(this, nccea);
      container = observableCollection;
    }

    protected DownloadableContainerImpl()
    {
      this.Init(null);
    }
  }
}