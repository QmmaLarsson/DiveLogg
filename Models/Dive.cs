using System.ComponentModel.DataAnnotations;

namespace DiveLogg.Models
{
    public class Dive
    {
        //Properties
        public int Id { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [Range(0, 100)]
        public int Depth { get; set; }

        [Required]
        public int DiveTime { get; set; }

        [Required]
        public int ExposureTime { get; set; }

        [Required]
        public int NitrogenLoad { get; set; }

        [Required]
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }

        [MaxLength(200)]
        public string? LocationName { get; set; }

        public string? Notes { get; set; }

        //Relationer
        //FK till person
        public int? DiveLeaderId { get; set; }
        //Navigations property till den person som leder dyket
        //Används för att visa ledare i vyer och för filtrering
        public Person? DiveLeader { get; set; }

        //Lista över deltagare
        //Används för att visa deltagare i vyer och för filtrering
        public ICollection<DiveParticipant> DiveParticipants { get; set; } = [];
    }
}