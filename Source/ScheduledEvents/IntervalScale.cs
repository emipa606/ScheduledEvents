using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace ScheduledEvents;

public class IntervalScale
{
    public static readonly IntervalScale Hours = new(0, "fair.ScheduledEvents.Hours", GenDate.TicksPerHour);

    private static readonly IntervalScale days = new(1, "fair.ScheduledEvents.Days", GenDate.TicksPerDay);

    private static readonly IntervalScale seasons = new(2, "fair.ScheduledEvents.Seasons", GenDate.TicksPerSeason);

    private static readonly IntervalScale years = new(3, "fair.ScheduledEvents.Years", GenDate.TicksPerYear);

    private readonly int id;
    public readonly string Label;
    public readonly int TicksPerUnit;

    private IntervalScale(int id, string label, int ticksPerUnit)
    {
        this.id = id;
        Label = label;
        TicksPerUnit = ticksPerUnit;
    }

    public static IEnumerable<IntervalScale> Values
    {
        get
        {
            yield return Hours;
            yield return days;
            yield return seasons;
            yield return years;
        }
    }

    // Custom save/load logic
    public static void Look(ref IntervalScale value, string label)
    {
        var id = value.id;
        Scribe_Values.Look(ref id, label, 0, true);
        var found = Values.FirstOrDefault(scale => scale.id == id) ?? Hours; // Default value

        value = found;
    }
}