using ClientCore.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TreeMon.Managers;
using TreeMon.Models.App;

namespace ClientCore
{
    public static class Application
    {
        public static bool SaveConfigSetting(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value))
                return false;

            try
            {
                string binFile = AppDomain.CurrentDomain.BaseDirectory + "TreeMon.Client.exe.config";
                ExeConfigurationFileMap map = new ExeConfigurationFileMap { ExeConfigFilename = binFile };
                Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);

                var section = (AppSettingsSection)configuration.GetSection("appSettings");

                if (section.Settings[key] == null)
                {
                    section.Settings.Add(new KeyValueConfigurationElement(key, value));
                }
                else
                {
                    section.Settings[key].Value = value;
                }
                configuration.Save();

                string projectFile = AppDomain.CurrentDomain.BaseDirectory.Replace("bin\\Debug", "").Replace("bin\\Release", "") + "App.config";
                File.Copy(configuration.FilePath, projectFile, true);

            }
            catch (Exception ex)
            {
                Debug.Assert(false, ex.Message);
                // _logger.InsertError(ex.Message, "WebApplication", "SaveConfigSetting");
                return false;
            }
            return true;
        }

        public static bool SaveConnectionString(string name, string connectionString, string providerName)
        {
            try
            {
                string binFile = AppDomain.CurrentDomain.BaseDirectory + "TreeMon.Client.exe.config";
                ExeConfigurationFileMap map = new ExeConfigurationFileMap { ExeConfigFilename = binFile };
                Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);

                var section = (ConnectionStringsSection)configuration.GetSection("connectionStrings");
                if (section.ConnectionStrings[name] == null)
                {
                    section.ConnectionStrings.Add(new ConnectionStringSettings(name, connectionString, providerName));
                }
                else
                {
                    section.ConnectionStrings[name].ConnectionString = connectionString;
                }
                configuration.Save();

                string projectFile = AppDomain.CurrentDomain.BaseDirectory.Replace("bin\\Debug", "").Replace("bin\\Release", "") + "App.config";
                File.Copy(configuration.FilePath, projectFile, true);

            }
            catch (Exception ex)
            {
                Debug.Assert(false, ex.Message);
                //  _logger.InsertError(ex.Message, "WebApplication", "SaveConnectionString");
                return false;
            }
            return true;
        }

        public static string GetSetting(string name, bool useDatabase, string dbConnectionKey, string sessionKey)
        {
            AppManager am = new AppManager(dbConnectionKey, "FORMS", sessionKey);
            return am.GetSetting(name, false)?.Value;
        }

        public static AppInfo GetSettings(bool useDatabase, string dbConnectionKey, string sessionKey)
        {
            AppInfo settings = new AppInfo();
            AppManager am = new AppManager(dbConnectionKey, "FORMS", sessionKey);
            settings.ActiveDbConnectionKey = am.GetSetting("DefaultDbConnection", false)?.Value;
            settings.AppKey = am.GetSetting("AppKey", false)?.Value;//this will comeback empty because we don't trust yet.
            return settings;
        }

        //public static TreeNode BuildMenu(string pluginName)
        //{
        //    TreeNode root = new TreeNode();
        //    //todo move data to settings table. make SettingsClass = "MenuItem" or "List<MenuItem>"
        //    //   this way we can set roleweight and operation. Store it in value as json string
        //    //
        //    switch (pluginName)
        //    {
        //        case "home":
        //            root.items.Add(new MenuItem()
        //            {
        //                label = "Locations",
        //                type = "Location",
        //                icon = "fa fa-house"
        //            });

        //            //root.items.Add(new MenuItem()
        //            //{
        //            //    label = "System",
        //            //    type = "link",
        //            //    icon = "fa-cogs",
        //            //    items = new List<MenuItem>()
        //            //    {
        //            //        new MenuItem() {   label = "Settings", type = "link" }
        //            //    }
        //            //});
        //            break;
        //    }
        //    root.items = root.items.OrderBy(o => o.SortOrder).ToList();
        //    return root;
        //}
    }
}
