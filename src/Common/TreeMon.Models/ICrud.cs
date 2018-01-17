// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System.Collections.Generic;
using TreeMon.Models;
using TreeMon.Models.App;

namespace TreeMon.Models
{
    public interface ICrud
    {
        ServiceResult Delete(INode n, bool purge = false);

        //List<INode> Search( string name);

        INode Get(string uuid);

        ServiceResult Insert(INode n, bool validateFirst = true);

        ServiceResult Update(INode n);
    }
}
