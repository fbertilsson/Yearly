using MediatR;

namespace YearlyBackend.RegisterEntry
{
    public class AddRegisterEntriesCommand : IRequest<AddRegisterEntryResult>
    {
        public RegisterEntriesModel Model { get; }
        public string IdentityName { get; }

        public AddRegisterEntriesCommand(RegisterEntriesModel model, string identityName)
        {
            Model = model;
            IdentityName = identityName;
        }
    }
}