using Microsoft.AspNetCore.Mvc.Rendering;
using DiveLogg.Models;

namespace DiveLogg.ViewModels
{
    //ViewModel som används vid skapande av dyk, använder data från dyk samt valda deltagare
    public class DiveCreateViewModel
    {
        //Objektet som sparas i databasen
        public Dive Dive { get; set; } = new Dive();

        //Lista över deltagare som ska sparas
        public List<DiveParticipantInput> Participants { get; set; } =
        [
            //1 = Dykare, 3 = Dykskötare
            new DiveParticipantInput { RoleId = 1 },
            new DiveParticipantInput { RoleId = 3 }
        ];

        //Dropdown-listor
        public SelectList? DiveLeaders { get; set; }
        public SelectList? Divers { get; set; }
        public SelectList? DiveSupports { get; set; }
    }

    //Representerar en deltagare i formuläret (används för att koppla en person till en roll)
    public class DiveParticipantInput
    {
        public int? PersonId { get; set; }
        public int RoleId { get; set; }
    }
}
