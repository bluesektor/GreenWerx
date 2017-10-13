// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
namespace TreeMon.Models.Datasets
{
    public class DataScreen
    {
        public int RoleWeight { get; set; }

        /// <summary>
        /// UI label.
        /// </summary>
        public string Caption { get; set; }

        /// <summary>
        /// db field we're going to filter by
        /// </summary>
        public string Field { get; set; }

        /// <summary>
        /// this will let us know how to process the filter
        /// types:  sql for reqular sql expressions
        ///         linq for processing by linq, you'll have create the logic, no builder yet.
        ///         
        /// </summary>
        //
        public string Type { get; set; }


        /// <summary>
        /// function we want to execut on the data.
        /// orderby,ORDERBYDESC, DISTINCT,DISTINCTBY,SearchBy,SEARCH!BY
        /// </summary>
        public string Command { get; set; }

        /// <summary>
        /// Comparison operators.
        /// >,>=, ==, <=, < 
        /// </summary>
        public string Operator { get; set; }

        /// <summary>
        /// The value  in the field we're operating on
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// AND, OR ... to join the QueryFilter
        /// </summary>
        public string Junction { get; set; }

        /// <summary>
        /// order which the filter is applied (for multi filters).
        /// </summary>
        public int Order { get; set; }

   
    }
}
