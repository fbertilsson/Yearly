using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using MediatR;
using Microsoft.ApplicationInsights;
using Periodic;
using YearlyWeb3.Models;
using Periodic.Ts;

namespace YearlyWeb3.Controllers
{
    [Authorize]
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
                var command = new AddRegistryEntryCommand(model, User.Identity.Name);
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
        public ActionResult SubmitRegisterEntries(RegisterEntriesModel model)
        {
            try
            {
                var timeseries = TsParser.ParseTimeseries(model.EntriesString);

                var repo = new RegistryEntryRepoFactory().GetRegistryEntryRepo(User.Identity.Name);
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

        [HttpGet]
        public ActionResult ListEntriesView()
        {
            try
            {
                var repo = new RegistryEntryRepoFactory().GetRegistryEntryRepo(User.Identity.Name);

                var entries = repo.GetRegistryEntries();
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

        [HttpPost]
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

            var repo = new RegistryEntryRepoFactory().GetRegistryEntryRepo(User.Identity.Name);
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


    public class AddRegistryEntryResult
    {
        public bool ValidatedOk { get; }
        public string ValidationError { get; }

        public AddRegistryEntryResult(bool validatedOk = true, string validationError = "")
        {
            ValidatedOk = validatedOk;
            ValidationError = validationError;
        }
    }   
    
    public class AddRegistryEntryCommand : IRequest<AddRegistryEntryResult>
    {
        public RegisterEntryModel Model { get; }
        public string IdentityName { get; }

        public AddRegistryEntryCommand(RegisterEntryModel model, string identityName)
        {
            Model = model;
            IdentityName = identityName;
        }
    }

    public class AddRegistryEntryCommandHandler : IRequestHandler<AddRegistryEntryCommand, AddRegistryEntryResult>
    {
        public Task<AddRegistryEntryResult> Handle(AddRegistryEntryCommand request, CancellationToken cancellationToken)
        {
            DateTime t;
            var ok = DateTime.TryParse(request.Model.DateString, out t);
            if (!ok)
            {
                return Task.FromResult(new AddRegistryEntryResult(false, "Datum kunde ej tolkas"));
            } // TODO better error handling

            int v;
            ok = int.TryParse(request.Model.RegisterValue, out v);
            if (!ok)
            {
                return Task.FromResult(new AddRegistryEntryResult(false, "Mätarställning kunde ej tolkas"));
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

            var repo = new RegistryEntryRepoFactory().GetRegistryEntryRepo(request.IdentityName);
            repo.AddRegistryEntry(tvq);
            var result = new AddRegistryEntryResult();
            return Task.FromResult(result);
        }
    }
}