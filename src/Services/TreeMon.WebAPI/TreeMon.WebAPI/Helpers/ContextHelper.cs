// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TreeMon.WebAPI.Helpers
{
    public class ContextHelper
    {
        protected ContextHelper() { }


        //Only use this when exceptions in the api.
        //
        public static string GetContextData()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            if (HttpContext.Current != null)
            {
                //Session
                if (HttpContext.Current.Session != null)
                {
                    foreach (string key in HttpContext.Current.Session.Keys)
                    {
                        sb.AppendLine($"Session: Key:{key} Value:{HttpContext.Current.Session[key]}");
                    }
                }

                //Cookie
                if (HttpContext.Current.Request.Cookies != null)
                {
                    foreach (string cookie in HttpContext.Current.Request.Cookies)
                    {
                        sb.AppendLine($"Cookie: Key:{HttpContext.Current.Request.Cookies[cookie].Name} Value:{HttpContext.Current.Request.Cookies[cookie].Value}");
                    }
                }


                //Form  
                if (HttpContext.Current.Request.Form != null)
                {
                    foreach (string item in HttpContext.Current.Request.Form)
                    {
                        sb.AppendLine($"Form: Key:{item} Value:{HttpContext.Current.Request.Form[item]}");
                    }
                }

                //URL 
                string[] urlParms = HttpContext.Current.Request.Url.Query.Split('&');
                if (urlParms.Length > 0)
                {
                    foreach (string parm in urlParms)
                    {
                        string[] de = parm.Split('=');
                        string val = de.Length == 2 ? de[1] : string.Empty;
                        sb.AppendLine($"URL: Key:{de[0]} Value:{val}");
                    }
                }

                sb.AppendLine($"Method: {HttpContext.Current.Request.HttpMethod}");
                sb.AppendLine($"IsAuthenticated: {HttpContext.Current.Request.IsAuthenticated}");
                sb.AppendLine($"IsLocal: {HttpContext.Current.Request.IsLocal}");
                sb.AppendLine($"IsSecureConnection: {HttpContext.Current.Request.IsSecureConnection}");
                sb.AppendLine($"RequestType: {HttpContext.Current.Request.RequestType}");
                sb.AppendLine($"Url: {HttpContext.Current.Request.Url}");
                sb.AppendLine($"UrlReferrer: {HttpContext.Current.Request.UrlReferrer}");
                sb.AppendLine($"UserAgent: {HttpContext.Current.Request.UserAgent}");
                sb.AppendLine($"UserHostAddress: {HttpContext.Current.Request.UserHostAddress}");

                if (HttpContext.Current.Request.ServerVariables.Count > 0)
                {
                    foreach (string sVar in HttpContext.Current.Request.ServerVariables)
                    {
                        if (sVar != "ALL_HTTP" && sVar != "ALL_RAW" && sVar != "AUTH_PASSWORD")
                            sb.AppendLine($"Server: Key:{sVar} Value:{HttpContext.Current.Request.ServerVariables[sVar]}");
                    }
                }
            }

            return sb.ToString();
        }

    }
}