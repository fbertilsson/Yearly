using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Periodic;
using YearlyWeb2.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage.Table;
using YearlyWeb2.DataLayer;

namespace YearlyWeb2.Controllers
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

        [HttpPost]
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

        [HttpPost]
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
    }
}