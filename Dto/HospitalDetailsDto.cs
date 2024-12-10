using System.ComponentModel.DataAnnotations;


public class HospitalDetailsDto_HospitalDetails
{
        public string HospitalName {get; set;} = "";
        public int HospitalAvailableSlots {get; set;} = 0;
        public string HospitalLocation {get; set;} = "";
}

public class HospitalDetailsDto_HospitalSlotBooking
{
        public string HospitalId {get; set;} ="";
        public string HospitalName {get; set;} = "";
}