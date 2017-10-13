// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;

namespace TreeMon.Models.Finance
{
    public class Fee : Node, INode
    {
        public Fee()
        {
            this.UUIDType = "Fee";
        }

        public decimal  Ammount              { get; set; }

        public string   Category         { get; set; }

        public DateTime DateAdded        { get; set; }

        public string ItemUUID { get; set; }

        public string      UserUUID         { get; set; }


        public string TransactionUUID { get; set; }

    }
}
