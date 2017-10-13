// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using System.IO;

namespace TreeMon.Utilites.Helpers
{
    public class EnvironmentEx
    {
        protected EnvironmentEx() { }

        public static string AppDataFolder
        {
            get
            {
                string tmp = AppDomain.CurrentDomain.BaseDirectory.Replace("bin\\Debug", "").Replace("bin\\Release", "");
                return Path.Combine(tmp, "App_Data");
            }
        }

    }
}
