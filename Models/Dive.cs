using System.ComponentModel.DataAnnotations;

namespace DiveLogg.Models
{
    public class Dive
    {
        //Properties
        public int Id { get; set; }

        public DateTime Date { get; set; }

        [Range(0, 100)]
        public int Depth { get; set; }

        public int DiveTime { get; set; }

        public int ExposureTime { get; set; }

        [RegularExpression("^[A-Z]$")]
        public string? NitrogenLoad { get; set; }

        [Range(-90, 90)]
        public double Latitude { get; set; }

        [Range(-180, 180)]
        public double Longitude { get; set; }

        [MaxLength(100)]
        public string? LocationName { get; set; }

        public string? Notes { get; set; }

        //Relationer
        //FK till person
        public int? DiveLeaderId { get; set; }
        public Person? DiveLeader { get; set; }

        //FK till person
        public int? DiverId { get; set; }
        public Person? Diver { get; set; }

        //FK till person
        public int? DiveSupportId { get; set; }
        public Person? DiveSupport { get; set; }
    }
}
