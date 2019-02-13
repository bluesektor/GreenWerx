// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using System.Diagnostics;
using System.Linq;
using TreeMon.Data;
using TreeMon.Data.Logging;
using TreeMon.Models;
using TreeMon.Models.App;
using TreeMon.Models.Logging;

namespace TreeMon.Managers.Logging
{
    public class LoggingManager : BaseManager, ICrud
    {
       
        private SystemLogger _logger = null;

        public LoggingManager(string connectionKey)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(connectionKey), "LoggingManager CONTEXT IS NULL!");

            _connectionKey = connectionKey;
            _logger = new SystemLogger(_connectionKey);
        }

        public ServiceResult Insert(INode n) 
        {
            if (n == null)
                return ServiceResponse.Error("Invalid log data.");

           // if (!this.DataAccessAuthorized(n, "POST", false)) return ServiceResponse.Error("You are not authorized this action.");

           // n.Initialize(this._requestingUser.UUID, this._requestingUser.AccountUUID, this._requestingUser.RoleWeight);

            var a = (AccessLog)n;

            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                if (context.Insert<AccessLog>(a))
                    return ServiceResponse.OK("", a);
            }
            return ServiceResponse.Error("System error, log was not added.");
        }

       public ServiceResult Delete(INode n, bool purge = false)
        {
            throw new NotImplementedException();
        }

        public INode Get(string uuid)
        {
            throw new NotImplementedException();
        }


        public ServiceResult Update(INode n)
        {
            throw new NotImplementedException();
        }
    }
}
