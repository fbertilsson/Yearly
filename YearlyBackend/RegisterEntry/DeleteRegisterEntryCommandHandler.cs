using System.Threading;
using System.Threading.Tasks;
using MediatR;
using YearlyBackend.DataLayer;

namespace YearlyBackend.RegisterEntry
{
    public class DeleteRegisterEntryCommandHandler : IRequestHandler<DeleteRegisterEntryCommand, bool>
    {
        private RegisterEntryRepoFactory _factory;

        public DeleteRegisterEntryCommandHandler(RegisterEntryRepoFactory factory)
        {
            _factory = factory;
        }
        public async Task<bool> Handle(DeleteRegisterEntryCommand request, CancellationToken cancellationToken)
        {
            var repo = _factory.GetRegistryEntryRepo(request.IdentityName);
            var ok = await repo.DeleteRegistryEntry(request.Time);
            return ok;
        }
    }
}
