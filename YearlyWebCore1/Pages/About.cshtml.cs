using Microsoft.AspNetCore.Mvc.RazorPages;

namespace YearlyWebCore1.Pages
{
    public class AboutModel : PageModel
    {
        public string Message { get; set; }

        public void OnGet()
        {
            Message = "Fredriks Elförbrukning";
        }
    }
}
