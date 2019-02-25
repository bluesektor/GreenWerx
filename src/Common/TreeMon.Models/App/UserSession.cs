// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TreeMon.Models.App
{
    [Table("UserSessions")]
    public  class UserSession
    {
        public UserSession()
        {
            UUID = Guid.NewGuid().ToString("N");
            UUIDType = "UserSession";
        }
        public int RoleWeight { get; set; }

        // [SQLite.PrimaryKey,SQLite.AutoIncrement]]
        [Key]
        public int Id { get; set; }


        public int ParentId { get; set; }

        [StringLength(32)]
        public string UUID { get; set; }
        /// <summary>
        /// Defines the type of SettingUUID used (guid, hash.<algo> )..
        /// </summary>
        [StringLength(32)]
        public string UUIDType { get; set; }

        [StringLength(32)]
        public string UserUUID { get; set; }
        public string AuthToken { get; set; }
        public System.DateTime Issued { get; set; }
        public System.DateTime Expires { get;set; }

        //this will be the default account they sign in under
        [StringLength(32)]
        public string AccountUUID { get; set; }

        public bool IsPersistent { get; set; }
        public string UserData { get; set; }
        [StringLength(32)]
        public string UserName { get; set; }

        public string UserType { get; set; }
        public string Message { get; set; }

        public string Captcha { get; set; }

        public string CartTrackingId { get; set; }

        public string ShoppingCartUUID { get; set; }

        // todo implement this as a security measure to
        // prevent token jacking. The this uuid is generated
        // on the client and sent during login and stored in session table.
        // Use browser info?
        #region javascript example
        /**.
         * NOTE: must be able to replicate this on client and server side.
         * test: have angular client login and to create a session. copy the
         * auth token and use postman to send a request. it should get rejected.
         * try doing this only for phone app clients at first.
* @function _guid
* @description Creates GUID for user based on several different browser variables
* It will never be RFC4122 compliant but it is robust
* @returns {Number}
* @private
*/
//        var guid = function() {

//    var nav = window.navigator;
//        var screen = window.screen;
//        var guid = nav.mimeTypes.length;
//        guid += nav.userAgent.replace(/\D+/g, '');
//    guid += nav.plugins.length;
//    guid += screen.height || '';
//    guid += screen.width || '';
//    guid += screen.pixelDepth || '';

//    return guid;
//};
    #endregion
        // public string ClientUUID { get; set; }
        /// <summary>
        /// In minutes
        /// </summary>
        [NotMapped]
        public int SessionLength { get; set; }
    }
}
