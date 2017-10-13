// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

//using Logging;
namespace Omni.Managers.Services
{
    using System;
    using System.Net;
    using System.Net.Mail;
    using System.Reflection;
    using System.Threading.Tasks;
    using TreeMon.Data.Logging;
    using TreeMon.Models.App;
    using TreeMon.Models.Services;
    using TreeMon.Utilites.Security;
    public class SMTP 
    {
        private readonly SystemLogger _logger = null;

        private readonly EmailSettings _settings = new EmailSettings();

        readonly string className = MethodBase.GetCurrentMethod().DeclaringType.ToString();


        public bool HasError { get; set; }
        public string ErrorMessage { get; set; }

        public string SiteFromAddress { get; set; }

        public SMTP(string  connectionKey, EmailSettings settings )
        {
            HasError = false;
            ErrorMessage = "";

            _logger = new SystemLogger(connectionKey);
            _settings.MailPort = 587;
            _settings.MailHost = "smtp.gmail.com";

            _settings = settings;

            if (string.IsNullOrWhiteSpace(settings.MailHost))
            {
               
                SetStatus(true, "The applications email host is not set in the config file.", false);
                _logger.InsertError(this.ErrorMessage, "SMTP", "constructor");
            }
           
             if (settings.MailPort <= 0)            {
                SetStatus(true, "The applications email host port is not set in the config file.", true);
                _logger.InsertError(this.ErrorMessage, "SMTP", "constructor");
            }

            if (string.IsNullOrWhiteSpace(settings.HostUser)) {
                   SetStatus(true, "The applications email host user is not set in the config file.", true);
                _logger.InsertError(this.ErrorMessage, "SMTP", "constructor");
            }

            if (string.IsNullOrWhiteSpace(settings.HostPassword ))
            {
                SetStatus(true, "The applications email host password is not set in the config file.", true);
                _logger.InsertError(this.ErrorMessage, "SMTP", "constructor");
            }
            
        }

        protected void SetStatus(bool hasError, string message, bool appendMessage)
        {
           HasError = hasError;
            if (appendMessage)
                ErrorMessage += message;
            else
                ErrorMessage = message;
        }

        public async Task<ServiceResult> SendMailAsync(MailMessage msg)
        {
            if (HasError)
                return ServiceResponse.Error(ErrorMessage);

            try
            {
                if(string.IsNullOrWhiteSpace(_settings.EncryptionKey))
                    return ServiceResponse.Error("The encryption key is not set.");


                //   
                string hostPassword = Cipher.Crypt("1000Gxzd8pGraRQD0LLUc2ITQmWIw6AkuQ8V0BpDutnqiG/zpjZOss/EWUYtgXpKsKJw", "@Th3Dug#P4rty", true);

                //this is dev.treemon.org
                 hostPassword = Cipher.Crypt("1000Gxzd8pGraRQD0LLUc2ITQmWIw6AkuQ8V0BpDutnqiG/zpjZOss/EWUYtgXpKsKJw", "IpDbcT0hVCrCCJ/rS0gtx3NW47rVfDBsczRLVtCy/IS1xo/K/3aAOeQUgrpJv30LdZfeL40rFzp7W3tdkh31K5g0fKu2y4Oqv2IC+WY835oGyfIcgsFPxJaz+dN7TWf5", false);
                // string hostPassword = Cipher.Crypt(_settings.EncryptionKey,_settings.HostPassword, false);
                //to use gmail without oauth you need to turn on 2 step verification in gmail account and generate an app key.

                SmtpClient smtp = new SmtpClient();
                smtp.Host = _settings.MailHost;
                smtp.Port = _settings.MailPort;
                smtp.EnableSsl = _settings.UseSSL;
                if (smtp.Host.Contains("gmail"))//this wasn't needed for other hosts.
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                //   smtp.Timeout = 20000
                smtp.Credentials = new NetworkCredential(_settings.HostUser, hostPassword);
                await smtp.SendMailAsync(msg);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                if (ex.InnerException != null)
                    ErrorMessage += ex.InnerException;

                _logger.InsertError(ErrorMessage, "SMTP", "SendMail(MailMessage)");
                return ServiceResponse.Error(ErrorMessage);
            }

           return ServiceResponse.OK();
        }

        public ServiceResult SendMail(MailMessage msg)
        {
            if (HasError)
                return ServiceResponse.Error(ErrorMessage);

            try
            {
                if (string.IsNullOrWhiteSpace(_settings.EncryptionKey))
                    return ServiceResponse.Error("The encryption key is not set.");

                string hostPassword = Cipher.Crypt(_settings.EncryptionKey, _settings.HostPassword, false); 

                //to user gmail without oauth you need to turn on 2 step verification in gmail account and generate an app key.
                SmtpClient smtp = new SmtpClient();
                
                smtp.Host = _settings.MailHost;
                smtp.Port = _settings.MailPort;
                smtp.EnableSsl = _settings.UseSSL;
                if (smtp.Host.Contains("gmail"))//this wasn't needed for other hosts.
                {
                    smtp.UseDefaultCredentials = false;
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                }
                //   smtp.Timeout = 20000
                smtp.Credentials = new NetworkCredential(_settings.HostUser, hostPassword);

                 smtp.Send(msg);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                if (ex.InnerException != null)
                    ErrorMessage += ex.InnerException;

                _logger.InsertError(ErrorMessage, "SMTP", "SendMail(MailMessage)");
                return ServiceResponse.Error(ErrorMessage);
            }

            return ServiceResponse.OK();
        }


        private void testsend()
        {
            try
            {
                MailMessage mail = new MailMessage();

                //set the addresses
                mail.From = new MailAddress("USERNAME@DOMAIN.com"); //IMPORTANT: This must be same as your smtp authentication address.
                mail.To.Add("USERNAME@DOMAIN.com");

                //set the content
                mail.Subject = "This is an email";
                mail.Body = "This is from system.net.mail using C sharp with smtp authentication.";
                //send the message
                SmtpClient smtp = new SmtpClient("mail.DOMAIN.com");
                smtp.Port = 25; //  8889
                //IMPORANT:  Your smtp login email MUST be same as your FROM address.
                NetworkCredential Credentials = new NetworkCredential("USERNAME@DOMAIN.com", "PASSWORD!");
                smtp.Credentials = Credentials;
                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                string Message = ex.Message;
                if (ex.InnerException != null)
                    Message += ex.InnerException;
                
                _logger.InsertError(Message, "SMTP", MethodInfo.GetCurrentMethod().Name);
            }
        }

 
    }
}

