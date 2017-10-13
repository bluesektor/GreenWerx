// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TreeMon.Models.Helpers;

namespace TreeMon.Models.Plant
{
    [Table("Strains")]
    public class Strain : Node
    {
        public Strain()
        {
            UUIDType = "Strain";

        }

        [JsonConverter(typeof(BoolConverter))]
        public bool AutoFlowering { get; set; }

        public string GrowthRate { get; set; }

        public string FlowerColor { get; set; }

        public string Generation { get; set; }

        /// <summary>
        /// Chronological  order
        /// </summary>
        public int ChronOrder { get; set; }

        /// <summary>
        /// reserved: this is for a future implementation, hopefully
        /// an easy way to uniquely identify the strain
        /// </summary>
        public string Signature { get; set; }

        [StringLength(32)]
        public string BreederUUID { get; set; }

        public decimal Height { get; set; }

        public string HeightUOM { get; set; }


        /// <summary>
        /// How many days from planting to harvest
        /// </summary>
        public int HarvestTime { get; set; }

        [StringLength(32)]
        public string CategoryUUID { get; set; }

        public decimal IndicaPercent { get; set; }

        public decimal SativaPercent { get; set; }

        public string Lineage { get; set; }


        //        Genetics--Terpene Profiles--
        //
        //        System is designed to analyze, characterize and codify the subtleties in terpene differences across a large number of separate genetic groups(as per the color coded system), different populations within those groups, and time series analysis tracking where applicable(i.e.: terpene ratio and/or quantity variation during final weeks of flower development). Individuals will be grouped into different color groups based initially on some qualitative characteristics such as `nose` (piney, fruity, etc.), and later quantitatively.Quantitative analysis will allow for each individual to be profiled into the database.
        // Chemotype Profiles--
        // These can have the same framework as the terpene program, but can include cannabinoids and other secondary metabolites of interest.
        // Bioinformatics--
        // The use of evolutionary algorithms to run computer models of mass breeding programs that can allow for increased efficiency in parent material selection as well as accurately estimating required population sizes for field trials.
        //   phenotype I, II, or IV plants that are high in THC, CBD, and/or CBG


        #region Pollenating
        //        Kind of plant____________________________________________

        //Seed parent_________________________________________________

        //ÝÝÝÝÝÝÝÝÝÝÝÝÝÝÝÝÝÝÝÝÝÝÝ(name or number)

        //Pollen parent________________________________________________

        //ÝÝÝÝÝÝÝÝÝÝÝÝÝÝÝÝÝÝÝÝÝÝÝ(name or number)

        //Date cross made _________ Number to be assigned offspring __________

        //Traits of seed parent__________________________________________

        //__________________________________________________________

        //Traits of pollen parent________________________________________

        //__________________________________________________________

        //Traits of desired
        //offspring________________________________________

        //        //Offspring notes:
        //        Date planted 
        //            Date of first flowering   
        //Traits:size,color,etc

        //Date of pollination:first,last
        //Harvesting information:date,no.of seed

        //Phenotype?
        //FlowerCompactness? how tight the buds are
        #endregion


        /// <summary>
        /// BAKCklog research this. string for now til get more info
        /// Creating a genetically stable variety involves selectively choosing male and female cannabis plants and breeding them over 
        /// the course of multiple generations.The final generation's seeds reliably grow into plants that exhibit the desired characteristics, 
        ///though some genetic variation will still occur.
        /// </summary>
        public string SeedStability { get; set; }


    }
}
