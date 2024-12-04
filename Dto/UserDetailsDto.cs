using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace v1Remastered.Dto
{
    public class UserDetailsDto_Login
    {
        [Required(ErrorMessage = "user id is required")]
        [Key]
        public string UserId {get; set;} = "";

        [Required(ErrorMessage = "user password is required")]
        [DataType(DataType.Password)]
        public string UserPassword {get; set;}="";
    }

    public class UserDetailsDto_Register
    {
        public string UserId {get; set;} = ""; // Default value

        [Required(ErrorMessage = "user password is required")]
        [DataType(DataType.Password)]
        public string UserPassword {get; set;}="";

        [NotMapped]
        [Compare("UserPassword", ErrorMessage = "password and confirm password should must match.")]
        public string UserConfirmPassword { get; set; } = "";

        [Required(ErrorMessage = "user id is required")]
        [Key]
        public string UserName {get; set;} = ""; // Default value

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

        public bool UserRole {get; set;} = false;

        public IFormFile? ProfilePicture { get; set; }

    }

    public class UserDetailsDto_UserProfileEdit
    {
        [DataType(DataType.Date)]
        public DateTime UserBirthdate { get; set; } = DateTime.MinValue;

        [DataType(DataType.PhoneNumber)]
        [Phone(ErrorMessage = "Invalid phone number")]
        [StringLength(10, ErrorMessage = "Phone number cannot be longer than 10 digits")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be 10 digits")]
        public string UserPhone { get; set; } = "";

        public IFormFile? ProfilePicture { get; set; }

        public string UserPassword {get; set;}="";
    }

    public class UserDetailsDto_UserProfile
    {
        [Required(ErrorMessage = "User ID is required")]
        [Key]
        public string UserId { get; set; }= "";

        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; } = "";

        [Required(ErrorMessage = "User birth date is required")]
        [DataType(DataType.Date)]
        public DateTime UserBirthdate { get; set; } = DateTime.MinValue;

        [Required(ErrorMessage = "User unique ID is required")]
        [StringLength(12, MinimumLength = 12, ErrorMessage = "UID must be 12 digits")]
        public string UserUid { get; set; } = "";

        [Required(ErrorMessage = "User gender is required")]
        public string UserGender { get; set; } = "";

        [Required(ErrorMessage = "User phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be 10 digits")]
        public string UserPhone { get; set; } = "";

        [Required(ErrorMessage = "Role is required")]
        public bool UserRole { get; set; } = true;

        [Required(ErrorMessage = "User vaccination ID is required")]

        public UserVaccineDetailsDto_VaccineDetails UserVaccineDetails {get; set; } = new UserVaccineDetailsDto_VaccineDetails();

        public BookingDetailsDto_UserBookingDetails UserBookingDetails { get; set; } = new BookingDetailsDto_UserBookingDetails();

        public string ProfilePicturePath { get; set; } = "";
    }


}