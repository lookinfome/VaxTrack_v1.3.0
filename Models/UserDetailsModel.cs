using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using v1Remastered.Dto;

namespace v1Remastered.Models
{
    public class UserDetailsModel
    {

        [Required(ErrorMessage = "user id is required")]
        [Key]
        public string UserId {get; set;} = "";

        [Required(ErrorMessage ="user full name is required")]
        public string UserName {get; set;} = "";

        [Required(ErrorMessage ="user birth date is required")]
        [DataType(DataType.Date)]
        public DateTime UserBirthdate {get; set;}

        [Required(ErrorMessage ="user unique id is required")]
        [StringLength(12, MinimumLength =12, ErrorMessage = "UID must be 12 digits number")]
        public string UserUid {get; set;} = "";

        [Required(ErrorMessage ="user gender is required")]
        public string UserGender {get; set;} = "";

        [Required(ErrorMessage ="user phone no. is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        [StringLength(10, ErrorMessage = "Phone number cannot be longer than 10 digits")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be 10 digits")]
        public string UserPhone {get; set;} = "";

        [Required(ErrorMessage = "Role is required")]
        public bool UserRole {get; set;} = true;

        public string ProfilePicturePath { get; set; } = "";

        public static implicit operator UserDetailsModel(UserDetailsDto_Register v)
        {
            throw new NotImplementedException();
        }
    }
}