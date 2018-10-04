using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Periodic.Ts;
using YearlyBackend.DataLayer;

namespace YearlyBackend.RegisterEntry
{
    public class AddRegisterEntryCommandHandler : IRequestHandler<AddRegisterEntryCommand, AddRegisterEntryResult>
    {
        private readonly RegisterEntryRepoFactory _factory;

        public AddRegisterEntryCommandHandler(RegisterEntryRepoFactory factory)
        {
            _factory = factory;
        }

        public async Task<AddRegisterEntryResult> Handle(AddRegisterEntryCommand request, CancellationToken cancellationToken)
        {
            var ok = DateTime.TryParse(request.Model.DateString, out var t);
            if (!ok)
            {
                return new AddRegisterEntryResult(false, "Datum kunde ej tolkas");
            } // TODO better error handling

            ok = int.TryParse(request.Model.RegisterValue, out var v);
            if (!ok)
            {
                return new AddRegisterEntryResult(false, "Mätarställning kunde ej tolkas");
            } // TODO better error handling

            var now = DateTime.Now;
            var isToday =
                t.Year == now.Year
                && t.Month == now.Month
                && t.Day == now.Day;

            var isMidnight = t.Hour == 0 && t.Minute == 0;
            if (isToday && isMidnight)
            {
                t = now;
            }

            var tvq = new Tvq(t, v, Quality.Ok);

            var repo = _factory.GetRegistryEntryRepo(request.IdentityName);
            await repo.AddRegistryEntries(new List<Tvq> { tvq}); 
            var result = new AddRegisterEntryResult();
            return result;
        }
    }
}