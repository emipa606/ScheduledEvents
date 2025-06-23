using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace ScheduledEvents;

public class IncidentTarget
{
    private static readonly IncidentTarget map = new(0, "fair.ScheduledEvents.MapEvent",
        "Map_PlayerHome", true, _ =>
        {
            return Find.Maps.Select(m => (IIncidentTarget)m);
            //return Enumerable.Repeat<IIncidentTarget>(Find.AnyPlayerHomeMap, 1);
        });

    private static readonly IncidentTarget world = new(1, "fair.ScheduledEvents.WorldEvent", "World",
        false, _ => Enumerable.Repeat<IIncidentTarget>(Find.World, 1));

    private static readonly IncidentTarget caravan = new(2, "fair.ScheduledEvents.CaravanEvent",
        "Caravan", true, _ => { return Find.WorldObjects.Caravans.Select(IIncidentTarget (c) => c); });

    public readonly bool HasTargetSelector;

    private readonly int id;
    public readonly string Label;
    private readonly string targetDefName;
    private readonly Func<ScheduledEvent, IEnumerable<IIncidentTarget>> targetGetter;

    private IncidentTarget(int id, string label, string targetDefName, bool hasTargetSelector,
        Func<ScheduledEvent, IEnumerable<IIncidentTarget>> targetGetter)
    {
        this.id = id;
        Label = label;
        this.targetDefName = targetDefName;
        this.targetGetter = targetGetter;
        HasTargetSelector = hasTargetSelector;
    }

    public static IEnumerable<IncidentTarget> Values
    {
        get
        {
            yield return map;
            yield return world;
            yield return caravan;
        }
    }

    // Custom save/load logic
    public static void Look(ref IncidentTarget value, string label)
    {
        var id = value?.id ?? -1;
        Scribe_Values.Look(ref id, label, 0, true);
        var found = Values.FirstOrDefault(target => target.id == id);
        value = found;
    }

    private IncidentTargetTagDef getTargetTag()
    {
        return DefDatabase<IncidentTargetTagDef>.AllDefs.FirstOrDefault(e => e.defName.Equals(targetDefName));
    }

    public IEnumerable<IncidentDef> GetAllIncidentDefs()
    {
        var targetDef = getTargetTag();
        return targetDef == null
            ? []
            : DefDatabase<IncidentDef>.AllDefs.Where(e => e.TargetTagAllowed(targetDef));
    }

    public IEnumerable<IIncidentTarget> GetCurrentTarget(ScheduledEvent e)
    {
        return targetGetter(e);
    }
}