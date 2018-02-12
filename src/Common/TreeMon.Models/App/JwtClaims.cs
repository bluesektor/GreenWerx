// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeMon.Models.App
{
    //payload section of the jwt
    //
    public  class JwtClaims
    {
       
        /// <summary>
        /// Issuer
        /// </summary>
        public string iss { get; set; }

        /// <summary>
        ///Subject
        /// </summary>
        public string sub { get; set; }

        /// <summary>
        ///Audience
        /// </summary>
        public string aud { get; set; }

        /// <summary>
        /// Expiration Time
        /// </summary>
        public string exp { get; set; }

        /// <summary>
        ///Not Before
        /// </summary>
        public string nbf { get; set; }

        /// <summary>
        ///  Issued At  when it was issued
        /// </summary>
        public string iat { get; set; }

        /// <summary>
        ///  Claim unique id
        /// </summary>
        public string jti { get; set; }

        /// <summary>
        /// csv usert role
        /// </summary>
        public string roleWeights { get; set; }
    }
}
