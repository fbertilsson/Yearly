using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace YearlyWeb2.Models
{
    public class RegisterEntriesModel
    {
        /// <summary>
        /// Register entries as a string that can be parsed into registry entries.
        /// </summary>
        [DataType(DataType.MultilineText)]
        [DisplayName("Mätarställningar")]
        public string EntriesString { get; set; }
    }
}