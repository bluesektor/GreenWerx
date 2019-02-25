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
using TreeMon.Data;
using TreeMon.Data.Logging.Models;
using TreeMon.Managers;
using TreeMon.Managers.Geo;
using TreeMon.Models.App;
using TreeMon.Models.Geo;
using TreeMon.Utilites.Extensions;
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
        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [System.Web.Http.AllowAnonymous]
        [EnableThrottling(PerSecond = 2, PerDay = 500)]
        [Route("api/Tools/TestCode")]
        public ServiceResult Test()
        {
            string authToken = Request.Headers?.Authorization?.Parameter;


            string directory = EnvironmentEx.AppDataFolder;
            string root = EnvironmentEx.AppDataFolder;
            string pathToFile = "";
            int index = 0;

            //PostalCodeManager pcm = new PostalCodeManager(Globals.DBConnectionKey, authToken);
            //pcm.ImportZipCodes(Path.Combine(root, "geoip\\zip.txt"));

            #region Country updates
            // pathToFile = Path.Combine(root, "geoip\\countryinfo.csv");

            // string[] countryLines = File.ReadAllLines(pathToFile);

            // foreach (string countryLine in countryLines)
            // {
            //     if (index == 0)
            //     {
            //         index++;
            //         continue; //skip headers.
            //     }

            //     string[] tokens = countryLine.Split(',');
            //     if (tokens.Length < 2)
            //         continue;

            //     string name = tokens[0];
            //     string code = tokens[1];

            //     using (var context = new TreeMonDbContext(Globals.DBConnectionKey))
            //     {
            //        var location = context.GetAll<Location>()?.FirstOrDefault(w => w.LocationType == "country" && w.Name.EqualsIgnoreCase(name) && string.IsNullOrWhiteSpace(w.Abbr));
            //         if (location == null)
            //             continue;
            //         location.Abbr = code;

            //         context.Update<Location>(location);
            //     }
            //}
            #endregion

            #region US STates updates
            //pathToFile = Path.Combine(root, "geoip\\usstates.csv");

            //string[] stateLines = File.ReadAllLines(pathToFile);
            //foreach (string state in stateLines)
            //{
            //    string[] tokens = state.Split(',');
            //    if (tokens.Length < 2)
            //        continue;

            //    string name = tokens[0];
            //    string code = tokens[1];

            //    using (var context = new TreeMonDbContext(Globals.DBConnectionKey))
            //    {
            //        var location = context.GetAll<Location>()?.FirstOrDefault(w => w.LocationType == "state" && w.UUParentID  == "6e9e15bbe8d44cd2a77b1bf9d51e9f40" && w.Name.EqualsIgnoreCase(name) && string.IsNullOrWhiteSpace(w.Abbr));
            //        if (location == null)
            //            continue;
            //        location.Abbr = code;

            //        context.Update<Location>(location);
            //    }
            //}
            #endregion

            #region canada updates
            //pathToFile = Path.Combine(root, "geoip\\canada.csv");

            //string[] canadaLines = File.ReadAllLines(pathToFile);
            //foreach (string region in canadaLines)
            //{


            //    string[] tokens = region.Split(',');
            //    if (tokens.Length < 2)
            //        continue;

            //    string name = tokens[0];
            //    string code = tokens[1];

            //    using (var context = new TreeMonDbContext(Globals.DBConnectionKey))
            //    {
            //        var location = context.GetAll<Location>()?.FirstOrDefault(w => w.LocationType == "state" && w.UUParentID == "a3f8adc547be4da9a73b000cb36a1b76" && w.Name.EqualsIgnoreCase(name) && string.IsNullOrWhiteSpace(w.Abbr));
            //        if (location == null)
            //            continue;
            //        location.Abbr = code;

            //        context.Update<Location>(location);
            //    }
            //}
            #endregion

            #region geoip code

            pathToFile = Path.Combine(root, "geoip\\blocks.csv");

            string[] blockLines = File.ReadAllLines(pathToFile);

            pathToFile = Path.Combine(root, "geoip\\location.csv");

            string[] locationLines = File.ReadAllLines(pathToFile);

            Dictionary<int, GeoIp> locations = new Dictionary<int, GeoIp>();

            index = 0;
            foreach (string block in blockLines)
            {
                if (index == 0)
                {
                    index++;
                    continue; //skip headers.
                }
                string[] tokens = block.Split(',');
                int locId = tokens[2].ConvertTo<int>();

                if (locations.ContainsKey(locId))
                    continue;

                GeoIp gip = new GeoIp();
                gip.LocationId = locId;
                gip.StartIpNum = tokens[0].ConvertTo<float>();
                gip.EndIpNum = tokens[1].ConvertTo<float>();
                locations.Add(locId, gip);
                index++;
            }


            index = 0;
            foreach (string location in locationLines)
            {
                if (index == 0)
                {
                    index++;
                    continue; //skip headers.
                }

                string[] tokens = location.Split(',');
                int locId = tokens[0].ConvertTo<int>();

                if (!locations.ContainsKey(locId))
                    continue;

                locations[locId].Country = tokens[1];
                locations[locId].Region = tokens[2];
                locations[locId].City = tokens[3];
                locations[locId].PostalCode = tokens[4];
                locations[locId].Latitude = tokens[5];
                locations[locId].Longitude = tokens[6];
                locations[locId].MetroCode = tokens[7];
                locations[locId].AreaCode = tokens[8];

                bool addCoordinate = false;

                Location loc = new Location();
                if (!string.IsNullOrWhiteSpace(locations[locId].City) && locations[locId].City.Contains("�"))
                {
                    loc = findCity(locations[locId].Country, locations[locId].City);

                    if (loc == null)
                        addCoordinate = true;
                }
                else if (!string.IsNullOrWhiteSpace(locations[locId].City))
                {//regular city name (no accent in characters)
                    using (var context = new TreeMonDbContext(Globals.DBConnectionKey))
                    {
                        loc = context.GetAll<Location>()?.FirstOrDefault(w => w.Abbr.EqualsIgnoreCase(locations[locId].Country) && w.Name.EqualsIgnoreCase(locations[locId].City));
                        if (loc == null)
                            addCoordinate = true;
                    }
                }
                else //just the country
                {
                    using (var context = new TreeMonDbContext(Globals.DBConnectionKey))
                    {
                        loc = context.GetAll<Location>()?.FirstOrDefault(w => w.Abbr.EqualsIgnoreCase(locations[locId].Country));
                        if (loc == null)
                            addCoordinate = true;
                    }
                }

                if (addCoordinate)
                {
                    loc = new Location();
                    loc.Name = string.IsNullOrWhiteSpace(locations[locId].City) ? locations[locId].Country : locations[locId].City;
                    loc.City = locations[locId].City;
                    loc.Country = locations[locId].Country;
                    loc.Postal = locations[locId].PostalCode;
                    loc.Latitude = locations[locId].Latitude.ConvertTo<float>();
                    loc.Longitude = locations[locId].Longitude.ConvertTo<float>();
                    loc.LocationType = "coordinate";
                    loc.RoleOperation = ">=";
                    loc.RoleWeight = 1;
                    loc.IpNumStart = locations[locId].StartIpNum;
                    loc.IpNumEnd = locations[locId].EndIpNum;
                    loc.DateCreated = DateTime.Now;
                    loc.CreatedBy = SystemFlag.Default.Account;
                    loc.AccountUUID = SystemFlag.Default.Account;
                    loc.Active = true;

                    using (var context = new TreeMonDbContext(Globals.DBConnectionKey))
                    { context.Insert<Location>(loc); }
                }
                else
                {   // sync loc fields and update

                    //locId,country, latitude,longitude,metroCode,areaCode
                    loc.City = string.IsNullOrWhiteSpace(loc.City) && !string.IsNullOrWhiteSpace(locations[locId].City) ? locations[locId].City : loc.City;
                    loc.Postal = string.IsNullOrWhiteSpace(loc.Postal) && !string.IsNullOrWhiteSpace(locations[locId].PostalCode) ? locations[locId].PostalCode : loc.Postal;

                    loc.Latitude = loc.Latitude == null ? locations[locId].Latitude.ConvertTo<float>() : loc.Latitude;
                    loc.Longitude = loc.Longitude == null ?  locations[locId].Longitude.ConvertTo<float>() : loc.Longitude;
                    loc.IpNumStart = loc.IpNumStart == null ? locations[locId].StartIpNum : loc.IpNumStart;
                    loc.IpNumEnd = loc.IpNumEnd == null ? locations[locId].EndIpNum : loc.IpNumEnd;

                    using (var context = new TreeMonDbContext(Globals.DBConnectionKey))
                    { context.Update<Location>(loc); }

                    //if any of these don't match then insert a coordinates record.
                    if (loc.Latitude    !=  locations[locId].Latitude.ConvertTo<float>() ||
                        loc.Longitude   != locations[locId].Longitude.ConvertTo<float>() ||
                        loc.IpNumStart  != locations[locId].StartIpNum ||
                        loc.IpNumEnd    != locations[locId].EndIpNum)
                    {
                        loc.UUID = Guid.NewGuid().ToString("N");
                        loc.Latitude = locations[locId].Latitude.ConvertTo<float>();
                        loc.Longitude = locations[locId].Longitude.ConvertTo<float>();
                        loc.IpNumStart = locations[locId].StartIpNum;
                        loc.IpNumEnd = locations[locId].EndIpNum;
                        loc.IpVersion = NetworkHelper.GetIpVersion(loc.IpNumStart.ToString());
                        loc.LocationType = "coordinate";
                        using (var context = new TreeMonDbContext(Globals.DBConnectionKey))
                        { context.Insert<Location>(loc); }
                       
                    }
                }
            }
            #endregion

            #region test code
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
            #endregion

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

        public Location findCity(string countryAbbr, string cityName )
        {
            string[] vowels = new string[] { "a", "e", "i", "o", "u", "y" };

            foreach (string vowel in vowels)
            {
                string name = cityName.Replace("�", vowel);

                using (var context = new TreeMonDbContext(Globals.DBConnectionKey))
                {
                    var location = context.GetAll<Location>()?.FirstOrDefault(w => w.Abbr == countryAbbr && w.Name.EqualsIgnoreCase(name));
                    if (location == null)
                        continue;

                    return location;
                }
            }
            return null;
        }
     
    }
}
