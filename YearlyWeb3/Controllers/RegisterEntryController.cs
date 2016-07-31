using System;
using System.Linq;
using System.Web.Http;
using System.Web.Mvc;
using Periodic;
using YearlyWeb3.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.Azure;
using Periodic.Ts;
using YearlyWeb3.DataLayer;

namespace YearlyWeb3.Controllers
{
    public class RegisterEntryController : Controller
    {
        // GET: RegisterEntry
        public ActionResult RegisterEntryView()
        {
            ViewBag.Title = "Registrera mätarställning";
            ViewBag.SubTitle = "Registrera aktuell mätarställning";
            ViewBag.Action = "Registrera";

            var model = new RegisterEntryModel
            {
                DateString = DateTime.Now.ToShortDateString(),
                RegisterValue = string.Empty,
            };

            return View(model);
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult SubmitRegisterEntry(RegisterEntryModel model)
        {
            bool ok;
            DateTime t;
            int v;
            ok = DateTime.TryParse(model.DateString, out t);
            if (!ok)
            {
                ViewBag.Title = "Fel vid registrering";
                ViewBag.SubTitle = "Datum kunde ej tolkas";
                return View(model);
            } // TODO better error handling

            ok = int.TryParse(model.RegisterValue, out v);
            if (!ok)
            {
                ViewBag.Title = "Fel vid registrering";
                ViewBag.SubTitle = "Mätarställning kunde ej tolkas";
                return View(model);
            } // TODO better error handling

            var now = DateTime.Now;
            var isToday =
                t.Year == now.Year
                && t.Month == now.Month
                && t.Day == now.Day;
            if (isToday)
            {
                t = now;
            }

            var tvq = new Tvq(t, v, Quality.Ok);

            var repo = GetRegistryEntryRepo();
            repo.AddRegistryEntry(tvq);

            ViewBag.Title = "Mätarställning registrerad";
            ViewBag.SubTitle = "Mätarställningen blev registrerad";
            return View(model);
        }

        private RegistryEntryRepo GetRegistryEntryRepo()
        {
            var storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));

            var repo = new RegistryEntryRepo(storageAccount);
            return repo;
        }


        public ActionResult RegisterEntriesView()
        {
            ViewBag.Title = "Registrera flera mätarställningar";
            ViewBag.SubTitle = "Registrera flera mätarställningar";
            ViewBag.Action = "Registrera";

            var model = new RegisterEntriesModel();
            return View(model);
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult SubmitRegisterEntries(RegisterEntriesModel model)
        {
            var timeseries = TsParser.ParseTimeseries(model.EntriesString);

            var repo = GetRegistryEntryRepo();
            foreach (var tvq in timeseries)
            {
                repo.AddRegistryEntry(tvq);
            }

            ViewBag.Title = "Mätarställningar registrerade";
            ViewBag.SubTitle = "Mätarställningarna blev registrerade";
            return View(model);
        }

        [System.Web.Mvc.HttpGet]
        public ActionResult ListEntriesView()
        {
            var repo = GetRegistryEntryRepo();

            var entries = repo.GetRegistryEntries();
            var sortedTvqs = entries.OrderBy(x => x.Time);
            var tsWithRegisterEntries = new Timeseries();
            tsWithRegisterEntries.AddRange(sortedTvqs.ToList());

            return View(tsWithRegisterEntries);
        }

        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteRegistryEntry([FromUri] string dateString)
        {
            DateTime t;
            var ok = DateTime.TryParse(dateString, out t);
            if (!ok)
            {
                ViewBag.Title = "Fel vid borttagning";
                ViewBag.SubTitle = "Datum kunde ej tolkas";
                return View("ListEntriesView");
            } // TODO better error handling

            var repo = GetRegistryEntryRepo();
            ok = repo.DeleteRegistryEntry(t);
            if (!ok)
            {
                ViewBag.Title = "Fel vid borttagning";
                ViewBag.SubTitle = "Kunde ej ta bort registreringen";
                return View("ListEntriesView");
            }

            return RedirectToAction("ListEntriesView");
        }
    }
}