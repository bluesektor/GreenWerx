// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using TreeMon.Data.Logging;
using TreeMon.Models;
using TreeMon.Models.App;
using TreeMon.Utilites.Extensions;
using TreeMon.Utilites.Helpers;

namespace TreeMon.Managers.Documents
{
    public  class DocumentManager : BaseManager
    {
        private SystemLogger _logger = null;
      

        public DocumentManager(string connectionKey, string sessionKey) : base(connectionKey, sessionKey)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(connectionKey), "DocumentManager CONTEXT IS NULL!");
            this._connectionKey = connectionKey;
            _logger = new SystemLogger(connectionKey);
            SessionKey = sessionKey;
        }

        public ServiceResult GetTemplate(string templateKey)
        {
            //Try looking up the setting in the database..
            AppManager am = new AppManager(this._connectionKey, "web", this.SessionKey);
            string pathToTemplate   = am.GetSetting(templateKey)?.Value;
            

            if (!string.IsNullOrWhiteSpace(pathToTemplate))
            {
                if (pathToTemplate.StartsWith("App_Data\\"))
                    pathToTemplate = pathToTemplate.Replace("App_Data\\", "");

                pathToTemplate = Path.Combine(EnvironmentEx.AppDataFolder, pathToTemplate);
            }
            //try default location..
            if (string.IsNullOrWhiteSpace(pathToTemplate))
                pathToTemplate = Path.Combine(EnvironmentEx.AppDataFolder, "Templates\\" + templateKey + ".html");

            if(string.IsNullOrEmpty(pathToTemplate))
                return ServiceResponse.Error("The applications template key:" + templateKey + " is not set in the config file.");

            pathToTemplate = pathToTemplate.Replace("\\bin\\Debug", "");

            if (!File.Exists(pathToTemplate))
                return ServiceResponse.Error("The applications template file" + pathToTemplate + " for key:" + templateKey + " is missing.");

            string template = "";
            try
            {
                template = File.ReadAllText(pathToTemplate);
            }
            catch (Exception ex)
            {
                _logger.InsertError(ex.Message, "DocumentManager", "GetTemplate:" + templateKey);
                return ServiceResponse.Error(ex.Message);
            }
            return ServiceResponse.OK("", template);
        }

        public ServiceResult DeleteImages(INode n, string userUploadsFolder )
        {
            if (n == null || string.IsNullOrWhiteSpace(userUploadsFolder))
                return ServiceResponse.Error("Bad request, invalid delete parameters.");

            if (!Directory.Exists(userUploadsFolder))
                return ServiceResponse.OK();//folder doesn't exist for user so nothing has been uploaded.

            if (!this.DataAccessAuthorized(n, "DELETE", false)) return ServiceResponse.Error("You are not authorized this action.");

            string pathToFile = userUploadsFolder + StringEx.Substring(n.Image, _requestingUser.UUID, string.Empty);
            try
            {
                if (File.Exists(pathToFile))
                {
                    File.Delete(pathToFile);

                    FileInfo fi = new FileInfo(pathToFile);
                    string pathToThumb = pathToFile.Replace(fi.Extension, ".tmb" + fi.Extension);
                    if (File.Exists(pathToThumb))
                        File.Delete(pathToThumb);
                }
            }
            catch(Exception ex ){
                Debug.Assert(false, ex.Message);
                //todo log this
            }

            // now get settings images for this item

            AppManager am = new AppManager(this._connectionKey, "web", this.SessionKey);

            string type = n.GetType().Name; //todo make sure this is the same type in InventoryController.PostFile
            List<Setting> settings = am.GetSettings(type)
                .Where(w => w.UUIDType.EqualsIgnoreCase("ImagePath") &&
                       w.Value == n.UUID).ToList();

            if (settings.Count == 0)
                return ServiceResponse.OK();

            foreach (Setting setting in settings)
            {
                try
                {
                    pathToFile = userUploadsFolder + StringEx.Substring(setting.Image, _requestingUser.UUID, string.Empty);
                    if (string.IsNullOrWhiteSpace(pathToFile))
                        continue;

                    if (File.Exists(pathToFile))
                    {
                        File.Delete(pathToFile);
                        FileInfo fi = new FileInfo(pathToFile);
                        string pathToThumb = pathToFile.Replace(fi.Extension, ".tmb" + fi.Extension);

                        if (File.Exists(pathToThumb))
                            File.Delete(pathToThumb);
                    }
                }
                catch (Exception ex)
                {
                    Debug.Assert(false, ex.Message);
                    //todo log this
                }
                am.DeleteSetting(n.UUID);
                
            }

            //var setting = new Setting()
            //{   // so when we query back. value == invenToryItem.UUID 
            //    Value = UUID,
            //    Name = "InventoryItem",
            //    AccountUUID = this.CurrentUser.AccountUUID,
            //    Type = SettingFlags.Types.String,
            //    AppType = "web",
            //    Image = pathToImage,
            //    UUIDType = "ImagePath",
            //    DateCreated = DateTime.UtcNow,
            //    CreatedBy = CurrentUser.UUID,
            //    RoleOperation = ">=",
            //    RoleWeight = 1,
            //    Private = false,
            //};

            //and the thumbnail
            // ".tmb" + fi.Extension
            throw new NotImplementedException();
        }


        public ServiceResult DeleteFile(INode n, string pathToFile )
        {
            if (n == null || string.IsNullOrWhiteSpace(pathToFile))
                return ServiceResponse.Error("Bad request, invalid delete parameters.");

             if (!this.DataAccessAuthorized(n, "DELETE", false)) return ServiceResponse.Error("You are not authorized this action.");

            try
            {
                if (File.Exists(pathToFile))
                {
                    File.Delete(pathToFile);
                    FileInfo fi = new FileInfo(pathToFile);
                    string pathToThumb = pathToFile.Replace(fi.Extension, ".tmb" + fi.Extension);
                    if (File.Exists(pathToThumb))
                        File.Delete(pathToThumb);
                }
            }
            catch (Exception ex)
            {
                Debug.Assert(false, ex.Message);
                //todo log this
                
            }
            return ServiceResponse.OK();
            
        }

      

    }
}
