using Microsoft.AspNetCore.Mvc.Rendering;
using DiveLogg.Models;

namespace DiveLogg.ViewModels
{
    //ViewModel som används vid skapande av dyk, använder data från dyk samt valda deltagare
    public class DiveEditViewModel
    {
        //Alla variabler som kan ändras för ett dyk
        public int Id { get; set; }
        public DateTime DiveDate { get; set; }

        public int? DiveLeaderId { get; set; }
        public int? DiverId { get; set; }
        public int? DiveSupportId { get; set; }

        public int? Depth { get; set; }
        public int? DiveTime { get; set; }
        public int? ExposureTime { get; set; }
        public string? NitrogenLoad { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? LocationName { get; set; }
        public string? Notes { get; set; }

        //Dropdown listor
        public SelectList? DiveLeaders { get; set; }
        public SelectList? Divers { get; set; }
        public SelectList? DiveSupports { get; set; }
    }

}