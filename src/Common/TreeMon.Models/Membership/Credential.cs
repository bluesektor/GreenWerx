// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TreeMon.Models.Membership
{

    //Credential          Recipient                       Credentialing Body              Participation
    //Certification       Individual                      Association/Agency              Voluntary
    //Licensure/State     Certification                   Individual Government Agency    Involuntary/Required
    //Accreditation       Institution or Program          Association/Agency              Voluntary

    [Table("Credentials")]
    public class Credential : Node, INode
    {
        public Credential()
        {
            UUIDType = "Credential";
        }
        /// <summary>
        ///  Accreditation ,  Certifications, Licenses, Achievement, Award and Recognition article?
        /// </summary>
        public string Type { get; set; }

        public DateTime? Issued { get; set; }

        public DateTime? Expires { get; set; }

        public string Number { get; set; }

        /// <summary>
        /// issuing body; state, county,city,person/entity/buisiness
        /// </summary>
        public string IssuingBody { get; set; }

        public string ReferenceURL { get; set; }

        public string CredentialURL { get; set; }

        [StringLength(32)]
        public string LocationUUID { get; set; }

        public string Description { get; set; }

        public string Exemptions { get; set; }

        /// <summary>
        /// who is recieving the creditation 
        /// </summary>
        [StringLength(32)]
        public string RecipientUUID { get; set; }
        /// <summary>
        /// vendor,dispensary, user etc...
        /// </summary>
        [StringLength(32)]
        public string RecipientType { get; set; }

        /// <summary>
        /// what is being credited
        /// </summary>
        [StringLength(32)]
        public string ProductUUID { get; set; }

        /// <summary>
        /// product, plant, strain etc
        /// </summary>
        [StringLength(32)]
        public string ProductType { get; set; }

     

    }
}