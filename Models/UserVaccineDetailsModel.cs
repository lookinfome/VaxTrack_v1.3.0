using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

public class UserVaccineDetailsModel
{
    [Required (ErrorMessage = "user vaccination id is required")]
    [Key]
    public string UserVaccinationId {get; set;} = "";

    [Required (ErrorMessage = "user vaccination status is required")]
    public int UserVaccinationStatus {get; set;} = 0;

    [Required (ErrorMessage = "user id is required")]
    public string UserId {get; set;} = "";
}