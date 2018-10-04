using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Periodic;
using YearlyBackend.DataLayer;

namespace YearlyBackend.RegisterEntry
{
    public class AddRegisterEntriesCommandHandler : IRequestHandler<AddRegisterEntriesCommand, AddRegisterEntryResult>
    {
        private readonly RegisterEntryRepoFactory _factory;

        public AddRegisterEntriesCommandHandler(RegisterEntryRepoFactory factory)
        {
            _factory = factory;
        }

        public async Task<AddRegisterEntryResult> Handle(AddRegisterEntriesCommand request, CancellationToken cancellationToken)
        {
            var timeseries = TsParser.ParseTimeseries(request.Model.EntriesString);
            var repo = _factory.GetRegistryEntryRepo(request.IdentityName);

            await repo.AddRegistryEntries(timeseries); 

            return new AddRegisterEntryResult();
        }
    }
}
