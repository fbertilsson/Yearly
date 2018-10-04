using System.Web.Mvc;
using YearlyBackend.Periodic;

namespace YearlyWeb3.Controllers
{
    [Authorize]
    public class PeriodicController : Controller
    {
        public ActionResult PeriodicView()
        {
            try
            {
                var monthlyAverages = ValuesController.GetMonthlyAverages(User.Identity.Name);
                return View(monthlyAverages);
            }
            catch (TooFewEntriesException tfe)
            {
                ViewBag.Title = "För få värden";
                ViewBag.Message = $"Det behövs värden i minst {tfe.MinEntries} månader för att jämföra månatligt.";
                return View("Error");
            }
        }
    }
}