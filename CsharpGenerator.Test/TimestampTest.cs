using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using BinaryBlocks.CsharpGenerator;

namespace BinaryBlocks.Test.CsharpGenerator
{
    [TestClass]
    public class TimestampTest
    {
        [TestMethod]
        public void Add()
        {
            const int MaxTimeMs = 100;

            using (Timer timer = new Timer((object o) => { Assert.Fail("Timed out"); }, null, MaxTimeMs, -1))
            {
                Timestamp source1 = new Timestamp();

                Timestamp result1 = source1.Add(TimeSpan.Zero);
                Assert.IsTrue(result1.Year == source1.Year);
                Assert.IsTrue(result1.Month == source1.Month);
                Assert.IsTrue(result1.Day == source1.Day);
                Assert.IsTrue(result1.Hour == source1.Hour);
                Assert.IsTrue(result1.Minute == source1.Minute);
                Assert.IsTrue(result1.Second == source1.Second);
                Assert.IsTrue(result1.Millisecond == source1.Millisecond);

                TimeSpan timespan = TimeSpan.FromSeconds(9999);

                Timestamp result2 = source1.Add(timespan);
                Assert.IsTrue(result2.Year == source1.Year);
                Assert.IsTrue(result2.Month == source1.Month);
                Assert.IsTrue(result2.Day == source1.Day);
                Assert.IsTrue(result2.Hour == 2);
                Assert.IsTrue(result2.Minute == 46);
                Assert.IsTrue(result2.Second == 39);
                Assert.IsTrue(result2.Millisecond == source1.Millisecond);

                DateTime now = DateTime.Now;
                Timestamp source2 = new Timestamp(now);
                now = now.Add(timespan);
                Timestamp result3 = source2.Add(timespan);
                Assert.IsTrue(result3.Year == now.Year);
                Assert.IsTrue(result3.Month == now.Month);
                Assert.IsTrue(result3.Day == now.Day);
                Assert.IsTrue(result3.Hour == now.Hour);
                Assert.IsTrue(result3.Second == now.Second);
                Assert.IsTrue(result3.Millisecond == now.Millisecond);
                Assert.IsTrue(result3.Date == now.Date);
            }
        }

        [TestMethod]
        public void AddDays()
        {
            const int MaxTimeMs = 100;

            using (Timer timer = new Timer((object o) => { Assert.Fail("Timed out"); }, null, MaxTimeMs, -1))
            {
                Timestamp source1 = new Timestamp();

                Timestamp result = source1.AddDays(0);
                Assert.IsTrue(result.Year == source1.Year);
                Assert.IsTrue(result.Month == source1.Month);
                Assert.IsTrue(result.Day == source1.Day);
                Assert.IsTrue(result.Hour == source1.Hour);
                Assert.IsTrue(result.Minute == source1.Minute);
                Assert.IsTrue(result.Second == source1.Second);
                Assert.IsTrue(result.Millisecond == source1.Millisecond);

                int days = 99;

                Timestamp result2 = source1.AddDays(days);
                Assert.IsTrue(result2.Year == source1.Year);
                Assert.IsTrue(result2.Month == 4);
                Assert.IsTrue(result2.Day == 10);
                Assert.IsTrue(result2.Hour == source1.Hour);
                Assert.IsTrue(result2.Minute == source1.Minute);
                Assert.IsTrue(result2.Second == source1.Second);
                Assert.IsTrue(result2.Millisecond == source1.Millisecond);

                DateTime now = DateTime.Now;
                Timestamp source2 = new Timestamp(now);
                now = now.AddDays(days);
                Timestamp result3 = source2.AddDays(days);
                Assert.IsTrue(result3.Year == now.Year);
                Assert.IsTrue(result3.Month == now.Month);
                Assert.IsTrue(result3.Day == now.Day);
                Assert.IsTrue(result3.Hour == now.Hour);
                Assert.IsTrue(result3.Second == now.Second);
                Assert.IsTrue(result3.Millisecond == now.Millisecond);
                Assert.IsTrue(result3.Date == now.Date);
            }
        }

        [TestMethod]
        public void AddHours()
        {
            const int MaxTimeMs = 100;

            using (Timer timer = new Timer((object o) => { Assert.Fail("Timed out"); }, null, MaxTimeMs, -1))
            {
                Timestamp source1 = new Timestamp();

                Timestamp result = source1.AddHours(0);
                Assert.IsTrue(result.Year == source1.Year);
                Assert.IsTrue(result.Month == source1.Month);
                Assert.IsTrue(result.Day == source1.Day);
                Assert.IsTrue(result.Hour == source1.Hour);
                Assert.IsTrue(result.Minute == source1.Minute);
                Assert.IsTrue(result.Second == source1.Second);
                Assert.IsTrue(result.Millisecond == source1.Millisecond);

                int hours = 99;

                Timestamp result2 = source1.AddHours(hours);
                Assert.IsTrue(result2.Year == source1.Year);
                Assert.IsTrue(result2.Month == source1.Month);
                Assert.IsTrue(result2.Day == 5);
                Assert.IsTrue(result2.Hour == 3);
                Assert.IsTrue(result2.Minute == source1.Minute);
                Assert.IsTrue(result2.Second == source1.Second);
                Assert.IsTrue(result2.Millisecond == source1.Millisecond);

                DateTime now = DateTime.Now;
                Timestamp source2 = new Timestamp(now);
                now = now.AddHours(hours);
                Timestamp result3 = source2.AddHours(hours);
                Assert.IsTrue(result3.Year == now.Year);
                Assert.IsTrue(result3.Month == now.Month);
                Assert.IsTrue(result3.Day == now.Day);
                Assert.IsTrue(result3.Hour == now.Hour);
                Assert.IsTrue(result3.Second == now.Second);
                Assert.IsTrue(result3.Millisecond == now.Millisecond);
                Assert.IsTrue(result3.Date == now.Date);
            }
        }

        [TestMethod]
        public void AddMilliseconds()
        {
            const int MaxTimeMs = 100;

            using (Timer timer = new Timer((object o) => { Assert.Fail("Timed out"); }, null, MaxTimeMs, -1))
            {
                Timestamp source1 = new Timestamp();

                Timestamp result = source1.AddMilliseconds(0);
                Assert.IsTrue(result.Year == source1.Year);
                Assert.IsTrue(result.Month == source1.Month);
                Assert.IsTrue(result.Day == source1.Day);
                Assert.IsTrue(result.Hour == source1.Hour);
                Assert.IsTrue(result.Minute == source1.Minute);
                Assert.IsTrue(result.Second == source1.Second);
                Assert.IsTrue(result.Millisecond == source1.Millisecond);

                int milliseconds = 999999;

                Timestamp result2 = source1.AddMilliseconds(milliseconds);
                Assert.IsTrue(result2.Year == source1.Year);
                Assert.IsTrue(result2.Month == source1.Month);
                Assert.IsTrue(result2.Day == source1.Day);
                Assert.IsTrue(result2.Hour == source1.Hour);
                Assert.IsTrue(result2.Minute == 16);
                Assert.IsTrue(result2.Second == 39);
                Assert.IsTrue(result2.Millisecond == 999);

                DateTime now = DateTime.Now;
                Timestamp source2 = new Timestamp(now);
                now = now.AddMilliseconds(milliseconds);
                Timestamp result3 = source2.AddMilliseconds(milliseconds);
                Assert.IsTrue(result3.Year == now.Year);
                Assert.IsTrue(result3.Month == now.Month);
                Assert.IsTrue(result3.Day == now.Day);
                Assert.IsTrue(result3.Hour == now.Hour);
                Assert.IsTrue(result3.Second == now.Second);
                Assert.IsTrue(result3.Millisecond == now.Millisecond);
                Assert.IsTrue(result3.Date == now.Date);
            }
        }

        [TestMethod]
        public void AddMonths()
        {
            const int MaxTimeMs = 100;

            using (Timer timer = new Timer((object o) => { Assert.Fail("Timed out"); }, null, MaxTimeMs, -1))
            {
                Timestamp source1 = new Timestamp();

                Timestamp result = source1.AddMonths(0);
                Assert.IsTrue(result.Year == source1.Year);
                Assert.IsTrue(result.Month == source1.Month);
                Assert.IsTrue(result.Day == source1.Day);
                Assert.IsTrue(result.Hour == source1.Hour);
                Assert.IsTrue(result.Minute == source1.Minute);
                Assert.IsTrue(result.Second == source1.Second);
                Assert.IsTrue(result.Millisecond == source1.Millisecond);

                int months = 99;

                Timestamp result2 = source1.AddMonths(months);
                Assert.IsTrue(result2.Year == 9);
                Assert.IsTrue(result2.Month == 4);
                Assert.IsTrue(result2.Day == source1.Day);
                Assert.IsTrue(result2.Hour == source1.Hour);
                Assert.IsTrue(result2.Minute == source1.Minute);
                Assert.IsTrue(result2.Second == source1.Second);
                Assert.IsTrue(result2.Millisecond == source1.Millisecond);

                DateTime now = DateTime.Now;
                Timestamp source2 = new Timestamp(now);
                now = now.AddMonths(months);
                Timestamp result3 = source2.AddMonths(months);
                Assert.IsTrue(result3.Year == now.Year);
                Assert.IsTrue(result3.Month == now.Month);
                Assert.IsTrue(result3.Day == now.Day);
                Assert.IsTrue(result3.Hour == now.Hour);
                Assert.IsTrue(result3.Second == now.Second);
                Assert.IsTrue(result3.Millisecond == now.Millisecond);
                Assert.IsTrue(result3.Date == now.Date);
            }
        }

        [TestMethod]
        public void AddSeconds()
        {
            const int MaxTimeMs = 100;

            using (Timer timer = new Timer((object o) => { Assert.Fail("Timed out"); }, null, MaxTimeMs, -1))
            {
                Timestamp source1 = new Timestamp();

                Timestamp result = source1.AddSeconds(0);
                Assert.IsTrue(result.Year == source1.Year);
                Assert.IsTrue(result.Month == source1.Month);
                Assert.IsTrue(result.Day == source1.Day);
                Assert.IsTrue(result.Hour == source1.Hour);
                Assert.IsTrue(result.Minute == source1.Minute);
                Assert.IsTrue(result.Second == source1.Second);
                Assert.IsTrue(result.Millisecond == source1.Millisecond);

                int seconds = 999;

                Timestamp result2 = source1.AddSeconds(seconds);
                Assert.IsTrue(result2.Year == source1.Year);
                Assert.IsTrue(result2.Month == source1.Month);
                Assert.IsTrue(result2.Day == source1.Day);
                Assert.IsTrue(result2.Hour == source1.Hour);
                Assert.IsTrue(result2.Minute == 16);
                Assert.IsTrue(result2.Second == 39);
                Assert.IsTrue(result2.Millisecond == source1.Millisecond);

                DateTime now = DateTime.Now;
                Timestamp source2 = new Timestamp(now);
                now = now.AddSeconds(seconds);
                Timestamp result3 = source2.AddSeconds(seconds);
                Assert.IsTrue(result3.Year == now.Year);
                Assert.IsTrue(result3.Month == now.Month);
                Assert.IsTrue(result3.Day == now.Day);
                Assert.IsTrue(result3.Hour == now.Hour);
                Assert.IsTrue(result3.Second == now.Second);
                Assert.IsTrue(result3.Millisecond == now.Millisecond);
                Assert.IsTrue(result3.Date == now.Date);
            }
        }

        [TestMethod]
        public void AddTicks()
        {
            const int MaxTimeMs = 100;

            using (Timer timer = new Timer((object o) => { Assert.Fail("Timed out"); }, null, MaxTimeMs, -1))
            {
                Timestamp source1 = new Timestamp();

                Timestamp result = source1.AddTicks(0);
                Assert.IsTrue(result.Year == source1.Year);
                Assert.IsTrue(result.Month == source1.Month);
                Assert.IsTrue(result.Day == source1.Day);
                Assert.IsTrue(result.Hour == source1.Hour);
                Assert.IsTrue(result.Minute == source1.Minute);
                Assert.IsTrue(result.Second == source1.Second);
                Assert.IsTrue(result.Millisecond == source1.Millisecond);

                long ticks = 999999999;

                Timestamp result2 = source1.AddTicks(ticks);
                Assert.IsTrue(result2.Year == source1.Year);
                Assert.IsTrue(result2.Month == source1.Month);
                Assert.IsTrue(result2.Day == source1.Day);
                Assert.IsTrue(result2.Hour == source1.Hour);
                Assert.IsTrue(result2.Minute == 1);
                Assert.IsTrue(result2.Second == 40);
                Assert.IsTrue(result2.Millisecond == 0);


                DateTime now = DateTime.Now;
                Timestamp source2 = new Timestamp(now);
                now = now.AddTicks(ticks);
                Timestamp result3 = source2.AddTicks(ticks);
                Assert.IsTrue(result3.Year == now.Year);
                Assert.IsTrue(result3.Month == now.Month);
                Assert.IsTrue(result3.Day == now.Day);
                Assert.IsTrue(result3.Hour == now.Hour);
                Assert.IsTrue(result3.Second == now.Second);
                Assert.IsTrue(result3.Millisecond == now.Millisecond);
                Assert.IsTrue(result3.Date == now.Date);
            }
        }

        [TestMethod]
        public void AddYears()
        {
            const int MaxTimeMs = 100;

            using (Timer timer = new Timer((object o) => { Assert.Fail("Timed out"); }, null, MaxTimeMs, -1))
            {
                Timestamp source1 = new Timestamp();

                Timestamp result = source1.AddYears(0);
                Assert.IsTrue(result.Year == source1.Year);
                Assert.IsTrue(result.Month == source1.Month);
                Assert.IsTrue(result.Day == source1.Day);
                Assert.IsTrue(result.Hour == source1.Hour);
                Assert.IsTrue(result.Minute == source1.Minute);
                Assert.IsTrue(result.Second == source1.Second);
                Assert.IsTrue(result.Millisecond == source1.Millisecond);

                int years = 99;

                Timestamp result2 = source1.AddYears(years);
                Assert.IsTrue(result2.Year == 100);
                Assert.IsTrue(result2.Month == source1.Month);
                Assert.IsTrue(result2.Day == source1.Day);
                Assert.IsTrue(result2.Hour == source1.Hour);
                Assert.IsTrue(result2.Minute == source1.Minute);
                Assert.IsTrue(result2.Second == source1.Second);
                Assert.IsTrue(result2.Millisecond == source1.Millisecond);

                DateTime now = DateTime.Now;
                Timestamp source2 = new Timestamp(now);
                now = now.AddYears(years);
                Timestamp result3 = source2.AddYears(years);
                Assert.IsTrue(result3.Year == now.Year);
                Assert.IsTrue(result3.Month == now.Month);
                Assert.IsTrue(result3.Day == now.Day);
                Assert.IsTrue(result3.Hour == now.Hour);
                Assert.IsTrue(result3.Second == now.Second);
                Assert.IsTrue(result3.Millisecond == now.Millisecond);
                Assert.IsTrue(result3.Date == now.Date);
            }
        }

        [TestMethod]
        public void ToString()
        {
            const int MaxTimeMs = 100;

            using (Timer timer = new Timer((object o) => { Assert.Fail("Timed out"); }, null, MaxTimeMs, -1))
            {
                Timestamp source1 = new Timestamp(2099, 3, 5, 12, 45);
                string result1 = source1.ToString();
                Assert.AreEqual(result1, "2099-03-05 CE 12:45:00Z");
            }
        }

        [TestMethod]
        public void Operators()
        {
            const int MaxTimeMs = 100;

            using (Timer timer = new Timer((object o) => { Assert.Fail("Timed out"); }, null, MaxTimeMs, -1))
            {
                DateTime datetime = DateTime.Now;
                TimeSpan timespan = TimeSpan.FromDays(79);
                Timestamp timestamp = new Timestamp(datetime);

                DateTime sample1 = datetime + timespan;
                Timestamp result1 = timestamp + timespan;
                Assert.IsTrue(result1.Year == sample1.Year);
                Assert.IsTrue(result1.Month == sample1.Month);
                Assert.IsTrue(result1.Day == sample1.Day);
                Assert.IsTrue(result1.Hour == sample1.Hour);
                Assert.IsTrue(result1.Second == sample1.Second);
                Assert.IsTrue(result1.Millisecond == sample1.Millisecond);
                Assert.IsTrue(result1.Date == sample1.Date);
                Assert.IsTrue(result1 == sample1);

                Thread.Sleep(1);

                DateTime sample2 = datetime - timespan;
                Timestamp result2 = timestamp - timespan;
                Assert.IsTrue(result2.Year == sample2.Year);
                Assert.IsTrue(result2.Month == sample2.Month);
                Assert.IsTrue(result2.Day == sample2.Day);
                Assert.IsTrue(result2.Hour == sample2.Hour);
                Assert.IsTrue(result2.Second == sample2.Second);
                Assert.IsTrue(result2.Millisecond == sample2.Millisecond);
                Assert.IsTrue(result2.Date == sample2.Date);
                Assert.IsTrue(result2 == sample2);

                DateTime datetime2 = DateTime.Now;
                Timestamp timestamp2 = new Timestamp(datetime2);

                TimeSpan sample3 = datetime - datetime2;
                TimeSpan result3 = timestamp - timestamp2;
                Assert.IsTrue(Math.Abs(sample3.TotalMilliseconds - result3.TotalMilliseconds) < 1);

                Assert.IsTrue(timestamp2 > timestamp);
                Assert.IsTrue(timestamp < timestamp2);
            }
        }
    }
}
