// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using System.Net;
using System.Net.Http;
using System.Web;

namespace TreeMon.Web.api.Helpers
{
    public class NetworkHelper
    {

         readonly private UInt64[][] _privateIps = new UInt64[][] {
          new UInt64[] { ConvertIPToLong("0.0.0.0"), ConvertIPToLong("2.255.255.255")},
          new UInt64[] {ConvertIPToLong("10.0.0.0"), ConvertIPToLong("10.255.255.255")},
          new UInt64[] {ConvertIPToLong("127.0.0.0"), ConvertIPToLong("127.255.255.255")},
          new UInt64[] {ConvertIPToLong("169.254.0.0"), ConvertIPToLong("169.254.255.255")},
          new UInt64[] {ConvertIPToLong("172.16.0.0"), ConvertIPToLong("172.31.255.255")},
          new UInt64[] {ConvertIPToLong("192.0.2.0"), ConvertIPToLong("192.0.2.255")},
          new UInt64[] {ConvertIPToLong("192.168.0.0"), ConvertIPToLong("192.168.255.255")},
          new UInt64[] {ConvertIPToLong("255.255.255.0"), ConvertIPToLong("255.255.255.255")}
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
                UInt64 ipToLong;
                //Is it valid IP address
                if (TryConvertIP(ip, out ipToLong))
                {
                    //Does it fall within a private network range
                    foreach (UInt64[] privateIp in _privateIps)
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

        public static bool TryConvertIP(string ip, out UInt64 ipResult )
        {
            ipResult = 0;
            try
            {
                ipResult = ConvertIPV6(ip);
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// https://lite.ip2location.com/
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        private static UInt64 ConvertIPV6(string ip)
        {
            string strIP = ip; //"2404:6800:4001:805::1006";
            System.Net.IPAddress address;
            UInt64 ipnum = 0;

            if (!System.Net.IPAddress.TryParse(strIP, out address))
                return 0;

             byte[] addrBytes = address.GetAddressBytes();

             if (System.BitConverter.IsLittleEndian)
             {
                 System.Collections.Generic.List<byte> byteList = new System.Collections.Generic.List<byte>(addrBytes);
                 byteList.Reverse();
                 addrBytes = byteList.ToArray();
             }

             if (addrBytes.Length > 8)
             {
                 //IPv6
                 ipnum = System.BitConverter.ToUInt64(addrBytes, 8);
                 ipnum <<= 64;
                 ipnum += System.BitConverter.ToUInt64(addrBytes, 0);
             }
             else
             {
                 ////IPv4
                 ipnum = System.BitConverter.ToUInt32(addrBytes, 0);
             }

            return ipnum;
        }

        public static float GetIpVersion(string ip)
        {
            if (string.IsNullOrWhiteSpace(ip))
                return 0;
                        
            System.Net.IPAddress address;
       
            if (!System.Net.IPAddress.TryParse(ip, out address))
                return 0;

            byte[] addrBytes = address.GetAddressBytes();

            if (System.BitConverter.IsLittleEndian)
            {
                System.Collections.Generic.List<byte> byteList = new System.Collections.Generic.List<byte>(addrBytes);
                byteList.Reverse();
                addrBytes = byteList.ToArray();
            }

            if (addrBytes.Length > 8)
                return 6;
            else
                return 4;
        }

        private static UInt64 ConvertIPToLong(string ip)
        {
            UInt64 res;
            TryConvertIP(ip, out res);
            return res;
           // string[] ipSplit = ip.Split('.');            return (16777216 * Convert.ToInt32(ipSplit[0]) + 65536 * Convert.ToInt32(ipSplit[1]) + 256 * Convert.ToInt32(ipSplit[2]) + Convert.ToInt32(ipSplit[3]));
        }
     


    }

}