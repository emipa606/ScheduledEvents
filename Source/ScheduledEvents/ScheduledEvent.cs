using System.Linq;
using RimWorld;
using Verse;

namespace ScheduledEvents;

public class ScheduledEvent
{
    // The reason we have to use name, is because when loading this, incident defs are not yet loaded.
    public readonly string incidentName;
    public readonly IncidentTarget incidentTarget;
    private bool enabled = true; // If the event is enabled

    public int interval = 1; // The interval of which the events occur
    public IntervalScale intervalScale = IntervalScale.HOURS; // The interval scale

    public int offset; // The offset before the events start
    public IntervalScale offsetScale = IntervalScale.HOURS; // The offset scale
    public TargetSelector targetSelector = TargetSelector.EVERY;

    public ScheduledEvent(IncidentTarget target, string incidentName)
    {
        incidentTarget = target;
        this.incidentName = incidentName;
    }

    public IncidentDef GetIncident()
    {
        return incidentName == null
            ? null
            : DefDatabase<IncidentDef>.AllDefs.FirstOrDefault(e => e.defName.Equals(incidentName));
    }

    public int GetNextEventTick(int currentTick)
    {
        var intervalInTicks = intervalScale.ticksPerUnit * interval;
        if (intervalInTicks <= 0)
        {
            return -1;
        }

        var offsetTicks = offsetScale.ticksPerUnit * offset;
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
        Scribe_Values.Look(ref enabled, "enabled", default, true);
        TargetSelector.Look(ref targetSelector, "targetSelector");
        Scribe_Values.Look(ref interval, "interval", default, true);
        IntervalScale.Look(ref intervalScale, "intervalScale");
        Scribe_Values.Look(ref offset, "offset", default, true);
        IntervalScale.Look(ref offsetScale, "offsetScale");
    }
}