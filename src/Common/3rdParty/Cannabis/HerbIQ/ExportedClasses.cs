using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HerbIQ
{
    /* 
 Licensed under the Apache License, Version 2.0
    
 http://www.apache.org/licenses/LICENSE-2.0
 */
    using System;
    using System.Xml.Serialization;
    using System.Collections.Generic;
    namespace Xml2CSharp
    {
        [XmlRoot(ElementName = "UserOptions")]
        public class UserOptions
        {
            [XmlElement(ElementName = "LastManualSensorLoad")]
            public string LastManualSensorLoad { get; set; }
            [XmlElement(ElementName = "EncryptionPassword")]
            public string EncryptionPassword { get; set; }
            [XmlElement(ElementName = "Maximized")]
            public string Maximized { get; set; }
            [XmlElement(ElementName = "Height")]
            public string Height { get; set; }
            [XmlElement(ElementName = "Width")]
            public string Width { get; set; }
            [XmlElement(ElementName = "StretchMode")]
            public string StretchMode { get; set; }
            [XmlElement(ElementName = "Top")]
            public string Top { get; set; }
            [XmlElement(ElementName = "Left")]
            public string Left { get; set; }
            [XmlElement(ElementName = "LoadSensorOnRefresh")]
            public string LoadSensorOnRefresh { get; set; }
            [XmlElement(ElementName = "LastGraphSensorLoad")]
            public string LastGraphSensorLoad { get; set; }
            [XmlElement(ElementName = "AutoRefresh")]
            public string AutoRefresh { get; set; }
            [XmlElement(ElementName = "AutoBackup")]
            public string AutoBackup { get; set; }
            [XmlElement(ElementName = "AutoBackupCount")]
            public string AutoBackupCount { get; set; }
            [XmlElement(ElementName = "CustomTrackHome")]
            public string CustomTrackHome { get; set; }
            [XmlElement(ElementName = "QuadA")]
            public string QuadA { get; set; }
            [XmlElement(ElementName = "QuadB")]
            public string QuadB { get; set; }
            [XmlElement(ElementName = "QuadC")]
            public string QuadC { get; set; }
            [XmlElement(ElementName = "QuadD")]
            public string QuadD { get; set; }
            [XmlElement(ElementName = "QuadATrackingDate")]
            public string QuadATrackingDate { get; set; }
            [XmlElement(ElementName = "QuadBTrackingDate")]
            public string QuadBTrackingDate { get; set; }
            [XmlElement(ElementName = "QuadCTrackingDate")]
            public string QuadCTrackingDate { get; set; }
            [XmlElement(ElementName = "QuadDTrackingDate")]
            public string QuadDTrackingDate { get; set; }
        }

        [XmlRoot(ElementName = "event")]
        public class Event
        {
            [XmlElement(ElementName = "DisplayName")]
            public string DisplayName { get; set; }
            [XmlElement(ElementName = "Date")]
            public string Date { get; set; }
            [XmlElement(ElementName = "Notes")]
            public string Notes { get; set; }
            [XmlElement(ElementName = "Type")]
            public string Type { get; set; }
            [XmlElement(ElementName = "InternalEventID")]
            public string InternalEventID { get; set; }
        }

      

        [XmlRoot(ElementName = "HeightEvent")]
        public class HeightEvent
        {
            [XmlElement(ElementName = "Height")]
            public string Height { get; set; }
            [XmlElement(ElementName = "Date")]
            public string Date { get; set; }
            [XmlElement(ElementName = "InternalHeightID")]
            public string InternalHeightID { get; set; }
        }

       

        [XmlRoot(ElementName = "plant")]
        public class Plant
        {
            [XmlElement(ElementName = "DisplayName")]
            public string DisplayName { get; set; }
            [XmlElement(ElementName = "Description")]
            public string Description { get; set; }
            [XmlElement(ElementName = "Breeder")]
            public string Breeder { get; set; }
            [XmlElement(ElementName = "AdvertisedHeight")]
            public string AdvertisedHeight { get; set; }
            [XmlElement(ElementName = "AdvertisedFloweringTime")]
            public string AdvertisedFloweringTime { get; set; }
            [XmlElement(ElementName = "IndicaSativa")]
            public string IndicaSativa { get; set; }
            [XmlElement(ElementName = "MotherPlantID")]
            public string MotherPlantID { get; set; }
            [XmlElement(ElementName = "FatherPlantID")]
            public string FatherPlantID { get; set; }
            [XmlElement(ElementName = "Quantity")]
            public string Quantity { get; set; }
            [XmlElement(ElementName = "CurrentPhase")]
            public string CurrentPhase { get; set; }
            [XmlElement(ElementName = "Female")]
            public string Female { get; set; }
            [XmlElement(ElementName = "Pollinated")]
            public List<string> Pollinated { get; set; }
            [XmlElement(ElementName = "Feminized")]
            public string Feminized { get; set; }
            [XmlElement(ElementName = "AutoFlower")]
            public string AutoFlower { get; set; }
            [XmlElement(ElementName = "VisibleID")]
            public string VisibleID { get; set; }
            [XmlElement(ElementName = "DateAcquired")]
            public string DateAcquired { get; set; }
            [XmlElement(ElementName = "GermedOn")]
            public string GermedOn { get; set; }
            [XmlElement(ElementName = "HarvestedOn")]
            public string HarvestedOn { get; set; }
            [XmlElement(ElementName = "CuringOn")]
            public string CuringOn { get; set; }
            [XmlElement(ElementName = "FloweredOn")]
            public string FloweredOn { get; set; }
            [XmlElement(ElementName = "VeggedOn")]
            public string VeggedOn { get; set; }
            [XmlElement(ElementName = "DateCloned")]
            public string DateCloned { get; set; }
            [XmlElement(ElementName = "TargetDate")]
            public string TargetDate { get; set; }
            [XmlElement(ElementName = "InternalID")]
            public string InternalID { get; set; }
            [XmlElement(ElementName = "ActualHeight")]
            public string ActualHeight { get; set; }
            [XmlElement(ElementName = "WetWeight")]
            public string WetWeight { get; set; }
            [XmlElement(ElementName = "DryWeight")]
            public string DryWeight { get; set; }
            [XmlElement(ElementName = "TrimWeight")]
            public string TrimWeight { get; set; }
            [XmlElement(ElementName = "HashWeight")]
            public string HashWeight { get; set; }
            [XmlElement(ElementName = "PhenoType")]
            public string PhenoType { get; set; }
            [XmlElement(ElementName = "TotalHours")]
            public string TotalHours { get; set; }
            [XmlElement(ElementName = "AutoTrack")]
            public string AutoTrack { get; set; }
            [XmlElement(ElementName = "Male")]
            public string Male { get; set; }
            [XmlElement(ElementName = "SeedsStoredSince")]
            public string SeedsStoredSince { get; set; }
            [XmlElement(ElementName = "InRoomVisibleID")]
            public string InRoomVisibleID { get; set; }
            [XmlElement(ElementName = "InRoomInternalId")]
            public string InRoomInternalId { get; set; }
            [XmlElement(ElementName = "RatingQuestion1")]
            public string RatingQuestion1 { get; set; }
            [XmlElement(ElementName = "Rating1")]
            public string Rating1 { get; set; }
            [XmlElement(ElementName = "RatingQuestion2")]
            public string RatingQuestion2 { get; set; }
            [XmlElement(ElementName = "Rating2")]
            public string Rating2 { get; set; }
            [XmlElement(ElementName = "RatingQuestion3")]
            public string RatingQuestion3 { get; set; }
            [XmlElement(ElementName = "Rating3")]
            public string Rating3 { get; set; }
            [XmlElement(ElementName = "RatingQuestion4")]
            public string RatingQuestion4 { get; set; }
            [XmlElement(ElementName = "Rating4")]
            public string Rating4 { get; set; }
            [XmlElement(ElementName = "RatingQuestion5")]
            public string RatingQuestion5 { get; set; }
            [XmlElement(ElementName = "Rating5")]
            public string Rating5 { get; set; }
            [XmlElement(ElementName = "RatingQuestion6")]
            public string RatingQuestion6 { get; set; }
            [XmlElement(ElementName = "Rating6")]
            public string Rating6 { get; set; }
            [XmlElement(ElementName = "RatingQuestion7")]
            public string RatingQuestion7 { get; set; }
            [XmlElement(ElementName = "Rating7")]
            public string Rating7 { get; set; }
            [XmlElement(ElementName = "RatingQuestion8")]
            public string RatingQuestion8 { get; set; }
            [XmlElement(ElementName = "Rating8")]
            public string Rating8 { get; set; }
            [XmlElement(ElementName = "RatingQuestion9")]
            public string RatingQuestion9 { get; set; }
            [XmlElement(ElementName = "Rating9")]
            public string Rating9 { get; set; }
            [XmlElement(ElementName = "RatingQuestion10")]
            public string RatingQuestion10 { get; set; }
            [XmlElement(ElementName = "Rating10")]
            public string Rating10 { get; set; }
            [XmlElement(ElementName = "RatingQuestion11")]
            public string RatingQuestion11 { get; set; }
            [XmlElement(ElementName = "Rating11")]
            public string Rating11 { get; set; }
            [XmlElement(ElementName = "RatingQuestion12")]
            public string RatingQuestion12 { get; set; }
            [XmlElement(ElementName = "Rating12")]
            public string Rating12 { get; set; }
            [XmlElement(ElementName = "RatingQuestion13")]
            public string RatingQuestion13 { get; set; }
            [XmlElement(ElementName = "Rating13")]
            public string Rating13 { get; set; }
            [XmlElement(ElementName = "RatingQuestion14")]
            public string RatingQuestion14 { get; set; }
            [XmlElement(ElementName = "Rating14")]
            public string Rating14 { get; set; }
            [XmlElement(ElementName = "RatingQuestion15")]
            public string RatingQuestion15 { get; set; }
            [XmlElement(ElementName = "Rating15")]
            public string Rating15 { get; set; }
            [XmlElement(ElementName = "RatingQuestion16")]
            public string RatingQuestion16 { get; set; }
            [XmlElement(ElementName = "Rating16")]
            public string Rating16 { get; set; }
            [XmlElement(ElementName = "RatingQuestion17")]
            public string RatingQuestion17 { get; set; }
            [XmlElement(ElementName = "Rating17")]
            public string Rating17 { get; set; }
            [XmlElement(ElementName = "RatingQuestion18")]
            public string RatingQuestion18 { get; set; }
            [XmlElement(ElementName = "Rating18")]
            public string Rating18 { get; set; }
            [XmlElement(ElementName = "RatingQuestion19")]
            public string RatingQuestion19 { get; set; }
            [XmlElement(ElementName = "Rating19")]
            public string Rating19 { get; set; }
            [XmlElement(ElementName = "RatingQuestion20")]
            public string RatingQuestion20 { get; set; }
            [XmlElement(ElementName = "Rating20")]
            public string Rating20 { get; set; }
            [XmlElement(ElementName = "RatingQuestion21")]
            public string RatingQuestion21 { get; set; }
            [XmlElement(ElementName = "Rating21")]
            public string Rating21 { get; set; }
            [XmlElement(ElementName = "RatingQuestion22")]
            public string RatingQuestion22 { get; set; }
            [XmlElement(ElementName = "Rating22")]
            public string Rating22 { get; set; }
            [XmlElement(ElementName = "RatingQuestion23")]
            public string RatingQuestion23 { get; set; }
            [XmlElement(ElementName = "Rating23")]
            public string Rating23 { get; set; }
            [XmlElement(ElementName = "RatingQuestion24")]
            public string RatingQuestion24 { get; set; }
            [XmlElement(ElementName = "Rating24")]
            public string Rating24 { get; set; }
            [XmlElement(ElementName = "RatingQuestion25")]
            public string RatingQuestion25 { get; set; }
            [XmlElement(ElementName = "Rating25")]
            public string Rating25 { get; set; }
            [XmlElement(ElementName = "RatingQuestion26")]
            public string RatingQuestion26 { get; set; }
            [XmlElement(ElementName = "Rating26")]
            public string Rating26 { get; set; }
            [XmlElement(ElementName = "RatingQuestion27")]
            public string RatingQuestion27 { get; set; }
            [XmlElement(ElementName = "Rating27")]
            public string Rating27 { get; set; }
            //[XmlElement(ElementName = "Events")]
            //public Events Events { get; set; }
            //[XmlElement(ElementName = "HeightTracking")]
            //public HeightTracking HeightTracking { get; set; }
        }

       

        [XmlRoot(ElementName = "nutrient")]
        public class Nutrient
        {
            [XmlElement(ElementName = "DisplayName")]
            public string DisplayName { get; set; }
            [XmlElement(ElementName = "Brand")]
            public string Brand { get; set; }
            [XmlElement(ElementName = "Type")]
            public string Type { get; set; }
            [XmlElement(ElementName = "TotalAmount")]
            public string TotalAmount { get; set; }
            [XmlElement(ElementName = "AmountRemaining")]
            public string AmountRemaining { get; set; }
            [XmlElement(ElementName = "DateManufactured")]
            public string DateManufactured { get; set; }
            [XmlElement(ElementName = "DateExpires")]
            public string DateExpires { get; set; }
            [XmlElement(ElementName = "InternalID")]
            public string InternalID { get; set; }
            [XmlElement(ElementName = "ID")]
            public string ID { get; set; }
        }

     

        [XmlRoot(ElementName = "ResEvent")]
        public class ResEvent
        {
            [XmlElement(ElementName = "ResHistoryTEMP")]
            public string ResHistoryTEMP { get; set; }
            [XmlElement(ElementName = "ResHistoryEC")]
            public string ResHistoryEC { get; set; }
            [XmlElement(ElementName = "ResHistoryTDS")]
            public string ResHistoryTDS { get; set; }
            [XmlElement(ElementName = "ResHistoryNotes")]
            public string ResHistoryNotes { get; set; }
            [XmlElement(ElementName = "ResHistoryPH")]
            public string ResHistoryPH { get; set; }
            [XmlElement(ElementName = "ResHistoryPPM")]
            public string ResHistoryPPM { get; set; }
            [XmlElement(ElementName = "ResHistoryDate")]
            public string ResHistoryDate { get; set; }
            [XmlElement(ElementName = "ResHistoryInternalID")]
            public string ResHistoryInternalID { get; set; }
        }

       

        [XmlRoot(ElementName = "reservoir")]
        public class Reservoir
        {
            [XmlElement(ElementName = "DisplayName")]
            public string DisplayName { get; set; }
            [XmlElement(ElementName = "Brand")]
            public string Brand { get; set; }
            [XmlElement(ElementName = "TotalCapacity")]
            public string TotalCapacity { get; set; }
            [XmlElement(ElementName = "CurrentCapacity")]
            public string CurrentCapacity { get; set; }
            [XmlElement(ElementName = "CurrentContents")]
            public string CurrentContents { get; set; }
            [XmlElement(ElementName = "ResevoirInRoomVisibleID")]
            public string ResevoirInRoomVisibleID { get; set; }
            [XmlElement(ElementName = "ResevoirInRoomInternalID")]
            public string ResevoirInRoomInternalID { get; set; }
            [XmlElement(ElementName = "ResVisibleID")]
            public string ResVisibleID { get; set; }
            [XmlElement(ElementName = "DateLastCleaned")]
            public string DateLastCleaned { get; set; }
            [XmlElement(ElementName = "DateLastChanged")]
            public string DateLastChanged { get; set; }
            [XmlElement(ElementName = "InternalID")]
            public string InternalID { get; set; }
            //[XmlElement(ElementName = "ResEvents")]
            //public ResEvents ResEvents { get; set; }
        }

    

        [XmlRoot(ElementName = "ROOM")]
        public class ROOM
        {
            [XmlElement(ElementName = "RoomInUse")]
            public string RoomInUse { get; set; }
            [XmlElement(ElementName = "RoomDisplayName")]
            public string RoomDisplayName { get; set; }
            [XmlElement(ElementName = "RoomVisibleID")]
            public string RoomVisibleID { get; set; }
            [XmlElement(ElementName = "RoomInternalID")]
            public string RoomInternalID { get; set; }
            [XmlElement(ElementName = "RoomLightCycle")]
            public string RoomLightCycle { get; set; }
        }

    

        [XmlRoot(ElementName = "BULB")]
        public class BULB
        {
            [XmlElement(ElementName = "BulbInUse")]
            public string BulbInUse { get; set; }
            [XmlElement(ElementName = "BulbTotalHoursUsed")]
            public string BulbTotalHoursUsed { get; set; }
            [XmlElement(ElementName = "BulbPurchasedDate")]
            public string BulbPurchasedDate { get; set; }
            [XmlElement(ElementName = "BulbDisplayName")]
            public string BulbDisplayName { get; set; }
            [XmlElement(ElementName = "BulbVisibleID")]
            public string BulbVisibleID { get; set; }
            [XmlElement(ElementName = "BulbInBallastVisibleID")]
            public string BulbInBallastVisibleID { get; set; }
            [XmlElement(ElementName = "BulbInBallastInternalID")]
            public string BulbInBallastInternalID { get; set; }
            [XmlElement(ElementName = "BulbInternalID")]
            public string BulbInternalID { get; set; }
            [XmlElement(ElementName = "BulbWattage")]
            public string BulbWattage { get; set; }
            [XmlElement(ElementName = "BulbAutoAge")]
            public string BulbAutoAge { get; set; }
        }

        

        [XmlRoot(ElementName = "BALLAST")]
        public class BALLAST
        {
            [XmlElement(ElementName = "BallastInUse")]
            public string BallastInUse { get; set; }
            [XmlElement(ElementName = "BallastTotalHoursUsed")]
            public string BallastTotalHoursUsed { get; set; }
            [XmlElement(ElementName = "BallastPurchasedDate")]
            public string BallastPurchasedDate { get; set; }
            [XmlElement(ElementName = "BallastDisplayName")]
            public string BallastDisplayName { get; set; }
            [XmlElement(ElementName = "BallastVisibleID")]
            public string BallastVisibleID { get; set; }
            [XmlElement(ElementName = "BallastInternalID")]
            public string BallastInternalID { get; set; }
            [XmlElement(ElementName = "BallastInRoomVisibleID")]
            public string BallastInRoomVisibleID { get; set; }
            [XmlElement(ElementName = "BallastInRoomInternalID")]
            public string BallastInRoomInternalID { get; set; }
            [XmlElement(ElementName = "BallastWattage")]
            public string BallastWattage { get; set; }
            [XmlElement(ElementName = "BallastAutoAge")]
            public string BallastAutoAge { get; set; }
        }

      
        [XmlRoot(ElementName = "FAN")]
        public class FAN
        {
            [XmlElement(ElementName = "FanInUse")]
            public string FanInUse { get; set; }
            [XmlElement(ElementName = "FanTotalHoursUsed")]
            public string FanTotalHoursUsed { get; set; }
            [XmlElement(ElementName = "FanPurchasedDate")]
            public string FanPurchasedDate { get; set; }
            [XmlElement(ElementName = "FanDisplayName")]
            public string FanDisplayName { get; set; }
            [XmlElement(ElementName = "FanVisibleID")]
            public string FanVisibleID { get; set; }
            [XmlElement(ElementName = "FanInternalID")]
            public string FanInternalID { get; set; }
            [XmlElement(ElementName = "FanInRoomVisibleID")]
            public string FanInRoomVisibleID { get; set; }
            [XmlElement(ElementName = "FanInRoomInternalID")]
            public string FanInRoomInternalID { get; set; }
            [XmlElement(ElementName = "FanCFM")]
            public string FanCFM { get; set; }
            [XmlElement(ElementName = "FanAutoAge")]
            public string FanAutoAge { get; set; }
        }

      

        [XmlRoot(ElementName = "PATIENT")]
        public class PATIENT
        {
            [XmlElement(ElementName = "PatientName")]
            public string PatientName { get; set; }
            [XmlElement(ElementName = "PatientVisibleID")]
            public string PatientVisibleID { get; set; }
            [XmlElement(ElementName = "InternalID")]
            public string InternalID { get; set; }
            [XmlElement(ElementName = "PatientPlantCount")]
            public string PatientPlantCount { get; set; }
            [XmlElement(ElementName = "PatientLicenseNumber")]
            public string PatientLicenseNumber { get; set; }
            [XmlElement(ElementName = "PatientAccountBalance")]
            public string PatientAccountBalance { get; set; }
            [XmlElement(ElementName = "PatientDispenseDate")]
            public string PatientDispenseDate { get; set; }
            [XmlElement(ElementName = "PatientLicenseExpiresDate")]
            public string PatientLicenseExpiresDate { get; set; }
            [XmlElement(ElementName = "PatientSinceDate")]
            public string PatientSinceDate { get; set; }
        }

      

        [XmlRoot(ElementName = "NOTE")]
        public class NOTE
        {
            [XmlElement(ElementName = "NoteDisplayName")]
            public string NoteDisplayName { get; set; }
            [XmlElement(ElementName = "NoteVisibleID")]
            public string NoteVisibleID { get; set; }
            [XmlElement(ElementName = "InternalID")]
            public string InternalID { get; set; }
            [XmlElement(ElementName = "NoteType")]
            public string NoteType { get; set; }
            [XmlElement(ElementName = "NoteDate")]
            public string NoteDate { get; set; }
            [XmlElement(ElementName = "NoteNote")]
            public string NoteNote { get; set; }
        }


        [XmlRoot(ElementName = "EXPENSE")]
        public class EXPENSE
        {
            [XmlElement(ElementName = "ExpenseDisplayName")]
            public string ExpenseDisplayName { get; set; }
            [XmlElement(ElementName = "ExpenseVisibleID")]
            public string ExpenseVisibleID { get; set; }
            [XmlElement(ElementName = "InternalID")]
            public string InternalID { get; set; }
            [XmlElement(ElementName = "ExpenseType")]
            public string ExpenseType { get; set; }
            [XmlElement(ElementName = "ExpenseDate")]
            public string ExpenseDate { get; set; }
            [XmlElement(ElementName = "ExpenseDescription")]
            public string ExpenseDescription { get; set; }
            [XmlElement(ElementName = "ExpenseDollarsOut")]
            public string ExpenseDollarsOut { get; set; }
        }

    

        [XmlRoot(ElementName = "INCOME")]
        public class INCOME
        {
            [XmlElement(ElementName = "IncomeDisplayName")]
            public string IncomeDisplayName { get; set; }
            [XmlElement(ElementName = "IncomeVisibleID")]
            public string IncomeVisibleID { get; set; }
            [XmlElement(ElementName = "InternalID")]
            public string InternalID { get; set; }
            [XmlElement(ElementName = "IncomeDescription")]
            public string IncomeDescription { get; set; }
            [XmlElement(ElementName = "IncomePatient")]
            public string IncomePatient { get; set; }
            [XmlElement(ElementName = "IncomeDollarsIn")]
            public string IncomeDollarsIn { get; set; }
            [XmlElement(ElementName = "IncomeDate")]
            public string IncomeDate { get; set; }
        }

      

        [XmlRoot(ElementName = "ENVIRONMENT")]
        public class ENVIRONMENT
        {
            [XmlElement(ElementName = "EnvironmentTemp")]
            public string EnvironmentTemp { get; set; }
            [XmlElement(ElementName = "EnvironmentHumidity")]
            public string EnvironmentHumidity { get; set; }
            [XmlElement(ElementName = "EnvironmentLight")]
            public string EnvironmentLight { get; set; }
            [XmlElement(ElementName = "EnvironmentC02")]
            public string EnvironmentC02 { get; set; }
            [XmlElement(ElementName = "EnvironmentPreassure")]
            public string EnvironmentPreassure { get; set; }
            [XmlElement(ElementName = "EnvironmentLoggingDate")]
            public string EnvironmentLoggingDate { get; set; }
            [XmlElement(ElementName = "EnvironmentInternalID")]
            public string EnvironmentInternalID { get; set; }
            [XmlElement(ElementName = "EnvironmentInRoomInternalID")]
            public string EnvironmentInRoomInternalID { get; set; }
        }

    

        [XmlRoot(ElementName = "CONTAINER")]
        public class CONTAINER
        {
            [XmlElement(ElementName = "ContainerDisplayName")]
            public string ContainerDisplayName { get; set; }
            [XmlElement(ElementName = "ContainerContainerSize")]
            public string ContainerContainerSize { get; set; }
            [XmlElement(ElementName = "ContainerMediumType")]
            public string ContainerMediumType { get; set; }
            [XmlElement(ElementName = "ContainerVisibleID")]
            public string ContainerVisibleID { get; set; }
            [XmlElement(ElementName = "ContainerQuantity")]
            public string ContainerQuantity { get; set; }
            [XmlElement(ElementName = "ContainerLastFlush")]
            public string ContainerLastFlush { get; set; }
            [XmlElement(ElementName = "ContainerLastFeeding")]
            public string ContainerLastFeeding { get; set; }
            [XmlElement(ElementName = "ContainerInternalID")]
            public string ContainerInternalID { get; set; }
            [XmlElement(ElementName = "ContainerInUse")]
            public string ContainerInUse { get; set; }
            [XmlElement(ElementName = "ContainerContainsPlant")]
            public string ContainerContainsPlant { get; set; }
        }

   
     

        [XmlRoot(ElementName = "SENSOR5")]
        public class SENSOR
        {
            [XmlElement(ElementName = "DataPath")]
            public string DataPath { get; set; }
            [XmlElement(ElementName = "Type")]
            public string Type { get; set; }
            [XmlElement(ElementName = "InRoomInternalID")]
            public string InRoomInternalID { get; set; }
            [XmlElement(ElementName = "InRoomVisibleID")]
            public string InRoomVisibleID { get; set; }
            [XmlElement(ElementName = "SensorInternalID")]
            public string SensorInternalID { get; set; }
        }
      

      

    }

}
