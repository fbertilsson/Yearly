using System;
using MediatR;

namespace YearlyBackend.RegisterEntry
{
    public class DeleteRegisterEntryCommand : IRequest<bool>
    {
        public DeleteRegisterEntryCommand(string identityName, DateTime t)
        {
            IdentityName = identityName;
            Time = t;
        }
        public string IdentityName { get; }

        /// <summary>
        /// Point in time of the value to remove.
        /// </summary>
        public DateTime Time { get; }
    }
}
