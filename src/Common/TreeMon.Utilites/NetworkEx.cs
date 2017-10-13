// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System.Net;

namespace TreeMon.Utilites
{
    public class NetworkEx
    {
        protected NetworkEx()
        {
        }

        public static long IPAddressToNumber(string ipAddress)
        {
            long num = 0;
            if (string.IsNullOrWhiteSpace(ipAddress))
                return num;
           
            string[] arrDec;
            arrDec = ipAddress.Split('.');
            num = (long.Parse(arrDec[3])) + (long.Parse(arrDec[2]) * 256) + (long.Parse(arrDec[1]) * 65536) + (long.Parse(arrDec[0]) * 16777216);
          
            return num;
        }
    }
}
