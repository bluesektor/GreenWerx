// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System.Collections.Generic;

namespace TreeMon.Models.Datasets
{
    public class DataFilter
    {
        public DataFilter()
        {
            PageResults = true;
            StartIndex = 0;
            PageSize = 25;
            Screens = new List<DataScreen>();
            UserRoleWeight = 0;
            IncludeDeleted = false;
            IncludeDeleted = false;
        }
        public bool PageResults { get; set; }

        public int StartIndex { get; set; }

        public int PageSize { get; set; }

        //these are initial sorts, additional sorting can be
        //added to the screens.
        public string SortBy { get; set; }

        public string SortDirection { get; set; }

        public List<DataScreen> Screens{ get; set; }

        //NOTE:The requesting user RoleWeight should be set before calling FilterInput
        public int UserRoleWeight { get; set; }

        public string TimeZone { get; set; } // todo

        public bool IncludePrivate { get; set; }   // todo

        public bool  IncludeDeleted { get; set; }  // todo


    }
}
