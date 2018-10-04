using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Periodic;
using YearlyBackend.Periodic;

namespace YearlyWebCore1.Features.Periodic
{
    [Authorize]
    public class PeriodicController : Controller
    {
        private readonly IMediator _mediator;

        public PeriodicController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<ActionResult> PeriodicView()
        {
            try
            {
                var command = new GetMonthlyAveragesCommand(User.Identity.Name);

                var monthlyAverages = await _mediator.Send(command);
                return View(monthlyAverages);
            }
            catch (TooFewEntriesException tfe)
            {
                ViewBag.Title = "För få värden";
                ViewBag.Message = $"Det behövs värden i minst {tfe.MinEntries} månader för att jämföra månatligt.";
                return View("Error");
            }
        }

        [Route("api/periodic/{registerId}/monthly/csv")]
        [HttpGet]
        public async Task<ContentResult> MonthlyValues()
        {
            var command = new GetMonthlyAveragesCommand(User.Identity.Name); // Ignoring the registerId parameter for now
            var monthlyAverages = await _mediator.Send(command);

            var splitPerYear = new Splitter().SplitPerYear(monthlyAverages);

            var writer = new StringWriter();
            var renderer = new MonthlyValueTextRenderer(CultureInfo.InvariantCulture);
            renderer.Render(splitPerYear, writer);
            
            var result = writer.ToString();
            
            return Content(result, "text/csv");
        }
    }
}