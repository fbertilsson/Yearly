using System;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using YearlyBackend.RegisterEntry;

namespace YearlyWebCore1.Features.RegisterEntry
{
    public class RegisterEntryController : Controller
    {
        private readonly IMediator _mediator;
        private readonly TelemetryClient _logger;

        public RegisterEntryController(IMediator mediator)
        {
            _mediator = mediator;
            _logger = new TelemetryClient();
        }

        // GET: RegisterEntry
        public ActionResult RegisterEntryView()
        {
            ViewBag.Title = "Registrera mätarställning";
            ViewBag.SubTitle = "Registrera aktuell mätarställning";
            ViewBag.Action = "Registrera";

            var now = DateTime.Now;
            var model = new RegisterEntryModel
            {
                DateString = $"{now.ToShortDateString()} {now.ToShortTimeString()}",
                RegisterValue = string.Empty,
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitRegisterEntry(RegisterEntryModel model)
        {
            try
            {
                var mappedModel = Mapper.Map<YearlyBackend.RegisterEntry.RegisterEntryModel>(model);

                var command = new AddRegisterEntryCommand(mappedModel, User.Identity.Name);
                var result = _mediator.Send(command).Result;

                if (result.ValidatedOk)
                {
                    ViewBag.Title = "Mätarställning registrerad";
                    ViewBag.SubTitle = "Mätarställningen blev registrerad";
                }
                else
                {
                    ViewBag.Title = "Fel vid registrering";
                    ViewBag.SubTitle = result.ValidationError;
                }
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SubmitRegisterEntries(RegisterEntriesModel model)
        {
            try
            {
                var command = new AddRegisterEntriesCommand(model, User.Identity.Name);
                await _mediator.Send(command);

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

        [HttpGet]
        public async Task<ActionResult> ListEntriesView()
        {
            try
            {
                var command = new ListRegisterEntriesCommand(User.Identity.Name);
                var result = await _mediator.Send(command);

                return View(result);
            }
            catch (Exception e)
            {
                _logger.TrackException(e);
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteRegistryEntry(string dateString)
        {
            var ok = DateTime.TryParse(dateString, out var t);
            if (!ok)
            {
                ViewBag.Title = "Fel vid borttagning";
                ViewBag.SubTitle = "Datum kunde ej tolkas";
                return View("ListEntriesView");
            } // TODO better error handling

            var command = new DeleteRegisterEntryCommand(User.Identity.Name, t);
            ok = await _mediator.Send(command);
            
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
