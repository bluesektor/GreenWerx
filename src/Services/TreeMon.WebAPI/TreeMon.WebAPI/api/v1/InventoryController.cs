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
using System.Threading.Tasks;
using System.Web.Http;
using TreeMon.Data.Logging;
using TreeMon.Data.Logging.Models;
using TreeMon.Managers;
using TreeMon.Managers.Equipment;
using TreeMon.Managers.Inventory;
using TreeMon.Managers.Store;
using TreeMon.Models.App;
using TreeMon.Models.Datasets;
using TreeMon.Models.Geo;
using TreeMon.Models.Inventory;
using TreeMon.Utilites.Extensions;
using TreeMon.Utilites.Helpers;
using TreeMon.Web;
using TreeMon.Web.api;
using TreeMon.Web.Filters;
using TreeMon.WebAPI.Models;

namespace TreeMon.WebAPI.api.v1
{
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
            return inventoryManager.Insert(n, true);
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 1)]
        [HttpPost]
        [HttpGet]
        [Route("api/Inventory/{name}")]
        public ServiceResult Get(string name )
        {
            if (string.IsNullOrWhiteSpace(name))
                return ServiceResponse.Error("You must provide a name for the strain.");

            InventoryManager inventoryManager = new InventoryManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            InventoryItem p = (InventoryItem)inventoryManager.Get(name);

            if (p == null)
                return ServiceResponse.Error("Inventory Item could not be located for the name " + name);

            return ServiceResponse.OK("", p);
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 1)]
        [HttpPost]
        [HttpGet]
        [Route("api/InventoryBy/{uuid}")]
        public ServiceResult GetBy(string uuid)
        {
            if (string.IsNullOrWhiteSpace(uuid))
                return ServiceResponse.Error("You must provide a name for the strain.");

            InventoryManager inventoryManager = new InventoryManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            InventoryItem p = (InventoryItem)inventoryManager.Get(uuid);

            if (p == null)
                return ServiceResponse.Error("Inventory Item could not be located for the uuid " + uuid);

            return ServiceResponse.OK("", p);
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [HttpPost]
        [HttpGet]
        [Route("api/Inventory")]
        public ServiceResult GetItem(string filter = "")
        {
            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");
           
            InventoryManager inventoryManager = new InventoryManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            List<dynamic> Inventory = (List<dynamic>)inventoryManager.GetItems(CurrentUser.AccountUUID).Cast<dynamic>().ToList();
          int count;

                            DataFilter tmpFilter = this.GetFilter(filter);
                Inventory = FilterEx.FilterInput(Inventory, tmpFilter, out count);
            
            return ServiceResponse.OK("", Inventory, count);
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [HttpPost]
        [HttpGet]
        [Route("api/Inventory/Location/{locationUUID}")]
        public ServiceResult GetItemsForLocation(string locationUUID, string filter = "")
        {
            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");
           
            InventoryManager inventoryManager = new InventoryManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            List<dynamic> Inventory = (List<dynamic>)inventoryManager.GetItems(CurrentUser.AccountUUID).Cast<dynamic>().ToList();

            Inventory = Inventory.Where(w => w.LocationUUID == locationUUID &&
                                w.Deleted == false )                               
                    .Cast<dynamic>().ToList();

            int count;

                            DataFilter tmpFilter = this.GetFilter(filter);
                Inventory = FilterEx.FilterInput(Inventory, tmpFilter, out count);
            return ServiceResponse.OK("", Inventory, count);
        }


     


        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 1)]
        [HttpPost]
        [HttpGet]
        [Route("api/Inventory/InventoryType/{type}/")]
        public ServiceResult GetLocatonsByInventoryType(string type, string filter = "")
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

            DataFilter tmpFilter = this.GetFilter(filter);
       
                locations = FilterEx.FilterInput(locations, tmpFilter, out count);
            if (locations == null || locations.Count == 0)
                return ServiceResponse.Error("No locations available.");

            return ServiceResponse.OK("", locations, count);

        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [HttpPost]
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
        [HttpPost]
        [HttpDelete]
        [Route("api/Inventory/Delete/{inventoryItemUUID}")]
        public ServiceResult Delete(string inventoryItemUUID)
        {
            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

            InventoryManager inventoryManager = new InventoryManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            InventoryItem p = (InventoryItem)inventoryManager.GetBy(inventoryItemUUID);

            if (p == null || string.IsNullOrWhiteSpace(p.UUID))
                return ServiceResponse.Error("Invalid account was sent.");

            return inventoryManager.Delete(p);
        }


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
            var dbP = (InventoryItem)inventoryManager.GetBy(pv.UUID);

            if (dbP == null)
                return ServiceResponse.Error("InventoryItemwas not found.");

          
            dbP.Name = pv.Name;
            dbP.Cost = pv.Cost;
            dbP.Condition = pv.Condition;
            dbP.Quality = pv.Quality;
            dbP.Deleted = pv.Deleted;
            dbP.Rating = pv.Rating;
            dbP.LocationUUID = pv.LocationUUID;
            dbP.LocationType = pv.LocationType;
            dbP.Quantity = pv.Quantity;
            dbP.ReferenceType = pv.ReferenceType;
            dbP.ReferenceUUID = pv.ReferenceUUID;
            dbP.Virtual = pv.Virtual;
            dbP.Published = pv.Published;
            dbP.Link = pv.Link;
            dbP.LinkProperties = pv.LinkProperties;
            return inventoryManager.Update(dbP);
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [HttpPost]
        [HttpPatch]
        [Route("api/Inventory/Updates")]
        public ServiceResult UpdateInventory()
        {
            ServiceResult res = ServiceResponse.OK();
            string root = EnvironmentEx.AppDataFolder;

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
                    var databaseItem = (InventoryItem)inventoryManager.GetBy(changedItem.UUID);

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
                            res.Message += sr.Message + Environment.NewLine;
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
                        res.Message += sru.Message + Environment.NewLine;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Assert(false, ex.Message);
            }
            return res;
        }


        //[HttpPost]
        //[Route("/api/upload")]
        //public async Task Upload(IFormFile file)
        //{
        //    if (file == null) throw new Exception("File is null");
        //    if (file.Length == 0) throw new Exception("File is empty");

        //    using (Stream stream = file.OpenReadStream())
        //    {
        //        using (var binaryReader = new BinaryReader(stream))
        //        {
        //            var fileContent = binaryReader.ReadBytes((int)file.Length);
        //         //   await _uploadService.AddFile(fileContent, file.FileName, file.ContentType);
        //        }
        //    }
        //}
        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [HttpPost]
        [Route("api/File/Upload/{UUID}/{type}")]
        public async Task<ServiceResult> PostFile(string UUID, string type)
        {
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

                string root = System.Web.HttpContext.Current.Server.MapPath("~/Content/Uploads/" + this.CurrentUser.AccountUUID);

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
                        List<string> kvp =  o.Result.Contents.First().Headers.Where(w => w.Key == "Content-Disposition").First().Value.ToList()[0].Split(';').ToList();
                        foreach(string value in kvp)
                        {
                            if(value.Trim().StartsWith("filename"))
                            {
                                String[] tmp = value.Split('=');
                                fileName = tmp[1].Trim().Replace("\"", "");
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

                        File.Move( file, destFile );
                        file = destFile;

                        string thumbFile = ImageEx.CreateThumbnailImage(file, 64);
                        string ImageUrl = fileName;
                        string fullUrl = this.Request.RequestUri.Scheme + "://" + this.Request.RequestUri.Authority + "/Content/Uploads/" + this.CurrentUser.AccountUUID + "/";
                        pathToImage = fullUrl + ImageUrl;
                        
                        this.UpdateImageURL(UUID, type, pathToImage);//Now update the database.
                        string results = "{ ImageUrl: \"" + fullUrl + ImageUrl + "\" , ImageThumbUrl:\"" + fullUrl + thumbFile +"\" }";
                        return ServiceResponse.OK(fileName + " uploaded.", results);
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
                case "ITEM":
                    InventoryManager im = new InventoryManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
                    InventoryItem i = (InventoryItem)im.GetBy(uuid);
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
                    dynamic d = em.GetAll(type).FirstOrDefault(w => w.UUID == uuid);
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