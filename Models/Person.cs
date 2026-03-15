using System.ComponentModel.DataAnnotations;

namespace DiveLogg.Models;

public class Person
{
    public int Id { get; set; }

    [MaxLength(100)]
    public string? Name { get; set; }

    //Personens grupptillhörighet
    [Required]
    public int GroupId { get; set; }

    public Group Group { get; set; } = null!;

    //Roller personen har (dykare, dykledare, dykskötare)
    public ICollection<PersonRole> PersonRoles { get; set; } = new List<PersonRole>();

    //Dyk där person är dykledare
    public ICollection<Dive> LedDives { get; set; } = new List<Dive>();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    //Ta bort person
    public bool IsDeleted { get; set; } = false;
}