﻿using Periodic;
using System;

namespace PeriodicTest
{
    class Tvqs
    {
        public Tvq Tvq20150101 { get; set; }
        public Tvq Tvq20150601 { get; set; }
        public Tvq Tvq20150701 { get; set; }
        public Tvq Tvq20151231 { get; set; }
        public Tvq Tvq20160101 { get; set; }
        public Tvq Tvq20160601 { get; set; }

        public Tvqs()
        {
            var t = new DateTime(2015, 01, 01, 0, 0, 0, 0);
            Tvq20150101 = new Tvq(t, 100, Quality.Ok);
            t = new DateTime(2015, 06, 01, 0, 0, 0, 0);
            Tvq20150601 = new Tvq(t, 200, Quality.Ok);
            t = new DateTime(2015, 07, 01, 0, 0, 0, 0);
            Tvq20150701 = new Tvq(t, 300, Quality.Ok);
            t = new DateTime(2015, 12, 31, 23, 59, 59);
            Tvq20151231 = new Tvq(t, 399, Quality.Ok);
            t = new DateTime(2016, 01, 01, 0, 0, 0, 0);
            Tvq20160101 = new Tvq(t, 400, Quality.Ok);
            t = new DateTime(2016, 06, 01, 0, 0, 0, 0);
            Tvq20160601 = new Tvq(t, 500, Quality.Ok);
        }

    }
}
