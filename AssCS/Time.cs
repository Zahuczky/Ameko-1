﻿using System;
using System.Collections.Generic;
using System.Text;

namespace AssCS
{
    /// <summary>
    /// Representation of a timestamp.
    /// This API supports millisecond percision, but note that
    /// precision will be dropped to centiseconds for export.
    /// </summary>
    public class Time : IAssComponent
    {
        private readonly TimeSpan localTime;

        public long Hours
        {
            get => localTime.Days * 24 + localTime.Hours;
        }

        public long Minutes
        {
            get => localTime.Minutes;
        }

        public long Seconds
        {
            get => localTime.Seconds;
        }

        public long Deciseconds
        {
            get => localTime.Milliseconds / 100;
        }

        public long Centiseconds
        {
            get => localTime.Milliseconds / 10;
        }

        public long Milliseconds
        {
            get => localTime.Milliseconds;
        }

        public double TotalSeconds
        {
            get => localTime.TotalSeconds;
        }

        public long TotalMilliseconds
        {
            get => (long)localTime.TotalMilliseconds;
        }

        public string AsAss()
        {
            return $"{Hours}:{Minutes:00}:{Seconds:00}.{Centiseconds:00}";
        }

        public string? AsOverride() => null;

        public string TextContent => AsAss();

        public Time()
        {
            localTime = TimeSpan.Zero;
        }

        public Time(Time t)
        {
            localTime = TimeSpan.FromMilliseconds(t.Milliseconds);
        }

        private Time(TimeSpan t)
        {
            localTime = t;
        }

        public static Time FromMillis(long millis) => new Time(TimeSpan.FromMilliseconds(millis));
        public static Time FromCentis(long centis) => new Time(TimeSpan.FromMilliseconds(centis * 10));
        public static Time FromDecis(long decis) => new Time(TimeSpan.FromMilliseconds(decis * 100));
        public static Time FromSeconds(long secs) => new Time(TimeSpan.FromSeconds(secs));
        public static Time FromTime(Time t) => new Time(t);

        /// <summary>
        /// Bootstrap a Time component from its representation in a file
        /// </summary>
        /// <param name="data">Line</param>
        public static Time FromAss(string data)
        {
            var splits = data.Split(':', '.');
            if (splits.Length != 4) throw new ArgumentException($"Time: {data} is an invalid timecode.");
            long millis = 0;
            int[] multiplier = { 3600 * 1000, 60 * 1000, 1000, 10 };
            for (int i = 0; i < splits.Length; i++)
            {
                millis += Convert.ToInt64(splits[i]) * multiplier[i];
            }
            return FromMillis(millis);
        }

        public override bool Equals(object? obj)
        {
            return obj is Time time &&
                   localTime.Equals(time.localTime);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(localTime);
        }

        public static Time operator +(Time a, Time b)
        {
            return new Time(a.localTime + b.localTime);
        }

        public static Time operator -(Time a, Time b)
        {
            return new Time(a.localTime - b.localTime);
        }

        public static bool operator >(Time a, Time b)
        {
            return a.localTime > b.localTime;
        }
        public static bool operator <(Time a, Time b)
        {
            return a.localTime < b.localTime;
        }
        public static bool operator >=(Time a, Time b)
        {
            return a.localTime >= b.localTime;
        }
        public static bool operator <=(Time a, Time b)
        {
            return a.localTime <= b.localTime;
        }
        public static bool operator ==(Time a, Time b)
        {
            return a.localTime == b.localTime;
        }
        public static bool operator !=(Time a, Time b)
        {
            return a.localTime != b.localTime;
        }
    }
}
