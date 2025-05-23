﻿using System;
using System.Linq;
using NUnit.Framework;
using QuickFix;

namespace UnitTests
{
    [TestFixture]
    public class SessionScheduleTests
    {
        #region Properties
        /// <summary>
        /// Windows/*nix-agnostic time zone ID
        /// (because it would be too easy if they could settle on a standard list of ids, right?)
        /// </summary>
        private static string EasternStandardTimeZoneId
        {
            get
            {
                return TimeZoneInfo.GetSystemTimeZones().Any(x => x.Id == "Eastern Standard Time")
                    ? "Eastern Standard Time"
                    : "America/New_York";
            }
        }

        /// <summary>
        /// Windows/*nix-agnostic time zone ID
        /// (because it would be too easy if they could settle on a standard list of ids, right?)
        /// </summary>
        private static string PacificStandardTimeZoneId
        {
            get
            {
                return TimeZoneInfo.GetSystemTimeZones().Any(x => x.Id == "Pacific Standard Time")
                    ? "Pacific Standard Time"
                    : "America/Los_Angeles";
            }
        }
        #endregion

        private const string DtFmt = "yyyyMMdd-HH:mm:ss";

        [Test]
        public void TestCtor_BasicDailySchedule()
        {
            SettingsDictionary settings = new SettingsDictionary();
            Exception ex = Assert.Throws(typeof(ConfigError), delegate { new SessionSchedule(settings); })!;
            Assert.That(ex.Message, Does.Contain("No value for key: StartTime"));

            settings.SetString(SessionSettings.START_TIME, "00:00:00");
            ex = Assert.Throws(typeof(ConfigError), delegate { new SessionSchedule(settings); })!;
            Assert.That(ex.Message, Does.Contain("No value for key: EndTime"));

            settings.SetString(SessionSettings.END_TIME, "00:0blkajsdf");
            ex = Assert.Throws(typeof(ConfigError), delegate { new SessionSchedule(settings); })!;
            Assert.That(ex.Message, Does.Contain("String '00:0blkajsdf' was not recognized as a valid TimeSpan"));

            settings.SetString(SessionSettings.END_TIME, "00:00:00");
            Assert.DoesNotThrow(delegate { new SessionSchedule(settings); });
        }

        [Test]
        public void TestCtor_WeeklongSession()
        {
            SettingsDictionary settings = new SettingsDictionary();
            settings.SetString(SessionSettings.START_TIME, "00:00:00");
            settings.SetString(SessionSettings.END_TIME, "00:00:00");

            settings.SetDay(SessionSettings.START_DAY, DayOfWeek.Thursday);
            Exception ex = Assert.Throws(typeof(ConfigError), delegate { new SessionSchedule(settings); })!;
            Assert.That(ex.Message, Does.Contain("StartDay used without EndDay"));

            settings.SetDay(SessionSettings.END_DAY, DayOfWeek.Friday);
            Assert.DoesNotThrow(delegate { new SessionSchedule(settings); });
        }

        [Test]
        public void TestCtor_WeekdaysSession()
        {
            SettingsDictionary settings = new SettingsDictionary();
            settings.SetString(SessionSettings.WEEKDAYS, "Sun,Tue,Fri");

            Exception ex = Assert.Throws(typeof(ConfigError), delegate { new SessionSchedule(settings); })!;
            Assert.That(ex.Message, Does.Contain("No value for key: StartTime"));

            settings.SetString(SessionSettings.START_TIME, "00:00:00");
            settings.SetString(SessionSettings.END_TIME, "00:00:00");
            Assert.DoesNotThrow(delegate { new SessionSchedule(settings); });

            settings.SetString(SessionSettings.START_DAY, "Tue");
            ex = Assert.Throws(typeof(ConfigError), delegate { new SessionSchedule(settings); })!;
            Assert.That(ex.Message, Does.Contain("StartDay/EndDay are not compatible with 'Weekdays' setting"));
        }

        [Test]
        public void TestCtor_NonStopSession()
        {
            SettingsDictionary settings = new SettingsDictionary();
            settings.SetBool(SessionSettings.NON_STOP_SESSION, true);
            Assert.DoesNotThrow(delegate { new SessionSchedule(settings); });

            settings.SetString(SessionSettings.START_DAY, "Monday");
            Exception ex = Assert.Throws(typeof(ConfigError), delegate { new SessionSchedule(settings); })!;
            Assert.That(ex.Message, Does.Contain("NonStopSession is not compatible with StartDay/EndDay and StartTime/EndTime"));

            settings = new SettingsDictionary();
            settings.SetBool(SessionSettings.NON_STOP_SESSION, true);
            settings.SetString(SessionSettings.START_TIME, "05:00:00");
            ex = Assert.Throws(typeof(ConfigError), delegate { new SessionSchedule(settings); })!;
            Assert.That(ex.Message, Does.Contain("NonStopSession is not compatible with StartDay/EndDay and StartTime/EndTime"));
        }

        [Test]
        public void TestDailyIsSessionTimeAllDay()
        {
            SettingsDictionary settings = new SettingsDictionary();
            settings.SetString(SessionSettings.START_TIME, "00:00:00");
            settings.SetString(SessionSettings.END_TIME, "00:00:00");

            SessionSchedule sched = new SessionSchedule(settings);

            Assert.That(sched.IsSessionTime(new DateTime(2011, 10, 17, 9, 43, 0, DateTimeKind.Utc)), Is.True);
            Assert.That(sched.IsSessionTime(new DateTime(2011, 10, 18, 9, 43, 0, DateTimeKind.Utc)), Is.True);
            Assert.That(sched.IsSessionTime(new DateTime(2011, 10, 18, 0, 0, 0, DateTimeKind.Utc)), Is.True);
            Assert.That(sched.IsSessionTime(new DateTime(2011, 10, 18, 23, 59, 59, DateTimeKind.Utc)), Is.True);
        }

        [Test]
        public void TestWeeklySessionSameDayAllWeek()
        {
            SettingsDictionary settings = new SettingsDictionary();
            settings.SetString(SessionSettings.START_TIME, "00:00:00");
            settings.SetString(SessionSettings.END_TIME, "00:00:00");

            settings.SetDay(SessionSettings.START_DAY, DayOfWeek.Monday);
            settings.SetDay(SessionSettings.END_DAY, DayOfWeek.Monday);
            SessionSchedule sched = new SessionSchedule(settings);

            //a sunday
            Assert.That(sched.IsSessionTime(new DateTime(2011, 10, 16, 9, 43, 0, DateTimeKind.Utc)), Is.True);
            Assert.That(sched.IsSessionTime(new DateTime(2011, 10, 16, 23, 59, 59, DateTimeKind.Utc)), Is.True);
            Assert.That(sched.IsSessionTime(new DateTime(2011, 10, 16, 0, 0, 0, DateTimeKind.Utc)), Is.True);

            //a monday
            Assert.That(sched.IsSessionTime(new DateTime(2011, 10, 17, 9, 43, 0, DateTimeKind.Utc)), Is.True);
            Assert.That(sched.IsSessionTime(new DateTime(2011, 10, 17, 23, 59, 59, DateTimeKind.Utc)), Is.True);
            Assert.That(sched.IsSessionTime(new DateTime(2011, 10, 17, 0, 0, 0, DateTimeKind.Utc)), Is.True);

            //a tuesday
            Assert.That(sched.IsSessionTime(new DateTime(2011, 10, 18, 9, 43, 0, DateTimeKind.Utc)), Is.True);
            Assert.That(sched.IsSessionTime(new DateTime(2011, 10, 18, 0, 0, 0, DateTimeKind.Utc)), Is.True);
        }

        [Test]
        public void TestWeeklySessionSameDayMostWeek()
        {
            SettingsDictionary settings = new SettingsDictionary();
            settings.SetString(SessionSettings.START_TIME, "05:00:00");
            settings.SetString(SessionSettings.END_TIME, "00:00:00");

            settings.SetDay(SessionSettings.START_DAY, DayOfWeek.Monday);
            settings.SetDay(SessionSettings.END_DAY, DayOfWeek.Monday);
            SessionSchedule sched = new SessionSchedule(settings);

            //a sunday
            Assert.That(sched.IsSessionTime(new DateTime(2011, 10, 16, 23, 59, 59, DateTimeKind.Utc)), Is.True);
            Assert.That(sched.IsSessionTime(new DateTime(2011, 10, 16, 0, 0, 0, DateTimeKind.Utc)), Is.True);

            //a monday
            Assert.That(sched.IsSessionTime(new DateTime(2011, 10, 17, 0, 0, 1, DateTimeKind.Utc)), Is.False);
            Assert.That(sched.IsSessionTime(new DateTime(2011, 10, 17, 4, 0, 1, DateTimeKind.Utc)), Is.False);
            Assert.That(sched.IsSessionTime(new DateTime(2011, 10, 17, 23, 59, 59, DateTimeKind.Utc)), Is.True);

            //a tuesday
            Assert.That(sched.IsSessionTime(new DateTime(2011, 10, 18, 9, 43, 0, DateTimeKind.Utc)), Is.True);
            Assert.That(sched.IsSessionTime(new DateTime(2011, 10, 18, 0, 0, 0, DateTimeKind.Utc)), Is.True);
        }

        [Test]
        public void TestWeeklySessionSameDayOneDay()
        {
            SettingsDictionary settings = new SettingsDictionary();
            settings.SetString(SessionSettings.START_TIME, "00:00:00");
            settings.SetString(SessionSettings.END_TIME, "05:00:00");

            settings.SetDay(SessionSettings.START_DAY, DayOfWeek.Monday);
            settings.SetDay(SessionSettings.END_DAY, DayOfWeek.Monday);
            SessionSchedule sched = new SessionSchedule(settings);

            //a sunday
            Assert.That(sched.IsSessionTime(new DateTime(2011, 10, 16, 23, 59, 59, DateTimeKind.Utc)), Is.False);
            Assert.That(sched.IsSessionTime(new DateTime(2011, 10, 16, 0, 0, 0, DateTimeKind.Utc)), Is.False);

            //a monday
            Assert.That(sched.IsSessionTime(new DateTime(2011, 10, 17, 0, 0, 1, DateTimeKind.Utc)), Is.True);
            Assert.That(sched.IsSessionTime(new DateTime(2011, 10, 17, 4, 0, 1, DateTimeKind.Utc)), Is.True);
            Assert.That(sched.IsSessionTime(new DateTime(2011, 10, 17, 6, 59, 59, DateTimeKind.Utc)), Is.False);

            //a tuesday
            Assert.That(sched.IsSessionTime(new DateTime(2011, 10, 18, 9, 43, 0, DateTimeKind.Utc)), Is.False);
            Assert.That(sched.IsSessionTime(new DateTime(2011, 10, 18, 0, 0, 0, DateTimeKind.Utc)), Is.False);
        }


        [Test]
        public void TestWeeklySessionMultiDay()
        {
            SettingsDictionary settings = new SettingsDictionary();
            settings.SetString(SessionSettings.START_TIME, "00:00:00");
            settings.SetString(SessionSettings.END_TIME, "00:00:00");

            //only on monday-thur (note end time)
            settings.SetDay(SessionSettings.START_DAY, DayOfWeek.Monday);
            settings.SetDay(SessionSettings.END_DAY, DayOfWeek.Friday);

            SessionSchedule sched = new SessionSchedule(settings);

            //a monday
            Assert.That(sched.IsSessionTime(new DateTime(2011, 10, 17, 0, 0, 0, DateTimeKind.Utc)), Is.True);
            Assert.That(sched.IsSessionTime(new DateTime(2011, 10, 17, 9, 43, 0, DateTimeKind.Utc)), Is.True);

            // a thursday
            Assert.That(sched.IsSessionTime(new DateTime(2011, 10, 20, 23, 59, 59, DateTimeKind.Utc)), Is.True);

            //a fri, sat, sun
            Assert.That(sched.IsSessionTime(new DateTime(2011, 10, 21, 23, 59, 59, DateTimeKind.Utc)), Is.False);
            Assert.That(sched.IsSessionTime(new DateTime(2011, 10, 22, 0, 0, 0, DateTimeKind.Utc)), Is.False);
            Assert.That(sched.IsSessionTime(new DateTime(2011, 10, 16, 9, 43, 0, DateTimeKind.Utc)), Is.False);
        }

        [Test]
        public void TestWeeklySessionMultiDayOverlap()
        {
            SettingsDictionary settings = new SettingsDictionary();
            settings.SetString(SessionSettings.START_TIME, "00:00:00");
            settings.SetString(SessionSettings.END_TIME, "00:00:00");

            //only on wed-sunday night
            settings.SetDay(SessionSettings.START_DAY, DayOfWeek.Wednesday);
            settings.SetDay(SessionSettings.END_DAY, DayOfWeek.Monday);

            SessionSchedule sched = new SessionSchedule(settings);

            //wed-monday
            Assert.That(sched.IsSessionTime(new DateTime(2011, 10, 19, 9, 43, 0, DateTimeKind.Utc)), Is.True);
            Assert.That(sched.IsSessionTime(new DateTime(2011, 10, 20, 9, 43, 0, DateTimeKind.Utc)), Is.True);
            Assert.That(sched.IsSessionTime(new DateTime(2011, 10, 21, 9, 43, 0, DateTimeKind.Utc)), Is.True);
            Assert.That(sched.IsSessionTime(new DateTime(2011, 10, 22, 9, 43, 0, DateTimeKind.Utc)), Is.True);
            Assert.That(sched.IsSessionTime(new DateTime(2011, 10, 16, 9, 43, 0, DateTimeKind.Utc)), Is.True);

            //monday
            Assert.That(sched.IsSessionTime(new DateTime(2011, 10, 17, 9, 43, 0, DateTimeKind.Utc)), Is.False);

            //tuesday
            Assert.That(sched.IsSessionTime(new DateTime(2011, 10, 18, 9, 43, 0, DateTimeKind.Utc)), Is.False);
        }

        [Test]
        public void TestWeeklySessionMultiDayHours()
        {
            SettingsDictionary settings = new SettingsDictionary();
            settings.SetString(SessionSettings.START_TIME, "07:00:00");
            settings.SetString(SessionSettings.END_TIME, "15:00:00");

            //only on monday-fri
            settings.SetDay(SessionSettings.START_DAY, DayOfWeek.Monday);
            settings.SetDay(SessionSettings.END_DAY, DayOfWeek.Friday);

            SessionSchedule sched = new SessionSchedule(settings);

            //Monday Scenarios
            Assert.That(sched.IsSessionTime(new DateTime(2011, 10, 17, 6, 59, 0, DateTimeKind.Utc)), Is.False);
            Assert.That(sched.IsSessionTime(new DateTime(2011, 10, 17, 7, 30, 0, DateTimeKind.Utc)), Is.True);
            Assert.That(sched.IsSessionTime(new DateTime(2011, 10, 17, 15, 30, 0, DateTimeKind.Utc)), Is.True);

            //Midweek Scenarios
            Assert.That(sched.IsSessionTime(new DateTime(2011, 10, 19, 6, 59, 0, DateTimeKind.Utc)), Is.True);
            Assert.That(sched.IsSessionTime(new DateTime(2011, 10, 19, 7, 30, 0, DateTimeKind.Utc)), Is.True);
            Assert.That(sched.IsSessionTime(new DateTime(2011, 10, 19, 15, 30, 0, DateTimeKind.Utc)), Is.True);

            //Friday Scenarios
            Assert.That(sched.IsSessionTime(new DateTime(2011, 10, 21, 6, 59, 0, DateTimeKind.Utc)), Is.True);
            Assert.That(sched.IsSessionTime(new DateTime(2011, 10, 21, 7, 30, 0, DateTimeKind.Utc)), Is.True);
            Assert.That(sched.IsSessionTime(new DateTime(2011, 10, 21, 15, 30, 0, DateTimeKind.Utc)), Is.False);

            //Weekend
            Assert.That(sched.IsSessionTime(new DateTime(2011, 10, 22, 6, 59, 0, DateTimeKind.Utc)), Is.False);
            Assert.That(sched.IsSessionTime(new DateTime(2011, 10, 22, 7, 30, 0, DateTimeKind.Utc)), Is.False);
            Assert.That(sched.IsSessionTime(new DateTime(2011, 10, 22, 15, 30, 0, DateTimeKind.Utc)), Is.False);
        }

        [Test]
        public void TestWeeklySessionMultiDayHoursOverlap()
        {
            SettingsDictionary settings = new SettingsDictionary();
            settings.SetString(SessionSettings.START_TIME, "15:00:00");
            settings.SetString(SessionSettings.END_TIME, "07:00:00");

            //only on monday-fri
            settings.SetDay(SessionSettings.START_DAY, DayOfWeek.Monday);
            settings.SetDay(SessionSettings.END_DAY, DayOfWeek.Friday);

            SessionSchedule sched = new SessionSchedule(settings);

            //weekdays
            Assert.That(sched.IsSessionTime(new DateTime(2011, 10, 17, 15, 30, 0, DateTimeKind.Utc)), Is.True);
            Assert.That(sched.IsSessionTime(new DateTime(2011, 10, 17, 6, 30, 0, DateTimeKind.Utc)), Is.False, "foo");

            Assert.That(sched.IsSessionTime(new DateTime(2011, 10, 21, 5, 30, 59, DateTimeKind.Utc)), Is.True);
            Assert.That(sched.IsSessionTime(new DateTime(2011, 10, 21, 15, 30, 59, DateTimeKind.Utc)), Is.False);
            Assert.That(sched.IsSessionTime(new DateTime(2011, 10, 22, 6, 59, 59, DateTimeKind.Utc)), Is.False);
            Assert.That(sched.IsSessionTime(new DateTime(2011, 10, 22, 7, 00, 1, DateTimeKind.Utc)), Is.False);
            Assert.That(sched.IsSessionTime(new DateTime(2011, 10, 22, 15, 30, 0, DateTimeKind.Utc)), Is.False);
        }


        [Test]
        public void TestDailyIsSessionTime()
        {
            SettingsDictionary settings = new SettingsDictionary();
            settings.SetString(SessionSettings.START_TIME, "00:12:00");
            settings.SetString(SessionSettings.END_TIME, "06:00:23");

            SessionSchedule sched = new SessionSchedule(settings);

            Assert.That(sched.IsSessionTime(new DateTime(2011, 10, 17, 0, 12, 0, DateTimeKind.Utc)), Is.True);
            Assert.That(sched.IsSessionTime(new DateTime(2011, 10, 17, 5, 43, 0, DateTimeKind.Utc)), Is.True);
            Assert.That(sched.IsSessionTime(new DateTime(2011, 10, 18, 6, 0, 23, DateTimeKind.Utc)), Is.True);

            Assert.That(sched.IsSessionTime(new DateTime(2011, 10, 18, 0, 11, 0, DateTimeKind.Utc)), Is.False);
            Assert.That(sched.IsSessionTime(new DateTime(2011, 10, 18, 6, 0, 24, DateTimeKind.Utc)), Is.False);
        }

        [Test]
        public void TestDailyIsSessionTimeWrapAround()
        {
            SettingsDictionary settings = new SettingsDictionary();
            settings.SetString(SessionSettings.START_TIME, "06:00:23");
            settings.SetString(SessionSettings.END_TIME, "00:12:00");

            SessionSchedule sched = new SessionSchedule(settings);

            Assert.That(sched.IsSessionTime(new DateTime(2011, 10, 17, 6, 0, 23, DateTimeKind.Utc)), Is.True);
            Assert.That(sched.IsSessionTime(new DateTime(2011, 10, 17, 8, 43, 0, DateTimeKind.Utc)), Is.True);
            Assert.That(sched.IsSessionTime(new DateTime(2011, 10, 18, 0, 12, 00, DateTimeKind.Utc)), Is.True);

            Assert.That(sched.IsSessionTime(new DateTime(2011, 10, 18, 0, 12, 1, DateTimeKind.Utc)), Is.False);
            Assert.That(sched.IsSessionTime(new DateTime(2011, 10, 18, 6, 0, 22, DateTimeKind.Utc)), Is.False);

        }

        [Test]
        public void TestInvalidTimeZone()
        {
            SettingsDictionary settings = new SettingsDictionary();
            settings.SetString(SessionSettings.START_TIME, "06:00:23");
            settings.SetString(SessionSettings.END_TIME, "00:12:00");
            settings.SetString(SessionSettings.TIME_ZONE, "Doh");

            Assert.Throws(typeof(TimeZoneNotFoundException), delegate { new SessionSchedule(settings); });
        }

        [Test]
        public void TestLocalTimeTimeZoneConflict()
        {
            SettingsDictionary settings = new SettingsDictionary();
            settings.SetString(SessionSettings.START_TIME, "06:00:23");
            settings.SetString(SessionSettings.END_TIME, "00:12:00");
            settings.SetString(SessionSettings.TIME_ZONE, "Doh");
            settings.SetString(SessionSettings.USE_LOCAL_TIME, "Y");
            settings.SetString(SessionSettings.TIME_ZONE, EasternStandardTimeZoneId);

            Assert.Throws(typeof(ConfigError), delegate { new SessionSchedule(settings); });
        }

        [Test]
        public void TestTimeZone()
        {
            SettingsDictionary settings = new SettingsDictionary();
            settings.SetString(SessionSettings.START_TIME, "09:30:00");
            settings.SetString(SessionSettings.END_TIME, "16:00:00");
            settings.SetString(SessionSettings.TIME_ZONE, EasternStandardTimeZoneId);

            SessionSchedule sched = new SessionSchedule(settings);

            Assert.That(sched.IsSessionTime(new DateTime(2011, 10, 17, 13, 29, 59, DateTimeKind.Utc)), Is.False);
            Assert.That(sched.IsSessionTime(new DateTime(2011, 10, 17, 13, 30, 0, DateTimeKind.Utc)), Is.True);
            Assert.That(sched.IsSessionTime(new DateTime(2011, 10, 17, 20, 0, 0, DateTimeKind.Utc)), Is.True);
            Assert.That(sched.IsSessionTime(new DateTime(2011, 10, 17, 20, 0, 1, DateTimeKind.Utc)), Is.False);
        }

        [Test]
        public void TestNextEndTime_takesUtcOnly()
        {
            SettingsDictionary settings = new SettingsDictionary();
            settings.SetString(SessionSettings.START_TIME, "09:30:00");
            settings.SetString(SessionSettings.END_TIME, "16:00:00");
            SessionSchedule sched = new SessionSchedule(settings);

            DateTime local = new DateTime(2001, 1, 1, 1, 1, 1, DateTimeKind.Local);
            DateTime none = new DateTime(2001, 1, 1, 1, 1, 1, DateTimeKind.Unspecified);

            Assert.Throws<ArgumentException>(delegate { sched.NextEndTime(local); });
            Assert.Throws<ArgumentException>(delegate { sched.NextEndTime(none); });
        }

        [Test]
        public void TestNextEndTime_Daily()
        {
            SettingsDictionary settings = new SettingsDictionary();
            settings.SetString(SessionSettings.START_TIME, "09:30:00");
            settings.SetString(SessionSettings.END_TIME, "16:00:00");
            SessionSchedule sched = new SessionSchedule(settings);

            Assert.That(sched.NextEndTime(new DateTime(2012, 10, 18, 15, 59, 59, DateTimeKind.Utc)).ToString(DtFmt),
                Is.EqualTo("20121018-16:00:00"));
            Assert.That(sched.NextEndTime(new DateTime(2012, 10, 18, 16, 00, 00, DateTimeKind.Utc)).ToString(DtFmt),
                Is.EqualTo("20121018-16:00:00"));
            Assert.That(sched.NextEndTime(new DateTime(2012, 10, 18, 16, 00, 01, DateTimeKind.Utc)).ToString(DtFmt),
                Is.EqualTo("20121019-16:00:00"));

            // ==========
            // Settings file is specified in a zone (est, -5)
            settings = new SettingsDictionary();
            settings.SetString(SessionSettings.START_TIME, "04:30:00"); // 09:30:00 utc
            settings.SetString(SessionSettings.END_TIME, "11:00:00");   // 16:00:00 utc
            settings.SetString(SessionSettings.TIME_ZONE, EasternStandardTimeZoneId); //-5
            sched = new SessionSchedule(settings);

            Assert.That(sched.NextEndTime(new DateTime(2012, 12, 18, 15, 59, 59, DateTimeKind.Utc)).ToString(DtFmt),
                Is.EqualTo("20121218-11:00:00"));
            Assert.That(sched.NextEndTime(new DateTime(2012, 12, 18, 16, 00, 00, DateTimeKind.Utc)).ToString(DtFmt),
                Is.EqualTo("20121218-11:00:00"));
            Assert.That(sched.NextEndTime(new DateTime(2012, 12, 18, 16, 00, 01, DateTimeKind.Utc)).ToString(DtFmt),
                Is.EqualTo("20121219-11:00:00"));

            // ==========
            // Time zone during Daylight Savings
            settings = new SettingsDictionary();
            settings.SetString(SessionSettings.START_TIME, "05:30:00"); // 09:30:00 utc
            settings.SetString(SessionSettings.END_TIME, "12:00:00");   // 16:00:00 utc
            settings.SetString(SessionSettings.TIME_ZONE, EasternStandardTimeZoneId); //-4 for DST
            sched = new SessionSchedule(settings);

            Assert.That(sched.NextEndTime(new DateTime(2012, 06, 18, 15, 59, 59, DateTimeKind.Utc)).ToString(DtFmt),
                Is.EqualTo("20120618-12:00:00"));
            Assert.That(sched.NextEndTime(new DateTime(2012, 06, 18, 16, 00, 00, DateTimeKind.Utc)).ToString(DtFmt),
                Is.EqualTo("20120618-12:00:00"));
            Assert.That(sched.NextEndTime(new DateTime(2012, 06, 18, 16, 00, 01, DateTimeKind.Utc)).ToString(DtFmt),
                Is.EqualTo("20120619-12:00:00"));
        }

        [Test]
        public void TestNextEndTime_Weekly()
        {
            SettingsDictionary settings = new SettingsDictionary();
            settings.SetString(SessionSettings.START_TIME, "09:30:00");
            settings.SetString(SessionSettings.END_TIME, "16:00:00");
            settings.SetDay(SessionSettings.START_DAY, DayOfWeek.Monday);
            settings.SetDay(SessionSettings.END_DAY, DayOfWeek.Friday);
            SessionSchedule sched = new SessionSchedule(settings);

            // Oct 15 and 22 are Mondays, 19 and 26 are Fridays
            Assert.That(sched.NextEndTime(new DateTime(2012, 10, 19, 15, 59, 59, DateTimeKind.Utc)).ToString(DtFmt),
                Is.EqualTo("20121019-16:00:00"));
            Assert.That(sched.NextEndTime(new DateTime(2012, 10, 19, 16, 00, 00, DateTimeKind.Utc)).ToString(DtFmt),
                Is.EqualTo("20121019-16:00:00"));
            Assert.That(sched.NextEndTime(new DateTime(2012, 10, 19, 16, 00, 01, DateTimeKind.Utc)).ToString(DtFmt),
                Is.EqualTo("20121026-16:00:00"));

            // ==========
            // Settings file is specified in a zone (est, -5)
            settings = new SettingsDictionary();
            settings.SetString(SessionSettings.START_TIME, "04:30:00"); // 09:30:00 utc
            settings.SetString(SessionSettings.END_TIME, "11:00:00");   // 16:00:00 utc
            settings.SetString(SessionSettings.TIME_ZONE, EasternStandardTimeZoneId); //-5
            settings.SetDay(SessionSettings.START_DAY, DayOfWeek.Monday);
            settings.SetDay(SessionSettings.END_DAY, DayOfWeek.Friday);
            sched = new SessionSchedule(settings);

            // Dec 14 and 21 are Fridays
            Assert.That(sched.NextEndTime(new DateTime(2012, 12, 14, 15, 59, 59, DateTimeKind.Utc)).ToString(DtFmt),
                Is.EqualTo("20121214-11:00:00"));
            Assert.That(sched.NextEndTime(new DateTime(2012, 12, 14, 16, 00, 00, DateTimeKind.Utc)).ToString(DtFmt),
                Is.EqualTo("20121214-11:00:00"));
            Assert.That(sched.NextEndTime(new DateTime(2012, 12, 14, 16, 00, 01, DateTimeKind.Utc)).ToString(DtFmt),
                Is.EqualTo("20121221-11:00:00"));

            // ==========
            // Time zone during Daylight Savings
            settings = new SettingsDictionary();
            settings.SetString(SessionSettings.START_TIME, "05:30:00"); // 09:30:00 utc
            settings.SetString(SessionSettings.END_TIME, "12:00:00");   // 16:00:00 utc
            settings.SetString(SessionSettings.TIME_ZONE, EasternStandardTimeZoneId); //-4 for DST
            settings.SetDay(SessionSettings.START_DAY, DayOfWeek.Monday);
            settings.SetDay(SessionSettings.END_DAY, DayOfWeek.Friday);
            sched = new SessionSchedule(settings);

            // June 15 and 22 are Fridays
            Assert.That(sched.NextEndTime(new DateTime(2012, 06, 15, 15, 59, 59, DateTimeKind.Utc)).ToString(DtFmt),
                Is.EqualTo("20120615-12:00:00"));
            Assert.That(sched.NextEndTime(new DateTime(2012, 06, 15, 16, 00, 00, DateTimeKind.Utc)).ToString(DtFmt),
                Is.EqualTo("20120615-12:00:00"));
            Assert.That(sched.NextEndTime(new DateTime(2012, 06, 15, 16, 00, 01, DateTimeKind.Utc)).ToString(DtFmt),
                Is.EqualTo("20120622-12:00:00"));
        }

        [Test]
        public void TestIsNewSession_takesUtcOnly()
        {
            SettingsDictionary settings = new SettingsDictionary();
            settings.SetString(SessionSettings.START_TIME, "09:30:00");
            settings.SetString(SessionSettings.END_TIME, "16:00:00");
            SessionSchedule sched = new SessionSchedule(settings);

            DateTime local = new DateTime(2001, 1, 1, 1, 1, 1, DateTimeKind.Local);
            DateTime utc = new DateTime(2001, 1, 1, 1, 1, 1, DateTimeKind.Utc);
            DateTime none = new DateTime(2001, 1, 1, 1, 1, 1, DateTimeKind.Unspecified);

            Assert.Throws<ArgumentException>(delegate { sched.IsNewSession(local, utc); });
            Assert.Throws<ArgumentException>(delegate { sched.IsNewSession(utc, local); });
            Assert.Throws<ArgumentException>(delegate { sched.IsNewSession(none, utc); });
            Assert.Throws<ArgumentException>(delegate { sched.IsNewSession(utc, none); });
        }

        [Test]
        public void TestIsNewSession_Daily()
        {
            SettingsDictionary settings = new SettingsDictionary();
            settings.SetString(SessionSettings.START_TIME, "09:30:00");
            settings.SetString(SessionSettings.END_TIME, "16:00:00");
            SessionSchedule sched = new SessionSchedule(settings);

            Assert.That(sched.IsNewSession(
                new DateTime(2012, 10, 18, 15, 59, 58, DateTimeKind.Utc),
                new DateTime(2012, 10, 18, 15, 59, 59, DateTimeKind.Utc)), Is.False);
            Assert.That(sched.IsNewSession(
                new DateTime(2012, 10, 18, 15, 59, 59, DateTimeKind.Utc),
                new DateTime(2012, 10, 18, 16, 00, 00, DateTimeKind.Utc)), Is.False);
            Assert.That(sched.IsNewSession(
                new DateTime(2012, 10, 18, 16, 00, 00, DateTimeKind.Utc),
                new DateTime(2012, 10, 18, 16, 00, 01, DateTimeKind.Utc)), Is.True);
            Assert.That(sched.IsNewSession(
                new DateTime(2012, 10, 18, 16, 00, 01, DateTimeKind.Utc),
                new DateTime(2012, 10, 18, 16, 00, 02, DateTimeKind.Utc)), Is.False);

            // ==========
            // Settings file is specified in a zone (est, -5)
            settings = new SettingsDictionary();
            settings.SetString(SessionSettings.START_TIME, "04:30:00"); // 09:30:00 utc
            settings.SetString(SessionSettings.END_TIME, "11:00:00");   // 16:00:00 utc
            settings.SetString(SessionSettings.TIME_ZONE, EasternStandardTimeZoneId); //-5
            sched = new SessionSchedule(settings);

            Assert.That(sched.IsNewSession(
                new DateTime(2012, 12, 18, 15, 59, 58, DateTimeKind.Utc),
                new DateTime(2012, 12, 18, 15, 59, 59, DateTimeKind.Utc)), Is.False);
            Assert.That(sched.IsNewSession(
                new DateTime(2012, 12, 18, 15, 59, 59, DateTimeKind.Utc),
                new DateTime(2012, 12, 18, 16, 00, 00, DateTimeKind.Utc)), Is.False);
            Assert.That(sched.IsNewSession(
                new DateTime(2012, 12, 18, 16, 00, 00, DateTimeKind.Utc),
                new DateTime(2012, 12, 18, 16, 00, 01, DateTimeKind.Utc)), Is.True);
            Assert.That(sched.IsNewSession(
                new DateTime(2012, 12, 18, 16, 00, 01, DateTimeKind.Utc),
                new DateTime(2012, 12, 18, 16, 00, 02, DateTimeKind.Utc)), Is.False);

            // ==========
            // Time zone during Daylight savings
            settings = new SettingsDictionary();
            settings.SetString(SessionSettings.START_TIME, "05:30:00"); // 09:30:00 utc
            settings.SetString(SessionSettings.END_TIME, "12:00:00");   // 16:00:00 utc
            settings.SetString(SessionSettings.TIME_ZONE, EasternStandardTimeZoneId); //-4 during dst
            sched = new SessionSchedule(settings);

            Assert.That(sched.IsNewSession(
                new DateTime(2012, 06, 18, 15, 59, 58, DateTimeKind.Utc),
                new DateTime(2012, 06, 18, 15, 59, 59, DateTimeKind.Utc)), Is.False);
            Assert.That(sched.IsNewSession(
                new DateTime(2012, 06, 18, 15, 59, 59, DateTimeKind.Utc),
                new DateTime(2012, 06, 18, 16, 00, 00, DateTimeKind.Utc)), Is.False);
            Assert.That(sched.IsNewSession(
                new DateTime(2012, 06, 18, 16, 00, 00, DateTimeKind.Utc),
                new DateTime(2012, 06, 18, 16, 00, 01, DateTimeKind.Utc)), Is.True);
            Assert.That(sched.IsNewSession(
                new DateTime(2012, 06, 18, 16, 00, 01, DateTimeKind.Utc),
                new DateTime(2012, 06, 18, 16, 00, 02, DateTimeKind.Utc)), Is.False);
        }

        [Test]
        public void TestAdjustDateTime()
        {
            SettingsDictionary settings = new SettingsDictionary();
            settings.SetString(SessionSettings.START_TIME, "09:30:00");
            settings.SetString(SessionSettings.END_TIME, "16:00:00");
            SessionSchedule sched = new SessionSchedule(settings);

            // 1) reject if param is not utc
            Assert.Throws<ArgumentException>(
                delegate { sched.AdjustUtcDateTime(new DateTime(2012, 10, 18, 12, 00, 00, DateTimeKind.Local)); });
            Assert.Throws<ArgumentException>(
                delegate { sched.AdjustUtcDateTime(new DateTime(2012, 10, 18, 12, 00, 00, DateTimeKind.Unspecified)); });

            // 2) if settings is UTC, don't convert
            DateTime d2 = new DateTime(2013, 01, 15, 12, 00, 00, DateTimeKind.Utc);
            Assert.That(sched.AdjustUtcDateTime(d2), Is.EqualTo(d2));

            // 3) if settings has a TimeZone, convert to TimeZone
            settings = new SettingsDictionary();
            settings.SetString(SessionSettings.START_TIME, "04:30:00"); // 09:30:00 utc
            settings.SetString(SessionSettings.END_TIME, "11:00:00");   // 16:00:00 utc
            settings.SetString(SessionSettings.TIME_ZONE, EasternStandardTimeZoneId); //-5
            sched = new SessionSchedule(settings);

            DateTime d3 = new DateTime(2013, 01, 15, 03, 00, 00, DateTimeKind.Utc);
            DateTime d3expected = new DateTime(2013, 01, 14, 22, 00, 00, DateTimeKind.Unspecified);
            DateTime d3actual = sched.AdjustUtcDateTime(d3);
            Util.UtcDateTimeSerializerTests.AssertHackyDateTimeEquality(d3expected, d3actual);

            //    now with Pacific Standard Time
            settings = new SettingsDictionary();
            settings.SetString(SessionSettings.START_TIME, "04:30:00"); // 09:30:00 utc
            settings.SetString(SessionSettings.END_TIME, "11:00:00");   // 16:00:00 utc
            settings.SetString(SessionSettings.TIME_ZONE, PacificStandardTimeZoneId); //-5
            sched = new SessionSchedule(settings);

            d3 = new DateTime(2013, 01, 15, 03, 00, 00, DateTimeKind.Utc);
            d3expected = new DateTime(2013, 01, 14, 19, 00, 00, DateTimeKind.Unspecified);
            d3actual = sched.AdjustUtcDateTime(d3);
            Util.UtcDateTimeSerializerTests.AssertHackyDateTimeEquality(d3expected, d3actual);

            // 4) TimeZone with daylight savings
            settings = new SettingsDictionary();
            settings.SetString(SessionSettings.START_TIME, "04:30:00"); // 09:30:00 utc
            settings.SetString(SessionSettings.END_TIME, "11:00:00");   // 16:00:00 utc
            settings.SetString(SessionSettings.TIME_ZONE, EasternStandardTimeZoneId); //-4 in June
            sched = new SessionSchedule(settings);

            DateTime d4 = new DateTime(2013, 06, 15, 03, 00, 00, DateTimeKind.Utc);
            DateTime d4expected = new DateTime(2013, 06, 14, 23, 00, 00, DateTimeKind.Unspecified);
            DateTime d4actual = sched.AdjustUtcDateTime(d4);
            Util.UtcDateTimeSerializerTests.AssertHackyDateTimeEquality(d4expected, d4actual);

            // 5) if settings has UseLocalTime, convert to local time
            settings = new SettingsDictionary();
            settings.SetString(SessionSettings.START_TIME, "04:30:00"); // 09:30:00 utc
            settings.SetString(SessionSettings.END_TIME, "11:00:00");   // 16:00:00 utc
            settings.SetString(SessionSettings.USE_LOCAL_TIME, "Y");
            sched = new SessionSchedule(settings);

            DateTime d5 = new DateTime(2013, 01, 15, 16, 00, 00, DateTimeKind.Utc);
            DateTime d5expected = d5.ToLocalTime();
            DateTime d5actual = sched.AdjustUtcDateTime(d5);
            Util.UtcDateTimeSerializerTests.AssertHackyDateTimeEquality(d5expected, d5actual);
            Assert.That(d5actual.Kind, Is.EqualTo(DateTimeKind.Local));
        }

        [Test]
        public void TestWeekdaysSession()
        {
            SettingsDictionary settings = new SettingsDictionary();
            settings.SetString(SessionSettings.WEEKDAYS, "Tue,Th");
            settings.SetString(SessionSettings.START_TIME, "08:00:00");
            settings.SetString(SessionSettings.END_TIME, "17:00:00");

            SessionSchedule sched = new SessionSchedule(settings);

            // bounds of Tuesday
            Assert.That(sched.IsSessionTime(new DateTime(2024, 05, 21, 07, 59, 59, DateTimeKind.Utc)), Is.False);
            Assert.That(sched.IsSessionTime(new DateTime(2024, 05, 21, 08, 00, 00, DateTimeKind.Utc)), Is.True);
            Assert.That(sched.IsSessionTime(new DateTime(2024, 05, 21, 08, 00, 10, DateTimeKind.Utc)), Is.True);
            Assert.That(sched.IsSessionTime(new DateTime(2024, 05, 21, 16, 59, 59, DateTimeKind.Utc)), Is.True);
            Assert.That(sched.IsSessionTime(new DateTime(2024, 05, 21, 17, 00, 00, DateTimeKind.Utc)), Is.False);
            Assert.That(sched.IsSessionTime(new DateTime(2024, 05, 21, 17, 00, 10, DateTimeKind.Utc)), Is.False);

            // bounds of Thursday
            Assert.That(sched.IsSessionTime(new DateTime(2024, 05, 23, 07, 59, 59, DateTimeKind.Utc)), Is.False);
            Assert.That(sched.IsSessionTime(new DateTime(2024, 05, 23, 08, 00, 00, DateTimeKind.Utc)), Is.True);
            Assert.That(sched.IsSessionTime(new DateTime(2024, 05, 23, 08, 00, 10, DateTimeKind.Utc)), Is.True);
            Assert.That(sched.IsSessionTime(new DateTime(2024, 05, 23, 16, 59, 59, DateTimeKind.Utc)), Is.True);
            Assert.That(sched.IsSessionTime(new DateTime(2024, 05, 23, 17, 00, 00, DateTimeKind.Utc)), Is.False);
            Assert.That(sched.IsSessionTime(new DateTime(2024, 05, 23, 17, 00, 10, DateTimeKind.Utc)), Is.False);
        }

        [Test]
        public void TestWeekdaysSession_ThatCrossesMidnight()
        {
            SettingsDictionary settings = new SettingsDictionary();
            settings.SetString(SessionSettings.WEEKDAYS, "Tue,Th");
            settings.SetString(SessionSettings.START_TIME, "17:00:00");
            settings.SetString(SessionSettings.END_TIME, "11:00:00");

            SessionSchedule sched = new SessionSchedule(settings);

            // bounds of Tuesday-Wednesday
            Assert.That(sched.IsSessionTime(new DateTime(2024, 05, 21, 16, 59, 59, DateTimeKind.Utc)), Is.False);
            Assert.That(sched.IsSessionTime(new DateTime(2024, 05, 21, 17, 00, 00, DateTimeKind.Utc)), Is.True);
            Assert.That(sched.IsSessionTime(new DateTime(2024, 05, 21, 17, 00, 10, DateTimeKind.Utc)), Is.True);
            Assert.That(sched.IsSessionTime(new DateTime(2024, 05, 22, 10, 59, 59, DateTimeKind.Utc)), Is.True);
            Assert.That(sched.IsSessionTime(new DateTime(2024, 05, 22, 11, 00, 00, DateTimeKind.Utc)), Is.False);
            Assert.That(sched.IsSessionTime(new DateTime(2024, 05, 22, 11, 00, 10, DateTimeKind.Utc)), Is.False);

            // bounds of Thursday-Friday
            Assert.That(sched.IsSessionTime(new DateTime(2024, 05, 23, 16, 59, 59, DateTimeKind.Utc)), Is.False);
            Assert.That(sched.IsSessionTime(new DateTime(2024, 05, 23, 17, 00, 00, DateTimeKind.Utc)), Is.True);
            Assert.That(sched.IsSessionTime(new DateTime(2024, 05, 23, 17, 00, 10, DateTimeKind.Utc)), Is.True);
            Assert.That(sched.IsSessionTime(new DateTime(2024, 05, 24, 10, 59, 59, DateTimeKind.Utc)), Is.True);
            Assert.That(sched.IsSessionTime(new DateTime(2024, 05, 24, 11, 00, 00, DateTimeKind.Utc)), Is.False);
            Assert.That(sched.IsSessionTime(new DateTime(2024, 05, 24, 11, 00, 10, DateTimeKind.Utc)), Is.False);
        }
    }
}
