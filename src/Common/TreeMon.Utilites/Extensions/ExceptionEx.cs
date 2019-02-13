// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeMon.Utilites.Extensions
{
    public static class ExceptionEx
    {
         public static string DeserializeException(this Exception ex, bool includeData = false)
        {
            #region requires pdb. The line will be 0 if no pdb. See below for generating pdb in release build
            // Get stack trace for the exception with source file information
            var st = new StackTrace(ex, true);
            // Get the top stack frame
            var frame = st.GetFrame(st.FrameCount - 1);

            // Get the line number from the stack frame
            //To generate PDB files for RELEASE builds by going to
            //Project Properties-- > Package / Publish Web-- > uncheck Exclude generated debug symbols
            var line = frame.GetFileLineNumber();
            #endregion

            StringBuilder sb = new StringBuilder("Line Number:" + line);
            sb.Append("ExceptionType:" + ex.GetType().ToString() + Environment.NewLine);
            sb.Append("InnerException:" + ((ex.InnerException == null) ? null : DeserializeException(ex.InnerException) + Environment.NewLine));
            sb.Append("Message:" + ex.Message + Environment.NewLine);
            sb.Append("Source:" + ex.Source + Environment.NewLine);
            sb.Append("StackTrace:" + ex.StackTrace + Environment.NewLine);

            if (includeData)
                sb.Append("Data:" + GetExceptionData(ex) + Environment.NewLine);

            return sb.ToString();
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
    }
}
