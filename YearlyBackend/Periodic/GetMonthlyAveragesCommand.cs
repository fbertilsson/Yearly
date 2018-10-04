using MediatR;
using Periodic.Ts;

namespace YearlyBackend.Periodic
{
    public class GetMonthlyAveragesCommand : IRequest<Timeseries>
    {
        public GetMonthlyAveragesCommand(string identityName)
        {
            IdentityName = identityName;
        }

        public string IdentityName { get; }
    }
}
