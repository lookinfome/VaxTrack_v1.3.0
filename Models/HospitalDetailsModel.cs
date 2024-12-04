using System.ComponentModel.DataAnnotations;
using Microsoft.VisualStudio.TextTemplating;


namespace v1Remastered.Models
{
    public class HospitalDetailsModel
    {
        [Required(ErrorMessage = "hospital id is required")]
        [Key]
        public string HospitalId {get; set;} = "";

        [Required(ErrorMessage = "hospital name is required")]
        public string HospitalName {get; set;} = "";

        [Required(ErrorMessage = "hospital available slot count is required")]
        public int HospitalAvailableSlots {get; set;} = 0;

        [Required(ErrorMessage = "hospital location is required")]
        public string HospitalLocation {get; set;} = "";
    }
}