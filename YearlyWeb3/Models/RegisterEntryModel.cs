using System.ComponentModel;

namespace YearlyWeb3.Models
{
    public class RegisterEntryModel
    {
        [DisplayName("Datum")]
        public string DateString { get; set; }
        
        /// <summary>
        /// Mätarställning
        /// </summary>
        [DisplayName("Mätarställning")]
        public string RegisterValue { get; set; }
    }
}