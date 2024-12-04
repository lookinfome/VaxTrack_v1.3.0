using System.ComponentModel.DataAnnotations;


namespace v1Remastered.Dto
{
    public class BookingDetailsDto_SlotBook
    {
        public DateTime Dose1BookDate {get; set;} = DateTime.MinValue;
        public DateTime Dose2BookDate {get; set;} = DateTime.MinValue;
    }

    public class BookingDetailsDto_V2SlotBook
    {
        public DateTime Dose1BookDate {get; set;} = DateTime.MinValue;
        public DateTime Dose2BookDate {get; set;} = DateTime.MinValue;
        public string HospitalName {get; set;} = "";
        public int HospitalAvailableSlots {get; set;} = 0;
        public string HospitalLocation {get; set;} = "";

    }

    public class BookingDetailsDto_UserBookingDetails
    {
        public string BookingId {get; set;} = "";
        public DateTime Dose1BookDate {get; set;} = DateTime.MinValue;
        public DateTime Dose2BookDate {get; set;} = DateTime.MinValue;
        public string D1HospitalId {get; set;} = "";
        public string D2HospitalId {get; set;} = "";
        public int D1SlotNumber {get; set;} = -1;
        public int D2SlotNumber {get; set;} = -1;
        public DateTime Dose1ApproveDate {get; set;} = DateTime.MinValue;
        public DateTime Dose2ApproveDate {get; set;} = DateTime.MinValue;
    }

}