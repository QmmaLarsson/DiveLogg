using System.ComponentModel.DataAnnotations;

namespace DiveLogg.Models;

public class Person
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    //Personens grupptillhörighet
    [Required]
    public int GroupId { get; set; }

    public Group? Group { get; set; }

    //Roller personen har (dykare, dykledare, dykskötare)
    public ICollection<PersonRole> PersonRoles { get; set; } = new List<PersonRole>();

    //Dyk där person är dykledare
    public ICollection<Dive> LedDives { get; set; } = new List<Dive>();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}