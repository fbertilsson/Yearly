using Periodic;
using System;

namespace PeriodicTest
{
    class Tvqs
    {
        public Tvq<int> Tvq20150101 { get; set; }
        public Tvq<int> Tvq20150601 { get; set; }
        public Tvq<int> Tvq20160101 { get; set; }
        public Tvq<int> Tvq20160601 { get; set; }

        public Tvqs()
        {
            var t = new DateTime(2015, 01, 01, 0, 0, 0, 0);
            Tvq20150101 = new Tvq<int>(t, 100, Quality.Ok);
            t = new DateTime(2015, 06, 01, 0, 0, 0, 0);
            Tvq20150601 = new Tvq<int>(t, 200, Quality.Ok);
            t = new DateTime(2016, 01, 01, 0, 0, 0, 0);
            Tvq20160101 = new Tvq<int>(t, 300, Quality.Ok);
            t = new DateTime(2016, 06, 01, 0, 0, 0, 0);
            Tvq20160601 = new Tvq<int>(t, 500, Quality.Ok);
        }

    }
}
