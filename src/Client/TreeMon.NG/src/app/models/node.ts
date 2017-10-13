// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.


export class Node {
    Id: number;

    ParentId: number;

    UUID: string;

    UUIDType: string;

    UUParentID: string;

    UUParentIDType: string;

    Name: string;

    Status: string;

    AccountUUID: string;

    Active = true;

    Deleted = false;

    Private = true;

    SortOrder = 0;

    CreatedBy: string;

    DateCreated: string;

    RoleWeight: number;

    RoleOperation: string;

    Image: string;

    SyncKey: string;

    ApexType: string;
}
