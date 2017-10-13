// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System.Diagnostics;
using TreeMon.Data.Logging;

namespace TreeMon.Managers.Logging
{
    public class LoggingManager
    {
        public LoggingManager(string connectionKey)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(connectionKey), "LoggingManager CONTEXT IS NULL!");

            
                 _dbConnectionKey = connectionKey;

            _logger = new SystemLogger(_dbConnectionKey);
        }
        private string _dbConnectionKey = null;
        private SystemLogger _logger = null;
    }
}
