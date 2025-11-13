using System.ComponentModel.DataAnnotations;

namespace VHPProjectDTOModel.MemberDTO.request
{
    public class AddMemberRequestDTO
    {
        [Required(ErrorMessage = "FirstName is required")]
        [MaxLength(15, ErrorMessage = "FirstName cannot exceed 15 characters")]
        [RegularExpression("^[A-Za-z]+$", ErrorMessage = "FirstName must contain alphabets only")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "LastName is required")]
        [MaxLength(15, ErrorMessage = "LastName cannot exceed 15 characters")]
        [RegularExpression("^[A-Za-z]+$", ErrorMessage = "LastName must contain alphabets only")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "MobileNumber is required")]
        [RegularExpression("^[0-9]{10}$", ErrorMessage = "MobileNumber must be exactly 10 digits")]
        public string MobileNumber { get; set; }

        [Required(ErrorMessage = "DateOfBirth is required")]
        [DataType(DataType.Date, ErrorMessage = "DateOfBirth must be a valid date")]
        public DateTime DateOfBirth { get; set; }
    }
}
