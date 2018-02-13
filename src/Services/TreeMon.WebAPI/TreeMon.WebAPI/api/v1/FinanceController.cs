// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web.Mvc;
using TreeMon.Data.Logging.Models;
using TreeMon.Managers.Finance;
using TreeMon.Managers.Store;
using TreeMon.Models;
using TreeMon.Models.App;
using TreeMon.Models.Datasets;
using TreeMon.Models.Finance;
using TreeMon.Models.Geo;
using TreeMon.Utilites.Extensions;
using TreeMon.Web;
using TreeMon.Web.api;
using TreeMon.Web.api.Helpers;
using TreeMon.Web.Filters;
using WebApiThrottle;

namespace TreeMon.WebAPI.api.v1
{
    public class FinanceController : ApiBaseController
    {
        public FinanceController()
        {
        }


        #region FinanceAccount Api 


        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/Finance/Accounts/Add")]
        [System.Web.Http.Route("api/Finance/Accounts/Insert")]
        public ServiceResult Insert(FinanceAccount n)
        {
            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");


            if (string.IsNullOrWhiteSpace(n.AccountUUID) || n.AccountUUID == SystemFlag.Default.Account)
                n.AccountUUID = CurrentUser.AccountUUID;

            if (string.IsNullOrWhiteSpace(n.CreatedBy))
                n.CreatedBy = CurrentUser.UUID;

            if (n.DateCreated == DateTime.MinValue)
                n.DateCreated = DateTime.UtcNow;

            FinanceAccountManager FinanceAccountManager = new FinanceAccountManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            return FinanceAccountManager.Insert(n);
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 1)]
        [System.Web.Http.HttpPost]
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/Finance/Accounts/{name}")]
        public ServiceResult Get(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return ServiceResponse.Error("You must provide a name for the FinanceAccount.");

            FinanceAccountManager FinanceAccountManager = new FinanceAccountManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            List<FinanceAccount> s = FinanceAccountManager.Search(name);

            if (s == null || s.Count == 0)
                return ServiceResponse.Error("FinanceAccount could not be located for the name " + name);

            return ServiceResponse.OK("", s);
        }


        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 1)]
        [System.Web.Http.HttpPost]
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/Finance/AccountsBy/{uuid}")]
        public ServiceResult GetBy(string uuid)
        {
            if (string.IsNullOrWhiteSpace(uuid))
                return ServiceResponse.Error("You must provide a uuid for the FinanceAccount.");

            FinanceAccountManager FinanceAccountManager = new FinanceAccountManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            FinanceAccount s = (FinanceAccount)FinanceAccountManager.Get(uuid);

            if (s == null)
                return ServiceResponse.Error("FinanceAccount could not be located for the uuid " + uuid);

            return ServiceResponse.OK("", s);
        }



        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [System.Web.Http.HttpPost]
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/Finance/Accounts/")]
        public ServiceResult GetFinanceAccounts(string filter = "")
        {
            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");



            FinanceAccountManager financeAccountManager = new FinanceAccountManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            List<dynamic> FinanceAccounts = financeAccountManager.GetFinanceAccounts(CurrentUser.AccountUUID).Cast<dynamic>().ToList();
            int count;

            DataFilter tmpFilter = this.GetFilter(filter);
            FinanceAccounts = FilterEx.FilterInput(FinanceAccounts, tmpFilter, out count);

            return ServiceResponse.OK("", FinanceAccounts, count);
        }

        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.HttpGet]
        [EnableThrottling(PerSecond = 3)]
        [System.Web.Http.Route("api/Finance/PaymentOptions")]
        public ServiceResult GetPaymentOptions()
        {
            //to make sure we're in sync with the locations table we'll use the default online store locations account uuid to get the payment options for the sites account.
            LocationManager lm = new LocationManager(Globals.DBConnectionKey, Request.Headers.Authorization?.Parameter);
            Location location = lm.GetAll()?.FirstOrDefault(w => w.isDefault == true && w.LocationType.EqualsIgnoreCase("ONLINE STORE"));

            if (location == null)
                return ServiceResponse.Error("Could not get location for payment option.");

            FinanceAccountManager financeAccountManager = new FinanceAccountManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            List<FinanceAccount> FinanceAccounts = financeAccountManager.GetPaymentOptions(location.AccountUUID).ToList();

            return ServiceResponse.OK("", FinanceAccounts);
        }


        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [System.Web.Http.HttpPost]
        [System.Web.Http.HttpDelete]
        [System.Web.Http.Route("api/Finance/Accounts/Delete/{uuid}")]
        public ServiceResult Delete(string uuid)
        {
            if (string.IsNullOrWhiteSpace(uuid))
                return ServiceResponse.Error("Invalid account was sent.");

            FinanceAccountManager FinanceAccountManager = new FinanceAccountManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            FinanceAccount fa = (FinanceAccount)FinanceAccountManager.Get(uuid);
            if (fa == null)
                return ServiceResponse.Error("Could not find finance account.");

            return FinanceAccountManager.Delete(fa);
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [System.Web.Http.HttpPost]
        [System.Web.Http.HttpDelete]
        [System.Web.Http.Route("api/Finance/Accounts/Delete")]
        public ServiceResult Delete(FinanceAccount n)
        {
            if (n == null)
                return ServiceResponse.Error("Invalid account was sent.");

            FinanceAccountManager FinanceAccountManager = new FinanceAccountManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            FinanceAccount fa = (FinanceAccount)FinanceAccountManager.Get(n.UUID);
            if (fa == null)
                return ServiceResponse.Error("Could not find finance account.");

            return FinanceAccountManager.Delete(fa);
        }


        /// <summary>
        /// Fields updated..
        ///     Category 
        ///     Name 
        ///     Cost
        ///     Price
        ///     Weight 
        ///     WeightUOM
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [System.Web.Http.HttpPost]
        [System.Web.Http.HttpPatch]
        [System.Web.Http.Route("api/Finance/Accounts/Update")]
        public ServiceResult Update(FinanceAccount s)
        {
            if (s == null)
                return ServiceResponse.Error("Invalid FinanceAccount sent to server.");

            FinanceAccountManager FinanceAccountManager = new FinanceAccountManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            var dbS = (FinanceAccount)FinanceAccountManager.Get(s.UUID);

            if (dbS == null)
                return ServiceResponse.Error("FinanceAccount was not found.");

            if (dbS.DateCreated == DateTime.MinValue)
                dbS.DateCreated = DateTime.UtcNow;

           
            dbS.Deleted = s.Deleted;
            dbS.Name = s.Name;
            dbS.Status = s.Status;
            dbS.SortOrder = s.SortOrder;
            dbS.AccountNumber = s.AccountNumber;
            dbS.CurrencyUUID = s.CurrencyUUID;
            dbS.Balance = s.Balance;
            dbS.Active = s.Active;
            dbS.LocationType = s.LocationType;
            dbS.ClientCode = s.ClientCode;

            if (string.IsNullOrWhiteSpace(s.Image) || s.Image.EndsWith("/"))
                dbS.Image = "/Content/Default/Images/bank.png";
            else
                dbS.Image = s.Image;
            //   
            //   AssetClass
            // Balance
            //
            //CurrencyName
            //  IsTest
            //Password
            //ServiceAddress
            //SourceClass
            //SourceUUID
            //  UsedBy
            //UsedByClass
            return FinanceAccountManager.Update(dbS);
        }
        #endregion


        #region PriceRule Api TODO refactor to seperate controller

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/Finance/PriceRules/Add")]
        [System.Web.Http.Route("api/Finance/PriceRules/Insert")]
        public ServiceResult Insert(PriceRule PriceRule)
        {
            if (PriceRule == null || string.IsNullOrWhiteSpace(PriceRule.Name))
                return ServiceResponse.Error("Invalid PriceRule sent to server.");

            string authToken = Request.Headers?.Authorization?.Parameter;

            UserSession us = SessionManager.GetSession(authToken);
            if (us == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

            if (string.IsNullOrWhiteSpace(us.UserData))
                return ServiceResponse.Error("Couldn't retrieve user data.");

            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");


            if (string.IsNullOrWhiteSpace(PriceRule.AccountUUID) || PriceRule.AccountUUID == SystemFlag.Default.Account)
                PriceRule.AccountUUID = CurrentUser.AccountUUID;

            if (string.IsNullOrWhiteSpace(PriceRule.CreatedBy))
                PriceRule.CreatedBy = CurrentUser.UUID;

            if (PriceRule.DateCreated == DateTime.MinValue  )
                PriceRule.DateCreated = DateTime.UtcNow;

            if (string.IsNullOrWhiteSpace(PriceRule.Image))
                PriceRule.Image = "/Content/Default/Images/PriceRule/default.png";

            PriceManager financeManager = new PriceManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            return financeManager.Insert(PriceRule);
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 1)]
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/Finance/PriceRules/{PriceRuleCode}")]
        public ServiceResult GetPriceRule(string PriceRuleCode)
        {
            if (string.IsNullOrWhiteSpace(PriceRuleCode))
                return ServiceResponse.Error("You must provide a name for the strain.");

            PriceManager financeManager = new PriceManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            PriceRule s = financeManager.GetPriceRuleByCode(PriceRuleCode);

            if (s == null)
                return ServiceResponse.Error("Invalid code " + PriceRuleCode);

            return ServiceResponse.OK("", s);
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [System.Web.Http.HttpPost]
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/Finance/PriceRules/")]
        public ServiceResult GetPriceRules(string filter = "")
        {
            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

            PriceManager financeManager = new PriceManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            List<dynamic> PriceRule = (List<dynamic>)financeManager.GetPriceRules(CurrentUser.AccountUUID, false).Cast<dynamic>().ToList();
            int count;
            DataFilter tmpFilter = this.GetFilter(filter);
            PriceRule = FilterEx.FilterInput(PriceRule, tmpFilter, out count);
            return ServiceResponse.OK("", PriceRule, count);
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [System.Web.Http.HttpPost]
        [System.Web.Http.HttpDelete]
        [System.Web.Http.Route("api/Finance/PriceRules/Delete")]
        public ServiceResult DeletePriceRule(PriceRule PriceRule)
        {
            if (PriceRule == null || string.IsNullOrWhiteSpace(PriceRule.UUID))
                return ServiceResponse.Error("Invalid account was sent.");

            PriceManager financeManager = new PriceManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            return financeManager.Delete(PriceRule);

        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [System.Web.Http.HttpPost]
        [System.Web.Http.HttpDelete]
        [System.Web.Http.Route("api/Finance/PriceRules/Delete/{uuid}")]
        public ServiceResult DeletePriceRuleBy(string uuid)
        {
            if (string.IsNullOrWhiteSpace(uuid))
                return ServiceResponse.Error("Invalid id.");

            PriceManager fm = new PriceManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            PriceRule c = (PriceRule ) fm.Get(uuid);
            if (c == null)
                return ServiceResponse.Error("Invalid uuid");

            return fm.Delete(c);
        }


        /// <summary>
        /// Fields updated..
        ///     Category 
        ///     Name 
        ///     Cost
        ///     Price
        ///     Weight 
        ///     WeightUOM
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [System.Web.Http.HttpPost]
        [System.Web.Http.HttpPatch]
        [System.Web.Http.Route("api/Finance/PriceRules/Update")]
        public ServiceResult Update(PriceRule fat)
        {
            if (fat == null)
                return ServiceResponse.Error("Invalid PriceRule sent to server.");

            PriceManager financeManager = new PriceManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            var dbS = (PriceRule)financeManager.Get(fat.UUID);

            if (dbS == null)
                return ServiceResponse.Error("PriceRule was not found.");

            if (dbS.DateCreated == DateTime.MinValue)
                dbS.DateCreated = DateTime.UtcNow;

            dbS.Name = fat.Name;

            dbS.Image = fat.Image;
            dbS.Deleted = fat.Deleted;
            dbS.Status = fat.Status;
            dbS.SortOrder = fat.SortOrder;
            dbS.Expires = fat.Expires;
            dbS.ReferenceType = fat.ReferenceType;
            dbS.Code = fat.Code;
            dbS.Operand = fat.Operand;
            dbS.Operator = fat.Operator;
            dbS.Minimum = fat.Minimum;
            dbS.Maximum = fat.Maximum;
            dbS.Mandatory = fat.Mandatory;
            dbS.MaxUseCount = fat.MaxUseCount;
          
            return financeManager.Update(dbS);
        }

        #endregion

        #region FinanceAccountTransaction Api TODO refactor to seperate controller

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/Finance/Accounts/Transactions/Add")]
        public ServiceResult AddFinanceAccountTransaction(FinanceAccountTransaction s)
        {
            if (s == null || string.IsNullOrWhiteSpace(s.Name))
                return ServiceResponse.Error("Invalid FinanceAccountTransaction sent to server.");

            string authToken = Request.Headers?.Authorization?.Parameter;

            UserSession us = SessionManager.GetSession(authToken);
            if (us == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

            if (string.IsNullOrWhiteSpace(us.UserData))
                return ServiceResponse.Error("Couldn't retrieve user data.");

            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");


            if (string.IsNullOrWhiteSpace(s.AccountUUID) || s.AccountUUID == SystemFlag.Default.Account)
                s.AccountUUID = CurrentUser.AccountUUID;

            if (string.IsNullOrWhiteSpace(s.CreatedBy))
                s.CreatedBy = CurrentUser.UUID;

            if (s.DateCreated == DateTime.MinValue  )
                s.DateCreated = DateTime.UtcNow;

            if (string.IsNullOrWhiteSpace(s.Image))
                s.Image = "/Content/Default/Images/FinanceAccountTransaction/default.png";

            FinanceAccountTransactionsManager financeManager = new FinanceAccountTransactionsManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            return financeManager.Insert(s);
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 1)]
        [System.Web.Http.HttpPost]
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/Finance/Accounts/Transactions/{name}")]
        public ServiceResult GetFinanceAccountTransactionByName(string name )
        {
            if (string.IsNullOrWhiteSpace(name))
                return ServiceResponse.Error("You must provide a name for the strain.");

            FinanceAccountTransactionsManager financeManager = new FinanceAccountTransactionsManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            List<FinanceAccountTransaction> s = financeManager.Search(name);

            if (s == null || s.Count == 0)
                return ServiceResponse.Error("FinanceAccountTransaction could not be located for the name " + name);

            return ServiceResponse.OK("", s);
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [System.Web.Http.HttpPost]
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/Finance/Accounts/Transactions/")]
        public ServiceResult GetAccountTransactions(string filter = "")
        {
            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

            FinanceAccountTransactionsManager financeManager = new FinanceAccountTransactionsManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            List<dynamic> FinanceAccountTransaction = (List<dynamic>)financeManager.GetFinanceAccountTransactions(CurrentUser.AccountUUID, false).Cast<dynamic>().ToList();

          int count;
                            DataFilter tmpFilter = this.GetFilter(filter);
                FinanceAccountTransaction = FilterEx.FilterInput(FinanceAccountTransaction, tmpFilter, out count);

            return ServiceResponse.OK("", FinanceAccountTransaction, count);
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [System.Web.Http.HttpPost]
        [System.Web.Http.HttpDelete]
        [System.Web.Http.Route("api/Finance/Accounts/Transactions/Delete")]
        public ServiceResult Delete(FinanceAccountTransaction s)
        {
            if (s == null || string.IsNullOrWhiteSpace(s.UUID))
                return ServiceResponse.Error("Invalid account was sent.");

            FinanceAccountTransactionsManager financeManager = new FinanceAccountTransactionsManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            return financeManager.Delete(s);

        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [System.Web.Http.HttpPost]
        [System.Web.Http.HttpDelete]
        [System.Web.Http.Route("api/Finance/Accounts/Transactions/Delete/{uuid}")]
        public ServiceResult DeleteFinanceAccountTransactionBy(string uuid)
        {
            if (string.IsNullOrWhiteSpace(uuid))
                return ServiceResponse.Error("Invalid id.");

            FinanceAccountTransactionsManager fm = new FinanceAccountTransactionsManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            FinanceAccountTransaction c = (FinanceAccountTransaction)fm.Get(uuid);
            if (c == null)
                return ServiceResponse.Error("Invalid uuid");

            return fm.Delete(c);
        }


        /// <summary>
        /// Fields updated..
        ///     Category 
        ///     Name 
        ///     Cost
        ///     Price
        ///     Weight 
        ///     WeightUOM
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [System.Web.Http.HttpPost]
        [System.Web.Http.HttpPatch]
        [System.Web.Http.Route("api/Finance/Accounts/Transactions/Update")]
        public ServiceResult Update(FinanceAccountTransaction fat)
        {
            if (fat == null)
                return ServiceResponse.Error("Invalid FinanceAccountTransaction sent to server.");

            FinanceAccountTransactionsManager financeManager = new FinanceAccountTransactionsManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            var dbS = (FinanceAccountTransaction)financeManager.Get(fat.UUID);

            if (dbS == null)
                return ServiceResponse.Error("FinanceAccountTransaction was not found.");

            if (dbS.DateCreated == DateTime.MinValue)
                dbS.DateCreated = DateTime.UtcNow;

            dbS.Name = fat.Name;
           
            dbS.Image = fat.Image;
            dbS.Deleted = fat.Deleted;
            dbS.Status = fat.Status;
            dbS.SortOrder = fat.SortOrder;
            //FinanceAccountUUID
            //PayToAccountUUID
            // PayFromAccountUUID
            // CreationDate
            // CustomerIp
            //LastPaymentStatusCheck 
            //    OrderUUID
            //    Amount
            //  TransactionType
            // TransactionDate
            // PaymentTypeUUID
            // AmountTransferred
            // SelectedPaymentTypeSymbol
            //SelectedPaymentTypeTotal 
            //        UserUUID
            //        CustomerEmail
            // CurrencyUUID
            return financeManager.Update(dbS);
        }

        #endregion



        #region Currency Api TODO refactor to seperate controller

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/Finance/Currency/Add")]
        public ServiceResult AddCurrency(Currency s)
        {
            if (s == null || string.IsNullOrWhiteSpace(s.Name))
                return ServiceResponse.Error("Invalid Currency sent to server.");

            string authToken = Request.Headers?.Authorization?.Parameter;

            UserSession us = SessionManager.GetSession(authToken);
            if (us == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

            if (string.IsNullOrWhiteSpace(us.UserData))
                return ServiceResponse.Error("Couldn't retrieve user data.");

            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");


            if (string.IsNullOrWhiteSpace(s.AccountUUID) || s.AccountUUID == SystemFlag.Default.Account)
                s.AccountUUID = CurrentUser.AccountUUID;

            if (string.IsNullOrWhiteSpace(s.CreatedBy))
                s.CreatedBy = CurrentUser.UUID;

            if (s.DateCreated == DateTime.MinValue )
                s.DateCreated = DateTime.UtcNow;

            if (string.IsNullOrWhiteSpace(s.Image))
                s.Image = "/Content/Default/Images/Currency/default.png";

            CurrencyManager financeManager = new CurrencyManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            return financeManager.Insert(s);
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 1)]
        [System.Web.Http.HttpPost]
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/Finance/Currency/{name}")]
        public ServiceResult GetCurrencyByName(string name )
        {
            if (string.IsNullOrWhiteSpace(name))
                return ServiceResponse.Error("You must provide a name for the strain.");

            CurrencyManager financeManager = new CurrencyManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            List<Currency> s = financeManager.Search(name);

            if (s == null || s.Count == 0)
                return ServiceResponse.Error("Currency could not be located for the name " + name);

            return ServiceResponse.OK("", s);
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [System.Web.Http.HttpPost]
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/Finance/Currency")]
        public ServiceResult GetCurrency(string filter = "")
        {
            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

            CurrencyManager financeManager = new CurrencyManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            List<dynamic> currency = (List<dynamic>)financeManager.GetCurrencies(CurrentUser.AccountUUID, false,true).Cast<dynamic>().ToList();

            int count;

            DataFilter tmpFilter = this.GetFilter(filter);
            currency = FilterEx.FilterInput(currency, tmpFilter, out count);
            return ServiceResponse.OK("", currency, count);
        }



        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [System.Web.Http.HttpPost]
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/Finance/Currency/Symbols")]
        public ServiceResult GetCurrencySymbols(string filter = "")
        {
            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");
           
            CurrencyManager financeManager = new CurrencyManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            List<string> symbols = financeManager.GetAll()
                                                 .Where( w => w.AccountUUID == SystemFlag.Default.Account || 
                                                         w.AccountUUID == CurrentUser.AccountUUID)
                                                 .OrderBy(o => o.Symbol)
                                                 .Select(s => s.Symbol)
                                                 .Distinct()
                                                 .ToList();
          int count =symbols.Count;
            return ServiceResponse.OK("", symbols, count);
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [System.Web.Http.HttpPost]
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/Finance/AssetClasses")]
        public ServiceResult GetAssetClasses(string filter = "")
        {
            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");
           
            CurrencyManager financeManager = new CurrencyManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            List<string> assetClasses = financeManager.GetAll()
                                                        .Where(w => w.AccountUUID == SystemFlag.Default.Account ||
                                                               w.AccountUUID == CurrentUser.AccountUUID)
                                                        .OrderBy(o => o.AssetClass)
                                                        .Select(s => s.AssetClass)
                                                        .Distinct()
                                                        .ToList();
          int count =assetClasses.Count;
            return ServiceResponse.OK("", assetClasses, count);
        }


        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [System.Web.Http.HttpPost]
        [System.Web.Http.HttpDelete]
        [System.Web.Http.Route("api/Finance/Currency/Delete")]
        public ServiceResult DeleteCurrency(Currency s)
        {
            if (s == null || string.IsNullOrWhiteSpace(s.UUID))
                return ServiceResponse.Error("Invalid account was sent.");

            CurrencyManager financeManager = new CurrencyManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            return financeManager.Delete(s);

        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [System.Web.Http.HttpPost]
        [System.Web.Http.HttpDelete]
        [System.Web.Http.Route("api/Finance/Currency/Delete/{currencyUUID}")]
        public ServiceResult DeleteCurrencyBy(string currencyUUID)
        {
            if (string.IsNullOrWhiteSpace(currencyUUID))
                return ServiceResponse.Error("Invalid id.");

            CurrencyManager fm = new CurrencyManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            Currency c = (Currency)fm.Get(currencyUUID);
            if (c == null)
                return ServiceResponse.Error("Invalid uuid");

            return fm.Delete(c);
        }


        /// <summary>
        /// Fields updated..
        ///     Category 
        ///     Name 
        ///     Cost
        ///     Price
        ///     Weight 
        ///     WeightUOM
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [System.Web.Http.HttpPost]
        [System.Web.Http.HttpPatch]
        [System.Web.Http.Route("api/Finance/Currency/Update")]
        public ServiceResult UpdateCurrency(Currency form)
        {
            if (form == null)
                return ServiceResponse.Error("Invalid Currency sent to server.");

            CurrencyManager financeManager = new CurrencyManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            var dbS = (Currency)financeManager.Get(form.UUID);

            if (dbS == null)
                return ServiceResponse.Error("Currency was not found.");

            if (dbS.DateCreated == DateTime.MinValue)
                dbS.DateCreated = DateTime.UtcNow;

            dbS.Name = form.Name;
            dbS.AssetClass = form.AssetClass;
            //// dbS.Country = form.Country;
            dbS.Symbol = form.Symbol;
            dbS.Test = form.Test;
            dbS.Image = form.Image;
            dbS.Deleted = form.Deleted;
            dbS.Status = form.Status;
            dbS.SortOrder = form.SortOrder;
            return financeManager.Update(dbS);
        }

        #endregion

        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.HttpPost]
        [EnableThrottling(PerSecond = 5)]
        [System.Web.Http.Route("api/PayPal/IPN")]
        public async System.Threading.Tasks.Task<HttpStatusCodeResult> ProcessIPN()
        {
            PaymentGatewayManager _gatewayManager = new PaymentGatewayManager(Globals.DBConnectionKey);
            NetworkHelper network = new NetworkHelper();
            string ip = network.GetClientIpAddress(this.Request);

          


#if DEBUG
            string ipnSample = @"mc_gross = 19.95 & protection_eligibility = Eligible & address_status = confirmed & payer_id = LPLWNMTBWMFAY & 
                                        tax = 0.00 & address_street = 1 + Main + St & payment_date = 20 % 3A12 % 3A59 + Jan + 13 % 2C + 2009 + PST & payment_status = Completed & 
                                        charset = windows - 1252 & address_zip = 95131 & first_name = Test & mc_fee = 0.88 & address_country_code = US & address_name = Test + User & 
                                        notify_version = 2.6 & custom = d5422cf40f364cd99cac5fb3df7c12f6 &payer_status = verified & address_country = United + States & address_city = San + Jose & quantity = 1 & 
                                        verify_sign = AtkOfCXbDm2hu0ZELryHFjY - Vb7PAUvS6nMXgysbElEn9v - 1XcmSoGtf & payer_email = gpmac_1231902590_per % 40paypal.com & txn_id = 61E67681CH3238416 & payment_type = instant & last_name = User & address_state = CA & receiver_email = gpmac_1231902686_biz % 40paypal.com & 
                                        payment_fee = 0.88 & receiver_id = S8XGHLYDW9T3S & txn_type = express_checkout & item_name = &mc_currency = USD & item_number = &residence_country = US & test_ipn = 1 & handling_amount = 0.00 & transaction_subject = &payment_gross = 19.95 & shipping = 0.00";

            _gatewayManager.ProcessIpn(ipnSample, ip);

#else
              byte[] paramArray = await Request.Content.ReadAsByteArrayAsync();
            var content = System.Text.Encoding.ASCII.GetString(paramArray);
           //Fire and forget verification task
            Thread t = new Thread(() => _gatewayManager.ProcessIpn(content, ip));
            t.Start();
#endif

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

    }
}

