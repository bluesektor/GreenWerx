// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using TreeMon.Managers;
using TreeMon.Models.App;
using TreeMon.Utilites.Helpers;
using TreeMon.Utilites.Security;
using TreeMon.Web.api.Helpers;
using TreeMon.Web.Filters;
using TreeMon.Web.Models;
using WebApiThrottle;

namespace TreeMon.Web.api.v1
{
    public class ToolsController : ApiBaseController
    {
        readonly NetworkHelper network = new NetworkHelper();

        public ToolsController()
        {
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [HttpGet]
        [Route("api/Tools/Cipher/{text}/Encrypt/{encrypt}")]
        public ServiceResult Cipher(string text, bool encrypt)
        {
            string data = "";
            try
            {
                if (string.IsNullOrWhiteSpace(Globals.Application.AppSetting("AppKey", "")))
                    return ServiceResponse.Error("The appkey is not set in the config file. It must have a value to use the encrypt string.");

                data = TreeMon.Utilites.Security.Cipher.Crypt(Globals.Application.AppSetting("AppKey"), text, encrypt);
            }
            catch (Exception ex)
            {
                Debug.Assert(false, ex.Message);
                return ServiceResponse.Error(ex.Message);
            }
            return ServiceResponse.OK("", data);
        }


        //This scans names in a table for duplicates
        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [HttpGet]
        [Route("api/App/Tables/ScanNames/{table}")]
        public ServiceResult ScanNames(string table)
        {
            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

            if(!CurrentUser.SiteAdmin)
                return ServiceResponse.Error("You are not authorized to use this function.");
            string processId = Guid.NewGuid().ToString();

            AppManager app = new AppManager(Globals.DBConnectionKey, "web", Request.Headers?.Authorization?.Parameter);
            return app.ScanForDuplicates(table, processId);
        }

        private void ScanTableNames(string table, string processId)
        {
            AppManager app = new AppManager(Globals.DBConnectionKey, "web", Request.Headers?.Authorization?.Parameter);
            app.ScanForDuplicates(table, processId);
        }

        //This scans names in a table for duplicates
        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [HttpPost]
        [Route("api/App/Tables/Search/{value}")]
        public ServiceResult SearchTables(string value)
        {
            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

            if (!CurrentUser.SiteAdmin)
                return ServiceResponse.Error("You are not authorized to use this function.");

            Task<string> content = ActionContext.Request.Content.ReadAsStringAsync();
            if (content == null)
                return ServiceResponse.Error("No users were sent.");

            string body = content.Result;

            if (string.IsNullOrEmpty(body))
                return ServiceResponse.Error("No users were sent.");

            List<string> values =JsonConvert.DeserializeObject<List<string>>(body);
            AppManager app = new AppManager(Globals.DBConnectionKey, "web", Request.Headers?.Authorization?.Parameter);
            return app.SearchTables(values.ToArray() );
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [HttpGet]
        [Route("api/Tools/Database/Backup")]
        public async Task<ServiceResult> BackupDatabaseAsync()
        {
            AppManager app = new AppManager(Globals.DBConnectionKey, "web", Request.Headers?.Authorization?.Parameter);
            ServiceResult res = await app.BackupDatabase(Globals.Application.AppSetting("DBBackupKey"));

            if (res.Code != 200)
                return res;

            ClearTempFiles();
            return res;

        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [HttpPost]
        [Route("api/Tools/Database/Restore")]
        public async Task<ServiceResult> RestoreDatabaseAsync()
        {
            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

            ServiceResult res;
            try
            {
                string body = await ActionContext.Request.Content.ReadAsStringAsync();

                if (string.IsNullOrEmpty(body))
                    return ServiceResponse.Error("You must send valid email info.");

                dynamic formData = JsonConvert.DeserializeObject<dynamic>(body);

                if (formData == null || formData[0] == null)
                    return ServiceResponse.Error("Invalid info.");

                string file = formData[0].FileName;

                AppManager app = new AppManager(Globals.DBConnectionKey, "web", Request.Headers?.Authorization?.Parameter);
                res = await app.RestoreDatabase(file, Globals.Application.AppSetting("DBBackupKey"));  

                ClearTempFiles();
             
            }
            catch (Exception ex)
            {
                Debug.Assert(false, ex.Message);
                return ServiceResponse.Error(ex.Message);
            }
            return res;
        }


        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [HttpGet]
        [Route("api/Tools/Dashboard")]
        public ServiceResult GetToolsDashboard()
        {
            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

            AppManager app = new AppManager(Globals.DBConnectionKey, "web", Request.Headers?.Authorization?.Parameter);

            ToolsDashboard frm = new ToolsDashboard();
            frm.Backups = app.GetDatabaseBackupFiles();
            frm.DefaultDatabase = app.GetDefaultDatabaseName();
            frm.ImportFiles = new DirectoryInfo(Path.Combine(EnvironmentEx.AppDataFolder, "Install\\SeedData\\")).GetFiles().Select(o => o.Name).ToList();
            return ServiceResponse.OK("", frm);
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [HttpGet]
        [Route("api/Tools/Import/{type}")]
        public ServiceResult ImportType(string type)
        {
            if (string.IsNullOrEmpty(type))
                return ServiceResponse.Error("No file sent.");

            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

            AppManager app = new AppManager(Globals.DBConnectionKey, "web", Request.Headers?.Authorization?.Parameter);

            return app.ImportData(type);
        }

        /// <summary>
        /// /
        /// </summary>
        /// <param name="type"></param>
        /// <param name="validate">NOTE:Does record exist by name && accountUUID. This will check the item being imported by name for the account of the currently logged in user.</param>
        /// <param name="validateGlobally">NOTE:Does record exist by name regardless of account.</param>
        /// <returns></returns>
        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [HttpPost]
        [Route("api/Tools/ImportFile/{type}/Validate/{validate}/ValidateGlobally/{validateGlobally}")]
        public async Task<ServiceResult> ImportFile(string type, bool validate = true, bool validateGlobally = false)
        {
            if (string.IsNullOrEmpty(type))
                return ServiceResponse.Error("You must select a type.");

            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

            StringBuilder statusMessage = new StringBuilder();

            try
            {
                if (!Request.Content.IsMimeMultipartContent())
                {                // Check if the request contains multipart/form-data.
                    return ServiceResponse.Error("Unsupported media type.");
                }
               

                string root = HttpContext.Current.Server.MapPath("~/App_Data/temp");
                var provider = new MultipartFormDataStreamProvider(root);

                // This is a work around for max content length, so you don't have to put it in the web.config!
                var content = new StreamContent(HttpContext.Current.Request.GetBufferlessInputStream(true));
                foreach (var header in Request.Content.Headers)
                {
                    content.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
                await content.ReadAsMultipartAsync(provider);
                //End work around.
               // Request.Content.ReadAsMultipartAsync(provider); //<== if you remove the work around you'll need to add this back in.

                //  get the file names.
                foreach (MultipartFileData file in provider.FileData)
                {
                    Trace.WriteLine(file.Headers.ContentDisposition.FileName);
                    Trace.WriteLine("Server file path: " + file.LocalFileName);

                    AppManager app = new AppManager(Globals.DBConnectionKey, "web", Request.Headers?.Authorization?.Parameter);
                    string fileName = file.Headers.ContentDisposition.FileName;

                    // Clean the file name..
                    foreach (var c in Path.GetInvalidFileNameChars()) { fileName = fileName.Replace(c, ' '); }

                    if (string.IsNullOrWhiteSpace(fileName))
                        continue;

                    fileName = fileName.ToUpper();
                    type = type.ToUpper();
                    if (!fileName.Contains(type))
                    {   //this is to keep the user from selecting a Product type and the selecting a user file etc.
                        //The file has to match the type.
                        return ServiceResponse.Error("File does not match the type selected.");
                            
                    }

                    statusMessage.AppendLine( app.ImportFile(type, file.LocalFileName, fileName, network.GetClientIpAddress(this.Request),validate,validateGlobally).Message );
                }
            }
            catch (Exception ex)
            {
                Debug.Assert(false, ex.Message);
                return ServiceResponse.Error(ex.Message);
            }

            return ServiceResponse.OK(statusMessage.ToString());
        }

        private void ClearTempFiles()
        {
            string pathToBackupFolder = Path.Combine(EnvironmentEx.AppDataFolder, "DBBackups");
            string[] tempFiles = Directory.GetFiles(pathToBackupFolder, "*.tmp");
            foreach (string file in tempFiles)
            {
                try
                {
                    File.Delete(file);
                }
                catch  {
                    // No need to log this because the file could still be locked.
                }
            }

        }

       
        [HttpGet]
        [System.Web.Http.AllowAnonymous]
        [EnableThrottling(PerSecond = 2, PerDay = 500)]
        [Route("api/Tools/TestCode")]
        public ServiceResult Test()
        {
            ///string authToken = Request.Headers?.Authorization?.Parameter;

            ////AppManager am = new AppManager(Globals.DBConnectionKey, "web", authToken);
            ////am.TestCode();

            ////Globals.Application.UseDatabaseConfig = false;
            ////string encryptionKey = Globals.Application.AppSetting("AppKey");

            ////if (string.IsNullOrWhiteSpace(encryptionKey))
            ////    return ServiceResponse.Error("Unable to get AppKey from .config.");

            ////ServiceResult  res = Globals.Application.ImportWebConfigToDatabase(authToken, encryptionKey, true);
            ////if (res.Code != 200)
            ////    return res;
            ////if (PasswordHash.IsCommonPassword("maTurE"))
            ////    return ServiceResponse.Error();

            ////if (string.IsNullOrWhiteSpace(authToken))
            ////    return new ServiceResult() { Code = 500, Status = "ERROR", Message = "You must login to view this page." };

            ////User u = Get(authToken);

            //////Test inserting default roles
            ////RoleManager rm = new RoleManager(Globals.DBConnectionKey, u);
            ////ServiceResult res = rm.InsertDefaults(u.AccountUUID, "web");
            ////if (res.Code != 200)
            ////    return res;

            //////Test seeding the database
            ////  AppManager am = new AppManager(Globals.DBConnectionKey, "web", authToken);
            ////string directory = EnvironmentEx.AppDataFolder;
            //// am.SeedDatabase(Path.Combine(directory, "Install\\SeedData\\"), u.AccountUUID);

            #region location import
            ////LocationManager lm = new LocationManager(Globals.DBConnectionKey, authToken);
            ////string pathToFile = Path.Combine(directory, "DBBackups\\geolocations.csv");
            ////if (!File.Exists(pathToFile))
            ////    return ServiceResponse.Error("File not found");

            ////string[] fileLines = File.ReadAllLines(pathToFile);

            ////foreach (string fileLine in fileLines)
            ////{
            ////    if (string.IsNullOrWhiteSpace(fileLine))
            ////        continue;

            ////    string[] locationTokens = fileLine.Split(',');

            ////    if (locationTokens.Count() < 9)
            ////        continue;

            ////    int locationID = StringEx.ConvertTo<int>(locationTokens[0]);
            ////    Location l = new Location();
            ////    l.UUID = Guid.NewGuid().ToString("N");
            ////    l.UUIDType = "Location";
            ////    l.AccountUUID = SystemFlag.Default.Account;
            ////    l.DateCreated = DateTime.UtcNow;
            ////    l.CreatedBy = CurrentUser.UUID;
            ////    l.RoleWeight = 1;
            ////    l.RoleOperation= ">=";
            ////    l.RootId = locationID;
            ////    l.ParentId = StringEx.ConvertTo<int>( locationTokens[1]);
            ////    l.Name = locationTokens[2];
            ////    l.Code = locationTokens[3];
            ////    l.LocationType = locationTokens[4];
            ////    l.Latitude = StringEx.ConvertTo<float>( locationTokens[5]);
            ////    l.Longitude = StringEx.ConvertTo<float>(locationTokens[6]);
            ////    l.TimeZoneUUID = StringEx.ConvertTo<int>(locationTokens[7]);
            ////    l.CurrencyUUID = StringEx.ConvertTo<int>(locationTokens[8]);

            ////    if (lm.Insert(l, false).Code != 200)
            ////        Debug.Assert(false, "shit");
            ////}

            ////List<Location> locations = lm.GetLocations(SystemFlag.Default.Account);

            ////foreach(Location l in locations)
            ////{
            ////    List<Location> childLocations = locations.Where(w => w.ParentId == l.RootId).ToList();

            ////    foreach(Location child in childLocations)
            ////    {
            ////        child.UUParentID = l.UUID;
            ////        child.UUParentIDType = l.State;

            ////       if(  lm.Updatechild).Code != 200)
            ////            Debug.Assert(false, "shit 2");
            ////    }
            ////}
            #endregion

            #region Equipment test code
            ////EquipmentManager equipmentManager = new EquipmentManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            ////List<dynamic> equipment = (List<dynamic>)equipmentManager.GetAll("BALLAST").Cast<dynamic>().ToList();

            ////equipment.AddRange((List<dynamic>)equipmentManager.GetAll("BULB").Cast<dynamic>().ToList());
            ////equipment.AddRange((List<dynamic>)equipmentManager.GetAll("CUSTOM").Cast<dynamic>().ToList());
            ////equipment.AddRange((List<dynamic>)equipmentManager.GetAll("FAN").Cast<dynamic>().ToList());
            ////equipment.AddRange((List<dynamic>)equipmentManager.GetAll("FILTER").Cast<dynamic>().ToList());
            ////equipment.AddRange((List<dynamic>)equipmentManager.GetAll("PUMP").Cast<dynamic>().ToList());
            ////equipment.AddRange((List<dynamic>)equipmentManager.GetAll("VEHICLE").Cast<dynamic>().ToList());

            ////foreach(dynamic d in equipment)
            ////{
            ////    d.AccountUUID = this.CurrentUser.AccountUUID;
            ////    d.CreatedBy = this.CurrentUser.UUID;
            ////    d.UUID = Guid.NewGuid().ToString("N");
            ////    equipmentManager.Update(d);

            ////}

            #endregion

            return ServiceResponse.OK();
        }
     
    }
}
