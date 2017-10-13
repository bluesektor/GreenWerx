// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using TreeMon.Data.Logging;
using TreeMon.Managers.Finance;
using TreeMon.Models.App;
using TreeMon.Web;
using TreeMon.Web.api;
using TreeMon.Web.api.Helpers;
using WebApiThrottle;

namespace TreeMon.WebAPI.api.v1
{
    public class PaymentGateway : ApiBaseController
    {
        readonly SystemLogger _logger = null;
        readonly PaymentGatewayManager _gatewayManager = null;

        public PaymentGateway()
        {
            _gatewayManager = new PaymentGatewayManager(Globals.DBConnectionKey);
            _logger = new SystemLogger(Globals.DBConnectionKey);
        }

        //Send posts 
        //https://developer.paypal.com/developer/ipnSimulator/
        //
        [AllowAnonymous]
        [HttpPost]
        [EnableThrottling(PerSecond = 5)]
        [Route("api/PaymentGateway/PayPalIPN")]
        //public async Task<HttpStatusCodeResult> ProcessPayPalIPN()
        public async Task<ServiceResult> ProcessPayPalIPN()
        {
            NetworkHelper network = new NetworkHelper();
            string ip = network.GetClientIpAddress(this.Request);

            byte[] paramArray = await Request.Content.ReadAsByteArrayAsync();
            var content = System.Text.Encoding.ASCII.GetString(paramArray);
          

#if DEBUG
            string ipnSample = @"mc_gross = 19.95 & protection_eligibility = Eligible & address_status = confirmed & payer_id = LPLWNMTBWMFAY & 
                                        tax = 0.00 & address_street = 1 + Main + St & payment_date = 20 % 3A12 % 3A59 + Jan + 13 % 2C + 2009 + PST & payment_status = Completed & 
                                        charset = windows - 1252 & address_zip = 95131 & first_name = Test & mc_fee = 0.88 & address_country_code = US & address_name = Test + User & 
                                        notify_version = 2.6 & custom = &payer_status = verified & address_country = United + States & address_city = San + Jose & quantity = 1 & 
                                        verify_sign = AtkOfCXbDm2hu0ZELryHFjY - Vb7PAUvS6nMXgysbElEn9v - 1XcmSoGtf & payer_email = gpmac_1231902590_per % 40paypal.com & txn_id = 61E67681CH3238416 & payment_type = instant & last_name = User & address_state = CA & receiver_email = gpmac_1231902686_biz % 40paypal.com & 
                                        payment_fee = 0.88 & receiver_id = S8XGHLYDW9T3S & txn_type = express_checkout & item_name = &mc_currency = USD & item_number = &residence_country = US & test_ipn = 1 & handling_amount = 0.00 & transaction_subject = &payment_gross = 19.95 & shipping = 0.00";
            content = ipnSample;

            _gatewayManager.ProcessIpn(content, ip);

#else
           //Fire and forget verification task
            Thread t = new Thread(() => _gatewayManager.ProcessIpn(content, ip));
            t.Start();
#endif

            // return new HttpStatusCodeResult(HttpStatusCode.OK);
            return ServiceResponse.OK();
        }
    }

}