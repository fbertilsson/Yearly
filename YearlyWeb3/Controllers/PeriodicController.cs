using System.IO;
using System.Web.Mvc;
using System.Linq;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Periodic;
using YearlyWeb3.DataLayer;

namespace YearlyWeb3.Controllers
{
    [Authorize]
    public class PeriodicController : Controller
    {
        public ActionResult PeriodicView()
        {
            string dummyRegisterid = string.Empty;
            var monthlyAverages = ValuesController.GetMonthlyAverages(dummyRegisterid);

            return View(monthlyAverages);
        }



    }
}