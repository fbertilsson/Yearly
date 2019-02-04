using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Periodic.Ts;
using YearlyBackend.Periodic;

namespace YearlyWebCore1.Pages
{
    public class DailyByWeekModel : PageModel
    {
        private readonly IMediator _mediator;

        public DailyByWeekModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Timeseries Data { get; private set; }

        public async Task OnGetAsync(GetDailyByWeekQuery query)
        {
            query.IdentityName = User.Identity.Name;
            Data = await _mediator.Send(query);
        }
    }
}