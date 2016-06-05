using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Http;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Periodic;
using Periodic.Algo;
using Periodic.Ts;
using YearlyWeb2.DataLayer;

namespace YearlyWeb2.Controllers
{
    //[Authorize]
    public class ValuesController : ApiController
    {
        [Route("api/periodic/{registerId}/monthly/csv")]
        [HttpGet]
        public string MonthlyValues(string registerId)
        {
            var monthlyAverages = GetMonthlyAverages(registerId);
            var tsMonthly = new Periodizer().MonthlyAverage(monthlyAverages);
            var splitPerYear = new Splitter().SplitPerYear(tsMonthly);

            var writer = new StringWriter();
            var renderer = new MonthlyValueTextRenderer(CultureInfo.InvariantCulture);
            renderer.Render(splitPerYear, writer);

            return writer.ToString();
        }


        public static Timeseries GetMonthlyAverages(string registerId)
        {
            var storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));

            var repo = new RegistryEntryRepo(storageAccount);
            var sortedTvqs = repo.GetRegistryEntries().OrderBy(x => x.Time);

            var tsWithRegisterEntries = new Timeseries();
            tsWithRegisterEntries.AddRange(sortedTvqs.ToList());

            var periodizer = new Periodizer();
            var monthlyRegisterEntries = periodizer.MonthlyAverage(tsWithRegisterEntries);

            var deltaOperator = new DeltaTsOperator();
            var monthlyAverages = deltaOperator.Apply(monthlyRegisterEntries);

            return monthlyAverages;
        }

        // GET api/values
        public IEnumerable<string> Get()
        {
            return new[] { DateTime.Now.ToShortTimeString(), "value2" };
        }

        /// <summary>
        /// Periodizes a timeseries
        /// </summary>
        /// <param name="id">a tab-separated list of times and values</param>
        /// <returns></returns>
        /// <remarks>It seems that the parameter must be named id for this
        /// method to be hit. Otherwise the parameterless Get is called.</remarks>
        public string Get(string id)
        {
            //var id = 12345;
            var ts = TsParser.ParseTimeseries(id);             // Problem: A tab in the request is not accepted by the browser
            var periodizer = new Periodizer();
            return periodizer.InsertPoints(ts, Interval.Year).ToString();
            //return $"the value of {id} is {id * id}";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
