using System.ComponentModel.DataAnnotations;
using DiveLogg.Models;

namespace DiveLogg.ViewModels
{
    public class PersonCreateViewModel
    {

        [MaxLength(100)]
        public string? Name { get; set; }

        [Required]
        public int GroupId { get; set; }


        // De roller som användaren bockar i
        public List<int> SelectedRoleIds { get; set; } = new();

        // Alla tillgängliga roller (för checkboxar)
        public List<Role> AvailableRoles { get; set; } = new();
    }
}
