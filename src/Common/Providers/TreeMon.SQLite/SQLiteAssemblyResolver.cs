using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SQLite
{
    class SQLiteAssemblyResolver
    {
        //private static void InstantiateMyTypeSucceed(AppDomain domain)
        //{
        //    try
        //    {
        //        string asmname = Assembly.GetCallingAssembly().FullName;
        //        domain.CreateInstance(asmname, "MyType");
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine();
        //        Console.WriteLine(e.Message);
        //    }
        //}

        //private static Assembly MyResolveEventHandler(object sender, ResolveEventArgs args)
        //{
        //    Console.WriteLine("Resolving...");
        //    return typeof(MyType).Assembly;
        //}

            //implementation example
        public static void HandleUnresovledAssemblies()
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            //references function below
         //   currentDomain.AssemblyResolve += currentDomain_AssemblyResolve;
        }

        public static Assembly ResolveSQLiteAssembly(object sender, ResolveEventArgs args)
        {
            if (args.Name == "System.Data.SQLite")
            {
                //        var path = Path.Combine(pathToWhereYourNativeFolderLives, "Native");

                string directory = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug", "");
                string file = "Dependencies\\SQLite\\x{0}\\sqlite3.dll";

                if (Environment.Is64BitProcess)// IntPtr.Size == 8
                {
                    file = string.Format(file, "64");
                }
                else
                {
                    file = string.Format(file, "86");
                }

                string pathToDepends = Path.Combine(directory, file);

                string pathToFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sqlite3.dll");

               //overwrite no matter what 
                File.Copy(pathToDepends, pathToFile,true);
               
               string  path = Path.Combine(pathToFile, "System.Data.SQLite.DLL");

                Assembly assembly = Assembly.LoadFrom(path);
                return assembly;
            }
                return null;
        }

        //refe function assigned to  currentDomain.AssemblyResolve +=
        //private static Assembly currentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        //{
        //    if (args.Name == "System.Data.SQLite")
        //    {
        //        var path = Path.Combine(pathToWhereYourNativeFolderLives, "Native");

        //string directory = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug", "");
        //string file = "Dependencies\\SQLite\\x{0}\\sqlite3.dll";
            
        //            if (Environment.Is64BitProcess)// IntPtr.Size == 8
        //            {
        //                file = string.Format(file, "64");
        //    }
        //            else
        //            {
        //                file = string.Format(file, "86");
        //}

        //string pathToDepends = Path.Combine(directory, file);

        //string pathToFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sqlite3.dll");

        //            if (!File.Exists(pathToFile))
        //            {
        //                File.Copy(pathToDepends, pathToFile);
        //            }
        //        path = Path.Combine(path, "System.Data.SQLite.DLL");

        //        Assembly assembly = Assembly.LoadFrom(path);
        //        return assembly;
        //    }

           // return null;
        //}
    }
}
