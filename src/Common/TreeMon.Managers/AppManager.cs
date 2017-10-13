// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using Dapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using TreeMon.Data;
using TreeMon.Data.Logging;
using TreeMon.Data.Logging.Models;
using TreeMon.Managers.Finance;
using TreeMon.Managers.General;
using TreeMon.Managers.Membership;
using TreeMon.Managers.Plant;
using TreeMon.Managers.Store;
using TreeMon.Models;
using TreeMon.Models.App;
using TreeMon.Models.Finance;
using TreeMon.Models.Flags;
using TreeMon.Models.General;
using TreeMon.Models.Geo;
using TreeMon.Models.Medical;
using TreeMon.Models.Membership;
using TreeMon.Models.Plant;
using TreeMon.Models.Store;
using TreeMon.Utilites.Extensions;
using TreeMon.Utilites.Helpers;
using TreeMon.Utilites.Security;


namespace TreeMon.Managers
{
    public class AppManager : BaseManager
    {
        private readonly string _sessionKey;

        readonly SystemLogger _logger;

        //todo check file instead of setting this in appscontroller
        public bool Installing { get; set; }

        /// <summary>
        /// AppTypes are web, forms, mobil(for mobile phone app, not mobile theme).
        /// </summary>
        /// <param name="appType"></param>
        public AppManager(string connectionKey, string appType, string sessionKey) : base(connectionKey, sessionKey)
        {
            this.Installing = false;
            AppType = appType;
            this._connectionKey = connectionKey;

            _sessionKey = sessionKey;
            _logger = new SystemLogger(connectionKey);
        }

        public ServiceResult ScanForDuplicates(string table, string processId)
        {
            if (string.IsNullOrWhiteSpace(table))
                return ServiceResponse.Error("Table name is empty.");

            List<Node> tableData = this.GetTableData(table);

            var duplicates = tableData.GroupBy(gb => gb.SafeName)
                .Where(w => w.Count() > 1)
                .Select(s => new
                {
                    key = s.Key,
                    group = s.ToList()
                });


            return ServiceResponse.OK("", duplicates);
        }


        public ServiceResult SearchTables(string[] values)
        {
            if (values == null)
                return ServiceResponse.Error("Search values is empty.");

            List<Node> res = new List<Node>();
            foreach (string value in values)
            {
                res.AddRange(SearchTables(value));
            }
            return ServiceResponse.OK("", res);
        }

        private List<Node> SearchTables(string value)
        {

            List<Node> res = new List<Node>();
            string pathToSql = Path.Combine(EnvironmentEx.AppDataFolder, "SQLScripts\\SearchAllTables.sql");
            string sql = File.ReadAllText(pathToSql);
            if (string.IsNullOrWhiteSpace(sql))
                return res;


            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                DynamicParameters parameters = new DynamicParameters();
                sql = sql.Replace("@SEARCHVALUE", value);
                IDataReader reader = context.Execute(sql, parameters);
                while (reader.Read())
                {
                    int fields = 3;
                    if (reader.FieldCount < 3)
                        fields = reader.FieldCount;

                    string foundRow = "";
                    for (int i = 0; i < fields; i++)
                    {
                        foundRow += reader[i]?.ToString();
                    }

                    res.Add(new Node()
                    {
                        UUID = reader["UUID"]?.ToString(),
                        Name = reader["Name"]?.ToString(),
                        AccountUUID = reader["AccountUUID"]?.ToString(),
                        SafeName = foundRow
                    });
                }
            }
            return res;
        }

        private List<Node> GetTableData(string table)
        {
            List<Node> tableData = new List<Node>();
            DynamicParameters parameters = new DynamicParameters();
            string sql = "SELECT * FROM " + table;


            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                IDataReader reader = context.Execute(sql, parameters);

                if (reader == null || reader.FieldCount == 0)
                    return tableData;

                try
                {
                    while (reader.Read())
                    {
                        Node res = new Node()
                        {
                            //Id = StringEx.ConvertTo<int>(reader["Id"].ToString()),//ParentId = StringEx.ConvertTo<int>(reader["ParentId"].ToString()),
                            UUID = reader["UUID"].ToString(),
                            Name = reader["Name"].ToString(),
                            SafeName = reader["Name"].ToString().ToSafeString(true)?.ToUpper()
                            //UUIDType = reader["UUIDType"].ToString(),//UUParentID = reader["UUParentID"].ToString(),
                            //UUParentIDType = reader["UUParentIDType"].ToString(),
                            //,Status = reader["Status"].ToString(),//AccountUUID = reader["AccountUUID"].ToString(),
                            //Active = StringEx.ConvertTo<bool>(reader["Active"].ToString()),//Deleted = StringEx.ConvertTo<bool>(reader["Deleted"].ToString()),
                            //Private = StringEx.ConvertTo<bool>(reader["Private"].ToString()),//SortOrder = StringEx.ConvertTo<int>(reader["SortOrder"].ToString()),
                            //DateCreated = StringEx.ConvertTo<DateTime>(reader["DateCreated"].ToString()),//CreatedBy = reader["CreatedBy"].ToString(),
                            //RoleWeight = StringEx.ConvertTo<int>(reader["RoleWeight"].ToString()),
                            //RoleOperation = reader["RoleOperation"].ToString(),//Image = reader["Image"].ToString()
                        };

                        tableData.Add(res);
                    }
                }
                catch (Exception ex)
                {
                    Debug.Assert(false, ex.Message);
                    _logger.InsertError(ex.Message, "AppManager", "GetDefaultData");
                }
                finally
                {
                    if (reader != null) { reader.Close(); }
                }
            }
            return tableData;
        }

        public void TestCode()
        {
            //    A.select from product where categoryuuid is booth,
            //B. if product name is not in accounts then add it to accounts
            //C.delete product
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                List<dynamic> products = context.GetAll<Product>().Where(w => w.CategoryUUID == "PRODUCT.BOOTH")
                    .Select(s => new {
                        Name = s.Name,
                        UUID = s.UUID,
                        SafeName = s.Name.ToSafeString(true)?.ToUpper()
                    }).Cast<dynamic>().ToList();


                foreach (dynamic p in products)
                {
                    dynamic account = context.GetAll<Account>().Where(w => w.Name.ToSafeString(true)?.ToUpper() == p.SafeName).FirstOrDefault();

                    if (account != null)
                    {
                        DynamicParameters param = new DynamicParameters();
                        param.Add("@UUID", p.UUID);
                        context.Delete<Product>("WHERE UUID=@UUID", param);
                        continue;//account exists move on.
                    }

                    try
                    {
                        context.Insert<Account>(new Account()
                        {
                            Name = p.Name,
                            UUID = p.UUID,
                            UUIDType = "Account",
                            CreatedBy = "system.default.account",
                            DateCreated = DateTime.UtcNow,
                            Active = true,
                            Breeder = false,
                            Deleted = false,
                            Private = true,
                            RoleWeight = 4,
                            RoleOperation = ">=",
                            SortOrder = 0
                        });
                    }
                    catch (Exception ex)
                    {
                        Debug.Assert(false, ex.Message);
                        continue;
                    }
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@UUID", p.UUID);
                    context.Delete<Product>("WHERE UUID=@UUID", parameters);
                }
            }
        }

        public ServiceResult DataTypes()
        {
            User currentUser = this.GetUser(_sessionKey);
            if (currentUser == null || !currentUser.SiteAdmin)
                return ServiceResponse.Error("Unauthorized access.");

            if (!this.DataAccessAuthorized(currentUser, "GET", false))
                return ServiceResponse.Error("Unauthorized access.");


            List<string> dataTypes;
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                dataTypes = context.GetDataTypes();
                if (dataTypes == null || dataTypes.Count == 0)
                {
                    context.LoadTableNames();
                    dataTypes = context.GetDataTypes();
                }
            }

            if (dataTypes == null)
                return ServiceResponse.Error("Unable to load table names. Check the log for details.");

            return ServiceResponse.OK("", dataTypes);
        }

        public ServiceResult CreateDatabase(AppInfo appSettings, string connectionString)
        {
            TreeMonDbContext appData = new TreeMonDbContext(true);
            appData.Configuration.AutoDetectChangesEnabled = false;

            return appData.CreateDatabase(appSettings, connectionString);

        }
        public ServiceResult CreateAccounts(AppInfo appSettings)
        {
            if (string.IsNullOrWhiteSpace(this._connectionKey))
                return ServiceResponse.Error("Connection key is not set.");

            // this._connectionKey = appSettings.ActiveDbConnectionKey;

            string accountUUID = Guid.NewGuid().ToString("N");

            //create account
            Account ua = new Account()
            {
                UUID = accountUUID,
                AccountUUID = accountUUID,
                UUIDType = "Account",
                AccountSource = appSettings.AccountName,
                Active = true,
                OwnerUUID = appSettings.UserName,
                OwnerType = "user.username",
                DateCreated = DateTime.UtcNow,
                Deleted = false,
                Email = appSettings.AccountEmail,
                Name = appSettings.AccountName,
                Private = true
            };

            using (var context = new TreeMonDbContext(this._connectionKey))
            {      // appSettings.ActiveDatabase, "");

                if (!context.Insert<Account>(ua))
                    return ServiceResponse.Error("Error inserting account.");
                //I.When installing website add location(so we can add items to inventory).the location will be the created account
                //a.name = domain user typed.  type = domain, default = true
                //b.to add current web site.name is domain name, custom type = domain, check box Set as default for current site.
                if (!context.Insert<Models.Geo.Location>(
                    new Models.Geo.Location()
                    {
                        DateCreated = DateTime.UtcNow,
                        Deleted = false,
                        RoleWeight = 4,
                        RoleOperation = ">=",
                        Virtual = true,

                        AccountUUID = ua.AccountUUID,
                        isDefault = true,
                        LocationType = "online store",//NOTE: by setting this to online store when a call to api/store is made it will be able to find the "location" and load the inventory for the store.
                                                      //if you don't want an online store by default then go to admin=> locations and change either the location type or deselect default or both.
                                                      //If you try to access api/store without these settings you'll get a Store location id could not be found. error
                        Name = appSettings.SiteDomain
                    }))
                {
                    return ServiceResponse.Error("Error inserting online store location.");
                }


                //Save the settings for this install first, incase it bombs, the
                //user won't need to retype the info.
                appSettings.UUParentID = ua.UUID;
                appSettings.UUParentIDType = "account";
                appSettings.DateCreated = DateTime.UtcNow;
                appSettings.AccountUUID = ua.UUID;

                if (!context.Insert<AppInfo>(appSettings))
                    return ServiceResponse.Error("Error inserting appSettings.");

                //if (!string.IsNullOrWhiteSpace(appSettings.ActiveDatabase))
                //    appSettings.ActiveDbPassword = Cipher.Crypt(appSettings.AppKey, appSettings.ActiveDbPassword, true);

                // appSettings.RunInstaller = false;//if you don't set this the client will keep redirecting to install page.

                //if (!appData.Insert<AppInfo>(appSettings))
                //    return ServiceResponse.Error("Error inserting application info.");

                //if (PasswordHash.CheckStrength(appSettings.UserPassword) < PasswordHash.PasswordScore.Strong)
                //    return ServiceResponse.Error("Your password is weak. Try again.");

                string tmpHashPassword = PasswordHash.CreateHash(appSettings.UserPassword);
                //create user
                User user = new User()
                {
                    UUIDType = "guid-N",
                    Name = appSettings.UserName,
                    Password = PasswordHash.ExtractHashPassword(tmpHashPassword),
                    PasswordSalt = PasswordHash.ExtractSalt(tmpHashPassword),
                    PasswordHashIterations = PasswordHash.ExtractIterations(tmpHashPassword),
                    PasswordAnswer = appSettings.UserSecurityAnswer,
                    Email = appSettings.UserEmail,
                    PasswordQuestion = appSettings.SecurityQuestion,

                    DateCreated = DateTime.UtcNow,
                    AccountUUID = ua.UUID,
                    SiteAdmin = true,//note this is only hard coded for here
                    Approved = true,//note this is only hard coded for here
                    Anonymous = false,
                    Banned = false,
                    LockedOut = false,

                    Private = true, // Since its a site admin we'll make it private  appSettings.UserIsPrivate,
                    LastActivityDate = DateTime.UtcNow,
                    FailedPasswordAnswerAttemptWindowStart = 0,
                    FailedPasswordAttemptCount = 0,
                    FailedPasswordAnswerAttemptCount = 0
                };

                if (!context.Insert<User>(user))
                    return ServiceResponse.Error("Failed to insert user.");

                ua.CreatedBy = user.UUID;

                if (context.Update<Account>(ua) <= 0)
                    return ServiceResponse.Error("Failed to update account creator.");

                AccountMember actMember = new AccountMember()
                {
                    AccountUUID = ua.UUID,
                    MemberUUID = user.UUID,
                    MemberType = "user",
                    Status = "active"
                };

                if (!context.Insert<AccountMember>(actMember))
                    return ServiceResponse.Error("Error adding user to account.");

                //This will create barebone roles and permissions.
                //Notice we're not using the globals object. Since this is an
                //install the provider may be different, so use the supplied provider.
                //
                RoleManager rm = new RoleManager(this._connectionKey, user);
                ServiceResult res = rm.InsertDefaults(ua.UUID, appSettings.AppType);
                if (res.Code != 200)
                    return res;
            }


            return ServiceResponse.OK("");
        }

        public ServiceResult Install(AppInfo appSettings)
        {
            if (string.IsNullOrWhiteSpace(appSettings.ActiveDbConnectionKey))
                return ServiceResponse.Error("Connection key is not set.");

            this.Installing = true;
            this._connectionKey = appSettings.ActiveDbConnectionKey;

            TreeMonDbContext appData = new TreeMonDbContext(true);
            appData.Configuration.AutoDetectChangesEnabled = false;


            string directory = EnvironmentEx.AppDataFolder;

            if (string.IsNullOrWhiteSpace(appSettings.AppKey))
                return ServiceResponse.Error("The appkey is not set in the config file. It must have a value to use the encrypt string.");

            ServiceResult re = ValidateInstallSettings(appSettings);
            if (re.Code != 200)
                return re;

            string connectionString = this.CreateConnectionString(appSettings);

            ServiceResult res = appData.CreateDatabase(appSettings, connectionString);

            if (re.Code != 200)
                return new ServiceResult() { Code = 500, Status = "ERROR", Message = res.Message };

            appSettings.ConfirmPassword = "";

            string accountUUID = Guid.NewGuid().ToString("N");

            //create account
            Account ua = new Account()
            {
                UUID = accountUUID,
                AccountUUID = accountUUID,
                UUIDType = "Account",
                AccountSource = appSettings.AccountName,
                Active = true,
                OwnerUUID = appSettings.UserName,
                OwnerType = "user.username",
                DateCreated = DateTime.UtcNow,
                Deleted = false,
                Email = appSettings.AccountEmail,
                Name = appSettings.AccountName,
                Private = true
            };

            if (!appData.Insert<Account>(ua))
                return ServiceResponse.Error("Error inserting account.");


            //I.When installing website add location(so we can add items to inventory).the location will be the created account
            //a.name = domain user typed.  type = domain, default = true
            //b.to add current web site.name is domain name, custom type = domain, check box Set as default for current site.
            LocationManager lm = new LocationManager(this._connectionKey, "");// appSettings.ActiveDatabase, "");
            lm.Insert(new Models.Geo.Location()
            {
                AccountUUID = ua.AccountUUID,
                isDefault = true,
                LocationType = "online store",//NOTE: by setting this to online store when a call to api/store is made it will be able to find the "location" and load the inventory for the store.
                                              //if you don't want an online store by default then go to admin=> locations and change either the location type or deselect default or both.
                                              //If you try to access api/store without these settings you'll get a Store location id could not be found. error
                Name = appSettings.SiteDomain
            });

            //Save the settings for this install first, incase it bombs, the
            //user won't need to retype the info.
            appSettings.UUParentID = ua.UUID;
            appSettings.UUParentIDType = "account";
            appSettings.DateCreated = DateTime.UtcNow;
            appSettings.AccountUUID = ua.UUID;

            if (!string.IsNullOrWhiteSpace(appSettings.ActiveDatabase))
                appSettings.ActiveDbPassword = Cipher.Crypt(appSettings.AppKey, appSettings.ActiveDbPassword, true);

            appSettings.RunInstaller = false;//if you don't set this the client will keep redirecting to install page.

            if (!appData.Insert<AppInfo>(appSettings))
                return ServiceResponse.Error("Error inserting application info.");

            if (PasswordHash.CheckStrength(appSettings.UserPassword) < PasswordHash.PasswordScore.Strong)
                return ServiceResponse.Error("Your password is weak. Try again.");

            string tmpHashPassword = PasswordHash.CreateHash(appSettings.UserPassword);
            //create user
            User user = new User()
            {
                UUIDType = "guid-N",
                Name = appSettings.UserName,
                Password = PasswordHash.ExtractHashPassword(tmpHashPassword),
                PasswordSalt = PasswordHash.ExtractSalt(tmpHashPassword),
                PasswordHashIterations = PasswordHash.ExtractIterations(tmpHashPassword),
                PasswordAnswer = appSettings.UserSecurityAnswer,
                Email = appSettings.UserEmail,
                PasswordQuestion = appSettings.SecurityQuestion,

                DateCreated = DateTime.UtcNow,
                AccountUUID = ua.UUID,
                SiteAdmin = true,//note this is only hard coded for here
                Approved = true,//note this is only hard coded for here
                Anonymous = false,
                Banned = false,
                LockedOut = false,

                Private = true, // Since its a site admin we'll make it private  appSettings.UserIsPrivate,
                LastActivityDate = DateTime.UtcNow,
                FailedPasswordAnswerAttemptWindowStart = 0,
                FailedPasswordAttemptCount = 0,
                FailedPasswordAnswerAttemptCount = 0
            };

            if (!appData.Insert<User>(user))
                return ServiceResponse.Error("Failed to insert user.");

            ua.CreatedBy = user.UUID;

            if (appData.Update<Account>(ua) <= 0)
                return ServiceResponse.Error("Failed to update account creator.");

            AccountMember actMember = new AccountMember()
            {
                AccountUUID = ua.UUID,
                MemberUUID = user.UUID,
                MemberType = "user",
                Status = "active"
            };

            if (!appData.Insert<AccountMember>(actMember))
                return ServiceResponse.Error("Error adding user to account.");

            //This will create barebone roles and permissions.
            //Notice we're not using the globals object. Since this is an
            //install the provider may be different, so use the supplied provider.
            //
            RoleManager rm = new RoleManager(appData.ConnectionKey, user);
            res = rm.InsertDefaults(ua.UUID, appSettings.AppType);
            if (res.Code != 200)
                return res;

            #region Seed Database  

            SeedDatabase(Path.Combine(directory, "Install\\SeedData\\"));
            #endregion

            //Delete install file so it doesn't re-run on next visit (or give us the option to run).
            string pathToCommands = Path.Combine(directory, "Install\\install.json");
            if (File.Exists(pathToCommands))
            {
                // File.Move(pathToCommands, pathToCommands + ".bak");
                File.Delete(pathToCommands);
            }
            //We need to make sure the file is gone before moving on because
            //the status flag may not get reset correctly to running.
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

            return ServiceResponse.OK("", user);
        }

        public ServiceResult TableNames()
        {
            User currentUser = this.GetUser(_sessionKey);
            if (currentUser == null || !currentUser.SiteAdmin)
                return ServiceResponse.Error("Unauthorized access.");

            if (!this.DataAccessAuthorized(currentUser, "GET", false))
                return ServiceResponse.Error("Unauthorized access.");


            List<string> tables;
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                tables = context.GetTableNames();
            }
            return ServiceResponse.OK("", tables);
        }
        public string AppType { get; set; }
        public List<string> AppCommands { get; set; }

        protected void LoadCommands(string pathToFile)
        {
            try
            {
                string line = "";
                StreamReader file = new StreamReader(pathToFile);

                while ((line = file.ReadLine()) != null)
                {
                    AppCommands.Add(line);
                }

                file.Close();

            }
            catch (Exception ex)
            {
                _logger.InsertError(ex.Message, "AppManager", "LoadCommands:" + pathToFile);
                Debug.Assert(false, ex.Message);
                return;
            }
        }



        /// <summary>
        /// NOTE: this populates the tables with data from the "App_Data\SeedData folder.
        /// Each folder is named after its corresponding table with a .json extension.
        /// The file(s) contain json encoded data.
        /// </summary>
        /// <param name="pathToFolder"></param>
        /// <returns></returns>
        public ServiceResult SeedDatabase(string pathToFolder)
        {
            ServiceResult res = new ServiceResult();
            res.Code = 200;
            List<string> tables = new List<string>();
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                //get table names from context
                //loop through table names add .json and try to parse the objects
                context.LoadTableNames();
                tables = context.GetTableNames();
            }

            foreach (string table in tables)
            {


                // bool useDefaultAccount = false;


                //if (table.Contains("Currency"))
                //    useDefaultAccount = true; 

                string pathToFile = Path.Combine(pathToFolder, table + ".json");
                if (!File.Exists(pathToFile))
                    continue;

                string type;
                using (var context = new TreeMonDbContext(this._connectionKey))
                {
                    type = context.GetTableType(table);

                }
                if (string.IsNullOrWhiteSpace(type))
                    continue;

                this.ImportFile(type, pathToFile, Path.GetFileName(pathToFile), "install.seed");

                //object tableObject = context.GetTableObject(table);

                //if (tableObject == null)
                //    continue;
                //string sqlSelect = "";
                //string sqlInsert = "";
                //string seedData = "";
                //int index = 0;
                //try
                //{
                //    seedData = File.ReadAllText(pathToFile);

                //    List<dynamic> dataItems = JsonConvert.DeserializeObject<List<dynamic>>(seedData);

                //    foreach (dynamic dataItem in dataItems)
                //    {
                //        index++;

                //        if (useDefaultAccount)
                //            dataItem.AccountUUID = SystemFlag.Default.Account;
                //        else
                //            dataItem.AccountUUID = accountUUID;

                //        dataItem.DateCreated = DateTime.UtcNow;
                //        //When importing the uuid/id can change per account, user etc., so we check the SyncKey since this should NOT change.
                //        // depricated.. OR Name='" + dataItem.Name?.ToString()?.Replace("'", "''") 
                //        //
                //        sqlSelect = GenerateSqlStatement("select", dataItem, "WHERE SyncKey='" + dataItem.SyncKey + "' AND AccountUUID='" + dataItem.AccountUUID + "'");
                //        sqlSelect = sqlSelect.Replace("SELECT *", "SELECT COUNT(*)");

                //        object count = context.ExecuteScalar(sqlSelect, null);
                //        if (count != null && ((int)count) > 0)
                //            continue;//do not insert if the id is already there.

                //        dataItem.RoleWeight = 1;
                //        dataItem.RoleOperation = ">=";

                //        sqlInsert = GenerateSqlStatement("insert", dataItem, "");
                //        if (context.ExecuteNonQuery(sqlInsert, null) == 0)
                //            res.Message += "insert failed for: " + dataItem.UUID + "<br/>";

                //    }
                //}
                //catch (Exception ex)
                //{
                //    _logger.InsertError(ex.Message, "AppManager", "SeedDatabase.tableObject:" + tableObject);
                //    res.Code = 500;
                //    res.Message += ex.Message + index.ToString() + "<br/>";
                //    continue;
                //}
            }
            return res;
        }
          


        /// <summary>
        ///  Some table data is required to be assinged to an account, during install the data is set to a default.
        /// this updates the data to show up in the account after install
        /// </summary>
        /// <param name="accountUUID"></param>
        public void SeedDataSetAccount(string accountUUID)
        {
            if (!this.Installing)
                return;

            using (var context = new TreeMonDbContext(this._connectionKey))
            {
               IEnumerable<Strain> strains =  context.GetAll<Strain>().Where(w => w.AccountUUID == SystemFlag.Default.Account);
                foreach (Strain s in strains)
                {
                    s.AccountUUID = accountUUID;
                    context.Update<Strain>(s);
                }
                IEnumerable<Category> cats = context.GetAll<Category>().Where(w => w.AccountUUID == SystemFlag.Default.Account);
                foreach (Category c in cats)
                {
                    c.AccountUUID = accountUUID;
                    context.Update<Category>(c);
                }

                IEnumerable<Product> prods = context.GetAll<Product>().Where(w => w.AccountUUID == SystemFlag.Default.Account);
                foreach (Product p in prods)
                {
                    p.AccountUUID = accountUUID;
                    context.Update<Product>(p);
                }

                IEnumerable<UnitOfMeasure> uoms = context.GetAll<UnitOfMeasure>().Where(w => w.AccountUUID == SystemFlag.Default.Account);
                foreach (UnitOfMeasure u in uoms)
                {
                   u.AccountUUID = accountUUID;
                    context.Update<UnitOfMeasure>(u);
                }
            }
        }

        public ServiceResult ImportFile(string type, string pathToFile, string fileName, string ipAddress, bool validate = true, bool validateGlobally = false)
        {
            User currentUser;

            if (!Installing)
            {
                currentUser = this.GetUser(SessionKey);

                if (currentUser == null || !currentUser.SiteAdmin)
                    return ServiceResponse.Error("Unauthorized access.");
            }

            if (string.IsNullOrWhiteSpace(fileName))
                return ServiceResponse.Error("FIle name is empty.");

            ServiceResult res; 

            try
            {
                string body = File.ReadAllText(pathToFile)?.Replace("\\","");

                if (string.IsNullOrWhiteSpace(body))
                    ServiceResponse.Error("Content is empty for file " + fileName);

                string extension =  Path.GetExtension(fileName)?.ToUpper();
                extension = StringEx.ToSafeString(extension, true);

                if (string.IsNullOrWhiteSpace(extension))
                    return ServiceResponse.Error("Unsupported file type " + extension);

                switch (extension) {
                    case "JSON":
                        res = this.ImportJson(type, body, validate,validateGlobally);
                        break;
                    case "CSV":
                        res = ServiceResponse.Error("File type .csv not implemented.");
                        break;
                    default:
                        res = ServiceResponse.Error("File type " + extension + " not supported.");
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.InsertError(ex.Message, "AppManager", "ImportFile" + type + " " + ipAddress);
                return ServiceResponse.Error( "Failed to import " + fileName );
            }

            if (res.Code == 200)
                return ServiceResponse.OK("Imported file " + fileName + Environment.NewLine + res.Message);

            return res;
        }

        private ServiceResult ImportJson(string type, string content, bool validate, bool validateGlobally)
        {
            User currentUser;
            string createdBy = SystemFlag.Default.Account;
            string accountUUID = SystemFlag.Default.Account;

            if (!this.Installing)
            {
                currentUser = this.GetUser(SessionKey);
                createdBy = currentUser.UUID;
                accountUUID = currentUser.AccountUUID;
            }

            string tableName;
            object tableObject;
            using (var tmpcontext = new TreeMonDbContext(this._connectionKey))
            {
                tableName = tmpcontext.GetTableName(type);

                if (string.IsNullOrWhiteSpace(tableName))
                    return ServiceResponse.Error("Could not find the table name for the type:" + type);

                 tableObject = tmpcontext.GetTableObject(tableName);
            }
            if (tableObject == null)
                return ServiceResponse.Error("Could not find the table for the table name:" + tableName);

            string msg = "";

            switch (tableObject.GetType().Name.ToUpper())
            {
                case "ACCOUNT":
                    #region
                    List<Account> newAccounts = new List<Account>();
                    List<Account> accounts = JsonConvert.DeserializeObject<List<Account>>(content);
                    using (var context = new TreeMonDbContext(this._connectionKey))
                    {
                        foreach (Account account in accounts)
                        {
                            //If the sync key doesn't exist as uuid then it hasn't been imported during the install.
                            if (context.GetAll<Account>().FirstOrDefault(x => x.UUID == account.SyncKey  ) == null 
                                &&  string.IsNullOrWhiteSpace(account.SyncKey) == false)
                                account.UUID = account.SyncKey;

                            //account is different because it's the tabe record that splits the rest of the records into accounts so it's all global in accounts.
                            if (context.GetAll<Account>().FirstOrDefault(x =>  x.Name.EqualsIgnoreCase(account.Name)   ) != null  )
                            {
                                msg += "Account already exists:" + account.Name + Environment.NewLine;
                                continue;
                            }
                            account.CreatedBy = createdBy;
                            if (string.IsNullOrWhiteSpace(account.AccountUUID))
                                account.AccountUUID = accountUUID;

                            account.DateCreated = DateTime.UtcNow;
                            newAccounts.Add(account);
                        }
                    }
                    try
                    {
                        using (var scope = new TransactionScope())
                        using (var context = new TreeMonDbContext(this._connectionKey))
                        {
                            context.Configuration.AutoDetectChangesEnabled = false;
                            context.AddRange<Account>(newAccounts);
                            context.SaveChanges();
                            scope.Complete();
                            context.Configuration.AutoDetectChangesEnabled = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        return ServiceResponse.Error(ex.Message);
                    }

                    break;
                    #endregion
                case "ANATOMY":
                    #region
                    List<Anatomy> newAnatomies = new List<Anatomy>();
                    List<Anatomy> anatomies = JsonConvert.DeserializeObject<List<Anatomy>>(content);
                    
                    using (var context = new TreeMonDbContext(this._connectionKey))
                    {
                        foreach (Anatomy anatomy in anatomies)
                        {
                            //If the sync key doesn't exist as uuid then it hasn't been imported during the install.
                            if (context.GetAll<Anatomy>().FirstOrDefault(x => x.UUID == anatomy.SyncKey ) == null && string.IsNullOrWhiteSpace(anatomy.SyncKey) == false)
                                anatomy.UUID = anatomy.SyncKey;

                            if (validate)
                            {
                                if (context.GetAll<Anatomy>().FirstOrDefault(x => x.Name.EqualsIgnoreCase(anatomy.Name) && x.AccountUUID == accountUUID) != null)
                                {
                                    msg += "Anatomy already exists:" + anatomy.Name + Environment.NewLine;
                                    continue;
                                }
                            }
                            else if (validateGlobally)
                            {
                                if (context.GetAll<Anatomy>().FirstOrDefault(x => x.Name.EqualsIgnoreCase(anatomy.Name)) != null)
                                {
                                    msg += "Anatomy already exists globally:" + anatomy.Name + Environment.NewLine;
                                    continue;
                                }
                            }
                            anatomy.CreatedBy = createdBy;
                            if (string.IsNullOrWhiteSpace(anatomy.AccountUUID))
                                anatomy.AccountUUID = accountUUID;

                            anatomy.DateCreated = DateTime.UtcNow;
                            newAnatomies.Add(anatomy);
                        }
                    }
                    try
                    {
                        using (var scope = new TransactionScope())
                        using (var context = new TreeMonDbContext(this._connectionKey))
                        {
                            context.Configuration.AutoDetectChangesEnabled = false;
                            context.AddRange<Anatomy>(newAnatomies);
                            context.SaveChanges();
                            scope.Complete();
                            context.Configuration.AutoDetectChangesEnabled = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        return ServiceResponse.Error(ex.Message);
                    }

                    break;
                    #endregion
                case "ANATOMYTAG":
                    #region
                    List<AnatomyTag> newAnatomyTags = new List<AnatomyTag>();
                    List<AnatomyTag> anatomyTags = JsonConvert.DeserializeObject<List<AnatomyTag>>(content);
                    using (var context = new TreeMonDbContext(this._connectionKey))
                    {
                        foreach (AnatomyTag AnatomyTag in anatomyTags)
                        {
                            //If the sync key doesn't exist as uuid then it hasn't been imported during the install.
                            if (context.GetAll<AnatomyTag>().FirstOrDefault(x => x.UUID == AnatomyTag.SyncKey  ) == null
                                 && string.IsNullOrWhiteSpace(AnatomyTag.SyncKey) == false)
                                AnatomyTag.UUID = AnatomyTag.SyncKey;

                            if (validate)
                            {
                                if (context.GetAll<AnatomyTag>().FirstOrDefault(x => x.Name.EqualsIgnoreCase(AnatomyTag.Name) && x.AccountUUID == accountUUID) != null)
                                {
                                    msg += "AnatomyTag already exists:" + AnatomyTag.Name + Environment.NewLine;
                                    continue;
                                }
                            }
                            else if (validateGlobally)
                            {
                                if (context.GetAll<AnatomyTag>().FirstOrDefault(x => x.Name.EqualsIgnoreCase(AnatomyTag.Name)) != null)
                                {
                                    msg += "AnatomyTag already exists globally:" + AnatomyTag.Name + Environment.NewLine;
                                    continue;
                                }
                            }
                           
                            AnatomyTag.CreatedBy = createdBy;
                            if (string.IsNullOrWhiteSpace(AnatomyTag.AccountUUID))
                                AnatomyTag.AccountUUID = accountUUID;

                            AnatomyTag.DateCreated = DateTime.UtcNow;
                            newAnatomyTags.Add(AnatomyTag);
                        }
                    }
                    try
                    {
                        using (var scope = new TransactionScope())
                        using (var context = new TreeMonDbContext(this._connectionKey))
                        {
                            context.Configuration.AutoDetectChangesEnabled = false;
                            context.AddRange<AnatomyTag>(newAnatomyTags);
                            context.SaveChanges();
                            scope.Complete();
                            context.Configuration.AutoDetectChangesEnabled = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        return ServiceResponse.Error(ex.Message);
                    }
                    break;
                    #endregion
                case "CATEGORY":
                    #region
                    List<Category> newCategories = new List<Category>();
                    List<Category> categories = JsonConvert.DeserializeObject<List<Category>>(content);
                    using (var context = new TreeMonDbContext(this._connectionKey))
                    {
                        foreach (Category category in categories)
                        {
                            //If the sync key doesn't exist as uuid then it hasn't been imported during the install.
                            if (context.GetAll<Category>().FirstOrDefault(x => x.UUID == category.SyncKey  ) == null
                                 && string.IsNullOrWhiteSpace(category.SyncKey) == false)
                                category.UUID = category.SyncKey;

                            if (validate)
                            {
                                if (context.GetAll<Category>().FirstOrDefault(x => x.Name.EqualsIgnoreCase(category.Name) && x.AccountUUID == accountUUID) != null)
                                {
                                    msg += "Category already exists:" + category.Name + Environment.NewLine;
                                    continue;
                                }
                            }
                            else if (validateGlobally)
                            {
                                if (context.GetAll<Category>().FirstOrDefault(x => x.Name.EqualsIgnoreCase(category.Name)) != null)
                                {
                                    msg += "Category already exists globally:" + category.Name + Environment.NewLine;
                                    continue;
                                }
                            }
                            
                            category.CreatedBy = createdBy;
                            if (string.IsNullOrWhiteSpace(category.AccountUUID))
                                category.AccountUUID = accountUUID;
                            category.DateCreated = DateTime.UtcNow;
                            newCategories.Add(category);
                        }
                    }
                    try
                    {
                        using (var scope = new TransactionScope())
                        using (var context = new TreeMonDbContext(this._connectionKey))
                        {
                            context.Configuration.AutoDetectChangesEnabled = false;
                            context.AddRange<Category>(newCategories);
                            context.SaveChanges();
                            scope.Complete();
                            context.Configuration.AutoDetectChangesEnabled = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        return ServiceResponse.Error(ex.Message);
                    }
                    break;
                    #endregion
                case "CURRENCY":
                    #region
                    List<Currency> newCurrencies = new List<Currency>();
                    List<Currency> currencies = JsonConvert.DeserializeObject<List<Currency>>(content);
                    using (var context = new TreeMonDbContext(this._connectionKey))
                    {
                        foreach (Currency currency in currencies)
                        {
                            //If the sync key doesn't exist as uuid then it hasn't been imported during the install.
                            if (context.GetAll<Currency>().FirstOrDefault(x => x.UUID == currency.SyncKey ) == null
                                   && string.IsNullOrWhiteSpace(currency.SyncKey) == false)
                                currency.UUID = currency.SyncKey;


                            if (validate)
                            {
                                if (context.GetAll<Currency>().FirstOrDefault(x => x.Name.EqualsIgnoreCase(currency.Name) && x.AccountUUID == accountUUID) != null)
                                {
                                    msg += "Currency already exists:" + currency.Name + Environment.NewLine;
                                    continue;
                                }
                            }
                            else if (validateGlobally)
                            {
                                if (context.GetAll<Currency>().FirstOrDefault(x => x.Name.EqualsIgnoreCase(currency.Name)) != null)
                                {
                                    msg += "Currency already exists globally:" + currency.Name + Environment.NewLine;
                                    continue;
                                }
                            }

                         
                            currency.CreatedBy = createdBy;
                            if (string.IsNullOrWhiteSpace(currency.AccountUUID))
                                currency.AccountUUID = accountUUID;
                            currency.DateCreated = DateTime.UtcNow;
                            newCurrencies.Add(currency);
                        }
                    }
                    try
                    {
                        using (var scope = new TransactionScope())
                        using (var context = new TreeMonDbContext(this._connectionKey))
                        {
                            context.Configuration.AutoDetectChangesEnabled = false;
                            context.AddRange<Currency>(newCurrencies);
                            context.SaveChanges();
                            scope.Complete();
                            context.Configuration.AutoDetectChangesEnabled = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        return ServiceResponse.Error(ex.Message);
                    }
                    break;
                #endregion
                case "LOCATION":
                    #region
                    List<Location> newLocations = new List<Location>();
                    List<Location> locations = JsonConvert.DeserializeObject<List<Location>>(content);
                    using (var context = new TreeMonDbContext(this._connectionKey))
                    {
                        context.Configuration.AutoDetectChangesEnabled = false;

                        foreach (Location loc in locations)
                        {
                            //If the sync key doesn't exist as uuid then it hasn't been imported during the install.
                            if (context.GetAll<Location>().FirstOrDefault(x => x.UUID == loc.SyncKey  ) == null
                                  && string.IsNullOrWhiteSpace(loc.SyncKey) == false)
                                loc.UUID = loc.SyncKey;


                            if (validate)
                            {
                                if (context.GetAll<Location>().FirstOrDefault(x => x.Name.EqualsIgnoreCase(loc.Name) && x.AccountUUID == accountUUID) != null)
                                {
                                    msg += "Location already exists:" + loc.Name + Environment.NewLine;
                                    continue;
                                }
                            }
                            else if (validateGlobally)
                            {
                                if (context.GetAll<Location>().FirstOrDefault(x => x.Name.EqualsIgnoreCase(loc.Name)) != null)
                                {
                                    msg += "Location already exists globally:" + loc.Name + Environment.NewLine;
                                    continue;
                                }
                            }
                          
                            loc.CreatedBy = createdBy;
                            if (string.IsNullOrWhiteSpace(loc.AccountUUID))
                                loc.AccountUUID = accountUUID;
                            loc.DateCreated = DateTime.UtcNow;
                            newLocations.Add(loc);
                        }
                    }
                    try
                    {
                         using (var scope = new TransactionScope())
                        using (var context = new TreeMonDbContext(this._connectionKey))
                        {
                            context.Configuration.AutoDetectChangesEnabled = false;
                            context.AddRange<Location>(newLocations);
                            context.SaveChanges();
                                scope.Complete();
                                context.Configuration.AutoDetectChangesEnabled = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        return ServiceResponse.Error(ex.Message);
                    }

                    break;
                    #endregion
                case "PRODUCT":
                    #region
                    List<Product> newProducts = new List<Product>();
                    List<Product> products = JsonConvert.DeserializeObject<List<Product>>(content);
                    using (var context = new TreeMonDbContext(this._connectionKey))
                    {
                        foreach (Product Product in products)
                        {
                            //If the sync key doesn't exist as uuid then it hasn't been imported during the install.
                            if (context.GetAll<Product>().FirstOrDefault(x => x.UUID == Product.SyncKey) == null
                                  && string.IsNullOrWhiteSpace(Product.SyncKey) == false)
                                Product.UUID = Product.SyncKey;

                            if (validate)
                            {
                                if (context.GetAll<Product>().FirstOrDefault(x => x.Name.EqualsIgnoreCase(Product.Name) && x.AccountUUID == accountUUID) != null)
                                {
                                    msg += "Product already exists:" + Product.Name + Environment.NewLine;
                                    continue;
                                }
                            }
                            else if (validateGlobally &&
                                    context.GetAll<Product>().FirstOrDefault(x => x.Name.EqualsIgnoreCase(Product.Name)) != null)
                            {
                                msg += "Product already exists globally:" + Product.Name + Environment.NewLine;
                                continue;
                            }

                            if (string.IsNullOrWhiteSpace(Product.CategoryUUID) || Product.CategoryUUID == "NULL")
                                Product.CategoryUUID = "PRODUCT.MISC";

                            Product.CategoryUUID = Product.CategoryUUID.Trim();
                            Product.CreatedBy =createdBy;
                            if (string.IsNullOrWhiteSpace(Product.AccountUUID))
                                Product.AccountUUID = accountUUID;
                            Product.DateCreated = DateTime.UtcNow;
                            newProducts.Add(Product);
                        }
                    }
                    try
                    {
                        using (var scope = new TransactionScope())
                        using (var context = new TreeMonDbContext(this._connectionKey))
                        {
                            context.Configuration.AutoDetectChangesEnabled = false;
                            context.AddRange<Product>(newProducts);
                            context.SaveChanges();
                            scope.Complete();
                            context.Configuration.AutoDetectChangesEnabled = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        return ServiceResponse.Error(ex.Message);
                    }
                    break;
                    #endregion
                case "STRAIN":
                    #region
                    List<Strain> newStrains = new List<Strain>();
                    List<Strain> Strains = JsonConvert.DeserializeObject<List<Strain>>(content);
                    using (var context = new TreeMonDbContext(this._connectionKey))
                    {
                        foreach (Strain Strain in Strains)
                        {
                            //If the sync key doesn't exist as uuid then it hasn't been imported during the install.
                            if (context.GetAll<Strain>().FirstOrDefault(x => x.UUID == Strain.SyncKey ) == null
                                  && string.IsNullOrWhiteSpace(Strain.SyncKey) == false)
                                Strain.UUID = Strain.SyncKey;

                            if (validate)
                            {
                                if (context.GetAll<Strain>().FirstOrDefault(x => x.Name.EqualsIgnoreCase(Strain.Name) && x.AccountUUID == accountUUID) != null)
                                {
                                    msg += "Strain already exists:" + Strain.Name + Environment.NewLine;
                                    continue;
                                }
                            }
                            else if (validateGlobally)
                            {
                                if (context.GetAll<Strain>().FirstOrDefault(x => x.Name.EqualsIgnoreCase(Strain.Name)) != null)
                                {
                                    msg += "Strain already exists globally:" + Strain.Name + Environment.NewLine;
                                    continue;
                                }
                            }
                            if ( string.IsNullOrWhiteSpace(Strain.CategoryUUID ) || Strain.CategoryUUID == "NULL" )
                                Strain.CategoryUUID = "VARIETY.UNASSIGNED";

                            Strain.CategoryUUID = Strain.CategoryUUID.Trim();
                            Strain.CreatedBy = createdBy;
                            if (string.IsNullOrWhiteSpace(Strain.AccountUUID))
                                Strain.AccountUUID = accountUUID;
                            Strain.DateCreated = DateTime.UtcNow;
                            newStrains.Add(Strain);
                        }
                    }
                    try
                    {
                        using (var scope = new TransactionScope())
                        using (var context = new TreeMonDbContext(this._connectionKey))
                        {
                            context.Configuration.AutoDetectChangesEnabled = false;
                            context.AddRange<Strain>(newStrains);
                            context.SaveChanges();
                            scope.Complete();
                            context.Configuration.AutoDetectChangesEnabled = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        return ServiceResponse.Error(ex.Message);
                    }
                    break;
                #endregion
                case "UNITOFMEASURE":
                    #region
                    List<UnitOfMeasure> newUnitsOfMeasure = new List<UnitOfMeasure>();
                    List<UnitOfMeasure> unitsOfMeasure = JsonConvert.DeserializeObject<List<UnitOfMeasure>>(content);
                    using (var context = new TreeMonDbContext(this._connectionKey))
                    {
                        foreach (UnitOfMeasure uom in unitsOfMeasure)
                        {
                            //If the sync key doesn't exist as uuid then it hasn't been imported during the install.
                            if (context.GetAll<UnitOfMeasure>().FirstOrDefault(x => x.UUID == uom.SyncKey ) == null
                                  && string.IsNullOrWhiteSpace(uom.SyncKey) == false)
                                uom.UUID = uom.SyncKey;

                            if (validate)
                            {
                                if (context.GetAll<UnitOfMeasure>().FirstOrDefault(x => x.Name.EqualsIgnoreCase(uom.Name) && x.AccountUUID == accountUUID) != null)
                                {
                                    msg += "Unit of measure already exists:" + uom.Name + Environment.NewLine;
                                    continue;
                                }
                            }
                            else if (validateGlobally)
                            {
                                if (context.GetAll<UnitOfMeasure>().FirstOrDefault(x => x.Name.EqualsIgnoreCase(uom.Name)) != null)
                                {
                                    msg += "Unit of measure already exists globally:" + uom.Name + Environment.NewLine;
                                    continue;
                                }
                            }
                           
                            uom.CreatedBy = createdBy;
                            if (string.IsNullOrWhiteSpace(uom.AccountUUID))
                                uom.AccountUUID = accountUUID;
                            uom.DateCreated = DateTime.UtcNow;
                            newUnitsOfMeasure.Add(uom);
                        }
                    }
                    try
                    {
                        using (var scope = new TransactionScope())
                        using (var context = new TreeMonDbContext(this._connectionKey))
                        {
                            context.Configuration.AutoDetectChangesEnabled = false;
                            context.AddRange<UnitOfMeasure>(newUnitsOfMeasure);
                            context.SaveChanges();
                            scope.Complete();
                            context.Configuration.AutoDetectChangesEnabled = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        return ServiceResponse.Error(ex.Message);
                    }
                    break;
                    #endregion
                case "VENDOR":
                    #region
                    List<Vendor> newVendors = new List<Vendor>();
                    List<Vendor> Vendors = JsonConvert.DeserializeObject<List<Vendor>>(content);
                    using (var context = new TreeMonDbContext(this._connectionKey))
                    {
                        foreach (Vendor Vendor in Vendors)
                        {
                            //If the sync key doesn't exist as uuid then it hasn't been imported during the install.
                            if (context.GetAll<Vendor>().FirstOrDefault(x => x.UUID == Vendor.SyncKey ) == null
                                  && string.IsNullOrWhiteSpace(Vendor.SyncKey) == false)
                                Vendor.UUID = Vendor.SyncKey;

                            if (validate)
                            {
                                if (context.GetAll<Vendor>().FirstOrDefault(x => x.Name.EqualsIgnoreCase(Vendor.Name) && x.AccountUUID == accountUUID) != null)
                                {
                                   msg += "Vendor already exists:" + Vendor.Name + Environment.NewLine;
                                    continue;
                                }
                            }
                            else if (validateGlobally && context.GetAll<Vendor>().FirstOrDefault(x => x.Name.EqualsIgnoreCase(Vendor.Name)) != null)
                            {
                                msg += "Vendor already exists globally:" + Vendor.Name + Environment.NewLine;
                                    continue;
                            }
                            
                            Vendor.CreatedBy = createdBy;
                            if (string.IsNullOrWhiteSpace(Vendor.AccountUUID))
                                Vendor.AccountUUID = accountUUID;
                            Vendor.DateCreated = DateTime.UtcNow;
                            newVendors.Add(Vendor);
                        }
                    }
                    try
                    {
                        using (var scope = new TransactionScope())
                        using (var context = new TreeMonDbContext(this._connectionKey))
                        {
                            context.Configuration.AutoDetectChangesEnabled = false;
                            context.AddRange<Vendor>(newVendors);
                            context.SaveChanges();
                            scope.Complete();
                            context.Configuration.AutoDetectChangesEnabled = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        return ServiceResponse.Error(ex.Message);
                    }
                    break;
                    #endregion
                default:
                    return ServiceResponse.Error("Import for type:" + type + " not supported yet.");
            }
            return ServiceResponse.OK(msg);
        }


        public ServiceResult ImportData(string type )
        {

            string fileName = type + ".json";
           
            object tableObject = new TreeMonDbContext(this._connectionKey).GetTableObject(type);

            if (tableObject == null)
                return ServiceResponse.Error("Table lookup for file wasn't found.");

            string pathToFile = Path.Combine(EnvironmentEx.AppDataFolder, "Install\\SeedData\\", fileName);

            if (!File.Exists(pathToFile))
                return ServiceResponse.Error("File not found.");

           string seedData = File.ReadAllText(pathToFile);

            this.ImportJson(type, seedData,true, false);
          
            return ServiceResponse.OK();
        }


        public ServiceResult ImportItem(INode n)
        {
            ServiceResult res;

            User user =  this.GetUser(this.SessionKey);

            if (user == null)
                return ServiceResponse.Error("User for session could not be found.");

            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                switch (n.UUIDType?.ToUpper())
                {
                    case "ACCOUNT":
                        AccountManager accountManager = new AccountManager(this._connectionKey, SessionKey);
                        Account a = (Account)accountManager.GetBy(n.UUID);
                        if (a == null)
                            return ServiceResponse.Error("Account was not found.");

                        a.UUID = Guid.NewGuid().ToString("N");
                        a.CreatedBy = SystemFlag.Default.Account;
                        a.DateCreated = DateTime.UtcNow;
                        a.AccountUUID = user.AccountUUID;
                        res = accountManager.Insert(a, false);

                    break;

                    case "ANATOMY":
                        AnatomyManager AnatomyManager = new AnatomyManager(this._connectionKey, SessionKey);
                        Anatomy anatomy = (Anatomy) AnatomyManager.GetBy(n.UUID);
                        if (anatomy == null)
                            return ServiceResponse.Error("Anatomy was not found.");

                        anatomy.UUID = Guid.NewGuid().ToString("N");
                        anatomy.CreatedBy = SystemFlag.Default.Account;
                        anatomy.DateCreated = DateTime.UtcNow;
                        anatomy.AccountUUID = user.AccountUUID;
                        res = AnatomyManager.Insert(anatomy, false);
                        break;

                    case "AnatomyTag":
                        AnatomyManager AnatomyTagManager = new AnatomyManager(this._connectionKey, SessionKey);
                        AnatomyTag AnatomyTag = AnatomyTagManager.GetAnatomyTagBy(n.UUID);
                        if (AnatomyTag == null)
                            return ServiceResponse.Error("Anatomy was not found.");
                        AnatomyTag.UUID = Guid.NewGuid().ToString("N");
                        AnatomyTag.CreatedBy = SystemFlag.Default.Account;
                        AnatomyTag.DateCreated = DateTime.UtcNow;
                        AnatomyTag.AccountUUID = user.AccountUUID;
                        res = AnatomyTagManager.Insert(AnatomyTag, false);
                        break;
                    case "CATEGORY":
                        CategoryManager categoryManager = new CategoryManager(this._connectionKey, SessionKey);
                        Category category = (Category)categoryManager.GetBy(n.UUID);
                        if (category == null)
                            return ServiceResponse.Error("Category was not found.");
                        category.UUID = Guid.NewGuid().ToString("N");
                        category.CreatedBy = SystemFlag.Default.Account;
                        category.DateCreated = DateTime.UtcNow;
                        category.AccountUUID = user.AccountUUID;
                        res = categoryManager.Insert(category, false);
                        break;
                    case "CURRENCY":
                        CurrencyManager financeManager = new CurrencyManager(this._connectionKey, SessionKey);
                        Currency currency = (Currency)financeManager.GetBy(n.UUID);
                        if (currency == null)
                            return ServiceResponse.Error("Currency was not found.");

                        currency.UUID = Guid.NewGuid().ToString("N");
                        currency.CreatedBy = SystemFlag.Default.Account;
                        currency.DateCreated = DateTime.UtcNow;
                        currency.AccountUUID = user.AccountUUID;
                        res = financeManager.Insert(currency, false);
                        break;
                    case "PRODUCT":
                        ProductManager productManager = new ProductManager(this._connectionKey, SessionKey);
                        Product product = (Product)productManager.GetBy(n.UUID);
                        if (product == null)
                            return ServiceResponse.Error("Product was not found.");
                        product.UUID = Guid.NewGuid().ToString("N");
                        product.CreatedBy = SystemFlag.Default.Account;
                        product.DateCreated = DateTime.UtcNow;
                        product.AccountUUID = user.AccountUUID;
                        res = productManager.Insert(product, false);
                        break;
                    case "STRAIN":
                        StrainManager strainManager = new StrainManager(this._connectionKey, SessionKey);
                        Strain strain = (Strain)strainManager.GetBy(n.UUID);
                        if (strain == null)
                            return ServiceResponse.Error("Product was not found.");

                        strain.UUID = Guid.NewGuid().ToString("N");
                        strain.CreatedBy = SystemFlag.Default.Account;
                        strain.DateCreated = DateTime.UtcNow;
                        strain.AccountUUID = user.AccountUUID;
                        res = strainManager.Insert(strain, false);
                        break;
                    case "VENDOR":
                        VendorManager VendorManager = new VendorManager(this._connectionKey, SessionKey);
                        Vendor Vendor = (Vendor)VendorManager.GetBy(n.UUID);
                        if (Vendor == null)
                            return ServiceResponse.Error("Product was not found.");
                        Vendor.UUID = Guid.NewGuid().ToString("N");
                        Vendor.CreatedBy = SystemFlag.Default.Account;
                        Vendor.DateCreated = DateTime.UtcNow;
                        Vendor.AccountUUID = user.AccountUUID;
                        res = VendorManager.Insert(Vendor,false);
                        break;
                    default:
                        return ServiceResponse.Error("Invalid type.");
                }
            }
        
            return res;
        }

        public ServiceResult Delete(string table, string UUID)
        {
            User currentUser = this.GetUser(SessionKey);

            if (currentUser == null || !currentUser.SiteAdmin)
                return ServiceResponse.Error("Unauthorized access.");


            if (string.IsNullOrWhiteSpace(table))
                return ServiceResponse.Error("You must send a tablename.");

            if (string.IsNullOrWhiteSpace(UUID))
                return ServiceResponse.Error("You must send a UUID.");

            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@UUIDPARAM", UUID);
                int count = context.ExecuteNonQuery("DELETE FROM " + table + " WHERE UUID=@UUIDPARAM", parameters);
                if (count > 0)
                    return ServiceResponse.OK();

               
            }
            return ServiceResponse.Error("Failed to delete record.");
    }

        /// <summary>
        /// NOTE: This has not been tested thoroughly.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="dataObject"></param>
        /// <param name="whereStatement"></param>
        /// <returns></returns>
        private string GenerateSqlStatement( string command,  dynamic dataObject , string whereStatement) 
        {
            if (string.IsNullOrWhiteSpace(command))
                return command;

            string tableName = new TreeMonDbContext(this._connectionKey).GetTableName((string)dataObject.UUIDType);

            string sqlCommand = command;
            switch (command.ToUpper())
            {
                case "INSERT":
                    sqlCommand = command.ToUpper() + " INTO " + tableName + " ( {0} ) VALUES( {1} )";
                    break;
                case "SELECT":
                    sqlCommand = command.ToUpper() + " * FROM " + tableName;
                    break;
            }


            JObject attributesAsJObject = dataObject;
            Dictionary<string, string> objectValues = attributesAsJObject.ToObject<Dictionary<string, string>>();

            string properties = objectValues.Select(s => s.Key).Aggregate((current, next) => current + "," + next);
            string values = "'" +  objectValues.Select(s => s.Value).Aggregate((current, next) => current + "','" + next?.Trim()?.Replace("'", "''"));
            values += "'";

            sqlCommand = string.Format(sqlCommand, properties, values) + " " + whereStatement;

            if ( (!sqlCommand.Contains(",UUID,") && !sqlCommand.Contains(" UUID,")) && command.EqualsIgnoreCase("INSERT" ) )
            {
                if (!sqlCommand.Contains("AccountUUID"))
                {
                    sqlCommand = sqlCommand.Replace(tableName + " (", tableName + " ( UUID,AccountUUID,").Replace("VALUES( ", "VALUES( '" + Guid.NewGuid().ToString("N") + "','" + dataObject.AccountUUID + "',");
                }
                else //just add the UUID
                {
                    sqlCommand = sqlCommand.Replace(tableName + " (", tableName + " ( UUID,").Replace("VALUES( ", "VALUES( '" + Guid.NewGuid().ToString("N") + "',");
                }
            }
       
            return sqlCommand;
        }

        public ServiceResult ValidateInstallSettings(AppInfo appSettings)
        {
            StringBuilder msg = new StringBuilder();

            if (string.IsNullOrWhiteSpace(appSettings.SiteDomain))
               msg.Append("Site domain name not set.");

            if (string.IsNullOrWhiteSpace(appSettings.ActiveDbProvider))//      < add key = "DefaultDbConnection" value = "mssql" />
                msg.Append( "Missing Database provider!" );

            if (this.GetSetting("DefaultDbConnection" )== null)
                 msg.Append("Missing DefaultDbConnection");

            if (this.GetSetting("AppKey" )== null)
                 msg.Append("Missing AppKey ");

            if (this.GetSetting("AddRequestPermissions") == null)
                msg.Append("Missing AddRequestPermissions");

            if (this.GetSetting("SiteDomain") == null)
                 msg.Append("Missing SiteDomain");

            if (this.GetSetting("ApiVersion") == null)
                 msg.Append("Missing ApiVersion");

            if (this.GetSetting("ClientValidationEnabled") == null)
                 msg.Append("Missing ClientValidationEnabled");

            if (this.GetSetting("UseDatabaseConfig") == null)
                 msg.Append("Missing UseDatabaseConfig");

            if (this.GetSetting("DBBackupKey") == null)
                 msg.Append("Missing DBBackupKey");

            if (this.GetSetting("SessionLength") == null)
                   msg.Append("Missing SessionLength");

            if (this.GetSetting("TemplateEmailNewMember") == null)
                 msg.Append("Missing TemplateEmailNewMember");

            if (this.GetSetting("TemplatePasswordResetEmail" )== null)
                 msg.Append("Missing TemplatePasswordResetEmail");

            if (this.GetSetting("TemplateUserInfoEmail") == null)
                 msg.Append("Missing TemplateUserInfoEmail");

            if (this.GetSetting("EmailStoreTemplateOrderStatusReceived") == null)
                 msg.Append("Missing EmailStoreTemplateOrderStatusReceived");

            if (this.GetSetting("SiteAdmins") == null)
                 msg.Append("Missing SiteAdmins");

            if (this.GetSetting("LocalSqlServer") == null)
                 msg.Append("Missing LocalSqlServer");

            Setting s = this.GetSetting("DefaultDbConnection");//this points to the connectionstring
            if (this.GetSetting(s.Value) == null)//get the connection string and make sure there's a value.
                 msg.Append("Missing Default database connection string.");

            if (this.GetSetting("ApiStatus") == null)
                msg.Append("Missing ApiStatus");


            if (msg.Length > 0)
                return ServiceResponse.Error(msg.ToString());

            return new ServiceResult() { Code = 200, Status = "OK" };
        }

        public AppInfo GetAppInfo(string accountUUID, string appType)
        {
            AppInfo app;

            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                app = context.GetAll<AppInfo>().FirstOrDefault(aw => aw.UUParentID == accountUUID && aw.UUParentIDType == "account");

                if (app == null)
                    return null;
            
                if (!this.Installing &&!this.DataAccessAuthorized(app,  "GET", false))
                    return null;


                app.Settings = context.GetAll<Setting>().Where(sw =>sw.AccountUUID == accountUUID && ( sw.AppType?.EqualsIgnoreCase(AppType) ??false ) ).ToList();
            }
            return app;
        }

        /// <summary>
        /// This returns data for the type argument where the AccountUUID is set to system.default.account.
        /// The returned data should NOT contain any sensitive information
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<Node> GetDefaultData(string type)
        {

            List<Node> res = new List<Node>();

            using (var context = new TreeMonDbContext(this._connectionKey))
            {
               string tableName =  context.GetTableName(type);

                if (string.IsNullOrWhiteSpace(tableName))
                    return res;

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@ACCOUNTUUID", SystemFlag.Default.Account);
                string sql = "SELECT * FROM " + tableName + " WHERE AccountUUID = @ACCOUNTUUID";
                IDataReader reader = context.Execute(sql, parameters);


                if (reader == null || reader.FieldCount == 0)
                   return res;

                try
                {
                    while (reader.Read())
                    {
                        res.Add(new Node()
                        {
                            Id =         StringEx.ConvertTo<int>(    reader["Id"].ToString()),
                            ParentId = StringEx.ConvertTo<int>(reader["ParentId"].ToString()),
                            UUID =               reader["UUID"].ToString(),
                            UUIDType =           reader["UUIDType"].ToString(),
                            UUParentID =         reader["UUParentID"].ToString(),
                            UUParentIDType =     reader["UUParentIDType"].ToString(),
                            Name =               reader["Name"].ToString(),
                            Status =             reader["Status"].ToString(),
                            AccountUUID =        reader["AccountUUID"].ToString(),
                            Active = StringEx.ConvertTo<bool>(reader["Active"].ToString()),
                            Deleted = StringEx.ConvertTo<bool>(reader["Deleted"].ToString()),
                            Private = StringEx.ConvertTo<bool>(reader["Private"].ToString()),
                            SortOrder = StringEx.ConvertTo<int>(reader["SortOrder"].ToString()),
                            DateCreated = StringEx.ConvertTo<DateTime>(reader["DateCreated"].ToString()),
                            CreatedBy = reader["CreatedBy"].ToString(),
                            RoleWeight = StringEx.ConvertTo<int>(reader["RoleWeight"].ToString()),
                            RoleOperation =reader["RoleOperation"].ToString(),
                            Image = reader["Image"].ToString()

                        });
 
                    }
                }
                catch (Exception ex)
                {
                    Debug.Assert(false, ex.Message);
                    _logger.InsertError(ex.Message,"AppManager",  "GetDefaultData");
                }
                finally
                {
                    if (reader != null) { reader.Close(); }
                }
                return res;
            }
        }

        #region Settings Code
        public ServiceResult Insert(INode n, string encryptionKey )
        {
            if (n == null ||   string.IsNullOrWhiteSpace(n.Name))
                return ServiceResponse.Error("Setting is empty or invalid.");

            n.Initialize(this._requestingUser.UUID, this._requestingUser.AccountUUID, this._requestingUser.RoleWeight);

            var setting = (Setting)n;

            if(string.IsNullOrWhiteSpace(setting.Type) || string.IsNullOrWhiteSpace(setting.Value) )
                return ServiceResponse.Error("Setting key or value is empty or invalid.");

            if (this.Installing == false && !this.DataAccessAuthorized(setting,  "POST", false))
                return ServiceResponse.Error("Unauthorized to complete this action.");

            if (!StringEx.ValueMatchesType(setting.Value, setting.Type))
                return ServiceResponse.Error("The value doesn't match the selected type.");

            //if we're adding a setting and it's supposed to be encrypted, then
            //the original value should be plain text (for now). 
            //
            if (setting.Type.EqualsIgnoreCase(SettingFlags.Types.EncryptedString))
            {
                
                if ( string.IsNullOrWhiteSpace(encryptionKey)) 
                    return ServiceResponse.Error("The encryption Key was not passed in. It must have a value to use the encrypt string.");

                setting.Value = Cipher.Crypt(encryptionKey, setting.Value, true);
            }
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                if (context.Insert<Setting>(setting))
                    return ServiceResponse.OK();
            }
            return ServiceResponse.Error();
        }

        public ServiceResult DeleteSetting(string uuid)
        {
            Setting s;
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                s = context.GetAll<Setting>().FirstOrDefault(sw => sw.UUID == uuid);


                if (s == null)
                    return ServiceResponse.Error("Setting " + uuid + " was not found. No action was taken.");


                if (!this.DataAccessAuthorized(s,  "DELETE", false))
                    return null;

                if (context.Delete<Setting>(s) > 0)
                    return ServiceResponse.OK();
            }
            return ServiceResponse.Error();
        }

        public ServiceResult Delete(INode n)
        {
            Setting s;
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                s = context.GetAll<Setting>().FirstOrDefault(sw => sw.UUID == n.UUID);


                if (s == null)
                    return ServiceResponse.Error("Setting " + n.UUID + " was not found. No action was taken.");


                if (!this.DataAccessAuthorized(s, "DELETE", false))
                    return null;

                if (context.Delete<Setting>(s) > 0)
                    return ServiceResponse.OK();
            }
            return ServiceResponse.Error();
        }


        public bool SettingExists(string name, string value)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(value))
                return false;

            //  if (!this.DataAccessAuthorized(s,  "GET", false))
            //     return null;

            try
            {
                using (var context = new TreeMonDbContext(this._connectionKey))
                {
                  return  context.GetAll<Setting>().Any(sw => ( (sw.Name?.EqualsIgnoreCase(name) ?? false) && 
                                                                (sw.Value?.EqualsIgnoreCase(value) ?? false) )  &&
                                                                sw.Deleted == false );
                }
            }
            catch (Exception ex)
            {
                _logger.InsertError(ex.Message, "AppManager", "GetSetting");
                return false;
            }
        }

        public Setting GetSetting(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;
            try
            {
                using (var context = new TreeMonDbContext(this._connectionKey))
                {
                    Setting s = context.GetAll<Setting>().FirstOrDefault(sw => (sw.Name?.EqualsIgnoreCase(name) ?? false));
                    //  if (!this.DataAccessAuthorized(s,  "GET", false))
                    //     return null;

                    return s;
                }
            }
            catch (Exception ex)
            {
                _logger.InsertError(ex.Message, "AppManager", "GetSetting");
                return null;
            }
        }


        public List<Setting> GetSettings(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return new List<Setting>();

            //  if (!this.DataAccessAuthorized(s,  "GET", false))
            //     return null;

            try
            {
                using (var context = new TreeMonDbContext(this._connectionKey))
                {
                    return context.GetAll<Setting>().Where(sw => (sw.Name?.EqualsIgnoreCase(name) ?? false)).ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.InsertError(ex.Message, "AppManager", "GetSetting");
                return new List<Setting>();
            }
        }

        public Setting GetBy(string UUID)
        {
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                Setting s = context.GetAll<Setting>().FirstOrDefault(sw => sw.UUID == UUID);
                if (!this.DataAccessAuthorized(s,  "GET", false))
                    return null;

                return s;
            }
        }

        public List<Setting> GetAppSettings(string appType)
        {
            User currentUser = this.GetUser(SessionKey);
            if (currentUser == null || !currentUser.SiteAdmin)
                return new List<Setting>();

            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                if(currentUser.SiteAdmin)
                    return context.GetAll<Setting>().Where(sw => sw.Deleted == false && (sw.AppType?.EqualsIgnoreCase(appType)??false) ).ToList();

                return context.GetAll<Setting>().Where(sw => sw.AccountUUID == currentUser.AccountUUID &&  sw.Deleted == false && (sw.AppType?.EqualsIgnoreCase(appType)??false) ).ToList();

            }
        }

        public List<Setting> GetPublicSettings(string appType, string accountUUID)
        {
          
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                return context.GetAll<Setting>().Where(sw => sw.Private == false && sw.AccountUUID == accountUUID && sw.Deleted == false && (sw.AppType?.EqualsIgnoreCase(appType) ??false)).ToList();

            }
        }

        /// <summary>
        /// Note: this only updates the fields in the form.
        /// If you add a field to the form then it should be done in the 
        /// client and mobile as well. If this turns into a hassle then the
        /// update code will be moved to each clients code i.e. this would go in the 
        /// controller. 
        /// Current fields updated are:
        /// *Name
        /// *Value
        /// *Type
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public ServiceResult Update(INode n, string encryptionKey)
        {
            if ( this.Installing == false && !this.DataAccessAuthorized(n,  "",false))
                    return ServiceResponse.Error("Unauthorized access.");

            Setting dbs = GetBy(n.UUID);
            if (dbs == null)
                return ServiceResponse.Error("Update setting couldn't find:" + n.Name );

            var s = (Setting)n;

            //if they changed types, make sure the type matches the value.
            if(dbs.Type != s.Type && StringEx.ValueMatchesType(s.Value, s.Type) == false)
                return ServiceResponse.Error("The value doesn't match the selected type.");

            //if the string type was changed to be encrypted, then encrypt it..
            if (dbs.Type != SettingFlags.Types.EncryptedString && s.Type == SettingFlags.Types.EncryptedString)
            {
                if (string.IsNullOrWhiteSpace(encryptionKey))
                    return ServiceResponse.Error("The encryption key is not set. It must have a value to use the encrypt string.");

                dbs.Value = Cipher.Crypt(encryptionKey, s.Value, true);
            }
            dbs.Type = s.Type;
            dbs.Value = s.Value;
            dbs.Name = s.Name;
            dbs.Active = s.Active;
            dbs.RoleOperation = s.RoleOperation;
            dbs.RoleWeight = s.RoleWeight;
            dbs.Private = s.Private;
           

            if (dbs.DateCreated == DateTime.MinValue)
                dbs.DateCreated = DateTime.UtcNow;
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                context.Update<Setting>(dbs);
            }
            return ServiceResponse.OK();
        }


     

        #endregion

        #region Database

        public string GetDbProviderName(string providerType)
        {
            string name = "";

            switch (providerType?.ToUpper())
            {
                case "MSSQL":
                    name = "System.Data.SqlClient";
                    break;
                case "MYSSQL":
                    name = "System.Data.MySqlClient";
                    break;
                case "SQLITE":
                    name = "System.Data.SQLite";
                    break;
            }
            return name;
        }

        public string CreateConnectionString(AppInfo appSettings)
        {
            string connectionString = "";

            switch (appSettings.ActiveDbProvider?.ToUpper())
            {
                case "MSSQL":
                    //connectionString = string.Format("<add name=\"{0}\" connectionString=\"Data Source={1};Initial Catalog={2};"+
                    //                                  "User Id={ 3}; Password={ 4}; MultipleActiveResultSets=true;\" providerName=\"{5}\" />" 
                    //                                  , appSettings.ActiveDbProvider, "localhost",
                    //                                    appSettings.ActiveDatabase, appSettings.ActiveDbUser,
                    //                                    appSettings.ActiveDbPassword, "System.Data.SqlClient");
                    connectionString = string.Format("Data Source={0};Initial Catalog={1};" +
                                             "User Id={2}; Password={3}; MultipleActiveResultSets=true;"
                                             ,  appSettings.DatabaseServer,appSettings.ActiveDatabase, appSettings.ActiveDbUser,appSettings.ActiveDbPassword);
                    break;
                case "MYSSQL":
                    //connectionString = string.Format("<add name=\"{0}\" connectionString=\"Server={1};Port=3306;Database={2};" +
                    //                 "uid={ 3}; pwd={4}; Convert Zero Datetime=True;\" providerName=\"{5}\" />"
                    //                 , appSettings.ActiveDbProvider, "localhost",
                    //                   appSettings.ActiveDatabase, appSettings.ActiveDbUser,
                    //                   appSettings.ActiveDbPassword, "System.Data.MySqlClient");
                    connectionString = string.Format("Server={0};Port=3306;Database={1};" +
                      "uid={2}; pwd={3}; Convert Zero Datetime=True;"
                      , appSettings.DatabaseServer, appSettings.ActiveDatabase, appSettings.ActiveDbUser,appSettings.ActiveDbPassword );
                    break;
                case "SQLITE":
                    string pathToDb = Path.Combine(EnvironmentEx.AppDataFolder, "TreeMon.sqlite");
                    //connectionString = string.Format("<add name=\"{0}\" connectionString=\"data source={1};\" providerName=\"{2}\" />",
                    //                                    appSettings.ActiveDbProvider, pathToDb , "System.Data.SQLite");
                    connectionString = string.Format("data source={0};", pathToDb );
                    break;
            }
            return connectionString;
        }

        public async  Task<ServiceResult> BackupDatabase(string encryptionKey)
        {
            User currentUser = this.GetUser(SessionKey);
            if (currentUser == null || !currentUser.SiteAdmin)
                return ServiceResponse.Error("Unauthorized access.");


            string pathToTempBackupFile = "";
            if (string.IsNullOrWhiteSpace(encryptionKey))
                return ServiceResponse.Error("The encryption key is not set.");

            string pathToScript = Path.Combine(EnvironmentEx.AppDataFolder, "SQLScripts\\MS.Backup.sql");

            if (!File.Exists(pathToScript))
                    return new ServiceResult() { Code = 500, Status = "ERROR", Message = "Backup sql script file missing in " + pathToScript };

            try
            {
                string pathToBackupFolder = Path.Combine(EnvironmentEx.AppDataFolder, "DBBackups");
                if (!Directory.Exists(pathToBackupFolder))
                    Directory.CreateDirectory(pathToBackupFolder);

                pathToTempBackupFile = Path.Combine(pathToBackupFolder, "TreeMon." + Guid.NewGuid().ToString("N") + ".tmp");


                FileInfo file = new FileInfo(pathToScript);
                string script = file.OpenText().ReadToEnd();
                script = script.Replace("{{DATABASE_NAME}}", "TreeMon");
                script = script.Replace("{{FILE_NAME_HERE}}", pathToTempBackupFile);
                using (var context = new TreeMonDbContext(this._connectionKey))
                {
                    await context.ExecuteNonQueryAsync(script, null);
                }
                if (!File.Exists(pathToTempBackupFile))
                    return ServiceResponse.Error("Failed to create temp backup database.");

                string pathToBackupFile = Path.Combine(pathToBackupFolder, "TreeMon." + string.Format("text-{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.UtcNow) + ".bak");
                int keyLength = Cipher.GenerateKey().Length;
                encryptionKey = encryptionKey.Substring(0, keyLength);
                if (!await Cipher.EncryptFileAsync(pathToTempBackupFile, pathToBackupFile, encryptionKey))
                    return ServiceResponse.Error("Failed to encrypt the backup.");

                if (!File.Exists(pathToBackupFile))
                    return ServiceResponse.Error("Failed to backup database.");
            }
            catch (Exception ex)
            {
                _logger.InsertError(ex.Message, "AppManager", "BackupDatabase");
                return ServiceResponse.Error("Failed to backup database.");
            }
            finally
            {
                try //backlog figure out file lock. won't let it delete file.
                {
                    File.Delete(pathToTempBackupFile);
                }
                catch {//
                }
            }

            return ServiceResponse.OK();

        }

        public async Task<ServiceResult> RestoreDatabase(string fileName, string encryptionKey)
        {
            string pathToDecryptedFile = "";
            string pathToFile = GetDatabaseBackupFiles(false).FirstOrDefault(w => w.Contains( fileName) );

            if(string.IsNullOrWhiteSpace(pathToFile))
                return ServiceResponse.Error("Could not find backup file.");
            try
            {
                FileInfo localFile = new FileInfo(pathToFile);
                if (localFile.Name?.ToUpper() != fileName.ToUpper())
                    return ServiceResponse.Error("Could not find backup file.");

                string pathToBackupFolder = Path.Combine(EnvironmentEx.AppDataFolder, "DBBackups");

                if (pathToBackupFolder.Length > 248)
                    return ServiceResponse.Error("Path to backup folder is too long.");

                //decrypt file
                if ( string.IsNullOrWhiteSpace(encryptionKey))
                    return ServiceResponse.Error("The encryption key is not set.");

              
                int keyLength = Cipher.GenerateKey().Length;
                encryptionKey = encryptionKey.Substring(0, keyLength);

                pathToDecryptedFile = Path.Combine(pathToBackupFolder, "TreeMon." + Guid.NewGuid().ToString("N") + ".tmp");

                if (!await Cipher.DecryptFileAsync(pathToFile, pathToDecryptedFile, encryptionKey))
                    return ServiceResponse.Error("Failed to decrypt the backup.");

                if (!File.Exists(pathToDecryptedFile))
                    return ServiceResponse.Error("Failed to decrypt the backup database.");


                //load restore script.
                string pathToScript = Path.Combine(EnvironmentEx.AppDataFolder, "SQLScripts\\MS.Restore.sql");

                if (!File.Exists(pathToScript))
                    return new ServiceResult() { Code = 500, Status = "ERROR", Message = "Restore sql script file missing in " + pathToScript };

                FileInfo file = new FileInfo(pathToScript);
                string script = file.OpenText().ReadToEnd();
                script = script.Replace("{{DATABASE_NAME}}", "TreeMon"); 
                script = script.Replace("{{FILE_NAME_HERE}}", pathToDecryptedFile);

                using (var context = new TreeMonDbContext(this._connectionKey))
                {
                    if (await context.ExecuteNonQueryAsync(script, null) == -1)
                        return ServiceResponse.Error("Failed to restore database.");
                }
            }
            catch(Exception ex)
            {
                _logger.InsertError(ex.Message, "AppManager", "RestoreDatabase:" + fileName);
                return ServiceResponse.Error(ex.Message);
                
            }
            finally
            {
                File.Delete(pathToDecryptedFile);
            }
          
            return ServiceResponse.OK();
        }

        public List<string> GetDatabaseBackupFiles(bool fileNamesOnly = true)
        {
            string pathToBackupFolder = Path.Combine(EnvironmentEx.AppDataFolder, "DBBackups");

            if (!Directory.Exists(pathToBackupFolder))
            {
                Directory.CreateDirectory(pathToBackupFolder);
                return new List<string>();
            }

            string[] backupFiles = Directory.GetFiles(pathToBackupFolder, "*.bak");

            if (!fileNamesOnly)
                return backupFiles.ToList();

            List<string> fileNames = new List<string>();
            foreach(string file in backupFiles)
            {
                if (string.IsNullOrWhiteSpace(file))
                    continue;

                fileNames.Add(new FileInfo(file).Name);
            }

            return fileNames;
        }

        public string GetDefaultDatabaseName()
        {
            string database = new TreeMonDbContext(this._connectionKey).Database.Connection.Database;
            return database;
        }


      

        //backlog v2 for future use.
        //public void DatabaseStatus()
        //{
        //    //stats
        //    //SELECT command, percent_complete, start_time FROM sys.dm_exec_requests where command = 'RESTORE DATABASE' 
        //    //SELECT command, percent_complete, start_time FROM sys.dm_exec_requests where command = 'BACKUP DATABASE' 
        //    //============================================
        //    //                SELECT r.session_id,r.command,CONVERT(NUMERIC(6, 2), r.percent_complete)
        //    //AS[Percent Complete],CONVERT(VARCHAR(20), DATEADD(ms, r.estimated_completion_time, GetDate()), 20) AS[ETA Completion Time],
        //    //CONVERT(NUMERIC(10, 2), r.total_elapsed_time / 1000.0 / 60.0) AS[Elapsed Min],
        //    //CONVERT(NUMERIC(10, 2), r.estimated_completion_time / 1000.0 / 60.0) AS[ETA Min],
        //    //CONVERT(NUMERIC(10, 2), r.estimated_completion_time / 1000.0 / 60.0 / 60.0) AS[ETA Hours],
        //    //CONVERT(VARCHAR(1000), (SELECT SUBSTRING(text, r.statement_start_offset / 2,
        //    //CASE WHEN r.statement_end_offset = -1 THEN 1000 ELSE(r.statement_end_offset - r.statement_start_offset) / 2 END)
        //    //FROM sys.dm_exec_sql_text(sql_handle)))
        //    //FROM sys.dm_exec_requests r WHERE command IN('RESTORE DATABASE', 'BACKUP DATABASE')
        //}
        #endregion

    }
}
