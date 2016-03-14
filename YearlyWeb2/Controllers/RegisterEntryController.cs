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
            ok = DateTime.TryParse(model.DateString, out t); // TODO error handling
            ok = int.TryParse(model.RegisterValue, out v); // TODO error handling
            // TODO FB if (! ok) return ...
            var tvq = new Tvq(t, v, Quality.Ok);


            ViewBag.Title = "Mätarställning registrerad";
            ViewBag.SubTitle = "Mätarställningen blev registrerad";
            return View(model);
        }
    }
}