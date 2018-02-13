// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TreeMon.Data;
using TreeMon.Models;
using TreeMon.Models.App;
using TreeMon.Models.Logging;
using TreeMon.Utilites.Extensions;

namespace TreeMon.Managers
{
    public class EmailLogManager: ICrud
    {
        private readonly string _dbConnectionKey;

        public EmailLogManager(string connectionKey)
    {
        Debug.Assert(!string.IsNullOrWhiteSpace(connectionKey), "EmailLogManager CONTEXT IS NULL!");

        
             _dbConnectionKey = connectionKey;
    }

        public ServiceResult Delete(INode n, bool purge = false)
        {
            ServiceResult res = ServiceResponse.OK();
            if (n == null)
                return ServiceResponse.Error("No record sent.");

            var s = (EmailLog)n;

            if (purge)
            {
                using (var context = new TreeMonDbContext(_dbConnectionKey))
                {
                    if (context.Delete<EmailLog>(s) == 0)
                        return ServiceResponse.Error(s.Name + " failed to delete. ");
                }
            }

            //get the EmailLog from the table with all the data so when its updated it still contains the same data.
            s = (EmailLog)this.Get(s.UUID);
            if (s == null)
                    return ServiceResponse.Error("Email log not found");

            s.Deleted = true;
            using (var context = new TreeMonDbContext(_dbConnectionKey))
            {
                if (context.Update<EmailLog>(s) == 0)
                    return ServiceResponse.Error(s.Name + " failed to delete. ");
            }
            return res;
        }

        public List<EmailLog> GetEmailLogs(string accountUUID, bool deleted = false)
        {
            using (var context = new TreeMonDbContext(_dbConnectionKey))
            {

                return context.GetAll<EmailLog>().Where(sw => (sw.AccountUUID == accountUUID) && sw.Deleted == deleted).OrderBy(ob => ob.DateSent).ToList();
            }
        }

        public List<EmailLog> Search(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return new List<EmailLog>();

            using (var context = new TreeMonDbContext(_dbConnectionKey))
            {
                return context.GetAll<EmailLog>().Where(sw => sw.Name.EqualsIgnoreCase(name)).ToList();
            }
        }


        public INode Get(string uuid)
        {
            if (string.IsNullOrWhiteSpace(uuid))
                return null;
            using (var context = new TreeMonDbContext(_dbConnectionKey))
            {
                return context.GetAll<EmailLog>().FirstOrDefault(sw => sw.UUID == uuid);
            }
        }

        /// <summary>
        /// Validate param not implemented
        /// </summary>
        /// <param name="n"></param>
        /// <param name="validateFirst"></param>
        /// <returns></returns>
        public ServiceResult Insert(INode n)
    {
            if (n == null)
                return ServiceResponse.Error("No record sent.");

            n.Initialize("", "", 5);

            var s = (EmailLog)n;
         
            using (var context = new TreeMonDbContext(_dbConnectionKey))
            {
                if (context.Insert<EmailLog>(s))
                    return ServiceResponse.OK("",s);
            }
            return ServiceResponse.Error("An error occurred inserting EmailLog " );
        }

        public ServiceResult Update(INode n)
        {
            if (n == null)
                return ServiceResponse.Error("Invalid EmailLog data.");
            var s = (EmailLog)n;
            using (var context = new TreeMonDbContext(_dbConnectionKey))
            {
                if (context.Update<EmailLog>(s) > 0)
                    return ServiceResponse.OK();
            }
            return ServiceResponse.Error("System error, EmailLog was not updated.");
        }

    }
}
