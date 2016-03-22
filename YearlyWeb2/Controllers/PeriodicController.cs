using System.Web.Mvc;
using System.Linq;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Periodic;
using YearlyWeb2.DataLayer;

namespace YearlyWeb2.Controllers
{
    public class PeriodicController : Controller
    {
        // GET: Periodic
        public ActionResult PeriodicView()
        {
            var storageAccount = CloudStorageAccount.Parse(
               CloudConfigurationManager.GetSetting("StorageConnectionString"));

            var repo = new RegistryEntryRepo(storageAccount);
            var sortedTvqs = repo.GetRegistryEntries().OrderBy(x => x.Time);
            var ts = new Timeseries();
            ts.AddRange(sortedTvqs);

            var periodizer = new Periodizer();
            var monthlyAverages = periodizer.MonthlyAverage(ts);

            return View(monthlyAverages);
        }
    }
}