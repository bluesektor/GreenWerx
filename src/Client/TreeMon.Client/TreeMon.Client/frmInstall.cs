using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TreeMon.Models.App;

namespace TreeMon.Client
{
    public partial class frmInstall : Form
    {
        AppInfo _appInfo = new AppInfo();

        List<string> _databaseProviders = new List<string>();

        public frmInstall()
        {
            _databaseProviders.Add("Select provider..");
            _databaseProviders.Add("MSSQL");
            InitializeComponent();
        }
        #region Installer code - Code is copied from api project to make sure I get everything covered..

        private void frmInstall_Load(object sender, EventArgs e)
        {
            //this._appInfo.SiteDomain = '';
            //this._appInfo.ActiveDbProvider = '';
            this.cboDatabaseProviders.DataSource = _databaseProviders;
            //this._appInfo.ActiveDbConnectionKey = '';
            //this._appInfo.ActiveDbPassword = '';


            //this._appInfo.ActiveDbUser = '';
            //this._appInfo.DatabaseServer = 'localhost';


            //this._appInfo.AccountEmail = '';
            //this._appInfo.AccountIsPrivate = true;
            //this._appInfo.AccountName = '';
            //this._appInfo.ActiveDatabase = '';

            //this._appInfo.AppType = 'web';
            //this._appInfo.ConfirmPassword = '';

            //this._appInfo.PasswordSalt = '';
            //this._appInfo.RunInstaller = true;
            //this._appInfo.SecurityQuestion = '';

            //this._appInfo.UserEmail = '';
            //this._appInfo.UserIsPrivate = true;
            //this._appInfo.UserName = 'bluesektor';
            //this._appInfo.UserPassword = '';
            //this._appInfo.UserSecurityAnswer = '';
        }



        /*
              //Install Step 1
        public ServiceResult CreateDatabase(AppInfo appSettings)
        {
            if (IsInstallReady() == false)
                return ServiceResponse.Error("Website is not ready to be installed. Check the intall.json file.");

            AppManager am = new AppManager(Globals.DBConnectionKey, "web", "");
            string connectionString = am.CreateConnectionString(appSettings);

            if (string.IsNullOrWhiteSpace(connectionString))
                return ServiceResponse.Error("Failed to create a database connectionstring.");

            if (appSettings.ActiveDbProvider != "SQLITE")
            {
                if (string.IsNullOrWhiteSpace(appSettings.ActiveDbUser))
                    return new ServiceResult() { Code = 500, Status = "ERROR", Message = "Database Username is empty." };

                if (string.IsNullOrWhiteSpace(appSettings.ActiveDbPassword))
                    return new ServiceResult() { Code = 500, Status = "ERROR", Message = "Database password is empty." };

                if (string.IsNullOrWhiteSpace(appSettings.ActiveDatabase))
                    return new ServiceResult() { Code = 500, Status = "ERROR", Message = "Database name is empty." };
            }

            string providerName = am.GetDbProviderName(appSettings.ActiveDbProvider);

            if (string.IsNullOrWhiteSpace(providerName))
                return ServiceResponse.Error("Failed to create a database providerName.");

            if (!Globals.Application.SaveConnectionString(appSettings.ActiveDbProvider, connectionString, providerName))
                return ServiceResponse.Error("Failed to save the connection string to the .config file.");

            appSettings.ActiveDbConnectionKey = appSettings.ActiveDbProvider; //set this so after the install it has something to reference.

            Globals.DBConnectionKey = appSettings.ActiveDbConnectionKey;

            //Sets the connection key
            if ( Globals.Application.SaveConfigSetting("DefaultDbConnection", appSettings.ActiveDbProvider) == false)
                return ServiceResponse.Error("Failed to save .config setting DefaultDbConnection for provider:" + appSettings.ActiveDbProvider);

            //if (!string.IsNullOrWhiteSpace(appSettings.ActiveDatabase))
            //    appSettings.ActiveDbPassword = Cipher.Crypt(appSettings.AppKey, appSettings.ActiveDbPassword, true);

            return am.CreateDatabase(appSettings, connectionString);
        }

        //Install Step 1.A
        // call this from client right after the databaase is created
        [HttpPost]
        [Route("api/Apps/Install/SaveSettings")]
        public ServiceResult SaveSettings(AppInfo appSettings)
        {
            if (IsInstallReady() == false)
                return ServiceResponse.Error("Website is not ready to be installed. Check the intall.json file.");

            if (string.IsNullOrWhiteSpace(appSettings.AppKey))
                appSettings.AppKey = PasswordHash.CreateHash(Guid.NewGuid().ToString("N"));

            WebApplication wa = new WebApplication();

            if (string.IsNullOrWhiteSpace(appSettings.AppKey))
                appSettings.AppKey = PasswordHash.CreateHash(Guid.NewGuid().ToString("N"));

            if (!wa.SaveConfigSetting("AppKey", appSettings.AppKey.Replace(":", "")))
                return ServiceResponse.Error("Failed to save AppKey:" + appSettings.AppKey);
           
            if (string.IsNullOrWhiteSpace(appSettings.AppType))
                appSettings.AppType = "web";

            //This will create permissions for request paths as the come in. Should only be used when adding a new
            //controller/feature.
            //
            if(!wa.SaveConfigSetting("AddRequestPermissions", "false"))
                return ServiceResponse.Error("Failed to save AddRequestPermissions." );
           
            if (!wa.SaveConfigSetting("SiteDomain", appSettings.SiteDomain))
                return ServiceResponse.Error("Failed to save SiteDomain:" + appSettings.SiteDomain);
           
            if( !wa.SaveConfigSetting("ApiVersion", "1.0"))//backlog: have it look in the api folder to get the version
                return ServiceResponse.Error("Failed to save ApiVersion:1.0");

            if (!wa.SaveConfigSetting("ClientValidationEnabled", "true"))
                return ServiceResponse.Error("Failed to save ClientValidationEnabled:true");

            if (!wa.SaveConfigSetting("UseDatabaseConfig", "true"))
                return ServiceResponse.Error("Failed to save UseDatabaseConfig:true");
          
            string key = PasswordHash.CreateHash(Guid.NewGuid().ToString("N"));
            if (!wa.SaveConfigSetting("DBBackupKey", key.Replace(":", "").Substring(0, 11)))
                return ServiceResponse.Error("Failed to save DBBackupKey:" + key.Replace(":", "").Substring(0, 11));
            
            if (!wa.SaveConfigSetting("SessionLength", "30"))
                return ServiceResponse.Error("Failed to save SessionLength:30" );

            if (!wa.SaveConfigSetting("TemplateEmailNewMember", "App_Data\\Templates\\Site\\EmailNewMember.html"))
                return ServiceResponse.Error("Failed to save TemplateEmailNewMember:App_Data\\Templates\\Site\\EmailNewMember.html");

            if (!wa.SaveConfigSetting("TemplatePasswordResetEmail", "App_Data\\Templates\\Site\\PasswordResetEmail.html"))
                return ServiceResponse.Error("Failed to save TemplatePasswordResetEmail:App_Data\\Templates\\Site\\PasswordResetEmail.html");

            if (!wa.SaveConfigSetting("TemplateUserInfoEmail", "App_Data\\Templates\\Site\\UserInfoEmail.html"))
                return ServiceResponse.Error("Failed to save TemplateUserInfoEmail:App_Data\\Templates\\Site\\UserInfoEmail.html");

            if (!wa.SaveConfigSetting("EmailStoreTemplateOrderStatusReceived", "App_Data\\Templates\\Store\\EmailOrderReceived.html"))
                return ServiceResponse.Error("Failed to save EmailStoreTemplateOrderStatusReceived:App_Data\\Templates\\Store\\EmailOrderReceived.html");

            #region depricate
            ////Razor versioning. Backlog: depricate when remaining razor tags are removed.
            //if (string.IsNullOrWhiteSpace(AppSetting("webpages:Version")))
            //    SaveConfigSetting("webpages:Version", "3.0.0.0");

            //if (string.IsNullOrWhiteSpace(AppSetting("webpages:Enabled")))
            //    SaveConfigSetting("webpages:Enabled", "false");

            //if (string.IsNullOrWhiteSpace(AppSetting("vs:EnableBrowserLink")))
            //    SaveConfigSetting("vs:EnableBrowserLink", "false");

            //if (string.IsNullOrWhiteSpace(AppSetting("UnobtrusiveJavaScriptEnabled")))
            //    SaveConfigSetting("UnobtrusiveJavaScriptEnabled", "true");
            #endregion

            return ServiceResponse.OK("", appSettings);
        }

        //Install Step 1.B
        [HttpPost]
        [Route("api/Apps/Install/SeedDatabase")]
        public ServiceResult SeedDatabase(AppInfo appSettings)
        {
            if (IsInstallReady() == false)
                return ServiceResponse.Error("Website is not ready to be installed. Check the intall.json file.");

            string directory = EnvironmentEx.AppDataFolder;
            AppManager am = new AppManager(Globals.DBConnectionKey, "web", "");
            am.Installing = true;
            return  am.SeedDatabase(Path.Combine(directory, "Install\\SeedData\\")  );
        }

        //Install Step 2
        //Save Account info
        [HttpPost]
        [Route("api/Apps/Install/Accounts")]
        public ServiceResult CreateAccounts(AppInfo appSettings)
        {
            if (IsInstallReady() == false)
                return ServiceResponse.Error("Website is not ready to be installed. Check the intall.json file.");

            if (string.IsNullOrWhiteSpace(appSettings.UserName))
                return new ServiceResult() { Code = 500, Status = "ERROR", Message = "Username is empty." };

            if (appSettings.UserPassword != appSettings.ConfirmPassword)
                return new ServiceResult() { Code = 500, Status = "ERROR", Message = "Passwords don't match." };


            if (PasswordHash.CheckStrength(appSettings.UserPassword) < PasswordHash.PasswordScore.Medium)
                return new ServiceResult() { Code = 500, Status = "ERROR", Message = "Password is too weak." };

            if (PasswordHash.CheckStrength(appSettings.UserPassword) < PasswordHash.PasswordScore.Strong)
                return ServiceResponse.Error("Your password is weak. Try again.");

            WebApplication wa = new WebApplication();
            if (!wa.SaveConfigSetting("SiteAdmins", appSettings.UserName?.ToLower()))
                return ServiceResponse.Error("Error saving SiteAdmins to .config:" + appSettings.UserName);

            //Create the initial account as the domain
            if (string.IsNullOrWhiteSpace(appSettings.AccountName))
                appSettings.AccountName = appSettings.SiteDomain;

            if (string.IsNullOrWhiteSpace(appSettings.UserEmail))
                return new ServiceResult() { Code = 500, Status = "ERROR", Message = "Email is empty." };


            if (string.IsNullOrWhiteSpace(appSettings.AccountEmail))
                appSettings.AccountEmail = appSettings.UserEmail;


            if (string.IsNullOrWhiteSpace(appSettings.SecurityQuestion))
                return new ServiceResult() { Code = 500, Status = "ERROR", Message = "Security question is empty." };

            if (string.IsNullOrWhiteSpace(appSettings.UserSecurityAnswer))
                return new ServiceResult() { Code = 500, Status = "ERROR", Message = "Security answer is empty." };


            if (string.IsNullOrWhiteSpace(Globals.DBConnectionKey)) //appSettings.ActiveDbConnectionKey))
                return ServiceResponse.Error("ActiveDbConnectionKey is not set. This must be set to save values to the database.");

            AppManager am = new AppManager(Globals.DBConnectionKey, "web", "");
            return am.CreateAccounts(appSettings);
        }

        //Install Step: finalize
        //Add, set and cleanup last of the installation settings
        [HttpPost]
        [Route("api/Apps/Install/Finalize")]
        public ServiceResult FinalizeInstall(AppInfo appSettings)
        {
            if (string.IsNullOrWhiteSpace(Globals.DBConnectionKey))// appSettings.ActiveDbConnectionKey))
                return ServiceResponse.Error("Unable to save to database. Connection key is not set.");

            UserManager um = new UserManager(Globals.DBConnectionKey, "");
             User user = um.GetUserByEmail(appSettings.UserEmail);

            string appKey = Globals.Application.AppSetting("AppKey"); //this should pull from config file. UseDatabaseConfig should be false.
            //After Config file is saved try saving to  database.
           
            AppManager am = new AppManager(Globals.DBConnectionKey, "web", "");
            am.Installing = true;

            string siteDomain = appSettings.SiteDomain.StartsWith("http://") ? appSettings.SiteDomain : "http://" + appSettings.SiteDomain;
           
            am.Insert(new Setting()
            {
                AccountUUID = user.AccountUUID,
                CreatedBy = user.UUID,
                Active = true,
                Deleted = false,
                AppType = "web",
                Value = siteDomain,
                Name = "AllowedOrigin",
                SettingClass = "string",
                RoleWeight = 4,
                RoleOperation = ">=",
                DateCreated = DateTime.UtcNow,
                 Type = "string"
            }, "");

            Globals.Application.ApiStatus = "PRIVATE";

            IEnumerable<string> originValues;
            Request.Headers.TryGetValues("Origin", out originValues);
            foreach (string origin in originValues)
            {
                //the installer is not on the same domain, so we need to be able to access it from the domain.
                //allow cors from the orginating domain
                if (!siteDomain.Contains(origin))
                {
                    Globals.Application.ApiStatus = "PROTECTED";
                }

                if (am.SettingExists("AllowedOrigin", origin))
                    continue;

                am.Insert(new Setting()
                {
                    AccountUUID = user.AccountUUID,
                    CreatedBy = user.UUID,
                    Active = true,
                    Deleted = false,
                    AppType = "web",
                    Value = origin,
                    Name = "AllowedOrigin",
                    SettingClass = "string",
                    RoleWeight = 4,
                    RoleOperation = ">=",
                    DateCreated = DateTime.UtcNow,
                    Type = "string"
                }, "");
            }
            WebApplication wa = new WebApplication();
            ServiceResult res;  

            if (!wa.SaveConfigSetting("ApiStatus", Globals.Application.ApiStatus))
                return ServiceResponse.Error("Failed to save ApiStatus:" + Globals.Application.ApiStatus);

            //Add to the database because the import won't catch it in time for the file to update.
            am.Insert(new Setting()
            {
                AccountUUID = user.AccountUUID,
                CreatedBy = user.UUID,
                Active = true,
                Deleted = false,
                AppType = "web",
                Value = Globals.Application.ApiStatus,
                Name = "ApiStatus",
                SettingClass = "string",
                RoleWeight = 4,
                RoleOperation = ">=",
                DateCreated = DateTime.UtcNow,
                Type = "string"
                
            }, "");

         
            am.SeedDataSetAccount(user.AccountUUID);

            //copy all the app settings to the database settings.
            res = wa.ImportWebConfigToDatabase(user, appKey, true, true);
            if (res.Code != 200)
                return res;

            //read back the settings and validate to make sure it's all good.
            AppInfo app = am.GetAppInfo(user.AccountUUID, "web");
            if (app == null)
                return ServiceResponse.Error("App settings could not be found.");


            //Confirm the minimum settings required to run this thing..
            res = am.ValidateInstallSettings(app);
            if (res.Code != 200)
                return res;
            
            ////We need to make sure the file is gone before moving on because
            ////the status flag may not get reset correctly to running.
            string directory = EnvironmentEx.AppDataFolder;
            string pathToCommands = Path.Combine(directory, "Install\\install.json");
            try
            {
                File.Delete(pathToCommands);
            }
            catch (Exception ex)
            {
                return ServiceResponse.Error(ex.Message);
            }
            bool fileDeleted = false;
            int waitCount = 1;
            do
            {
                if (!File.Exists(pathToCommands))
                    fileDeleted = true;

                if (!fileDeleted)
                    Thread.Sleep(1000);

                waitCount++;

                if (waitCount > 6)
                    return ServiceResponse.Error("Unable to delete install.json file.");
            }
            while (!fileDeleted);

            Globals.Application.Status = "RUNNING";
            Globals.Application.UseDatabaseConfig = true;

            return ServiceResponse.OK();
        }
 
          
          
         */
        #endregion
    }
}


