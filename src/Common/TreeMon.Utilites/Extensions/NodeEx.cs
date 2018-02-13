// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using TreeMon.Models;

namespace TreeMon.Utilites.Extensions
{
    public static class NodeEx
    {
        public static void Initialize(this INode item, string userUUID, string accountUUID, int roleWeight)
        {

            if (string.IsNullOrWhiteSpace(item.CreatedBy))
                item.CreatedBy = userUUID;

            if (string.IsNullOrWhiteSpace(item.AccountUUID))
                item.AccountUUID = accountUUID;

            if (string.IsNullOrWhiteSpace(item.UUID))
                item.UUID = Guid.NewGuid().ToString("N");

            if(string.IsNullOrWhiteSpace(item.UUIDType))
                item.UUIDType = item.GetType().Name;

            if (string.IsNullOrWhiteSpace(item.SyncKey))
                item.SyncKey = item.UUID;

            if (string.IsNullOrWhiteSpace(item.SyncType))
                item.SyncType = item.GetType().Name;

            if (item.DateCreated == DateTime.MinValue)
                item.DateCreated = DateTime.UtcNow;

            if (string.IsNullOrWhiteSpace(item.RoleOperation))
                item.RoleOperation = ">=";

            
            item.RoleWeight = roleWeight;
            item.Deleted = false;
            item.Private = true;
                 
                 

        }

        /// <summary>
        ///Exmaple how to convert to node list.
        ///List<User> users = am.GetAccountMembers(accounts[0].AccountUUID);
        ///LoadListViewDelegate delAccounts = new LoadListViewDelegate(LoadListView);
        ///List<INode> n = users.ConvertAll(new Converter<User, INode>(NodeEx.ObjectToNode));
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        //
        public static INode ObjectToNode(object o)
        {
            if (o == null)
                return new Node();

            return (INode)o;
        }

    }
}
