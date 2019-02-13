// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TreeMon.Data.Helpers;
using TreeMon.Data.Logging.Models;

namespace TreeMon.Data.Logging
{
    public class SystemLogger 
    {
        private readonly string _dbConnectionKey= null;
       
        public string PathToFileLog { get; set; }
        private string FileLogFolder { get; set; }


        private readonly FileLog _fileLog = new FileLog();

        public bool UseFileLogging { get; set; }

        public SystemLogger(string connectionKey, bool useFileLog = false, string pathToFileLog = "" )
        {
            UseFileLogging = useFileLog;
            FileLogFolder = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug", "") + "App_Data\\Logs\\";

            if (!Directory.Exists(FileLogFolder))
            {
                Directory.CreateDirectory(FileLogFolder);
            }

            if (string.IsNullOrWhiteSpace(pathToFileLog)) {
                PathToFileLog = FileLogFolder + DateTime.UtcNow.Year.ToString() + DateTime.UtcNow.Month.ToString() + DateTime.UtcNow.Day.ToString() + ".log";
            }
            if (!string.IsNullOrWhiteSpace(connectionKey))
                _dbConnectionKey = connectionKey;
            else
            {
                if (!File.Exists(PathToFileLog))
                {
                    try
                    {
                        File.Create(PathToFileLog);
                    }
                    catch (Exception ex) {
                        Debug.Assert(false, ex.Message);
                    }
                }
                UseFileLogging = true;
            }
        }

        public  int Delete(LogEntry l)
        {
            using (var context = new TreeMonDbContext(_dbConnectionKey))
            {
                return context.Delete<LogEntry>(l);
            }
        }

        public int DeleteAll()
        {
            using (var context = new TreeMonDbContext(_dbConnectionKey))
            {
                string table = DatabaseEx.GetTableName<LogEntry>();
                return context.ExecuteNonQuery("DELETE FROM " + table, null);
            }
        }

        public LogEntry Get(int id)
        {
            using (var context = new TreeMonDbContext(_dbConnectionKey))
            {
                return context.Get<LogEntry>(id);
            }
        }

        public IEnumerable<LogEntry> GetAll()
        {
            using (var context = new TreeMonDbContext(_dbConnectionKey))
            {
                return context.GetAll<LogEntry>();
            }
        }

        public bool Insert(LogEntry l)
        {
            if (UseFileLogging)
            {
                _fileLog.LogFile(PathToFileLog);
                Exception ex = _fileLog.Write(
                    l.LogDate.ToString() + "," +
                     l.Level + "," +
                     l.StackTrace + "," +
                     l.Source + "," +
                     l.InnerException);

                if (ex == null)
                    return true;
                else
                    return false;
            }
            using (var context = new TreeMonDbContext(_dbConnectionKey))
            {
                return context.Insert<LogEntry>(l);
            }
        }

        protected bool Insert(string message, string source, string function, string level)
        {
            return this.Insert(
            new LogEntry()
            {
                StackTrace = function,
                Source = source,
                InnerException = message,
                Level = level,
                LogDate = DateTime.UtcNow
                //Type
            });
        }

        public async Task<bool> InsertAsync(LogEntry l)
        {
            if (UseFileLogging)
            {
                _fileLog.LogFile(PathToFileLog);
                Exception ex = _fileLog.Write(
                    l.LogDate.ToString() + "," +
                     l.Level + "," +
                     l.StackTrace + "," +
                     l.Source + "," +
                     l.InnerException);

                if (ex == null)
                    return true;
                else
                    return false;
            }

            using (var context = new TreeMonDbContext(_dbConnectionKey))
            {
                return await context.InsertAsync<LogEntry>(l) > 0 ? true : false;
            }

        }

        protected async Task<bool> InsertAsync(string message, string source, string function, string level)
        {
            return  await this.InsertAsync(
            new LogEntry()
            {
                StackTrace = function,
                Source = source,
                InnerException = message,
                Level = level,
                LogDate = DateTime.UtcNow
                //Type
            });
        }

        public bool InsertException(Exception ex, string source, string function, string exData="")
        {
          
            string MachineName = System.Environment.MachineName + Environment.NewLine;
            string user = System.Environment.UserDomainName + Environment.NewLine;
            string exception = GetFullException(ex) + Environment.NewLine;
            string exceptionData = GetExceptionData(ex) + Environment.NewLine;

            return this.Insert(
            new LogEntry()
            {
                StackTrace = function,
                Source = source,
                InnerException = string.Format("{0} {1} {2} ", exception, exceptionData, exData),
                Level = SystemFlag.Level.Exception,
                LogDate = DateTime.UtcNow,
                Type = MachineName,
                User = user
                
            });

        }

        private static string GetExceptionData(Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            foreach (System.Collections.DictionaryEntry entry in ex.Data)
            {
                sb.AppendLine($"Key={entry.Key.ToString()} : \tValue={entry.Value.ToString()}");
            }
            return sb.ToString();
        }

        private static string GetFullException(System.Exception ex)
        {
            #region requires pdb. The line will be 0 if no pdb. See below for generating pdb in release build
            // Get stack trace for the exception with source file information
            var st = new StackTrace(ex, true);
            // Get the top stack frame
            var frame = st.GetFrame(st.FrameCount - 1);
            
            // Get the line number from the stack frame
            //To generate PDB files for RELEASE builds by going to Project Properties-- > Package / Publish Web-- > uncheck Exclude generated debug symbols
            var line = frame.GetFileLineNumber();
            #endregion

            StringBuilder sb = new StringBuilder("Line Number:" + line);
            sb.Append("ExceptionType:" + ex.GetType().ToString() + Environment.NewLine);
            sb.Append("InnerException:" + ( (ex.InnerException == null) ? null : GetFullException(ex.InnerException) + Environment.NewLine));
            sb.Append("Message:" + ex.Message + Environment.NewLine);
            sb.Append("Source:" + ex.Source + Environment.NewLine);
            sb.Append("StackTrace:" + ex.StackTrace + Environment.NewLine);
            return sb.ToString();
        }

        //DEBUG - The DEBUG Level designates fine-grained informational events
        //that are most useful to debug an application.
        //
        public bool InsertDebug(string message, string source, string function)
        {
            return Insert(message, source, function, SystemFlag.Level.Debug);
        }

        // ERROR - The ERROR level designates error events that might
        //still allow the application to continue running.
        public bool InsertError(string message, string source, string function)
        {
            return Insert(message, source, function, SystemFlag.Level.Error);
        }

        //  FATAL - The FATAL level designates very severe error events 
        //that will presumably lead the application to abort. 
        public bool InsertFatal(string message, string source, string function)
        {
            return Insert(message, source, function, SystemFlag.Level.Fatal);
        }

        //INFO - The INFO level designates informational messages that highlight
        //the progress of the application at coarse-grained level.
        public bool InsertInfo(string message, string source, string function)
        {
            return Insert(message, source, function, SystemFlag.Level.Info);
        }

        /// <summary>
        /// SECURITY - This is to log events that may comprimise system security.
        /// public const string SecurityLevel = "SECURITY";
        /// </summary>
        /// <param name="message"></param>
        /// <param name="source"></param>
        /// <param name="function"></param>
        /// <returns></returns>
        public bool InsertSecurity(string message, string source, string function)
        {
            return Insert(message, source, function, SystemFlag.Level.Security);
        }

        public async Task<bool> InsertSecurityAsync(string message, string source, string function)
        {
            return  await InsertAsync(message, source, function, SystemFlag.Level.Security);
        }

        // WARN - The WARN level designates potentially harmful situations.
        //
        public bool InsertWarning(string message, string source, string function)
        {
            return Insert(message, source, function, SystemFlag.Level.Warning);
        }
    }
}
