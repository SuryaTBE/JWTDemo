using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace DemoJWTMVC.Models
{
    public class UserTbl
    {
        [Key]
        public int UserId { get; set; }
        public string? UserName { get; set; }
        [Required(ErrorMessage = "E-Mail field is required..")]
        [DisplayName("E-Mail")]
        public string? mail { get; set; }
        [Required(ErrorMessage = "Password Field Can't be empty")]
        public string? password { get; set; }
    }
}
