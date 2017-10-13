// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

import {  Injectable } from '@angular/core';
import { Http, Headers, RequestOptions } from '@angular/http';
import 'rxjs/add/operator/map';
import { WebApiService } from '../services/webApi.service';
import { SessionService } from '../services/session.service';
import { Filter } from '../models/filter';
import { Screen } from '../models/screen';

@Injectable()
export class RoleService extends WebApiService {

    constructor(http: Http, sessionService: SessionService) {
        super(http, sessionService);

    }

    addRole(role) {

        return this.invokeRequest('POST', 'api/Roles/Add', role);
    }

    deleteRole(roleUUID) {

        return this.invokeRequest('DELETE', 'api/Roles/Delete/' + roleUUID );
    }

    getRoles() {
        return this.invokeRequest('GET', 'api/Roles/');
    }

    getRole(roleUUID) {
        return this.invokeRequest('GET', 'api/RolesBy/' + roleUUID);
    }


    getNonMembers(roleUUID) {
        return this.invokeRequest('GET', 'api/Roles/' + roleUUID + '/Users/Unassigned');
    }

    getMembers(roleUUID) {

        return this.invokeRequest('GET', 'api/Roles/' + roleUUID + '/Users');
    }

    addUsersToRole(roleUUID: string, users: Node[]) {

        const newMembers = JSON.stringify(users);
        return this.invokeRequest('POST', 'api/Roles/' + roleUUID + '/Users/Add', newMembers);
    }

    removeUsersFromRole(roleUUID: string, users: Node[]) {
        const removeMembers = JSON.stringify(users);
        return this.invokeRequest('POST', 'api/Roles/' + roleUUID + '/Users/Remove', removeMembers);
    }

    updateRole(role) {
       return this.invokeRequest('PATCH', 'api/Roles/Update', role);
    }


    getAvailablePermisssions(roleUUID: string, searchFilter: Filter) {
        return this.invokeRequest('GET', 'api/Roles/' + roleUUID + '/Permissions/Unassigned?filter=' + JSON.stringify(searchFilter));
    }

    getSelectedPermisssions(roleUUID, searchFilter: Filter) {

        return this.invokeRequest('GET', 'api/Roles/' + roleUUID + '/Permissions?filter=' + JSON.stringify(searchFilter));
    }

    addPermissionsToRole(roleUUID: string, permissions: Node[]) {
        const newPermissions = JSON.stringify(permissions);
        return this.invokeRequest('POST', 'api/Roles/' + roleUUID + '/Permissions/Add', newPermissions);
    }

    removePermissionsFromRole(roleUUID: string, permissions: Node[]) {
        const removePermissions = JSON.stringify(permissions);
        return this.invokeRequest('POST', 'api/Roles/' + roleUUID + '/Permissions/Delete', removePermissions);
    }
}
