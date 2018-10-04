using System.ComponentModel;

namespace YearlyWebCore1.Features.RegisterEntry
{
    public class RegisterEntryModel
    {
        [DisplayName("Tidpunkt")]
        public string DateString { get; set; }
        
        /// <summary>
        /// Mätarställning
        /// </summary>
        [DisplayName("Mätarställning")]
        public string RegisterValue { get; set; }
    }
}