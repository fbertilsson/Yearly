using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Web.Http;
using Periodic;
using Periodic.Algo;
using Periodic.Ts;

namespace YearlyWeb3.Controllers
{
    [Authorize]
    public class ValuesController : ApiController
    {
        [Route("api/periodic/{registerId}/monthly/csv")]
        [HttpGet]
        public HttpResponseMessage MonthlyValues()
        {
            var monthlyAverages = GetMonthlyAverages(); // Ignoring the registerId parameter for now
            var splitPerYear = new Splitter().SplitPerYear(monthlyAverages);

            var writer = new StringWriter();
            var renderer = new MonthlyValueTextRenderer(CultureInfo.InvariantCulture);
            renderer.Render(splitPerYear, writer);
            
            var result = writer.ToString();
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(result)
            };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/csv");
            return response;
        }


        public static Timeseries GetMonthlyAverages()
        {
            var repo = new RegistryEntryRepoFactory().GetRegistryEntryRepo();
            var sortedTvqs = repo.GetRegistryEntries(Thread.CurrentPrincipal).OrderBy(x => x.Time);

            var tsWithRegisterEntries = new Timeseries();
            tsWithRegisterEntries.AddRange(sortedTvqs.ToList());

            var periodizer = new Periodizer();
            var monthlyAverages = periodizer.MonthlyAverage(tsWithRegisterEntries);

            //const int minMonths = 2;
            //var tooFewEntries = monthlyRegisterEntries.Count < minMonths;
            //var areTooFewEntries = tooFewEntries;
            //if (areTooFewEntries)
            //{
            //    throw new TooFewEntriesException(minMonths);
            //}

            //var deltaOperator = new DeltaTsOperator();
            //var monthlyAverages = deltaOperator.Apply(monthlyRegisterEntries);

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
