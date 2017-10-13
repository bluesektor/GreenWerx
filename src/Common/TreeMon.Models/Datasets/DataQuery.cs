// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
namespace TreeMon.Models.Datasets
{
    public class DataQuery
    {
        public string SQL { get; set; }

        public object[] Parameters{ get; set; }
    }
}
