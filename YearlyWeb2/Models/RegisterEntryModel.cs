using System.ComponentModel;
using System.Web.Mvc.Html;
using System.Web.UI.WebControls;

namespace YearlyWeb2.Models
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