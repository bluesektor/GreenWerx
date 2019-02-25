// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Text;
using System.Threading;
using TreeMon.Data.Logging;
using TreeMon.Data.Logging.Models;
using TreeMon.Managers;
using TreeMon.Managers.Membership;
using TreeMon.Models.App;
using TreeMon.Models.Flags;
using TreeMon.Models.Membership;
using TreeMon.Utilites.Extensions;
using TreeMon.Utilites.Helpers;
using TreeMon.Utilites.Security;

namespace TreeMon.Web
{
    public class WebApplication
    {
        readonly private string _dbConnectionKey = null;
        readonly SystemLogger _logger = null;

        public WebApplication(string connectionKey)
        {
            _dbConnectionKey = connectionKey;
            _logger = new SystemLogger(_dbConnectionKey);

            if (ConfigurationManager.AppSettings["UseDatabaseConfig"] != null)
                UseDatabaseConfig  = StringEx.ConvertTo<bool>( ConfigurationManager.AppSettings["UseDatabaseConfig"].ToString());
        }

        public WebApplication()
        {
            Initialized = false;
            if (ConfigurationManager.AppSettings["UseDatabaseConfig"] != null)
                UseDatabaseConfig = StringEx.ConvertTo<bool>(ConfigurationManager.AppSettings["UseDatabaseConfig"].ToString());

        }

        //"ApiStatus" value="private
        public string ApiStatus { get; set; }

        public bool Initialized { get; set; }

        public string Status { get; set; }

        public bool UseDatabaseConfig{ get; set; }

        public void Start()
        {
            string pathToInstallCommands = EnvironmentEx.AppDataFolder + "\\Install\\install.json";

            if (File.Exists(pathToInstallCommands)  )
            {
                Status = "REQUIRES_INSTALL";
                UseDatabaseConfig = false;
                return;
            }

            UseDatabaseConfig = false;

            //need to get the connection key and connection string from the web.config 
            Globals.DBConnectionKey = AppSetting("DefaultDbConnection", "MSSQL");

            UseDatabaseConfig = true;//default to true  don't keep up dating the web.config 

            if (ConfigurationManager.AppSettings["UseDatabaseConfig"] != null)
                UseDatabaseConfig = StringEx.ConvertTo<bool>(ConfigurationManager.AppSettings["UseDatabaseConfig"].ToString());

            // todo get setting for  data limit multiplier. default is 25
            //    Globals.DefaultDataFilter

            this.ApiStatus = AppSetting("ApiStatus", "PRIVATE");
            Globals.AddRequestPermissions = StringEx.ConvertTo<bool>(Globals.Application.ConnectionString("AddRequestPermissions", "false"));

            Status = "RUNNING";
            Initialized = true;
        }

        public string AppSetting(string key, string defaultValue="")
        {
            string res = defaultValue;

            if (UseDatabaseConfig)
            {
                AppManager appManager = new AppManager(Globals.DBConnectionKey, "web", "");
                Setting setting = appManager.GetSetting(key);
                if (setting == null)
                    return res;

                return setting.Value;
            }
            else //save to web.config
            {
                if (ConfigurationManager.AppSettings[key] != null)
                    res = ConfigurationManager.AppSettings[key].ToString();
            }

            if (string.IsNullOrWhiteSpace(res) && string.IsNullOrWhiteSpace(defaultValue) == false)
                res = defaultValue;

            return res;
        }

        public string ConnectionString(string key, string defaultValue = "")
        {
            string res = "";

            if (UseDatabaseConfig)
                return AppSetting(key, defaultValue);

            if (ConfigurationManager.ConnectionStrings[key] != null)
                res = ConfigurationManager.ConnectionStrings[key].ToString();

            if (string.IsNullOrWhiteSpace(res) && string.IsNullOrWhiteSpace(defaultValue) == false)
                res = defaultValue;

            return res;
        }

        //Only saves to web.config
        //NOTE: Don't use this as a primary settings database if your settings change
        // often. Saving the .config file resets the app pool which will jack up connected users.
        //
        public bool SaveConfigSetting(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key)||string.IsNullOrWhiteSpace(value))
                return false;

            try
            {
                    //this is opening the web.config in the root of the project. we need the web.config in the bin folder
                    var configuration = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");

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

                    string binFile = AppDomain.CurrentDomain.BaseDirectory + "bin\\TreeMon.WebAPI.dll.config";
                    File.Copy(configuration.FilePath, binFile, true);
              
            }
            catch (Exception ex)
            {
                _logger.InsertError(ex.Message, "WebApplication", "SaveConfigSetting");
                return false;
            }
            return true;
        }

        public bool SaveConnectionString(string name, string connectionString,  string providerName)
        {
            try
            {
                var configuration = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
                var section = (ConnectionStringsSection)configuration.GetSection("connectionStrings");
                if(section.ConnectionStrings[name]== null)
                {
                    section.ConnectionStrings.Add(new ConnectionStringSettings(name, connectionString, providerName));
                }
                else
                {
                    section.ConnectionStrings[name].ConnectionString = connectionString;
                }
                configuration.Save();

                string binFile = AppDomain.CurrentDomain.BaseDirectory + "bin\\TreeMon.WebAPI.dll.config";
                File.Copy(configuration.FilePath, binFile,true);

            }
            catch (Exception ex)
            {
                _logger.InsertError(ex.Message, "WebApplication", "SaveConnectionString");
                return false;
            }
            return true;
        }
     
        public ServiceResult ImportWebConfigToDatabase(User user,string encryptionKey,  bool overwrite, bool appInstalling =false)   
        {
            if (user == null)
                return ServiceResponse.Error("Inavlid use passed in ImportWebConfigToDatabase.");
            ServiceResult res = ServiceResponse.OK();
            StringBuilder msg = new StringBuilder();
            NameValueCollection settings = ConfigurationManager.AppSettings;

            AppManager appManager = new AppManager(Globals.DBConnectionKey, "web", "");
            appManager.Installing = appInstalling;

            foreach (string key in settings.Keys)
            {
                Setting setting = appManager.GetSetting(key);

                bool settingExists = false;

                if (setting != null)  
                {
                    settingExists = true;
                    if( overwrite == false )
                        continue;  //if exists and no overwrite continue.
                }

                string type = StringEx.GetValueType(settings[key].ToString());
                Setting s = new Setting()
                {
                    UUID =  settingExists ? setting.UUID : Guid.NewGuid().ToString("N"),
                    CreatedBy = user.UUID,
                    Type = type, 
                    Name = key,
                    Value = settings[key].ToString(),
                    AccountUUID = user.AccountUUID,
                    Active = true,
                    AppType = "web",
                    DateCreated = DateTime.UtcNow,
                    Deleted = false,
                    Private = true,
                    RoleOperation = ">=",
                    RoleWeight = 4,
                    UUIDType = "Setting"
                };

                ServiceResult r = null;

                if (settingExists)
                  r =   appManager.Update (s, encryptionKey);
                else
                  r = appManager.Insert(s, encryptionKey);

                if (r.Code != 200)
                {
                   msg.AppendLine( r.Message );
                    res.Code = r.Code;
                    res.Status = r.Status;
                }
            }

            int connectionCount =  ConfigurationManager.ConnectionStrings.Count;

            for(int i = 0; i < connectionCount; i++)
            {
               
               Setting setting = appManager.GetSetting(ConfigurationManager.ConnectionStrings[i].Name);

                if (setting != null && overwrite == false)
                    continue;  //if exists and no overwrite continue.

                Setting s = new Setting()
                {
                    CreatedBy = user.UUID,
                    Type = SettingFlags.Types.EncryptedString,
                    Name = ConfigurationManager.ConnectionStrings[i].Name,
                    Value = ConfigurationManager.ConnectionStrings[i].ConnectionString,
                    AccountUUID = user.AccountUUID,
                    Active = true,
                    AppType = "web",
                    DateCreated = DateTime.UtcNow,
                    Deleted = false,
                    Private = true,
                    RoleOperation = ">=",
                    RoleWeight = 4,
                    UUIDType = "Setting"

                };
                ServiceResult r = null;

                if (setting != null)
                    r = appManager.Update (s, encryptionKey);
                else
                    r = appManager.Insert(s, encryptionKey);

                if (r.Code != 200)
                {
                    msg.AppendLine(r.Message);
                    res.Code = r.Code;
                    res.Status = r.Status;
                }
            }
            res.Message = msg.ToString();
            return res;

        }

        //Creates and updates some specific keys in the .config file during install.
        //
        public ServiceResult InitializeConfigFile(AppInfo appSettings)
        {
            AppManager am = new AppManager(Globals.DBConnectionKey, "web", "");

            if (string.IsNullOrWhiteSpace(appSettings.AppKey))
            {
                appSettings.AppKey = PasswordHash.CreateHash(Guid.NewGuid().ToString("N"));

                SaveConfigSetting("AppKey", appSettings.AppKey.Replace(":", ""));
            }

            if (string.IsNullOrWhiteSpace(appSettings.AppType))
                appSettings.AppType = "web";

            if (string.IsNullOrWhiteSpace(appSettings.AccountEmail))
                appSettings.AccountEmail = appSettings.UserEmail;

            ServiceResult res = am.ValidateInstallSettings(appSettings);
            if (res.Code != 200)
                return res;

            if (string.IsNullOrWhiteSpace(appSettings.ActiveDbConnectionKey))
                appSettings.ActiveDbConnectionKey = appSettings.ActiveDbProvider;

            //Create the initial account as the domain
            if (string.IsNullOrWhiteSpace(appSettings.AccountName))
                appSettings.AccountName = appSettings.SiteDomain;

            #region DB connection 
            string connectionString = am.CreateConnectionString(appSettings);

            if (string.IsNullOrWhiteSpace(connectionString))
                return ServiceResponse.Error("Failed to create a database connectionstring.");

            string providerName = am.GetDbProviderName(appSettings.ActiveDbProvider);

            if (string.IsNullOrWhiteSpace(providerName))
                return ServiceResponse.Error("Failed to create a database providerName.");

            SaveConnectionString(appSettings.ActiveDbProvider, connectionString, providerName);

            if (string.IsNullOrWhiteSpace(Globals.DBConnectionKey))
                Globals.DBConnectionKey = appSettings.ActiveDbProvider; //set this so after the install it has something to reference.

            //Sets the connection key
            if (!string.IsNullOrWhiteSpace(appSettings.ActiveDbProvider))
                SaveConfigSetting("DefaultDbConnection", appSettings.ActiveDbProvider);
            #endregion

            //This will create permissions for request paths as the come in. Should only be used when adding a new
            //controller/feature.
            //
            if (string.IsNullOrWhiteSpace(AppSetting("AddRequestPermissions")))
                SaveConfigSetting("AddRequestPermissions", "false");

            if (!string.IsNullOrWhiteSpace(appSettings.SiteDomain))
                SaveConfigSetting("SiteDomain", appSettings.SiteDomain);

            if (string.IsNullOrWhiteSpace(AppSetting("ApiVersion")))
                SaveConfigSetting("ApiVersion", "1.0");//backlog: have it look in the api folder to get the version

            if (string.IsNullOrWhiteSpace(AppSetting("ClientValidationEnabled")))
                SaveConfigSetting("ClientValidationEnabled", "true");

            if (string.IsNullOrWhiteSpace(AppSetting("UseDatabaseConfig")))
                SaveConfigSetting("UseDatabaseConfig", "true");

            if (string.IsNullOrWhiteSpace(AppSetting("ApiStatus")))
            {
#if DEBUG
                //using NG server will cut off access because it's on a different port.
                SaveConfigSetting("ApiStatus", "PROTECTED");
#else
                SaveConfigSetting("ApiStatus", "PRIVATE");         
#endif
            }



            if (string.IsNullOrWhiteSpace(AppSetting("DBBackupKey")))
            {
                string key = PasswordHash.CreateHash(Guid.NewGuid().ToString("N"));
                SaveConfigSetting("DBBackupKey", key.Replace(":", "").Substring(0, 11));
            }

            if (!string.IsNullOrWhiteSpace(appSettings.UserName))
                SaveConfigSetting("SiteAdmins", appSettings.UserName?.ToLower());

            if (string.IsNullOrWhiteSpace(AppSetting("SessionLength")))
                SaveConfigSetting("SessionLength", "30");

            if (string.IsNullOrWhiteSpace(AppSetting("TemplateEmailNewMember")))
                SaveConfigSetting("TemplateEmailNewMember", "App_Data\\Templates\\Site\\EmailNewMember.html");

            if (string.IsNullOrWhiteSpace(AppSetting("TemplatePasswordResetEmail")))
                SaveConfigSetting("TemplatePasswordResetEmail", "App_Data\\Templates\\Site\\PasswordResetEmail.html");

            if (string.IsNullOrWhiteSpace(AppSetting("TemplateUserInfoEmail")))
                SaveConfigSetting("TemplateUserInfoEmail", "App_Data\\Templates\\Site\\UserInfoEmail.html");

            if (string.IsNullOrWhiteSpace(AppSetting("EmailStoreTemplateOrderStatusReceived")))
                SaveConfigSetting("EmailStoreTemplateOrderStatusReceived", "App_Data\\Templates\\Store\\EmailOrderReceived.html");

            //Razor versioning. Backlog: depricate when remaining razor tags are removed.
            if (string.IsNullOrWhiteSpace(AppSetting("webpages:Version")))
               SaveConfigSetting("webpages:Version", "3.0.0.0");

            if (string.IsNullOrWhiteSpace(AppSetting("webpages:Enabled")))
                SaveConfigSetting("webpages:Enabled", "false");

            if (string.IsNullOrWhiteSpace(AppSetting("vs:EnableBrowserLink")))
                SaveConfigSetting("vs:EnableBrowserLink", "false");

            if (string.IsNullOrWhiteSpace(AppSetting("UnobtrusiveJavaScriptEnabled")))
                SaveConfigSetting("UnobtrusiveJavaScriptEnabled", "true");

          


            return res;
        }
    }
}