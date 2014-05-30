using System;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace MangaReader
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void App_OnExit(object sender, ExitEventArgs e)
        {
            Library.Save();
            History.Save();
            Cache.Save();
            Environment.Exit(0);
        }

        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly;
        }

        static Assembly ResolveAssembly(object sender, ResolveEventArgs args)
        {
            var thisAssembly = Assembly.GetExecutingAssembly();
            var name = args.Name.Substring(0, args.Name.IndexOf(',')) + ".dll";
            var resourceName = thisAssembly.GetManifestResourceNames().First(s => s.EndsWith(name));
            using (var stream = thisAssembly.GetManifestResourceStream(resourceName))
            {
                var block = new byte[stream.Length];
                stream.Read(block, 0, block.Length);
                return Assembly.Load(block);
            }
        }
    }
}
