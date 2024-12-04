using System.ComponentModel.DataAnnotations;

namespace v1Remastered.Models
{
    public class BookingDetailsModel
    {
        [Required(ErrorMessage = "booking id is required")]
        [Key]
        public string BookingId {get; set;} = "";

        [Required(ErrorMessage = "dose 1 date is required")]
        public DateTime Dose1BookDate {get; set;} = DateTime.MinValue;

        [Required(ErrorMessage = "dose 2 date is required")]
        public DateTime Dose2BookDate {get; set;} = DateTime.MinValue;

        [Required(ErrorMessage = "dose 1 hospital name is required")]
        public string D1HospitalId {get; set;} = "";

        [Required(ErrorMessage = "dose 2 hospital name is required")]
        public string D2HospitalId {get; set;} = "";

        [Required(ErrorMessage = "dose 1 slot number is required")]
        public int D1SlotNumber {get; set;} = -1;

        [Required(ErrorMessage = "dose 2 slot number is required")]
        public int D2SlotNumber {get; set;} = -1;


        public DateTime Dose1ApproveDate {get; set;} = DateTime.MinValue;
        public DateTime Dose2ApproveDate {get; set;} = DateTime.MinValue;


        [Required(ErrorMessage = "user id is required")]
        public string UserId {get; set;} = "";

        [Required(ErrorMessage = "user vaccination id is required")]
        public string UserVaccinationId {get; set;} = "";
    }
}