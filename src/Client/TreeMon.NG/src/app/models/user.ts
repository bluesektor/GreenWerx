// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

import { Node } from '../models/node';

export class User extends Node {

    Email: string;

    Password: string;

    ConfirmPassword: string;

    PasswordQuestion: string;

    PasswordAnswer: string;
}
/*
                AccountUUID
                Name
                Password
                Active
                DateCreated
                Deleted
                PasswordSalt
                PasswordHashIterations
                SiteAdmin
                Approved
                Anonymous
                Banned
                LockedOut
                Private
                FailedPasswordAnswerAttemptWindowStart
                FailedPasswordAttemptCount
                FailedPasswordAnswerAttemptCount
                ProviderUserKey
                ProviderName
*/
