using Microsoft.AspNetCore.Mvc.Rendering;
using DiveLogg.Models;

namespace DiveLogg.ViewModels
{
    //ViewModel som används vid skapande av dyk, använder data från dyk samt valda deltagare
    public class DiveCreateViewModel
    {
        //Objektet som sparas i databasen
        public Dive Dive { get; set; } = new Dive();

        //Dropdown-listor
        public SelectList? DiveLeaders { get; set; }
        public SelectList? Divers { get; set; }
        public SelectList? DiveSupports { get; set; }
    }
}
