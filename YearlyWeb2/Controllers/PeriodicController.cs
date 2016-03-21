using System.Web.Mvc;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
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
            var tvqs = repo.GetRegistryEntries();

            return View(tvqs);
        }
    }
}