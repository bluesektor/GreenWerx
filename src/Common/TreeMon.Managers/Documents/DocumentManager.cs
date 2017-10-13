// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using System.Diagnostics;
using System.IO;
using TreeMon.Data.Logging;
using TreeMon.Models.App;
using TreeMon.Utilites.Helpers;

namespace TreeMon.Managers.Documents
{
    public  class DocumentManager : BaseManager
    {
        private SystemLogger _logger = null;
        string _sessionKey = "";

        public DocumentManager(string connectionKey, string sessionKey) : base(connectionKey, sessionKey)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(connectionKey), "DocumentManager CONTEXT IS NULL!");
            this._connectionKey = connectionKey;
            _logger = new SystemLogger(connectionKey);
            _sessionKey = sessionKey;
        }

        public ServiceResult GetTemplate(string templateKey)
        {
            //Try looking up the setting in the database..
            AppManager am = new AppManager(this._connectionKey, "web", this._sessionKey);
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
    }
}
