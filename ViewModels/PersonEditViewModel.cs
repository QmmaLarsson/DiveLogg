using DiveLogg.Models;
using System.ComponentModel.DataAnnotations;

namespace DiveLogg.ViewModels
{
    public class PersonEditViewModel
    {
        public int Id { get; set;}

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public int GroupId { get; set; }

        // Visar alla tillgängliga roller
        public List<Role> AvailableRoles { get; set; } = new List<Role>();

        // Roller som är valda för personen
        public List<int> SelectedRoleIds { get; set; } = new List<int>();

        public DateTime CreatedAt { get; set; }
    }
}