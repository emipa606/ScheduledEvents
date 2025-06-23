using System.Linq;
using RimWorld;
using Verse;

namespace ScheduledEvents;

public class ScheduledEvent(IncidentTarget target, string incidentName)
{
    // The reason we have to use name, is because when loading this, incident defs are not yet loaded.
    public readonly string IncidentName = incidentName;
    public readonly IncidentTarget IncidentTarget = target;
    private bool enabled = true; // If the event is enabled

    public int interval = 1; // The interval of which the events occur
    public IntervalScale IntervalScale = IntervalScale.Hours; // The interval scale

    public int Offset; // The offset before the events start
    public IntervalScale OffsetScale = IntervalScale.Hours; // The offset scale
    public TargetSelector TargetSelector = TargetSelector.Every;

    public IncidentDef GetIncident()
    {
        return IncidentName == null
            ? null
            : DefDatabase<IncidentDef>.AllDefs.FirstOrDefault(e => e.defName.Equals(IncidentName));
    }

    public int GetNextEventTick(int currentTick)
    {
        var intervalInTicks = IntervalScale.TicksPerUnit * interval;
        if (intervalInTicks <= 0)
        {
            return -1;
        }

        var offsetTicks = OffsetScale.TicksPerUnit * Offset;
        if (currentTick < offsetTicks)
        {
            currentTick = offsetTicks;
        }

        var nextTick = currentTick - (currentTick % intervalInTicks) + intervalInTicks;
        nextTick += offsetTicks % intervalInTicks;

        return nextTick;
    }

    public void Scribe()
    {
        Scribe_Values.Look(ref enabled, "enabled", false, true);
        TargetSelector.Look(ref TargetSelector, "targetSelector");
        Scribe_Values.Look(ref interval, "interval", 0, true);
        IntervalScale.Look(ref IntervalScale, "intervalScale");
        Scribe_Values.Look(ref Offset, "offset", 0, true);
        IntervalScale.Look(ref OffsetScale, "offsetScale");
    }
}