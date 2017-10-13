// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Transactions;
using TreeMon.Data;
using TreeMon.Data.Logging;
using TreeMon.Models;
using TreeMon.Models.App;
using TreeMon.Models.Finance;
using TreeMon.Models.Finance.PaymentGateways;
using TreeMon.Models.Flags;
using TreeMon.Models.Store;
using TreeMon.Utilites.Extensions;

namespace TreeMon.Managers.Finance
{
    public class PaymentGatewayManager
    {
      
        private readonly string _dbConnectionKey;
        private readonly SystemLogger _logger;

        public PaymentGatewayManager(string connectionKey) 
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(connectionKey), "PaymentGatewayLogger CONTEXT IS NULL!");


            _dbConnectionKey = connectionKey;

            _logger = new SystemLogger(connectionKey);

          
        }

        public PaymentGatewayLog GetBy(string uuid)
        {
            if (string.IsNullOrWhiteSpace(uuid))
                return null;
            using (var context = new TreeMonDbContext(_dbConnectionKey))
            {
                //if (!this.DataAccessAuthorized(dbP, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
                return context.GetAll<PaymentGatewayLog>().FirstOrDefault(sw => sw.UUID == uuid);
            }
        }

        public PaymentGatewayLog Get(string gateway)
        {
            if (string.IsNullOrWhiteSpace(gateway))
                return null;
            using (var context = new TreeMonDbContext(_dbConnectionKey))
            {
                //if (!this.DataAccessAuthorized(dbP, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
                return context.GetAll<PaymentGatewayLog>().FirstOrDefault(sw => (sw.Gateway?.Trim()?.EqualsIgnoreCase(gateway?.Trim())??false) ) ;
            }
        }

        public List<PaymentGatewayLog> GetAll()
        {
            using (var context = new TreeMonDbContext(_dbConnectionKey))
            {
                //if (!this.DataAccessAuthorized(dbP, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
                return context.GetAll<PaymentGatewayLog>().ToList();
            }
        }

        public ServiceResult Update(PaymentGatewayLog p)
        {
           // if (!this.DataAccessAuthorized(p, "PATCH", false)) return ServiceResponse.Error("You are not authorized this action.");

            ServiceResult res = ServiceResponse.OK();
            using (var context = new TreeMonDbContext(_dbConnectionKey))
            {

                if (context.Update<PaymentGatewayLog>(p) == 0)
                    return ServiceResponse.Error(p.Gateway + " failed to update. ");
            }
            return res;

        }


        /// <summary>
        /// This was created for use in the bulk process..
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <param name="checkName">This will check the PaymentGatewayLogs by name to see if they exist already. If it does an error message will be returned.</param>
        /// <returns></returns>
        public ServiceResult Insert(PaymentGatewayLog p)
        {
            if (string.IsNullOrWhiteSpace(p.UUID))
                p.UUID = Guid.NewGuid().ToString("N");
            
           // if (!this.DataAccessAuthorized(p, "POST", false)) return ServiceResponse.Error("You are not authorized this action.");

            using (var context = new TreeMonDbContext(_dbConnectionKey))
            {
                if (context.Insert<PaymentGatewayLog>(p))
                    return ServiceResponse.OK("", p);
            }
            return ServiceResponse.Error("An error occurred inserting PaymentGatewayLog " + p.Gateway);
        }


        #region PayPal

        public void ProcessIpn(string ipnData, string ipAddress)
        {
            Insert(new PaymentGatewayLog()
            {
                Gateway = "PayPal",
                IpAddress = ipAddress,
                Payload = ipnData,
                PayloadType = "http.request",
                RequestDate = DateTime.UtcNow,
                RequestDirection = "in",
                RequestType = "post"
            });

            VerifyIpn(ipnData);

        }
        private void VerifyIpn(string requestBody) //HttpRequestMessage ipnRequest)//HttpRequestBase ipnRequest)
        {
            var verificationResponse = string.Empty;

            try
            {
                Debug.Assert(false, "TODO GET REQUEST URL");
                var verificationRequest = (HttpWebRequest)WebRequest.Create("https://www.sandbox.paypal.com/cgi-bin/webscr");

                //Set values for the verification request
                verificationRequest.Method = "POST";
                verificationRequest.ContentType = "application/x-www-form-urlencoded";
                //   var param = Request.BinaryRead(ipnRequest.ContentLength);
                //   var strRequest = Encoding.ASCII.GetString(param);

                //Add cmd=_notify-validate to the payload
                requestBody = "cmd=_notify-validate&" + requestBody;
                verificationRequest.ContentLength = requestBody.Length;

                //Attach payload to the verification request
                var streamOut = new StreamWriter(verificationRequest.GetRequestStream(), Encoding.ASCII);
                streamOut.Write(requestBody);
                streamOut.Close();

                //Send the request to PayPal and get the response
                var streamIn = new StreamReader(verificationRequest.GetResponse().GetResponseStream());
                verificationResponse = streamIn.ReadToEnd();
                streamIn.Close();
                if (VeriFyPurchase(verificationResponse))
                {
                    PayPalResponse payPalResponse = this.DeserializeIpn(requestBody);
                    ProcessPayPalPurchase(payPalResponse);
                }
            }
            catch (Exception ex)
            {
                Debug.Assert(false, ex.Message);
                _logger.InsertError(ex.Message, "PaymentGateway", "ProcessPayPalIPN");
            }
        }

        private bool VeriFyPurchase(string verificationResponse)
        {
            if (verificationResponse.Equals("VERIFIED"))
                return true;
            else if (verificationResponse.Equals("INVALID"))
            {
                _logger.InsertSecurity("INVALID PAYPAL IPN", "PaymentGateway", "VerifyPurchase");
            }
            return false;
        }

        private void ProcessPayPalPurchase(PayPalResponse ipnResponse)
        {
            if (ipnResponse == null)
                return;

            if (ipnResponse.payment_status?.ToLower() != "completed")
                return;
            try
            {
                using (var transactionScope = new TransactionScope())
                using (var context = new TreeMonDbContext(_dbConnectionKey))
                {
                    Order o = context.GetAll<Order>().FirstOrDefault(w => w.CartUUID == ipnResponse.custom);

                    if (o == null)
                    {   //  get order by shoppingCartUUID == ipnResponse.custom
                        Debug.Assert(false, "ORDER NOT FOUND");
                        _logger.InsertError("ORDER NOT FOUND custom value:" + ipnResponse.custom, "PaymentGateway", "ProcessPayPalPurchase");
                        return;
                    }

                    if (o.TransactionID == ipnResponse.txn_id)
                    { // check that Txn_id has not been previously processed
                        Debug.Assert(false, "TRANSACTION ALREADY PROCESSED");
                        _logger.InsertError("TRANSACTION ALREADY PROCESSED:" + ipnResponse.txn_id, "PaymentGateway", "ProcessPayPalPurchase");
                        return;
                    }

                    if (o.Total > ipnResponse.mc_gross)
                    {
                        // Debug.Assert(false, "UNDERPAYMENT RECIEVED");
                        o.PayStatus = LedgerFlag.Status.PaymentPartialRecieved;
                        _logger.InsertInfo("UNDERPAYMENT RECIEVED order uuid:" + o.UUID, "PaymentGateway", "ProcessPayPalPurchase");
                       // return;
                    }
                    if (o.Total < ipnResponse.mc_gross)
                    {
                        o.PayStatus = LedgerFlag.Status.OverPaymentReceived;
                        //Debug.Assert(false, "OVERPAYMENT RECIEVED");
                        _logger.InsertInfo("OVERPAYMENT RECIEVED order uuid:" + o.UUID, "PaymentGateway", "ProcessPayPalPurchase");
                       // return;
                    }
                    if (o.Total == ipnResponse.mc_gross)
                    {
                        o.PayStatus = LedgerFlag.Status.Paid;
                    }

                    FinanceAccount financeAccount = context.GetAll<FinanceAccount>().FirstOrDefault(w => w.UUID == o.FinancAccountUUID);

                    if (financeAccount == null)
                    {
                        Debug.Assert(false, "Unable to find finance account.");
                        _logger.InsertInfo("Unable to find finance account.:" + o.FinancAccountUUID, "PaymentGateway", "ProcessPayPalPurchase");
                        return;
                    }

                    if (!financeAccount.Email.EqualsIgnoreCase(ipnResponse.receiver_email))
                    {   // check that Receiver_email is your Primary PayPal email
                        Debug.Assert(false, "Receiver_email doesn't match financeAccount Email");
                        _logger.InsertInfo("Receiver_email doesn't match financeAccount Email:" + ipnResponse.receiver_email + ":" + financeAccount.Email, "PaymentGateway", "ProcessPayPalPurchase");
                        return;
                    }
                    Currency currency =context.GetAll<Currency>().FirstOrDefault(w => w.UUID == o.CurrencyUUID);
                    if (currency == null)
                    {
                        Debug.Assert(false, "Unable to find currency .");
                        _logger.InsertInfo("Unable to find currency .:" + o.CurrencyUUID, "PaymentGateway", "ProcessPayPalPurchase");
                        return;
                    }
                    if (!currency.Code.EqualsIgnoreCase(ipnResponse.mc_currency))
                    {                        // check that mc_gross/mc_currency = USD are correct
                        Debug.Assert(false, "mc_currency doesn't match currency.Code");
                        _logger.InsertInfo("mc_currency doesn't match currency.Code:" + ipnResponse.mc_currency + ":" + currency.Code, "PaymentGateway", "ProcessPayPalPurchase");
                        return;
                    }

                    if (o.PayStatus == LedgerFlag.Status.Paid || o.PayStatus == LedgerFlag.Status.OverPaymentReceived)
                    {
                        List<OrderItem> orderItems = context.GetAll<OrderItem>().Where(w => w.OrderUUID == o.UUID).ToList();
                        foreach (OrderItem oi in orderItems)
                        {
                            oi.AccessGranted = true;
                            oi.AccessExpires = DateTime.UtcNow.AddDays(120); //todo make configurable.
                            context.Update<OrderItem>(oi);
                        }
                    }
                    //update order status to paid or complete etc.
                    FinanceAccountTransaction payment = new FinanceAccountTransaction()
                        {
                          AccountEmail = financeAccount.Email,
                        DateCreated = DateTime.UtcNow,
                        Image = financeAccount.Image,
                        CurrencyUUID = financeAccount.CurrencyUUID,
                        //CustomerIp = ipAddress,
                        CreationDate = DateTime.Now,
                        LastPaymentStatusCheck = DateTime.UtcNow,
                        OrderUUID = o.UUID,
                        Balance = o.Total - ipnResponse.mc_gross,
                        AmountTransferred = 0,
                        TransactionDate = DateTime.UtcNow,
                        TransactionType = LedgerFlag.TransactionTypes.Credit,
                        Status = LedgerFlag.Status.PendingIncome,
                        SelectedPaymentTypeTotal = o.Total,
                        UserUUID = o.UserUUID,
                        //   PayFromAccountUUID = todo this is the customers account id. won't need it for now. we could also use it to set up accounts where users
                        //                          can order and be billed later.
                        FinanceAccountUUID = financeAccount.UUID,
                        PayToAccountUUID = financeAccount.AccountNumber, //todo this should be the store account", 
                        PaymentTypeUUID = "PayPal",
                        SelectedPaymentTypeSymbol = currency?.Symbol
                        //    // = affiliateId,
                    };
                    context.Insert<FinanceAccountTransaction>(payment);
                  
                    transactionScope.Complete();
                }
            }
            catch (Exception ex)
            {
                Debug.Assert(false, ex.Message);
                _logger.InsertError(ex.Message, "PaymentGateway", "ProcessPayPalPurchase");
               
            }
        }

        private PayPalResponse DeserializeIpn(string response)
        {
            if (string.IsNullOrWhiteSpace(response))
                return null;

            PayPalResponse res = new PayPalResponse();

            string[] kvp = response.Split('&');

            if (kvp.Length == 0)
                return null;

            for (int i = 0; i < kvp.Length; i++)
            {
                try
                {
                        string[] tokens =  kvp[i].Split('=');

                        if (tokens.Length < 2)
                            continue;

                        string key = tokens[0].Trim().ToLower();
                        string value = tokens[1].Trim();

                        if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value))
                            continue;
                        switch (key)
                        {
                 
                              case "mc_gross": res.mc_gross = StringEx.ConvertTo<decimal>(value); break; 
                              case "protection_eligibility": res.protection_eligibility = value; break;  
                              case "address_status": res.address_status = value; break;   
                              case "payer_id":res.payer_id = value; break;  
                              case "tax": res.tax = StringEx.ConvertTo<decimal>(value); break;  
                              case "address_street": res.address_street = value; break;  
                              case "payment_date":   res.payment_date = value; break;  
                              case "payment_status": res.payment_status = value; break;   
                              case "charset":        res.charset = value; break;  
                              case "address_zip":    res.address_zip = value; break;   
                              case "first_name":     res.first_name = value; break;   
                              case "mc_fee": res.mc_fee = StringEx.ConvertTo<decimal>(value); break; 
                              case "address_country_code":        res.address_country_code = value; break;   
                              case "address_name":   res.address_name = value; break;  
                              case "notify_version": res.notify_version = StringEx.ConvertTo<float>(value); break; 
                              case "custom": res.custom = value; break; //this is set to the cartUUID, the Order has a cartUUID field. Pull the order and update all the tables. see StoreManager.ProcessPayment() to figure out all the tables to update
                              case "payer_status":   res.payer_status = value; break;   
                              case "address_country": res.address_country = value; break; 
                              case "address_city":   res.address_city = value; break;  
                              case "quantity":       res.quantity = StringEx.ConvertTo<int>(value); break;  
                              case "verify_sign":    res.verify_sign = value; break;  
                              case "payer_email":    res.payer_email = value; break;
                              case "txn_id": res.txn_id = value; break;    
                              case "payment_type":   res.payment_type = value; break;   
                              case "last_name":      res.last_name = value; break;   
                              case "address_state":  res.address_state = value; break;   
                              case "receiver_email": res.receiver_email = value; break;  
                              case "payment_fee":    res.payment_fee = StringEx.ConvertTo<decimal>(value); break;  
                              case "receiver_id":    res.receiver_id = value; break;  
                              case "txn_type":       res.txn_type = value; break;  
                              case "item_name":      res.item_name = value; break; 
                              case "mc_currency":    res.mc_currency = value; break;  
                              case "item_number":    res.item_number = value; break;
                              case "residence_country": res.residence_country = value; break;   
                              case "test_ipn":       res.test_ipn = value; break; 
                              case "handling_amount": res.handling_amount = StringEx.ConvertTo<decimal>(value); break;   
                              case "transaction_subject": res.transaction_subject = value; break; 
                              case "payment_gross":  res.payment_gross = StringEx.ConvertTo<decimal>(value); break;   
                              case "shipping": res.shipping = StringEx.ConvertTo<decimal>(value); break;
               
                    }
                }
                catch (Exception ex)
                {
                    Debug.Assert(false, ex.Message);
                    _logger.InsertError(ex.Message + " index:" + i.ToString() + " response:" + response, "PaymentGatewayManager", "DeserializeIpn");
                    continue;
                }
            }

            return res;
        }
        #endregion
     
    }
}
