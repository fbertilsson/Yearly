using Microsoft.AspNetCore.Mvc.RazorPages;

namespace YearlyWebCore1.Pages
{
    public class ContactModel : PageModel
    {
        public string Message { get; set; }

        public void OnGet()
        {
            Message = "Ingen kontakt";
        }
    }
}
