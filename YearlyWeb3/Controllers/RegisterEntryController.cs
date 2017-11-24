using System;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using Microsoft.ApplicationInsights;
using Periodic;
using YearlyWeb3.Models;
using Periodic.Ts;

namespace YearlyWeb3.Controllers
{
    [Authorize]
    public class RegisterEntryController : Controller
    {
        private readonly TelemetryClient _logger;

        public RegisterEntryController()
        {
            _logger = new TelemetryClient();
        }

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
            try
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

                var isMidnight = t.Hour == 0 && t.Minute == 0;
                if (isToday && isMidnight)
                {
                    t = now;
                }

                var tvq = new Tvq(t, v, Quality.Ok);

                var repo = new RegistryEntryRepoFactory().GetRegistryEntryRepo();
                repo.AddRegistryEntry(tvq);
                ViewBag.Title = "Mätarställning registrerad";
                ViewBag.SubTitle = "Mätarställningen blev registrerad";
                return View(model);
            }
            catch (Exception e)
            {
                _logger.TrackException(e);
                ViewBag.Title = "Ett fel uppstod";
                ViewBag.SubTitle = "Ett fel uppstod vid registrering av mätarställning";
                return View(model);
            }
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
            try
            {
                var timeseries = TsParser.ParseTimeseries(model.EntriesString);

                var repo = new RegistryEntryRepoFactory().GetRegistryEntryRepo();
                foreach (var tvq in timeseries)
                {
                    repo.AddRegistryEntry(tvq);
                }

                ViewBag.Title = "Mätarställningar registrerade";
                ViewBag.SubTitle = "Mätarställningarna blev registrerade";
                return View(model);
            }
            catch (Exception e)
            {
                _logger.TrackException(e);
                return View("Error");
            }
        }

        [System.Web.Mvc.HttpGet]
        public ActionResult ListEntriesView()
        {
            try
            {
                var repo = new RegistryEntryRepoFactory().GetRegistryEntryRepo();

                var entries = repo.GetRegistryEntries(Thread.CurrentPrincipal);
                var sortedTvqs = entries.OrderBy(x => x.Time);
                var tsWithRegisterEntries = new Timeseries();
                tsWithRegisterEntries.AddRange(sortedTvqs.ToList());

                return View(tsWithRegisterEntries);
            }
            catch (Exception e)
            {
                _logger.TrackException(e);
                return View("Error");
            }
        }

        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteRegistryEntry([System.Web.Http.FromUri] string dateString)
        {
            DateTime t;
            var ok = DateTime.TryParse(dateString, out t);
            if (!ok)
            {
                ViewBag.Title = "Fel vid borttagning";
                ViewBag.SubTitle = "Datum kunde ej tolkas";
                return View("ListEntriesView");
            } // TODO better error handling

            var repo = new RegistryEntryRepoFactory().GetRegistryEntryRepo();
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