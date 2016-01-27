using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Periodic;

namespace YearlyWeb2.Controllers
{
    //[Authorize]
    public class ValuesController : ApiController
    {
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
