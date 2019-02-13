// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeMon.Models.Files
{
    public class FileEx
    {
        public string UUID { get; set; }

        public string UUIDType { get; set; }

        public string Name { get; set; }

        public string Image { get; set; }

        public string ImageThumb { get; set; }

        public string Path { get; set; }

        public float Size { get; set; }

        public string Status { get; set; }

        public bool Default { get; set; }
    }
}
