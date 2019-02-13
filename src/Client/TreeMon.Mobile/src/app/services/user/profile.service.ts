// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

import {  Injectable } from '@angular/core';
import 'rxjs/add/operator/map';
import { Api } from '../api/api'; // '../api/api.service';
import {Profile } from '../../models/profile';

@Injectable()
export class ProfileService  {
    CurrentProfile: Profile;

    constructor(private api: Api) {    }

    getProfile() {
      return this.api.invokeRequest('GET', 'api/Users/Profile');
    }

    getProfileBy(uuid: string) {
        return this.api.invokeRequest('GET', 'api/Users/Profile/' + uuid);
    }

    getProfiles() {
        return this.api.invokeRequest('GET', 'api/Users/Profiles');
    }

    setActiveProfile(uuid: string) {
        return this.api.invokeRequest('PATCH', 'api/Users/Profile/' + uuid + '/SetActive' );
    }

    saveProfile(profile: any) {
      return this.api.invokeRequest('POST', 'api/Users/Profile/Save', profile);
    }

    deleteProfile(profileUUID) {
        return this.api.invokeRequest('DELETE', 'api/Users/Profile/' + profileUUID);
    }
}
