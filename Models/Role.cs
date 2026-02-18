using System.ComponentModel.DataAnnotations;

namespace DiveLogg.Models;

public class Role
{
    public int Id { get; set; }

    [Required]
    [MaxLength(20)]
    public string Name { get; set; } = string.Empty;

    //Koppling till personer
    public ICollection<PersonRole> PersonRoles { get; set; } = [];

    //Koppling till dyk
    public ICollection<DiveParticipant> DiveParticipants { get; set; } = [];

}