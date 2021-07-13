using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace ScheduledEvents
{
    public class IntervalScale
    {
        public static readonly IntervalScale HOURS =
            new IntervalScale(0, "fair.ScheduledEvents.Hours", GenDate.TicksPerHour);

        private static readonly IntervalScale DAYS =
            new IntervalScale(1, "fair.ScheduledEvents.Days", GenDate.TicksPerDay);

        private static readonly IntervalScale SEASONS =
            new IntervalScale(2, "fair.ScheduledEvents.Seasons", GenDate.TicksPerSeason);

        private static readonly IntervalScale YEARS =
            new IntervalScale(3, "fair.ScheduledEvents.Years", GenDate.TicksPerYear);

        private readonly int id;
        public readonly string label;
        public readonly int ticksPerUnit;

        private IntervalScale(int id, string label, int ticksPerUnit)
        {
            this.id = id;
            this.label = label;
            this.ticksPerUnit = ticksPerUnit;
        }

        public static IEnumerable<IntervalScale> Values
        {
            get
            {
                yield return HOURS;
                yield return DAYS;
                yield return SEASONS;
                yield return YEARS;
            }
        }

        // Custom save/load logic
        public static void Look(ref IntervalScale value, string label)
        {
            var id = value.id;
            Scribe_Values.Look(ref id, label, default, true);
            var found = Values.FirstOrDefault(scale => scale.id == id);
            if (found == null)
            {
                found = HOURS; // Default value
            }

            value = found;
        }
    }
}