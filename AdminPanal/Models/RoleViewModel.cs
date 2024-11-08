using System.ComponentModel.DataAnnotations;

namespace AdminPanel.Models
{
    public class RoleViewModel
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "Name is Required !!")]
        [MaxLength(256)]
        [Display(Name = "Role Name")]
        public string RoleName { get; set; }
        public bool IsSelected { get; set; }
    }
}
