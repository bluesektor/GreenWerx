// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using Dapper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TreeMon.Data;
using TreeMon.Data.Logging;
using TreeMon.Data.Logging.Models;
using TreeMon.Models;
using TreeMon.Models.App;
using TreeMon.Models.Membership;
using TreeMon.Models.Plant;
using TreeMon.Models.Store;
using TreeMon.Utilites.Extensions;

namespace TreeMon.Managers.Membership
{
    public class AccountManager : BaseManager, ICrud
    {
        readonly RoleManager _roleManager = null;
        private readonly SystemLogger _logger = null;

        public AccountManager(string connectionKey, string sessionKey) : base(connectionKey, sessionKey)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(connectionKey), "AccountManager CONTEXT IS NULL!");
            this._connectionKey = connectionKey;
            _roleManager = new RoleManager(connectionKey, this.GetUser(SessionKey));
            _logger = new SystemLogger(this._connectionKey);
        }

        public ServiceResult AddUsersToAccount(string accountUUID, List<AccountMember> users, User requestingUser )
        {
            //if (!this.DataAccessAuthorized(a, "PATCH", false)) return ServiceResponse.Error("You are not authorized this action.");
            foreach (AccountMember am in users)
            {
                ServiceResult res = AddUserToAccount(accountUUID, am.MemberUUID, requestingUser);
                if (res.Code == 500)
                    return res;
            }
            return ServiceResponse.OK(string.Format("{0} users added to account.", users.Count()));
        }

        public ServiceResult AddUserToAccount(string accountUUID, string userUUID, User requestingUser)
        {
            if (string.IsNullOrWhiteSpace(accountUUID))
                return ServiceResponse.Error("Invalid account id");

            if (string.IsNullOrWhiteSpace(userUUID))
                return ServiceResponse.Error("Invalid user id");
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                Account a = context.GetAll<Account>().FirstOrDefault(w => w.UUID == accountUUID);
                if (a == null)
                    return ServiceResponse.Error("Account not found.");

                if (!this.DataAccessAuthorized(a, "DELETE", false)) return ServiceResponse.Error("You are not authorized this action.");
                // if (!_roleManager.DataAccessAuthorized(a, requestingUser, "post", false))
                //   return ServiceResponse.Error("Access denied for account " + a.Name );

                User u = context.GetAll<User>().FirstOrDefault(w => w.UUID == userUUID) ;
                if (u == null)
                    return ServiceResponse.Error("User not found.");

                if (!_roleManager.DataAccessAuthorized(u, requestingUser,"post", false))
                    return ServiceResponse.Error("Access denied for user " + u.Name);


                if (IsUserInAccount(accountUUID, userUUID))
                    return ServiceResponse.OK("User is already a member of the account.");

                if (context.Insert<AccountMember>(new AccountMember() { AccountUUID = accountUUID, MemberUUID = userUUID, MemberType = "User" }))
                    return ServiceResponse.OK(string.Format("User {0} added account.", u.Name));
            }
            return ServiceResponse.Error("Server error, member was not added to the account.");
        }

        public Account AddAccountFromStrain(Strain s)
        {
            if (s == null)
                return null;
            //if (!this.DataAccessAuthorized(v, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");

            Account v = (Account)Get(s.BreederUUID);

            if (v != null)
                return v;
            //try getting the Account by name with the UUID because the ui allows adding
            //via text/combobox.
            v = (Account)Get(s.BreederUUID);


            if (v != null)
                return v;

            v = new Account()
            {
                AccountUUID = s.AccountUUID,
                Active = true,
                CreatedBy = s.CreatedBy,
                DateCreated = DateTime.UtcNow,
                Deleted = false,
                Name = s.BreederUUID,
                UUIDType = "Account"
            };

            ServiceResult res = this.Insert(v);

            if (res.Code == 200)
                return (Account)res.Result;

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accountUUID"></param>
        /// <param name="purge">If this is true they system will do a hard delete, otherwise it set the delete flag to false.</param>
        /// <returns></returns>
        public ServiceResult Delete(string accountUUID,  bool purge = false)
        {
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                Account a = context.GetAll<Account>().FirstOrDefault(w => w.UUID == accountUUID) ;
                if (a == null)
                    return ServiceResponse.Error("Account not found.");

                if (!this.DataAccessAuthorized(a, "DELETE", false)) return ServiceResponse.Error("You are not authorized this action.");

                //if (!_roleManager.DataAccessAuthorized(a, requestingUser,"delete", false))
                  //  return ServiceResponse.Error("You are not authorized access to this account.");

                if (accountUUID == _requestingUser.AccountUUID)//todo check if any user has this as default account
                    return ServiceResponse.Error("Cannot delete a default account. You must select another account as default before deleting this one.");

                if (!purge)
                {
                    a.Deleted = true;
                   return Update(a);
                }

                try
                {
                    if (context.Delete<Account>(a) > 0)
                        return ServiceResponse.OK();

                    return ServiceResponse.Error("No records deleted.");
                }
                catch (Exception ex)
                {
                    _logger.InsertError(ex.Message, "AccountManager", "DeleteAccount:" + accountUUID);
                    Debug.Assert(false, ex.Message);
                    return ServiceResponse.Error(ex.Message);
                }
            }
        }

        public ServiceResult Delete(INode n,  bool purge = false)
        {
            if (n == null)
                return ServiceResponse.Error("No account record sent.");

            if (n.UUID ==  this._requestingUser.AccountUUID)//todo check if any user has this as default account
                return ServiceResponse.Error("Cannot delete a default account. You must select another account as default before deleting this one.");

           if (!this.DataAccessAuthorized(n,  "DELETE", false)) return ServiceResponse.Error("You are not authorized this action.");

            var account = (Account)n;

            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                Account a = context.GetAll<Account>().FirstOrDefault(w => w.UUID == account.UUID);

                if (!this.DataAccessAuthorized(a, "DELETE", false)) return ServiceResponse.Error("You are not authorized this action.");

                if (!purge)
                {
                    Account dbAcct = context.GetAll<Account>().FirstOrDefault(w => w.UUID == a.UUID) ;

                    if (dbAcct == null)
                        return ServiceResponse.Error("Account not found.");

                    dbAcct.Deleted = true;

                    return Update(dbAcct);
                }

                try
                {
                    if (context.Delete<Account>(a) > 0)
                        return ServiceResponse.OK();

                    return ServiceResponse.Error("No records deleted.");
                }
                catch (Exception ex)
                {
                    _logger.InsertError(ex.Message, "AccountManager", "DeleteAccount:" + account.UUID);
                    Debug.Assert(false, ex.Message);
                    return ServiceResponse.Error(ex.Message);
                }
            }
           
        }

        private List<Account> FindAccount(string uuid, string name, string AccountUUID, bool includeDefaultAccount = true)
        {
            List<Account> res = null;

            if (string.IsNullOrWhiteSpace(uuid) && string.IsNullOrWhiteSpace(name) == false)
                res = this.Search(name);
            //else if (string.IsNullOrWhiteSpace(uuid) == false && string.IsNullOrWhiteSpace(name))
            //    res = (Account)GetBy(uuid);
            else
            {
                using (var context = new TreeMonDbContext(this._connectionKey))
                {
                    res = context.GetAll<Account>().Where(w => w.UUID == uuid ||( w.Name?.EqualsIgnoreCase(name)??false)).ToList();
                }
            }
            if (res == null)
                return res;

        //    if (!this.DataAccessAuthorized(res, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");

            if (includeDefaultAccount && ( res.FirstOrDefault().AccountUUID == AccountUUID))
                return res;

            if (res.FirstOrDefault().AccountUUID == AccountUUID)
                return res;

            return new List<Account>();
        }

        public List<Account> Search(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                return context.GetAll<Account>().Where(aw => aw.Name.EqualsIgnoreCase(name)).ToList();
            }
            //if (!this.DataAccessAuthorized(u, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
        }

        public INode Get(string uuid)
        {
            if (string.IsNullOrWhiteSpace(uuid))
                return null;
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                return context.GetAll<Account>().FirstOrDefault(aw => aw.UUID == uuid);
            }
            //if (!this.DataAccessAuthorized(u, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
        }

        //public INode GetBySyncKey(string syncKey)
        //{
        //    if (string.IsNullOrWhiteSpace(syncKey))
        //        return null;
        //    using (var context = new TreeMonDbContext(this._connectionKey))
        //    {
        //        //try to get the record by account.
        //        INode a = context.GetAll<Account>().FirstOrDefault(aw => aw.SyncKey == syncKey && aw.AccountUUID == _requestingUser.AccountUUID);

        //        //if not get the record matching the default account
        //        if(a == null)
        //            a = context.GetAll<Account>().FirstOrDefault(aw => aw.SyncKey == syncKey &&  aw.AccountUUID == SystemFlag.Default.Account );

        //        return a;
        //    }
        //    //if (!this.DataAccessAuthorized(u, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
        //}

        /// <summary>
        /// Gets all accounts the user is a member of.
        ///     userUUID  xref to  UsersInAccount => Accounts
        /// </summary>
        /// <param name="userUUID"></param>
        /// <returns></returns>
        public List<Account> GetUsersAccounts(string userUUID)
        {
            //AccountMember = UsersInAccount table.
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                IEnumerable<Account> userAccounts =
              from am in context.GetAll<AccountMember>().Where(amw => amw.MemberUUID == userUUID)/// && rrw.UUParentIDType == "User")// && rrw.AccountUUID == accountUUID)
              join accounts in context.GetAll<Account>().Where(uw => uw.Deleted == false) on am.AccountUUID equals accounts.UUID
              select accounts;

                //if (!this.DataAccessAuthorized(u, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
                return userAccounts.ToList();
            }
        }

        /// <summary>
        /// Returns accounts the users is not a member of
        /// </summary>
        /// <param name="userUUID"></param>
        /// <returns></returns>
        public List<Account> GetNonMemberAccounts(string userUUID)
        {
            string sql = @"SELECT
                            Accounts.Id,
                            Accounts.ParentId,
                            Accounts.UUID,
                            Accounts.UUIDType,
                            Accounts.UUParentID,
                            Accounts.UUParentIDType,
                            Accounts.Name,
                            Accounts.Status,
                            UsersInAccount.MemberUUID
                            FROM
                            Accounts ,
                            UsersInAccount
                            WHERE
                            UsersInAccount.MemberUUID <> @MEMBERID OR
                            Accounts.UUID <> '' and Accounts.Deleted = 0";
            DynamicParameters p = new DynamicParameters();
            p.Add("@MEMBERID", userUUID);
         
                List<Account> userAccounts;
                using (var context = new TreeMonDbContext(this._connectionKey))
                {
                    userAccounts = context.Select<Account>(sql, p).ToList();
                }
            //if (!this.DataAccessAuthorized(u, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
            return userAccounts;
        }

        public List<User> GetAccountMembers(string accountUUID, bool clearSensitiveData = true)
        {
            List<User> accountMembers;
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                accountMembers = context.GetAll<AccountMember>().Where(w => w.AccountUUID == accountUUID)
                                        .Join(
                                            context.GetAll<User>()
                                                .Where(w => w.Deleted == false),
                                            acct => acct.MemberUUID,
                                            users => users.UUID,
                                            (acct, users) => new { acct, users }
                                         )
                                         .Select(s => s.users)
                                         .ToList();
            }
            if (accountMembers == null)
                return new List<User>();

            if (clearSensitiveData)
                accountMembers = new UserManager(this._connectionKey, SessionKey).ClearSensitiveData(accountMembers);

            //if (!this.DataAccessAuthorized(u, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
            return accountMembers;
        }

        /// <summary>
        /// Gets all users not in the account
        /// </summary>
        /// <param name="accountUUID"></param>
        /// <param name="clearSensitiveData"></param>
        /// <returns></returns>
        public List<User> GetAccountNonMembers(string accountUUID, bool clearSensitiveData = true)
        {
            List<User> nonMembers;
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                //this query is from a bug, just kludge it in for now. backlog fix this damn mess.
                 nonMembers = context.GetAll<User>().Where(w => w.Deleted == false && string.IsNullOrWhiteSpace(w.AccountUUID) == true).ToList();

                nonMembers.AddRange(context.GetAll<AccountMember>().Where(w => w.AccountUUID != accountUUID)
                                           .Join(
                                               context.GetAll<User>()
                                                     .Where(w => w.Deleted == false),
                                               acct => acct.MemberUUID,
                                               users => users.UUID,
                                               (acct, users) => new { acct, users }
                                            )
                                            .Select(s => s.users)
                                            .ToList());
            }
            List<User> members = GetAccountMembers(accountUUID, clearSensitiveData);

            if (members != null)
                nonMembers = nonMembers.Except(members).ToList();

            //if (!this.DataAccessAuthorized(u, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
            return nonMembers;
        }

        public List<Account> GetAccounts(string userUUID)
        {
            List<Account> acts;
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
               acts =  context.GetAll<AccountMember>().Where(w => w.MemberUUID == userUUID)
                        .Join(context.GetAll<Account>().Where(w => w.Deleted == false),
                            am => am.AccountUUID,
                            act => act.UUID,
                            (am, acct) => new { am, acct }
                        )
                        .Select(s => s.acct).ToList();
            }
            //if (!this.DataAccessAuthorized(u, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
            return acts;
        }

        public List<Account> GetAllAccounts()
        {
            List<Account> acts;
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
               acts = context.GetAll<Account>().Where(w => w.Deleted == false).ToList();
            }
            //if (!this.DataAccessAuthorized(u, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
            return acts;
        }

        public ServiceResult Insert(INode n, bool validateFirst = true)
        {
            if (n == null)
                return ServiceResponse.Error("Invalid account data.");

            if (!this.DataAccessAuthorized(n, "POST", false)) return ServiceResponse.Error("You are not authorized this action.");

            n.Initialize(this._requestingUser.UUID, this._requestingUser.AccountUUID, this._requestingUser.RoleWeight);

            var a = (Account)n;

            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                if (validateFirst)
                {
                    Account dbU = context.GetAll<Account>().FirstOrDefault(wu => (wu.Name?.EqualsIgnoreCase(a.Name)??false) && wu.AccountUUID == a.AccountUUID);

                    if (dbU != null)
                        return ServiceResponse.Error("Account already exists.");
                }

                if (context.Insert<Account>(a))
                    return ServiceResponse.OK("",a);
            }
            return ServiceResponse.Error("System error, account was not added." );

        }

        public bool IsUserInAccount(string accountUUID, string userUUID)
        {
            if (string.IsNullOrWhiteSpace(userUUID))
                return false;
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                if (context.GetAll<AccountMember>().Any(w => w.AccountUUID == accountUUID && w.MemberUUID == userUUID))
                    return true;
            }
            return false;
        }
     
        
        ///This removes the user from all accounts
        public ServiceResult RemoveUserFromAllAccounts(string userUUID)
        {
            ServiceResult res = ServiceResponse.OK();

            User u = null;
            try
            {
                using (var context = new TreeMonDbContext(this._connectionKey))
                {
                    //Make sure correct userid is passed in.
                    u = context.GetAll<User>().FirstOrDefault(w => w.UUID == userUUID);

                    if (u == null)
                        return ServiceResponse.Error("Invalid user id.");

                    if (!this.DataAccessAuthorized(u, "PATCH", false)) return ServiceResponse.Error("You are not authorized this action.");

                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@MEMBERID", userUUID);
                    context.Delete<AccountMember>("WHERE MemberUUID=@MEMBERID", parameters);

                    //SQLITE
                    //Remove the reference in the xref table
                    //object[] parameters = new object[] { userUUID };
                    //context.Delete<AccountMember>("WHERE MemberUUID=?", parameters);

                    //now make sure the primary account in the user table is emptied.
                    u.AccountUUID = "";
                    if( context.Update<User>(u) == 0)
                        return ServiceResponse.Error(u.Name + " failed to update. ");
                }
            }
            catch (Exception ex)
            {
                _logger.InsertError(ex.Message, "AccountManager", "RemoveUserFromAllAccounts:userUUID:" + userUUID);
                Debug.Assert(false, ex.Message);
                return ServiceResponse.Error("Exception occured while deleting this record.");
            }
            return res;
        }

       public ServiceResult RemoveUsersFromAccount(string accountUUID, List<AccountMember> users, User requestingUser)
        {
            ServiceResult res = ServiceResponse.OK();

            foreach (AccountMember am in users)
            {
                ServiceResult removeRes = RemoveUserFromAccount(accountUUID, am.MemberUUID);
                if (res.Code != 200)
                {
                    res.Message += removeRes.Message + Environment.NewLine;
                    res.Status = "ERROR";
                }
            }
            return res;
        }

        /// <summary>
        /// 1. remove record in usersinaccounts AccountMember where AccountUUID and user id match
        /// 2. check user record and make sure the account id doesn't match. If it does erase the field.
        /// TBD may need other proccessing
        /// </summary>
        /// <param name="accountUUID"></param>
        /// <param name="userUUID"></param>
        /// <returns></returns>
        public ServiceResult RemoveUserFromAccount(string accountUUID, string userUUID)
        {
            if (string.IsNullOrWhiteSpace(accountUUID))
                return ServiceResponse.Error("Invalid account id");

            if (string.IsNullOrWhiteSpace(userUUID))
                return ServiceResponse.Error("Invalid user id");

            if (!IsUserInAccount(accountUUID, userUUID))
                return ServiceResponse.OK();

            //if (!this.DataAccessAuthorized(a, "PATCH", false)) return ServiceResponse.Error("You are not authorized this action.");

            User u = null;
            try
            {
                using (var context = new TreeMonDbContext(this._connectionKey))
                {
                    u = context.GetAll<User>().FirstOrDefault(w => w.UUID == userUUID);
                    if (u == null)
                        return ServiceResponse.Error("Invalid user id");

                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@AccountUUID", accountUUID);
                    parameters.Add("@MEMBERID", userUUID);

                    int res = context.Delete<AccountMember>("WHERE AccountUUID=@AccountUUID AND  MemberUUID=@MEMBERID", parameters);
                    //Remove the reference in the xref table
                    //SQLITE SYNTAX
                    //    object[] parameters = new object[] { accountUUID, userUUID };
                    // int res =  context.Delete<AccountMember>("WHERE AccountUUID=? AND  MemberUUID=?", parameters);

                    if (u.AccountUUID == accountUUID)
                    {
                        u.AccountUUID = "";
                        if (context.Update<User>(u) == 0)
                            return ServiceResponse.Error("Failed to remove user " + u.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.InsertError(ex.Message, "AccountManager", "RemoveUserFromAccount:accountUUID:" + accountUUID + " userUUID"+ userUUID);
                Debug.Assert(false, ex.Message);
                return ServiceResponse.Error(ex.Message);
            }
            return ServiceResponse.OK( string.Format("User {0} removed from account.", u.Name) );
        }

        /// <summary>
        /// This sets the AccountUUID in the Users table. This is the default or active account
        /// the user is accociated with and all data will be loaded according to this field.
        /// </summary>
        /// <param name="accountUUID"></param>
        /// <param name="userUUID"></param>
        /// <returns></returns>
        public ServiceResult SetActiveAccount(string accountUUID, string userUUID, User requestingUser)
        {
            if (string.IsNullOrWhiteSpace(userUUID))
                return ServiceResponse.Error("Invalid user id");

            if(!IsUserInAccount(accountUUID,userUUID))
                return ServiceResponse.Error("User must be added to the account before setting it as the active account.");

            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                User u = context.GetAll<User>().FirstOrDefault(w => w.UUID == userUUID);
                if (u == null)
                    return ServiceResponse.Error("Invalid user id.");

                if (u.AccountUUID == accountUUID)
                    return ServiceResponse.OK("This account is already the default for this user.");

                if (!this.DataAccessAuthorized(u, "PATCH", false)) return ServiceResponse.Error("You are not authorized this action.");

                u.AccountUUID = accountUUID;

                if (context.Update<User>(u) > 0)
                    return ServiceResponse.OK("This account is now the default for this user.");
            }
            return ServiceResponse.Error("Error occurred while updating the user.");
        }


        /// <summary>
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public ServiceResult Update(INode n)
        {
            if (n == null)
                return ServiceResponse.Error("Invalid account data.");

            if (!this.DataAccessAuthorized(n, "PATCH", false)) return ServiceResponse.Error("You are not authorized this action.");


            var a = (Account)n;

            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                if (context.Update<Account>(a) > 0)
                    return ServiceResponse.OK();
            }
            return ServiceResponse.Error("System error, account was not updated.");
        }

   
    }
}
