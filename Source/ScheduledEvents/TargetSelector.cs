using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace ScheduledEvents;

public class TargetSelector
{
    public static readonly TargetSelector EVERY = new TargetSelector(0, "fair.ScheduledEvents.SelEvery",
        (targets, action) =>
        {
            foreach (var target in targets)
            {
                action(target);
            }
        });

    private static readonly TargetSelector RANDOM_ONE = new TargetSelector(1, "fair.ScheduledEvents.SelRandomOne",
        (targets, action) =>
        {
            if (!targets.Any())
            {
                return;
            }

            var target = targets.RandomElementWithFallback();
            if (target != null)
            {
                action(target);
            }
        });

    private static readonly TargetSelector FIRST = new TargetSelector(2, "fair.ScheduledEvents.SelFirst",
        (targets, action) =>
        {
            var target = targets.FirstOrFallback();
            if (target != null)
            {
                action(target);
            }
        });

    private readonly Action<IEnumerable<IIncidentTarget>, Action<IIncidentTarget>> action;

    private readonly int id;
    public readonly string label;

    private TargetSelector(int id, string label,
        Action<IEnumerable<IIncidentTarget>, Action<IIncidentTarget>> action)
    {
        this.id = id;
        this.label = label;
        this.action = action;
    }

    public static IEnumerable<TargetSelector> Values
    {
        get
        {
            yield return EVERY;
            yield return RANDOM_ONE;
            yield return FIRST;
        }
    }

    // Custom save/load logic
    public static void Look(ref TargetSelector value, string label)
    {
        var id = value.id;
        Scribe_Values.Look(ref id, label, default, true);
        var found = Values.FirstOrDefault(sel => sel.id == id);
        if (found == null)
        {
            found = EVERY; // Default value
        }

        value = found;
    }

    public void RunOn(IEnumerable<IIncidentTarget> targets, Action<IIncidentTarget> incidentTarget)
    {
        action(targets, incidentTarget);
    }
}