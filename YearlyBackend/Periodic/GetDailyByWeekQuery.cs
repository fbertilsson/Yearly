using MediatR;
using Periodic.Ts;

namespace YearlyBackend.Periodic
{
    public class GetDailyByWeekQuery : IRequest<Timeseries>
    {
        public GetDailyByWeekQuery()
        {            
        }

        public GetDailyByWeekQuery(string identityName)
        {
            IdentityName = identityName;
        }

        public string IdentityName { get; set; }
    }
}
