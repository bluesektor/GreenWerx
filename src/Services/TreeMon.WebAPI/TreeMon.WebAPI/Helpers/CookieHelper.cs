// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System.Linq;
using System.Web;

namespace TreeMon.Web.Helpers
{
    public class CookieHelper
    {
        public static string GetValue(HttpCookieCollection cookies, string key)
        {
            if (!cookies.AllKeys.Contains(key))
                return string.Empty;


            HttpCookie cookie = cookies.Get(key);

            //// Check if the cookie exists
            if (cookie == null)
            {
                return string.Empty;
            }
            return cookie.Value; 
        }
    }
}