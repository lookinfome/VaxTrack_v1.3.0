namespace v1Remastered.Dto
{
    public class AdminDetailsDto_UserWithPendingApproval
    {
        public string UserId {get; set;} = "";
        public string Username {get; set;} = "";
        public int UserVaccinationStatus {get; set;} = 0; 
        public string BookingId {get; set;} = "";
        public DateTime Dose1Date {get; set;} = DateTime.MinValue;
        public DateTime Dose2Date {get; set;} = DateTime.MinValue;
        public string D1HospitalName {get; set;} = "";
        public string D2HospitalName {get; set;} = "";
    }

    public class AdminDetailsDto_HospitalsList
    {
        public string HospitalId {get; set;} = "";
        public string HospitalName {get; set;} = "";
        public int HospitalAvailableSlots {get; set;} = 0;
    }

    public class AdminDetailsDto_BookingMonthCount
    {
        public string BookingMonth {get; set;} = "";
        public int BookingCount {get; set;} = 0;

    }

    // not in use curently
    public class AdminDetailsDto_UsersWithVaccineStatus
    {
        public string Username {get; set;} = "";
        public string UserVaccineationStatus {get; set;} = "";
        
    }
}