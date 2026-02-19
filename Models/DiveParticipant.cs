using System.ComponentModel.DataAnnotations;

namespace DiveLogg.Models
{
    public class DiveParticipant
    {
        //Properties
        public int Id { get; set; }

        //Relationer
        //FK till Dive
        [Required]
        public int DiveId { get; set; }
        //Navigations property till dyket
        //Används för att få information om dyket
        public Dive? Dive { get; set; }

        //FK till Person
        [Required]
        public int PersonId { get; set; }
        //Navigations property till person
        //Används för att få information om person
        public Person? Person { get; set; }

        //FK till Role
        [Required]
        public int RoleId { get; set; }
        //Navigations property till roll
        //Används för att visa roll i vyer och för filtrering
        public Role? Role { get; set; }
    }
}