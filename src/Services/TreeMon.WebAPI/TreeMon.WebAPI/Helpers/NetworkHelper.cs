// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using System.Net.Http;
using System.Web;

namespace TreeMon.Web.api.Helpers
{
    public class NetworkHelper
    {

         readonly private long[][] _privateIps = new long[][] {
          new long[] { ConvertIPToLong("0.0.0.0"), ConvertIPToLong("2.255.255.255")},
          new long[] {ConvertIPToLong("10.0.0.0"), ConvertIPToLong("10.255.255.255")},
          new long[] {ConvertIPToLong("127.0.0.0"), ConvertIPToLong("127.255.255.255")},
          new long[] {ConvertIPToLong("169.254.0.0"), ConvertIPToLong("169.254.255.255")},
          new long[] {ConvertIPToLong("172.16.0.0"), ConvertIPToLong("172.31.255.255")},
          new long[] {ConvertIPToLong("192.0.2.0"), ConvertIPToLong("192.0.2.255")},
          new long[] {ConvertIPToLong("192.168.0.0"), ConvertIPToLong("192.168.255.255")},
          new long[] {ConvertIPToLong("255.255.255.0"), ConvertIPToLong("255.255.255.255")}
        };

        /// <summary>
        /// This is called from api controllers (derived from ApiController ).
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public string GetClientIpAddress(HttpRequestMessage request)
        {
            string ip = "";
            if (request.Properties.ContainsKey("MS_HttpContext"))
            {
                var ctx = request.Properties["MS_HttpContext"] as HttpContextWrapper;
                if (ctx != null)
                {
                    ip = ctx.Request.UserHostAddress;
                }
            }
            return ip;
        }

        /// <summary>
        /// Called from MVC controllers ( derived from Controller ).
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public string GetClientIpAddress(HttpRequestBase request)
        {
            string strIpAddress;
            if (request == null)
                return "bogon";
   
            strIpAddress = request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (string.IsNullOrEmpty(strIpAddress))
                strIpAddress = request.ServerVariables["REMOTE_ADDR"];

            if (strIpAddress == "::1")
                strIpAddress = "127.0.0.1";

            if (string.IsNullOrEmpty(strIpAddress))
                return "bogon";
 
            return strIpAddress;

        }

        private bool CheckIP(string ip)
        {
            if (!String.IsNullOrEmpty(ip))
            {
                long ipToLong;
                //Is it valid IP address
                if (TryConvertIPToLong(ip, out ipToLong))
                {
                    //Does it fall within a private network range
                    foreach (long[] privateIp in _privateIps)
                        if ((ipToLong >= privateIp[0]) && (ipToLong <= privateIp[1]))
                            return false;
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }

        private bool TryConvertIPToLong(string ip, out long ipToLong)
        {
            try
            {
                ipToLong = ConvertIPToLong(ip);
                return true;
            }
            catch
            {
                ipToLong = -1;
                return false;
            }
        }

        private static long ConvertIPToLong(string ip)
        {
            string[] ipSplit = ip.Split('.');
            return (16777216 * Convert.ToInt32(ipSplit[0]) + 65536 * Convert.ToInt32(ipSplit[1]) + 256 * Convert.ToInt32(ipSplit[2]) + Convert.ToInt32(ipSplit[3]));
        }
     


    }

}