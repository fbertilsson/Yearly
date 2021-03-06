﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Periodic;
using Periodic.Algo;
using Periodic.Ts;
using YearlyBackend.DataLayer;

namespace YearlyBackend.Periodic
{
    public class GetMonthlyAveragesCommandHandler : IRequestHandler<GetMonthlyAveragesCommand, Timeseries>
    {
        private readonly RegisterEntryRepoFactory _factory;

        public GetMonthlyAveragesCommandHandler(RegisterEntryRepoFactory factory)
        {
            _factory = factory;
        }
        public async Task<Timeseries> Handle(GetMonthlyAveragesCommand request, CancellationToken cancellationToken)
        {
            var repo = _factory.GetRegistryEntryRepo(request.IdentityName);
            var sortedTvqs = (await repo.GetRegistryEntries()).OrderBy(x => x.Time);

            var tsWithRegisterEntries = new Timeseries();
            tsWithRegisterEntries.AddRange(sortedTvqs.ToList());

            var periodizer = new Periodizer();
            var monthlyAverages = periodizer.MonthlyAverage(tsWithRegisterEntries);

            return monthlyAverages;
        }
    }
}
