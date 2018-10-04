using MediatR;
using Periodic.Ts;

namespace YearlyBackend.RegisterEntry
{
    public class ListRegisterEntriesCommand : IRequest<Timeseries>
    {
        public ListRegisterEntriesCommand(string identityName)
        {
            IdentityName = identityName;
        }

        public string IdentityName { get; }
    }
}
