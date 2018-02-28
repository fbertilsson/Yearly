using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Periodic.Ts;

namespace YearlyWeb4.Controllers
{
    [Produces("application/json")]
    [Route("api/RegisterEntry")]
    public class RegisterEntryController : Controller
    {
        public IConfiguration Configuration { get; }

        public RegisterEntryController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        [HttpPost("SubmitRegisterEntry")]
        public IActionResult  SubmitRegisterEntry([FromBody] RegisterEntryModel model)
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
                    return BadRequest(model);
                } // TODO better error handling

                ok = int.TryParse(model.RegisterValue, out v);
                if (!ok)
                {
                    ViewBag.Title = "Fel vid registrering";
                    ViewBag.SubTitle = "Mätarställning kunde ej tolkas";
                    return BadRequest(model);
                } // TODO better error handling

                // TODO: Probably delete this section
                //var now = DateTime.Now;
                //var isToday =
                //    t.Year == now.Year
                //    && t.Month == now.Month
                //    && t.Day == now.Day;

                //var isMidnight = t.Hour == 0 && t.Minute == 0;
                //if (isToday && isMidnight)
                //{
                //    t = now;
                //}

                // TODO implement:
                var tvq = new Tvq(t, v, Quality.Ok);

                var repo = new RegistryEntryRepoFactory().GetRegistryEntryRepo(Configuration);
                repo.AddRegistryEntry(tvq);
                ViewBag.Title = "Mätarställning registrerad";
                ViewBag.SubTitle = "Mätarställningen blev registrerad";
            }
            catch (Exception e)
            {
                // _logger.TrackException(e);
                ViewBag.Title = "Ett fel uppstod";
                ViewBag.SubTitle = "Ett fel uppstod vid registrering av mätarställning";
                return StatusCode(500);
            }
            return Ok(model);
        }
    }

    public class RegisterEntryModel
    {
        public string DateString { get; set; }
        
        public string RegisterValue { get; set; }
    }
}