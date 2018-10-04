using MediatR;

namespace YearlyBackend.RegisterEntry
{
    public class AddRegisterEntryCommand : IRequest<AddRegisterEntryResult>
    {
        public RegisterEntryModel Model { get; }
        public string IdentityName { get; }

        public AddRegisterEntryCommand(RegisterEntryModel model, string identityName)
        {
            Model = model;
            IdentityName = identityName;
        }
    }
}