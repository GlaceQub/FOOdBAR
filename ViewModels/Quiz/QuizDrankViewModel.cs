namespace Restaurant.ViewModels.Quiz
{
    public class QuizDrankViewModel
    {
        public int CurrentQuestion { get; set; } = 0;

        public int Zoet { get; set; }
        public int Bitter { get; set; }
        public int Fris { get; set; }
        public int Alcoholisch { get; set; }
        public int Warm { get; set; }
        public int Koud { get; set; }
        public int Fruitig { get; set; }
        public int Exotisch { get; set; }
        public int Kruidig { get; set; }

        // Quiz vragen — één vraag per eigenschap 
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