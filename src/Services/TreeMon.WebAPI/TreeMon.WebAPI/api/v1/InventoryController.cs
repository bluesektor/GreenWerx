// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using Newtonsoft.Json;
using Omni.Base.Multimedia;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using TreeMon.Data.Logging;
using TreeMon.Data.Logging.Models;
using TreeMon.Managers;
using TreeMon.Managers.Documents;
using TreeMon.Managers.Equipment;
using TreeMon.Managers.Inventory;
using TreeMon.Managers.Membership;
using TreeMon.Managers.Store;
using TreeMon.Models;
using TreeMon.Models.App;
using TreeMon.Models.Datasets;
using TreeMon.Models.Files;
using TreeMon.Models.Flags;
using TreeMon.Models.Geo;
using TreeMon.Models.Inventory;
using TreeMon.Models.Membership;
using TreeMon.Utilites.Extensions;
using TreeMon.Utilites.Helpers;
using TreeMon.Web;
using TreeMon.Web.api;
using TreeMon.Web.Filters;
using TreeMon.WebAPI.Models;
using WebApi.OutputCache.V2;

namespace TreeMon.WebAPI.api.v1
{
    [CacheOutput(ClientTimeSpan = 100, ServerTimeSpan = 100)]
    public class InventoryController : ApiBaseController
    {
        readonly SystemLogger _logger = null;
        

        public InventoryController()
        {
            _logger = new SystemLogger(Globals.DBConnectionKey);
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [HttpPost]
        [Route("api/Inventory/Add")]
        [Route("api/Inventory/Insert")]
        public ServiceResult Insert(InventoryItem n)
        {
            if (n == null)
                return ServiceResponse.Error("Invalid location posted to server.");

            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

            if (string.IsNullOrWhiteSpace(n.CreatedBy))
            {
                n.CreatedBy = CurrentUser.UUID;
                n.AccountUUID = CurrentUser.AccountUUID;
                n.DateCreated = DateTime.UtcNow;
            }
          

            InventoryManager inventoryManager = new InventoryManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            return inventoryManager.Insert(n);
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 1)]
        [HttpGet]
        [Route("api/Inventory/{name}")]
        public ServiceResult Search(string name )
        {
            if (string.IsNullOrWhiteSpace(name))
                return ServiceResponse.Error("You must provide a name for the item.");

            InventoryManager inventoryManager = new InventoryManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            List<InventoryItem> s = inventoryManager.Search(name);

            if (s == null || s.Count == 0)
                return ServiceResponse.Error("Inventory Item could not be located for the name " + name);

            return ServiceResponse.OK("", s);
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 1)]
        [HttpGet]
        [Route("api/InventoryBy/{uuid}")]
        public ServiceResult GetBy(string uuid)
        {
            if (string.IsNullOrWhiteSpace(uuid))
                return ServiceResponse.Error("You must provide an id for the item.");

            InventoryManager inventoryManager = new InventoryManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            InventoryItem p = (InventoryItem)inventoryManager.Get(uuid);

            if (p == null)
                return ServiceResponse.Error("Inventory Item could not be located for the uuid " + uuid);

            return ServiceResponse.OK("", p);
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 1)]
        [HttpPatch]
        [Route("api/Inventory/Publish/{uuid}")]
        public ServiceResult PublishItem(string uuid)
        {
            if (string.IsNullOrWhiteSpace(uuid))
                return ServiceResponse.Error("You must provide an id for the item.");

            InventoryManager inventoryManager = new InventoryManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            InventoryItem p = (InventoryItem)inventoryManager.Get(uuid);

            if (p == null)
                return ServiceResponse.Error("Item could not be located for the uuid " + uuid);

            if(p.Deleted == true)
                return ServiceResponse.Error("Item cannote be published as it was deleted.");

            p.Published = true;
            return inventoryManager.Update(p);

            
        }


        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 1)]
        [HttpGet]
        [Route("api/Item/{uuid}/Details")]
        public ServiceResult GetItemDetails(string uuid)
        {
            if (string.IsNullOrWhiteSpace(uuid))
                return ServiceResponse.Error("You must provide an id for the item.");

            InventoryManager inventoryManager = new InventoryManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            return inventoryManager.GetItemDetails(uuid);
        }

       

        //[ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        //[HttpPost]
        //[HttpGet]
        //[Route("api/Inventory")]
        //public ServiceResult GetItem()
        //{
        //    if (CurrentUser == null)
        //        return ServiceResponse.Error("You must be logged in to access this function.");
           
        //    InventoryManager inventoryManager = new InventoryManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
        //    List<dynamic> Inventory = (List<dynamic>)inventoryManager.GetItems(CurrentUser.AccountUUID).Cast<dynamic>().ToList();
        //  int count;

        //                     DataFilter tmpFilter = this.GetFilter(Request);
        //        Inventory = Inventory.Filter( tmpFilter, out count);
            
        //    return ServiceResponse.OK("", Inventory, count);
        //}

        //[ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        //[HttpPost]
        //[HttpGet]
        //[Route("api/Inventory/Location/{locationUUID}")]
        //public ServiceResult GetItemsForLocation(string locationUUID)
        //{
        //    if (CurrentUser == null)
        //        return ServiceResponse.Error("You must be logged in to access this function.");
           
        //    InventoryManager inventoryManager = new InventoryManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
        //    List<dynamic> Inventory = (List<dynamic>)inventoryManager.GetItems(CurrentUser.AccountUUID).Cast<dynamic>().ToList();

        //    Inventory = Inventory.Where(w => w.LocationUUID == locationUUID &&
        //                        w.Deleted == false )                               
        //            .Cast<dynamic>().ToList();

        //    int count;

        //    DataFilter tmpFilter = this.GetFilter(Request);
        //    Inventory = Inventory.Filter( tmpFilter, out count);
        //    return ServiceResponse.OK("", Inventory, count);
        //}

        //[HttpPost]
        //[HttpGet]
        //[Route("api/Inventory/{locationName}/distance/{distance}")] 
        //public ServiceResult GetPublishedInventoryByLocation(string locationName, int distance) 
        //{
        //    //if (CurrentUser == null)
        //   //     return ServiceResponse.Error("You must be logged in to access this function.");
            
        //    InventoryManager inventoryManager = new InventoryManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
        //    List<dynamic> Inventory = (List<dynamic>)inventoryManager.GetItems(locationName,distance ).Cast<dynamic>().ToList();  // && w.Expires && w.Private == false

        //    int count;
         
        //    DataFilter tmpFilter = this.GetFilter(Request);
        //    Inventory = Inventory.Filter(tmpFilter, out count);
        //    return ServiceResponse.OK("", Inventory, count);
        //}


        //[HttpPost]
        //[HttpGet]
        //[Route("api/Inventory/Published")]
        //public ServiceResult GetPublishedInventory()
        //{
        //    //if (CurrentUser == null)
        //    //     return ServiceResponse.Error("You must be logged in to access this function.");

        //    InventoryManager inventoryManager = new InventoryManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
        //    List<dynamic> Inventory = (List<dynamic>)inventoryManager.GetPublishedItems().Cast<dynamic>().ToList();  // && w.Expires && w.Private == false

        //    int count;

        //    DataFilter tmpFilter = this.GetFilter(Request);
        //    Inventory = Inventory.Filter(tmpFilter, out count);
        //    return ServiceResponse.OK("", Inventory, count);
        //}

        //[HttpPost]
        //[HttpGet]
        //[Route("api/Inventory/{locationName}/distance/{distance}/Search")]
        //public ServiceResult SearchPublishedInventory(string locationName, int distance)
        //{
        //    //if (CurrentUser == null)
        //    //     return ServiceResponse.Error("You must be logged in to access this function.");

        //    InventoryManager inventoryManager = new InventoryManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
        //    List<dynamic> Inventory = (List<dynamic>)inventoryManager.GetItems(locationName, distance).Cast<dynamic>().ToList();  // && w.Expires && w.Private == false

        //    int count;

        //    DataFilter tmpFilter = this.GetFilter(Request);
            
        //    if(tmpFilter == null || tmpFilter.Screens.Count == 0)
        //        return ServiceResponse.OK("", Inventory, Inventory.Count);


        //    Inventory = Inventory.Search(tmpFilter, out count);

        //    tmpFilter.Screens = tmpFilter.Screens.Where(w => w.Command?.ToUpper() != "SEARCHBY" || w.Command?.ToUpper() != "SEARCH!BY").ToList();

        //    if ( tmpFilter.Screens.Count == 0)
        //        return ServiceResponse.OK("", Inventory, count);

        //    Inventory = Inventory.Filter(tmpFilter, out count);
        //    return ServiceResponse.OK("", Inventory, count);
        //}

        [HttpPost]
        [HttpGet]
        [Route("api/Inventory/User/{uuid}")]
        public ServiceResult GetUserInventory(string uuid)
        {
            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

            if(CurrentUser.UUID != uuid) //CurrentUser.SiteAdmin != true
                return ServiceResponse.Error("You are not authorized.");


            InventoryManager inventoryManager = new InventoryManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            List<dynamic> Inventory = (List<dynamic>)inventoryManager.GetUserItems(CurrentUser.AccountUUID, CurrentUser.UUID).Cast<dynamic>().ToList(); ;
             

            int count;

            DataFilter tmpFilter = this.GetFilter(Request);
            Inventory = Inventory.Filter(tmpFilter, out count);
            return ServiceResponse.OK("", Inventory, count);
        }



        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 1)]
        [HttpPost]
        [HttpGet]
        [Route("api/Inventory/InventoryType/{type}")]
        public ServiceResult GetLocationsByInventoryType(string type)
        {
            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

            InventoryManager inventoryManager = new InventoryManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            List<dynamic> locations = (List<dynamic>)inventoryManager.GetItems(CurrentUser.AccountUUID)
                                                                    .Where(pw => (pw.AccountUUID == SystemFlag.Default.Account)
                                                                                    && (pw.ReferenceType?.EqualsIgnoreCase(type)??false)
                                                                                    && pw.Deleted == false
                                                                                    && string.IsNullOrWhiteSpace(pw.ReferenceType) == false)
                                                                                    .Cast<dynamic>().ToList();
            int count;

             DataFilter tmpFilter = this.GetFilter(Request);
       
                locations = locations.Filter( tmpFilter, out count);
            if (locations == null || locations.Count == 0)
                return ServiceResponse.Error("No locations available.");

            return ServiceResponse.OK("", locations, count);

        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [HttpDelete]
        [Route("api/Inventory/Delete")]
        public ServiceResult Delete(InventoryItem n)
        {
            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

            if (n == null || string.IsNullOrWhiteSpace(n.UUID))
                return ServiceResponse.Error("Invalid account was sent.");

            InventoryManager inventoryManager = new InventoryManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            return inventoryManager.Delete(n);
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [HttpDelete]
        [Route("api/Inventory/Delete/{inventoryItemUUID}")]
        public ServiceResult Delete(string inventoryItemUUID)//todo bookmark latest test this.
        {
            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

            InventoryManager inventoryManager = new InventoryManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            InventoryItem p = (InventoryItem)inventoryManager.Get(inventoryItemUUID);

            if (p == null || string.IsNullOrWhiteSpace(p.UUID))
                return ServiceResponse.Error("Invalid account was sent.");

            //TODO DELETE THE FILE FROM THE IMAGE FIELD, AND SETTINGS
            //Fire and forget delete task
            //Thread t = new Thread(() =>{
             string root = System.Web.HttpContext.Current.Server.MapPath("~/Content/Uploads/" + this.CurrentUser.UUID);

            DocumentManager dm = new DocumentManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            dm.DeleteImages(p,root);
            //});
            //t.Start();
            return inventoryManager.Delete(p);
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [HttpDelete]
        [Route("api/Inventory/Delete/{inventoryItemUUID}/File/{fileName}")]
        public ServiceResult DeleteFile(string inventoryItemUUID, string fileName)//todo bookmark latest test this.
        {
            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

            InventoryManager inventoryManager = new InventoryManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            InventoryItem p = (InventoryItem)inventoryManager.Get(inventoryItemUUID);

            if (p == null || string.IsNullOrWhiteSpace(p.UUID))
                return ServiceResponse.Error("Invalid account was sent.");

            string root = System.Web.HttpContext.Current.Server.MapPath("~/Content/Uploads/" + this.CurrentUser.UUID);

            string pathToFile = Path.Combine(root, fileName);
            DocumentManager dm = new DocumentManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            if (dm.DeleteFile(p, pathToFile).Code != 200)
                return ServiceResponse.Error("Failed to delete file " + fileName);

            //now update the image field or delete the setting..
            if (p.Image.EqualsIgnoreCase( pathToFile) )
            {
                p.Image = string.Empty; //todo v2? check settings for more images, automatically make the next image the primary(?) may not want to do this
                return this.Update(p);
            }

            //not the object field so it must be a setting.
            AppManager am = new AppManager(Globals.DBConnectionKey, "web", Request.Headers?.Authorization?.Parameter);
            
            List<Setting> settings = am.GetSettings(p.UUIDType)
                .Where(w => w.UUIDType.EqualsIgnoreCase("ImagePath") &&
                       w.Value == p.UUID &&
                       w.Image.Contains(fileName)).ToList();

            foreach (Setting setting in settings)
            {
                if (am.DeleteSetting(setting.UUID).Code != 200)
                    return ServiceResponse.Error("Failed to delete image setting for file " + fileName);
            }

            return ServiceResponse.OK();
        }

        //[ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        //[HttpGet]
        //[Route("api/Inventory/{inventoryItemUUID}/Images")]
        //public ServiceResult GetImageLinks(string inventoryItemUUID)
        //{
        //    if (CurrentUser == null)
        //        return ServiceResponse.Error("You must be logged in to access this function.");

        //    InventoryManager inventoryManager = new InventoryManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
        //    InventoryItem item = (InventoryItem)inventoryManager.Get(inventoryItemUUID);

        //    if (item == null )
        //        return ServiceResponse.Error("Invalid id was sent.");

        //    string root = System.Web.HttpContext.Current.Server.MapPath("~/Content/Uploads/" + this.CurrentUser.UUID);

        //    List<FileEx> files = new List<FileEx>();
        //    //get the default image, assigned to the object.
        //    FileEx file = new FileEx();
        //    file.UUID = item.UUID;
        //    file.UUIDType = item.UUIDType;
        //    file.Default = true;
           
        //    file.Status = "saved";
        //    file.Name = item.Image.GetFileNameFromUrl();
        //    file.Path = Path.Combine(root, file.Name);
        //    file.Image = item.Image;
        //    string fullUrl = this.Request.RequestUri.Scheme + "://" + this.Request.RequestUri.Authority + "/Content/Uploads/" + this.CurrentUser.UUID + "/";
        //    file.ImageThumb = fullUrl + ImageEx.GetThumbFileName(file.Path);
        //    files.Add(file);
            
        //    //Get secondary images assigned to the settings table.
        //    AppManager am = new AppManager(Globals.DBConnectionKey, "web", Request.Headers?.Authorization?.Parameter);

        //    List<Setting> settings = am.GetSettings(item.UUIDType)
        //        .Where(w => w.UUIDType.EqualsIgnoreCase("ImagePath") &&
        //               w.Value == item.UUID).ToList();

        //    foreach (Setting setting in settings)
        //    {
        //        file = new FileEx();
               
        //        file.UUID = setting.UUID;
        //        file.UUIDType = setting.UUIDType;
        //        file.Default = false;
        //        file.Status = "saved";
        //        file.Image = setting.Image;
        //        file.Name = file.Image.GetFileNameFromUrl();
        //        file.Path = Path.Combine(root, file.Name);
        //        file.ImageThumb = fullUrl + ImageEx.GetThumbFileName(file.Path);
             
              
        //        files.Add(file);
        //    }

        //    return ServiceResponse.OK("",files,files.Count);
        //}



        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [HttpPost]
        [HttpPatch]
        [Route("api/Inventory/Update")]
        public ServiceResult Update(InventoryItem pv)
        {
            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

            if (pv == null)
                return ServiceResponse.Error("Invalid location sent to server.");

            InventoryManager inventoryManager = new InventoryManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            var dbP = (InventoryItem)inventoryManager.Get(pv.UUID);

            if (dbP == null)
                return ServiceResponse.Error("InventoryItemwas not found.");

            dbP.Name = pv.Name;
            dbP.Cost = pv.Cost;
            dbP.Price = pv.Price;
            dbP.UUIDType = pv.UUIDType;
            dbP.CategoryUUID = pv.CategoryUUID;
            dbP.Description = pv.Description;
            dbP.Condition = pv.Condition;
            dbP.Quality = pv.Quality;
            dbP.Deleted = pv.Deleted;
            dbP.Rating = pv.Rating;
            dbP.LocationUUID = pv.LocationUUID; //todo when updating itn the  client it's resetting it to non coordinate type
            dbP.LocationType = pv.LocationType; //todo when updating itn the  client it's resetting it to non coordinate type
            dbP.Quantity = pv.Quantity;
            dbP.ReferenceType = pv.ReferenceType;
            dbP.ReferenceUUID = pv.ReferenceUUID;
            dbP.Virtual = pv.Virtual;
            dbP.Published = pv.Published;
            dbP.Link = pv.Link;
            dbP.LinkProperties = pv.LinkProperties;
            //todo bookmark latest. image is saving as blank when doing the update. may need to requery for the item.
            dbP.Image = pv.Image;
            return inventoryManager.Update(dbP);
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [HttpPost]
        [HttpPatch]
        [Route("api/Inventory/Updates")]
        public ServiceResult UpdateInventory()
        {
            ServiceResult res = ServiceResponse.OK();
            StringBuilder msg = new StringBuilder();
            try
            {
                Task<string> content = ActionContext.Request.Content.ReadAsStringAsync();
                if (content == null)
                    return ServiceResponse.Error("No permissions were sent.");

                string body = content.Result;

                if (string.IsNullOrEmpty(body))
                    return ServiceResponse.Error("No permissions were sent.");

                List<InventoryItem> changedItems = JsonConvert.DeserializeObject<List<InventoryItem>>(body);

                InventoryManager inventoryManager = new InventoryManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

                foreach (InventoryItem changedItem in changedItems)
                {
                    var databaseItem = (InventoryItem)inventoryManager.Get(changedItem.UUID);

                    if (string.IsNullOrWhiteSpace(changedItem.CreatedBy))
                        changedItem.CreatedBy = this.CurrentUser.UUID;

                    if (string.IsNullOrWhiteSpace(changedItem.AccountUUID))
                        changedItem.AccountUUID = this.CurrentUser.AccountUUID;

                     if (string.IsNullOrWhiteSpace(changedItem.UUID))
                        changedItem.UUID = Guid.NewGuid().ToString("N");

                    if (databaseItem == null)
                    {
                        changedItem.UUIDType = "InventoryItem";
                        changedItem.DateCreated = DateTime.UtcNow;

                        ServiceResult sr = inventoryManager.Insert(changedItem);
                        if(sr.Code != 200)
                        {
                            res.Code = 500;
                            msg.AppendLine( sr.Message );
                        }
                        continue;
                    }

                    databaseItem.Name = changedItem.Name;
                    databaseItem.Cost = changedItem.Cost;
                    databaseItem.Condition = changedItem.Condition;
                    databaseItem.Quality = changedItem.Quality;
                    databaseItem.Deleted = changedItem.Deleted;
                    databaseItem.Rating = changedItem.Rating;
                    databaseItem.LocationUUID = changedItem.LocationUUID;
                    databaseItem.LocationType = changedItem.LocationType;
                    databaseItem.Quantity = changedItem.Quantity;
                    databaseItem.ReferenceType = changedItem.ReferenceType;
                    databaseItem.ReferenceUUID = changedItem.ReferenceUUID;
                    databaseItem.Virtual = changedItem.Virtual;
                    databaseItem.Published = changedItem.Published;
                    databaseItem.Link = changedItem.Link;
                    databaseItem.LinkProperties = changedItem.LinkProperties;
                    if (CurrentUser.SiteAdmin)
                    {
                        databaseItem.RoleOperation = changedItem.RoleOperation;
                        databaseItem.RoleWeight= changedItem.RoleWeight;
                    }
                    ServiceResult sru  =inventoryManager.Update(changedItem);
                    if (sru.Code != 200)
                    {
                        res.Code = 500;
                        msg.AppendLine(sru.Message );
                    }
                }
            }
            catch (Exception ex)
            {
                msg.AppendLine(ex.Message);
                Debug.Assert(false, ex.Message);
            }
            res.Message = msg.ToString();
            return res;
        }
      
        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [HttpPost]
        [Route("api/File/Upload/{UUID}/{type}")]
        public async Task<ServiceResult> PostFile(string UUID, string type)
        {
            var fileResult = new FileEx();
            fileResult.Default = false;
            string pathToImage = "";
            
            try
            {
                if (this.CurrentUser == null)
                    return ServiceResponse.Error("You must be logged in to upload.");

                #region non async
                //var httpRequest = HttpContext.Current.Request;
                //if (httpRequest.Files.Count < 1)
                //{
                //    return ServiceResponse.Error("Bad request");
                //}

                //foreach (string file in httpRequest.Files)
                //{
                //    var postedFile = httpRequest.Files[file];
                //    var filePath = HttpContext.Current.Server.MapPath("~/" + postedFile.FileName);
                //    postedFile.SaveAs(filePath);

                //}

                //return ServiceResponse.OK();
                #endregion

                HttpRequestMessage request = this.Request;
                if (!request.Content.IsMimeMultipartContent())
                    return ServiceResponse.Error("Unsupported media type.");

                string root = System.Web.HttpContext.Current.Server.MapPath("~/Content/Uploads/" + this.CurrentUser.UUID);

                if (!Directory.Exists(root))
                    Directory.CreateDirectory(root);

                var provider = new MultipartFormDataStreamProvider(root);
                

                ServiceResult res = await request.Content.ReadAsMultipartAsync(provider).
                    ContinueWith<ServiceResult>(o =>
                    {
                        if (o.IsFaulted || o.IsCanceled)
                        {
                            throw new HttpResponseException(HttpStatusCode.InternalServerError);
                        }
                        string fileName = "";
                        List<string> kvp =  o.Result.Contents.First().Headers.First(w => w.Key == "Content-Disposition").Value.ToList()[0].Split(';').ToList();
                        foreach(string value in kvp)
                        {
                            if(value.Trim().StartsWith("filename") )
                            {
                                String[] tmp = value.Split('=');
                                fileName = DateTime.UtcNow.ToString("yyyyMMdd_hhmmss") + tmp[1].Trim().Replace("\"", "");
                            }

                            if (value.Contains("defaultImage"))    //value.Trim().StartsWith("name"))
                            {
                                fileResult.Default = true;
                            }
                        }
                        // this is the file name on the server where the file was saved 
                        string file = provider.FileData.First().LocalFileName;
                        string originalFilename = Path.GetFileName(file);
                        string destFile = file.Replace(originalFilename, fileName);
                        try
                        {
                            if (File.Exists(destFile))
                                File.Delete(destFile);
                        } catch { //file may still be locked so don't worry about it.
                        }
                        try
                        {
                            File.Move(file, destFile);
                        }
                        catch { }//ditto from above catch
                        file = destFile;

                        string thumbFile = ImageEx.CreateThumbnailImage(file, 64);
                        string ImageUrl = fileName;
                        string fullUrl = this.Request.RequestUri.Scheme + "://" + this.Request.RequestUri.Authority + "/Content/Uploads/" + this.CurrentUser.UUID + "/";
                        pathToImage = fullUrl + ImageUrl;

                        if (fileResult.Default)
                            this.UpdateImageURL(UUID, type, pathToImage);//Now update the database.
                        else{   
                               //add other images to settings
                            AppManager am = new AppManager(Globals.DBConnectionKey, "web", Request.Headers?.Authorization?.Parameter);

                            var setting = new Setting()
                            {   // so when we query back. value == invenToryItem.UUID 
                                Value = UUID,
                                Name = type,
                                AccountUUID = this.CurrentUser.AccountUUID,
                                Type = SettingFlags.Types.String,
                                AppType = "web",
                                Image = pathToImage,
                                UUIDType = "ImagePath",
                                DateCreated = DateTime.UtcNow,
                                CreatedBy = CurrentUser.UUID,
                                RoleOperation = ">=",
                                RoleWeight = 1,
                                Private = false,
                            };
                            am.Insert(setting, "");
                        }
                       
                        fileResult.UUID = UUID;
                        fileResult.UUIDType = type;
                        fileResult.Status = "saved";
                        fileResult.Image = fullUrl + ImageUrl;
                        fileResult.ImageThumb = fullUrl + ImageEx.GetThumbFileName(destFile); //todo check this
                        fileResult.Name = fileName;
                        
                        return ServiceResponse.OK(fileName + " uploaded.", fileResult);
                    }
                );
              return res;
            }catch(Exception ex)
            {
                _logger.InsertError(ex.Message, "InventoryController", "PostFile");
                return ServiceResponse.Error("Upload failed.");
            }
        }

        public void UpdateImageURL(string uuid, string type, string imageURL)
        {
            switch (type.ToUpper())
            {
                case "ACCOUNT":
                    AccountManager am = new AccountManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
                    Account a = (Account)am.Get(uuid);
                    if (a != null)
                    {
                        a.Image = imageURL;
                        am.Update(a);
                    }
                    break;
                case "USER":
                    UserManager  um = new UserManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
                    User u = (User)um.Get(uuid);
                    if (u != null)
                    {
                        u.Image = imageURL;
                        um.UpdateUser(u, true);
                    }
                    break;
                case "ITEM":
                    InventoryManager im = new InventoryManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
                    InventoryItem i = (InventoryItem)im.Get(uuid);
                    if(i!= null)
                    {
                        i.Image = imageURL;
                        im.Update(i);
                    }
                   break;

                case "PLANT":
                case "BALLAST":
                case "BULB":
                case "CUSTOM":
                case "FAN":
                case "FILTER":
                case "PUMP":
                case "VEHICLE":
                    Debug.Assert(false, "TODO MAKE SURE CORRECT TABLE IS UPDATED");
                    EquipmentManager em = new EquipmentManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
                    dynamic d = em.GetAll(type)?.FirstOrDefault(w => w.UUID == uuid);
                    if(d != null)
                    {
                        d.Image = imageURL;
                        em.Update(d);
                    }
                    break;
            }
        }
    }
}
 