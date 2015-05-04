using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace WinFormsSample
{
    interface IBootsTrapper
    {
        void Bootstrap();
    }

    class NativeBootsTrapper : IBootsTrapper
    {
        private readonly string _path;

        public NativeBootsTrapper(string path)
        {
            if (String.IsNullOrEmpty(path)) throw new ArgumentNullException("path");
            if (!Directory.Exists(path)) throw new ArgumentException("Directory does not exist", "path");
            _path = path;
        }

        public void Bootstrap()
        {
            // Prepend path to environment path, to ensure the right libs are being used.
            var path = Environment.GetEnvironmentVariable("PATH");
            path = String.Concat(_path, ";", path);
            Environment.SetEnvironmentVariable("PATH", path);

            // add custom assembly resolver
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
                {
                    var assemblyName = args.Name.Split(',')[0];
                    var assemblyFileName = Path.Combine(_path, assemblyName + ".dll");
                    if (File.Exists(assemblyFileName))
                    {
                        var assembly = Assembly.LoadFrom(assemblyFileName);
                        Trace.WriteLine(String.Format("Resolved assembly \"{0}\" from \"{1}\"", assemblyName, _path));
                        return assembly;
                    }
                    return null;
                };
        }
    }
}