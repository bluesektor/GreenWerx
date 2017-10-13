// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
namespace TreeMon.Data.Logging.Models
{
    public class SystemFlag
    {
        public struct Level
        {
            //DEBUG - The DEBUG Level designates fine-grained informational events
            //that are most useful to debug an application.
            //
            public const string Debug = "DEBUG";

            // ERROR - The ERROR level designates error events that might
            //still allow the application to continue running.
            //
            public const string Error = "ERROR";

            // EXCEPTION - Something blew up. Most likely bad.
            /// 
            public const string Exception = "EXCEPTION";

            //  FATAL - The FATAL level designates very severe error events
            //that will presumably lead the application to abort.
            //
            public const string Fatal = "FATAL";

            //INFO - The INFO level designates informational messages that highlight
            //the progress of the application at coarse-grained level.
            //
            public const string Info = "INFO";

            //SECURITY - This is to log events that may comprimise system security.
            public const string Security = "SECURITY";

            // WARN - The WARN level designates potentially harmful situations.
            //
            public const string Warning = "WARN";


        }

        public struct Default
        {
            //this is generic value for data imported via process that are used for
            //default data (non user generated). User should NOT delete these!
            //
            public const string Account = "system.default.account";

            public const string AppType = "system.default.app";
        }
    }
}
