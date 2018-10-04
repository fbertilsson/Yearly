using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Periodic.Ts;
using YearlyBackend.DataLayer;

namespace YearlyBackend.RegisterEntry
{
    public class ListRegisterEntriesCommandHandler : IRequestHandler<ListRegisterEntriesCommand, Timeseries>
    {
        private readonly RegisterEntryRepoFactory _factory;

        public ListRegisterEntriesCommandHandler(RegisterEntryRepoFactory factory)
        {
            _factory = factory;
        }

        public async Task<Timeseries> Handle(ListRegisterEntriesCommand request, CancellationToken cancellationToken)
        {
            var repo = _factory.GetRegistryEntryRepo(request.IdentityName);

            var entries = await repo.GetRegistryEntries();
            var sortedTvqs = entries.OrderBy(x => x.Time);
            var tsWithRegisterEntries = new Timeseries();
            tsWithRegisterEntries.AddRange(sortedTvqs.ToList());
            return tsWithRegisterEntries;
        }
    }
}
