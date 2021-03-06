﻿using System;
using Periodic.Ts;

namespace PeriodicTest
{
    public class Tvqs
    {
        public Tvq Tvq20150101 { get; set; }
        public Tvq Tvq20150105 { get; set; }
        public Tvq Tvq20150110 { get; set; }
        public Tvq Tvq20150131 { get; set; }
        public Tvq Tvq20150301 { get; set; }
        public Tvq Tvq20150601 { get; set; }
        public Tvq Tvq20150622 { get; set; }
        public Tvq Tvq20150630 { get; set; }
        public Tvq Tvq20150701 { get; set; }
        public Tvq Tvq20151201 { get; set; }
        public Tvq Tvq20151231 { get; set; }
        public Tvq Tvq20160101 { get; set; }
        public Tvq Tvq20160201 { get; set; }
        public Tvq Tvq20160601 { get; set; }
        public Tvq Tvq20170601 { get; set; }

        public Tvqs()
        {
            var t = new DateTime(2015, 01, 01, 0, 0, 0, 0);
            Tvq20150101 = new Tvq(t, 100, Quality.Ok);
            t = new DateTime(2015, 01, 05, 0, 0, 0, 0);
            Tvq20150105 = new Tvq(t, 105, Quality.Ok);
            t = new DateTime(2015, 01, 10, 0, 0, 0, 0);
            Tvq20150110 = new Tvq(t, 105, Quality.Ok);
            t = new DateTime(2015, 01, 31, 23, 59, 59);
            Tvq20150131 = new Tvq(t, 105, Quality.Ok);
            t = new DateTime(2015, 03, 01, 0, 0, 0, 0);
            Tvq20150301 = new Tvq(t, 110, Quality.Ok);
            t = new DateTime(2015, 06, 01, 0, 0, 0, 0);
            Tvq20150601 = new Tvq(t, 200, Quality.Ok);
            t = new DateTime(2015, 06, 22, 0, 0, 0, 0);
            Tvq20150622 = new Tvq(t, 222, Quality.Ok);
            t = new DateTime(2015, 06, 30, 23, 59, 59);
            Tvq20150630 = new Tvq(t, 230, Quality.Ok);
            t = new DateTime(2015, 07, 01, 0, 0, 0, 0);
            Tvq20150701 = new Tvq(t, 300, Quality.Ok);
            t = new DateTime(2015, 12, 1, 0, 0, 0);
            Tvq20151201 = new Tvq(t, 310, Quality.Ok);
            t = new DateTime(2015, 12, 31, 23, 59, 59);
            Tvq20151231 = new Tvq(t, 399, Quality.Ok);
            t = new DateTime(2016, 01, 01, 0, 0, 0, 0);
            Tvq20160101 = new Tvq(t, 400, Quality.Ok);
            t = new DateTime(2016, 02, 01, 0, 0, 0, 0);
            Tvq20160201 = new Tvq(t, 410, Quality.Ok);
            t = new DateTime(2016, 06, 01, 0, 0, 0, 0);
            Tvq20160601 = new Tvq(t, 500, Quality.Ok);
            t = new DateTime(2017, 06, 01, 0, 0, 0, 0);
            Tvq20170601 = new Tvq(t, 500, Quality.Ok);
        }
    }
}
