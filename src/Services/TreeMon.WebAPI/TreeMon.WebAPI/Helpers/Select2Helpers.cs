// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using System.Collections.Generic;
using TreeMon.Models;

namespace TreeMon.Web.Helpers
{
    public class JtableOption
    {
        public string DisplayText { get; set; }
        public string Value { get; set; }

        
    }

    //Extra classes to format the results the way the select2 dropdown wants them
    public class Select2PagedResult
    {
        public int Total { get; set; }
        public List<Select2Result> Results { get; set; }
    }

    public class Select2Result
    {
        public string id { get; set; }
        public string text { get; set; }
    }

    
    public static class ListFormat
    {
        //List<T> should be paginaged.
        //totalListCount is the listcount before the pagination split
        //
        public static Select2PagedResult ToSelect2<T>(this List<T> pagedList, int totalListCount)
        {
            if (pagedList == null)
                return new Select2PagedResult();

            Select2PagedResult pagedResult = new Select2PagedResult();
            pagedResult.Results = new List<Select2Result>();

            //Loop through  and translate it into a text value and an id for the select list
            //
           for(int i = 0; i <= pagedList.Count; i++)
            {
                Node a = null;
                try
                {
                    a = (Node) Convert.ChangeType(pagedList[i], typeof(T));
                }
                catch
                {
                    continue;
                }
                if (a == null)
                    continue;

                pagedResult.Results.Add(new Select2Result { id = a.UUID.ToString(), text = a.Name});
            }
            //Set the total count of the results from the query.
            pagedResult.Total = totalListCount;

            return pagedResult;
        }

        public static Select2PagedResult ToSelect2(this List<dynamic> pagedList, int totalListCount)
        {
            if (pagedList == null)
                return new Select2PagedResult();

            Select2PagedResult pagedResult = new Select2PagedResult();
            pagedResult.Results = new List<Select2Result>();

            //Loop through our list and translate it into a text value and an id for the select list
            foreach(dynamic a in pagedList)
            {
                if (a == null)
                    continue;

                pagedResult.Results.Add(new Select2Result { id = a.UUID.ToString(), text = a.Name });
            }
            //Set the total count of the results from the query.
            pagedResult.Total = totalListCount;

            return pagedResult;
        }



        public static List<JtableOption> ToJtableSelect<T>(this List<T> pagedList, int totalListCount)
        {
            if (pagedList == null)
                return new List<JtableOption>();

            List<JtableOption> options = new List<JtableOption>();
           

            for (int i = 0; i <= pagedList.Count; i++)
            {
                Node a = null;
                try
                {
                    a = (Node)Convert.ChangeType(pagedList[i], typeof(T));
                }
                catch
                {
                    continue;
                }
                if (a == null)
                    continue;

                options.Add(new JtableOption {  Value =  a.UUID.ToString(),  DisplayText = a.Name });
            }
            return options;
        }
    }
}