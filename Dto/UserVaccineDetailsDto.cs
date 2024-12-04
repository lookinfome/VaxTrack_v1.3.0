using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace v1Remastered.Dto
{
    public class UserVaccineDetailsDto_VaccineDetails
    {
        public string UserVaccinationId {get; set;} = "";
        public string UserVaccinationStatus {get; set;} = "";
        public string UserId {get; set;} = "";
    }
}