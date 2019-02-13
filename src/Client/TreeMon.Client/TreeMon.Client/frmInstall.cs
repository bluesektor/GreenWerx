using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TreeMon.Managers;
using TreeMon.Managers.Membership;
using TreeMon.Models.App;
using TreeMon.Models.Membership;
using TreeMon.Utilites.Helpers;
using TreeMon.Utilites.Security;
using static TreeMon.Utilites.Security.PasswordHash;

namespace TreeMon.Client
{
    public partial class frmInstall : Form
    {
        private AppInfo _appInfo = new AppInfo();
        private string _connectionString = "";
        List<string> _databaseProviders = new List<string>();
        private bool _passwordEncrypted = false;
        private bool _appKeyEncrypted = false;
             
             

        public frmInstall()
        {
            _databaseProviders.Add("Select provider..");
            _databaseProviders.Add("MSSQL"); 
            InitializeComponent();

            txtSiteDomain.Text = "TreeMon.org";
            txtDatabaseServer.Text = "";
            txtDatabaseName.Text = "TreemonFormsApp";
            txtDatabaseUser.Text = "";
            txtDatabasePassword.Text = "";
        }

        private void frmInstall_Load(object sender, EventArgs e)
        {
            this.cboDatabaseProviders.DataSource = _databaseProviders;

            this._appInfo.AppType = "FORMS";

            if (string.IsNullOrWhiteSpace(_appInfo.AppKey))
                _appInfo.AppKey = PasswordHash.CreateHash(Guid.NewGuid().ToString("N").Replace(":","")); //todo encrypt this with user password before saving

        }

        private void btnInstallApp_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSiteDomain.Text))
            {
                MessageBox.Show("Application name is empty!", "Critical Warning", MessageBoxButtons.OK);
                return;
            }
            btnInstallApp.Enabled = false;
            _appInfo.UserName = txtUserName.Text.Trim();
            _appInfo.UserPassword = txtPassword.Text;
            _appInfo.ConfirmPassword = txtConfirmPassword.Text;
            _appInfo.UserSecurityAnswer = txtPasswordAnswer.Text;
            _appInfo.SecurityQuestion = txtPasswordQuestion.Text;
            _appInfo.AccountName = txtSiteDomain.Text;
            _appInfo.AccountIsPrivate = true;
            _appInfo.UserIsPrivate = true;
            
            _appInfo.SiteDomain         = txtSiteDomain.Text;
            _appInfo.ActiveDbProvider   = this.cboDatabaseProviders.SelectedValue.ToString();
            _appInfo.DatabaseServer     = txtDatabaseServer.Text;
            _appInfo.ActiveDatabase     = this.txtDatabaseName.Text;
            _appInfo.ActiveDbUser       = txtDatabaseUser.Text;
            _appInfo.ActiveDbPassword   = txtDatabasePassword.Text;

            txtMsg.ForeColor = Color.Black;
            txtMsg.Text = "Creating connection string..";

            ServiceResult res = CreateConnectionString(_appInfo);

            if (res.Code != 200)
            {
                btnInstallApp.Enabled = true;
                MessageBox.Show(res.Message, "Critical Warning", MessageBoxButtons.OK);
                return;
            }

            //TODO first screen should be this persons username and password, create an appkey then encrypt the appkey for each user based on their password.
            // will need to append username to appkey then query by that, decrypt the appkey to cipher the dbpassword
            //I. on change password the appkey will need to be updated.
            txtMsg.ForeColor = Color.Black;
            txtMsg.Text = "Creating database..";

            //todo v2 can connect?
            //  if already exists ask to connect?
            // else
            res = CreateDatabase(_appInfo);

            if (res.Code != 200)
            {
                btnInstallApp.Enabled = true;
                MessageBox.Show(res.Message, "Critical Warning", MessageBoxButtons.OK);
                return;
            }
            txtMsg.ForeColor = Color.Black;
            txtMsg.Text = "Seeding database..";

            res = SeedDatabase(_appInfo);

            if (res.Code != 200)
            {
                btnInstallApp.Enabled = true;
                MessageBox.Show(res.Message, "Critical Warning", MessageBoxButtons.OK);
                return;
            }
            txtMsg.ForeColor = Color.Black;
            txtMsg.Text = "Creating accounts..";


            res = CreateAccounts(_appInfo);

            if (res.Code != 200)
            {
                btnInstallApp.Enabled = true;
                MessageBox.Show(res.Message, "Critical Warning", MessageBoxButtons.OK);
                return;
            }

            txtMsg.ForeColor = Color.Black;
            txtMsg.Text = "Saving settings..";


            res = SaveSettings(_appInfo);

            if (res.Code != 200)
            {
                btnInstallApp.Enabled = true;
                MessageBox.Show(res.Message, "Critical Warning", MessageBoxButtons.OK);
                return;
            }
            txtMsg.ForeColor = Color.Black;
            txtMsg.Text = "Finalizing install..";

            res = FinalizeInstall(_appInfo);

            if (res.Code != 200)
            {
                btnInstallApp.Enabled = true;
                MessageBox.Show(res.Message, "Critical Warning", MessageBoxButtons.OK);
                return;
            }
            txtMsg.ForeColor = Color.Black;
            txtMsg.Text = "Install complete..";
    
            this.DialogResult = DialogResult.OK;
        }

        //Install Step 1.0 - This is done as user is typing in controls.
        //
        private void ValidateUserAccount(object sender, EventArgs e)
        {
            this.grpDatabaseInstall.Enabled = false;
            txtMsg.Text = "";

            if (string.IsNullOrWhiteSpace(txtUserName.Text))
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                this.txtMsg.Text = "Password cannot be blank!";
                return;
            }

            if (PasswordHash.IsCommonPassword(txtPassword.Text))
            {
                this.txtMsg.Text = "Easily hacked password!";
                return;
            }

            if (PasswordHash.CheckStrength(txtPassword.Text) < PasswordScore.Medium)
            {
                this.txtMsg.Text = "Password is WEAK!";
                return;
            }


            if (txtPassword.Text != txtConfirmPassword.Text)
            {
                this.txtMsg.Text = "Password doesn't match the confirmation password.";
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPasswordQuestion.Text) ||
                string.IsNullOrWhiteSpace(txtPasswordAnswer.Text))
            {
                this.txtMsg.Text = "Password hint question or answer cannot be empty.";
                return;
            }

            if (txtPasswordAnswer.Text == txtPasswordQuestion.Text)
            {
                this.txtMsg.Text = "Hint answer cannot be the same as the question.";
                return;
            }

            this.grpDatabaseInstall.Enabled = true;

        }

        //Install Step 1.2
        private ServiceResult CreateConnectionString(AppInfo appSettings)
        {

                AppManager am = new AppManager("", "FORMS", "");

                if (string.IsNullOrWhiteSpace(appSettings.ActiveDbProvider))
                    return new ServiceResult() { Code = 500, Status = "ERROR", Message = "Database provider is empty." };

                if (string.IsNullOrWhiteSpace(appSettings.ActiveDatabase))
                    return new ServiceResult() { Code = 500, Status = "ERROR", Message = "Database name is empty." };


                if (appSettings.ActiveDbProvider != "SQLITE")
                {
                    if (string.IsNullOrWhiteSpace(appSettings.ActiveDbUser))
                        return new ServiceResult() { Code = 500, Status = "ERROR", Message = "Database Username is empty." };

                    if (string.IsNullOrWhiteSpace(appSettings.ActiveDbPassword))
                        return new ServiceResult() { Code = 500, Status = "ERROR", Message = "Database password is empty." };
                }

                _connectionString = am.CreateConnectionString(appSettings);

                if (string.IsNullOrWhiteSpace(_connectionString))
                    return ServiceResponse.Error("Failed to create a database connectionstring.");

                string providerName = am.GetDbProviderName(appSettings.ActiveDbProvider);

                if (string.IsNullOrWhiteSpace(providerName))
                    return ServiceResponse.Error("Failed to create a database providerName.");

                if (!ClientCore.Application.SaveConnectionString(appSettings.ActiveDbProvider, _connectionString, providerName))
                    return ServiceResponse.Error("Failed to save the connection string to the .config file.");

                _appInfo.ActiveDbProvider = appSettings.ActiveDbProvider;
                _appInfo.ActiveDbConnectionKey = appSettings.ActiveDbProvider;

                //Sets the connection key
                if (ClientCore.Application.SaveConfigSetting("DefaultDbConnection", appSettings.ActiveDbProvider) == false)
                    return ServiceResponse.Error("Failed to save .config setting DefaultDbConnection for provider:" + appSettings.ActiveDbProvider);

            if (!string.IsNullOrWhiteSpace(appSettings.ActiveDbPassword) && _passwordEncrypted == false && _appKeyEncrypted == false)
            {
                _appInfo.ActiveDbPassword = Cipher.Crypt(appSettings.AppKey, appSettings.ActiveDbPassword, true);
            }
          
            return ServiceResponse.OK();

        }

        //Install Step 1.5
        private ServiceResult CreateDatabase(AppInfo appSettings)
        {
            AppManager am = new AppManager(appSettings.ActiveDbConnectionKey, "FORMS", ""); //
            if (string.IsNullOrWhiteSpace(_connectionString))
                return ServiceResponse.Error("Connection string is not set.");

            return am.CreateDatabase(appSettings, _connectionString);
        }
       
        //Install Step 1.6
        // call this from client right after the database is created
        private ServiceResult SaveSettings(AppInfo appSettings)
        {
            if (string.IsNullOrWhiteSpace(appSettings.AppKey) && _appKeyEncrypted == false )
            {
                _appInfo.AppKey = Cipher.Crypt(appSettings.UserPassword, PasswordHash.CreateHash(Guid.NewGuid().ToString("N")).Replace(":", ""), true);
                appSettings.AppKey = _appInfo.AppKey;
            }

            if (!ClientCore.Application.SaveConfigSetting("AppKey", appSettings.AppKey ))
                return ServiceResponse.Error("Failed to save AppKey:" + appSettings.AppKey);

            if (string.IsNullOrWhiteSpace(appSettings.AppType))
                appSettings.AppType = "FORMS";

            //This will create permissions for request paths as the come in. Should only be used when adding a new
            //controller/feature.
            //
            if (!ClientCore.Application.SaveConfigSetting("AddRequestPermissions", "false"))
                return ServiceResponse.Error("Failed to save AddRequestPermissions.");

            if (!ClientCore.Application.SaveConfigSetting("SiteDomain", appSettings.SiteDomain))
                return ServiceResponse.Error("Failed to save SiteDomain:" + appSettings.SiteDomain);

            if (!ClientCore.Application.SaveConfigSetting("ApiVersion", "1.0"))//backlog: have it look in the api folder to get the version
                return ServiceResponse.Error("Failed to save ApiVersion:1.0");

            if (!ClientCore.Application.SaveConfigSetting("ClientValidationEnabled", "true"))
                return ServiceResponse.Error("Failed to save ClientValidationEnabled:true");

            if (!ClientCore.Application.SaveConfigSetting("UseDatabaseConfig", "true"))
                return ServiceResponse.Error("Failed to save UseDatabaseConfig:true");

            string key = PasswordHash.CreateHash(Guid.NewGuid().ToString("N"));
            if (!ClientCore.Application.SaveConfigSetting("DBBackupKey", key.Replace(":", "").Substring(0, 11)))
                return ServiceResponse.Error("Failed to save DBBackupKey:" + key.Replace(":", "").Substring(0, 11));

            if (!ClientCore.Application.SaveConfigSetting("SessionLength", "30"))
                return ServiceResponse.Error("Failed to save SessionLength:30");

            if (!ClientCore.Application.SaveConfigSetting("TemplateEmailNewMember", "App_Data\\Templates\\Site\\EmailNewMember.html"))
                return ServiceResponse.Error("Failed to save TemplateEmailNewMember:App_Data\\Templates\\Site\\EmailNewMember.html");

            if (!ClientCore.Application.SaveConfigSetting("TemplatePasswordResetEmail", "App_Data\\Templates\\Site\\PasswordResetEmail.html"))
                return ServiceResponse.Error("Failed to save TemplatePasswordResetEmail:App_Data\\Templates\\Site\\PasswordResetEmail.html");

            if (!ClientCore.Application.SaveConfigSetting("TemplateUserInfoEmail", "App_Data\\Templates\\Site\\UserInfoEmail.html"))
                return ServiceResponse.Error("Failed to save TemplateUserInfoEmail:App_Data\\Templates\\Site\\UserInfoEmail.html");

            if (!ClientCore.Application.SaveConfigSetting("EmailStoreTemplateOrderStatusReceived", "App_Data\\Templates\\Store\\EmailOrderReceived.html"))
                return ServiceResponse.Error("Failed to save EmailStoreTemplateOrderStatusReceived:App_Data\\Templates\\Store\\EmailOrderReceived.html");


            return ServiceResponse.OK("", appSettings);
      }

        //Install Step 1.B
        private ServiceResult SeedDatabase(AppInfo appSettings)
        {
            string directory = EnvironmentEx.AppDataFolder;
            AppManager am = new AppManager(appSettings.ActiveDbConnectionKey, "FORMS", "");
            am.Installing = true;
            return am.SeedDatabase(Path.Combine(directory, "SeedData\\"));
        }

        //Install Step 2
        //Save Account info
        private ServiceResult CreateAccounts(AppInfo appSettings)
        {
            if (string.IsNullOrWhiteSpace(appSettings.UserName))
                return new ServiceResult() { Code = 500, Status = "ERROR", Message = "Username is empty." };

            if (appSettings.UserPassword != appSettings.ConfirmPassword)
                return new ServiceResult() { Code = 500, Status = "ERROR", Message = "Passwords don't match." };

            if (PasswordHash.CheckStrength(appSettings.UserPassword) < PasswordHash.PasswordScore.Medium)
                return ServiceResponse.Error("Your password is weak. Try again.");

            if (!ClientCore.Application.SaveConfigSetting("SiteAdmins", appSettings.UserName?.ToLower()))
                return ServiceResponse.Error("Error saving SiteAdmins to .config:" + appSettings.UserName);

            //Create the initial account as the domain
            if (string.IsNullOrWhiteSpace(appSettings.AccountName))
                appSettings.AccountName = appSettings.SiteDomain;

            ////if (string.IsNullOrWhiteSpace(appSettings.UserEmail))return ServiceResponse.Error("Email is empty.");

            ////if (string.IsNullOrWhiteSpace(appSettings.AccountEmail))appSettings.AccountEmail = appSettings.UserEmail;

            if (string.IsNullOrWhiteSpace(appSettings.SecurityQuestion))
                return ServiceResponse.Error("Security question is empty." );

            if (string.IsNullOrWhiteSpace(appSettings.UserSecurityAnswer))
                return ServiceResponse.Error("Security answer is empty." );

            if (!_passwordEncrypted ){
                appSettings.UserSecurityAnswer = Cipher.Crypt(appSettings.AppKey, appSettings.UserSecurityAnswer, true);
                _appInfo.UserSecurityAnswer = appSettings.UserSecurityAnswer;
            }

            if (string.IsNullOrWhiteSpace(appSettings.ActiveDbConnectionKey)) 
                return ServiceResponse.Error("ActiveDbConnectionKey is not set. This must be set to save values to the database.");

            _passwordEncrypted = true;
            AppManager am = new AppManager(appSettings.ActiveDbConnectionKey, "FORMS", "");
            ServiceResult res = am.CreateAccounts(appSettings);
            if (res.Code != 200)
                return res;

            _appInfo.UserPassword = appSettings.UserPassword;
            _appInfo.ConfirmPassword = appSettings.ConfirmPassword;

            return res;
        }

        ////Install Step: finalize
        ////Add, set and cleanup last of the installation settings
        //
        public ServiceResult FinalizeInstall(AppInfo appSettings)
        {
            if (string.IsNullOrWhiteSpace(appSettings.ActiveDbConnectionKey))
                return ServiceResponse.Error("Unable to save to database. Connection key is not set.");

            UserManager um = new UserManager(appSettings.ActiveDbConnectionKey, "");
            User user = um.GetUserByEmail(appSettings.UserEmail);

            AppManager am = new AppManager(appSettings.ActiveDbConnectionKey, "FORMS", "");
            am.Installing = true;

            am.SeedDataSetAccount(user.AccountUUID);
            

            ////  string appKey = appSettings.AppKey; //this should pull from config file. UseDatabaseConfig should be false.

            ////copy all the app settings to the database settings.
            ////res = wa.ImportWebConfigToDatabase(user, appKey, true, true);
            ////if (res.Code != 200)
            ////    return res;

            ////read back the settings and validate to make sure it's all good.
            ////AppInfo app = am.GetAppInfo(user.AccountUUID, "web");
            ////if (app == null)
            ////    return ServiceResponse.Error("App settings could not be found.");


            ////Confirm the minimum settings required to run this thing..
            ////res = am.ValidateInstallSettings(app);
            ////if (res.Code != 200)
            ////    return res;

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

            return ServiceResponse.OK();
        }

    }
}


