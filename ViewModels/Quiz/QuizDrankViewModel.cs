namespace Restaurant.ViewModels.Quiz
{
    public class QuizDrankViewModel
    {
        public int CurrentQuestion { get; set; } = 0;

        public int IsZoet { get; set; }
        public int IsBitter { get; set; }
        public int IsFris { get; set; }
        public int IsAlcoholisch { get; set; }
        public int IsWarm { get; set; }
        public int IsKoud { get; set; }
        public int IsFruitig { get; set; }
        public int IsExotisch { get; set; }
        public int IsKruidig { get; set; }

        // Quiz vragen — één vraag per eigenschap (nu 9 vragen)
        public static readonly List<string> Vragen = new()
        {
            "Hou je van zoete dranken?",         // IsZoet
            "Hou je van bittere dranken?",       // IsBitter
            "Hou je van frisse dranken?",        // IsFris
            "Drink je graag alcoholische dranken?", // IsAlcoholisch
            "Hou je van warme dranken?",         // IsWarm
            "Hou je van koude dranken?",         // IsKoud
            "Hou je van fruitige dranken?",      // IsFruitig
            "Hou je van exotische dranken?",     // IsExotisch
            "Hou je van kruidige dranken?"       // IsKruidig
        };
    }
}