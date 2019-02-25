// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using Omni.Managers.Services;
using System;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Mvc;
using TreeMon.Managers;
using TreeMon.Managers.Membership;
using TreeMon.Models.App;
using TreeMon.Models.Logging;
using TreeMon.Models.Services;
using TreeMon.Utilites.Extensions;
using TreeMon.Utilites.Security;
using TreeMon.Web.api.Helpers;
using TreeMon.Web.Models;
using WebApiThrottle;

namespace TreeMon.Web.api.v1
{
    public class SiteController : ApiBaseController
    {

        public SiteController()
        {

        }

        [HttpPost]
        [System.Web.Http.AllowAnonymous]
        [EnableThrottling( PerHour = 1,  PerDay = 3)]
        [Route("api/Site/SendMessage")]
        public async Task<ServiceResult> SendMessage(Message form)//        public  ServiceResult SendMessage(Message form)
        {
            if(form == null)
              return  ServiceResponse.Error("No form was posted to the server.");

        
            form.DateSent = DateTime.UtcNow;

            bool isValidFormat = Validator.IsValidEmailFormat(form.SentFrom);
            if (string.IsNullOrWhiteSpace(form.SentFrom) || isValidFormat == false)
            {
                ServiceResponse.Error("You must provide a valid email address.");
            }

            if (string.IsNullOrWhiteSpace(form.Comment))
            {
                ServiceResponse.Error("You must provide a message.");
            }
            NetworkHelper network = new NetworkHelper();
            string ipAddress = network.GetClientIpAddress(this.Request);

            EmailLog emailLog = new EmailLog();
            emailLog.Message = form.Comment + "<br/><br/><br/>Message Key:" + emailLog.UUID;
            emailLog.Subject = form.Subject;
            emailLog.EmailFrom = Cipher.Crypt(Globals.Application.AppSetting("AppKey"), form.SentFrom, true);
            emailLog.UUIDType += "." + form.Type;

            if (form.Type?.ToLower() != "contactus")
                emailLog.EmailTo = Globals.Application.AppSetting("SiteEmail");

            emailLog.DateCreated = DateTime.UtcNow;
            emailLog.IpAddress = ipAddress;
            emailLog.Status = "not_sent";

            if (CurrentUser != null)
            {
                emailLog.CreatedBy = CurrentUser.UUID;
                emailLog.AccountUUID = CurrentUser.AccountUUID;
            }
            else
            {
                emailLog.CreatedBy = "ContactUsForm";
                emailLog.AccountUUID = "ContactUsForm";
            }

            EmailLogManager emailLogManager = new EmailLogManager(Globals.DBConnectionKey);
            if (emailLogManager.Insert(emailLog).Code == 500)
            {
                return ServiceResponse.Error("Failed to save the email. Try again later.");
            }

            EmailSettings settings = new EmailSettings();
            settings.EncryptionKey = Globals.Application.AppSetting("AppKey");
            settings.HostPassword = Globals.Application.AppSetting("EmailHostPassword");
            settings.HostUser = Globals.Application.AppSetting("EmailHostUser");
            settings.MailHost = Globals.Application.AppSetting("MailHost");
            settings.MailPort = StringEx.ConvertTo<int>(Globals.Application.AppSetting("MailPort"));
            settings.SiteDomain = Globals.Application.AppSetting("SiteDomain");
            settings.EmailDomain = Globals.Application.AppSetting("EmailDomain");
            settings.SiteEmail = Globals.Application.AppSetting("SiteEmail");
            settings.UseSSL = StringEx.ConvertTo<bool>(Globals.Application.AppSetting("UseSSL"));

            MailAddress ma = new MailAddress(settings.SiteEmail, settings.SiteEmail);
            MailMessage mail = new MailMessage();
            mail.From = ma;
            // mail.ReplyToList.Add( ma );
            mail.ReplyToList.Add(form.SentFrom);
            mail.To.Add(emailLog.EmailTo);
            mail.Subject = emailLog.Subject;
            mail.Body = emailLog.Message + "<br/><br/><br/>IP:" + ipAddress; 
            mail.IsBodyHtml = true;
            SMTP svc = new SMTP(Globals.DBConnectionKey, settings);
            return await svc.SendMailAsync(mail);
        }
    }
}
