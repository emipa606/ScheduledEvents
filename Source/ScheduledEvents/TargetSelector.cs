using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace ScheduledEvents;

public class TargetSelector
{
    public static readonly TargetSelector Every = new(0, "fair.ScheduledEvents.SelEvery",
        (targets, action) =>
        {
            foreach (var target in targets)
            {
                action(target);
            }
        });

    private static readonly TargetSelector randomOne = new(1, "fair.ScheduledEvents.SelRandomOne",
        (targets, action) =>
        {
            var incidentTargets = targets as IIncidentTarget[] ?? targets.ToArray();
            if (!incidentTargets.Any())
            {
                return;
            }

            var target = incidentTargets.RandomElementWithFallback();
            if (target != null)
            {
                action(target);
            }
        });

    private static readonly TargetSelector first = new(2, "fair.ScheduledEvents.SelFirst",
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
    public readonly string Label;

    private TargetSelector(int id, string label,
        Action<IEnumerable<IIncidentTarget>, Action<IIncidentTarget>> action)
    {
        this.id = id;
        Label = label;
        this.action = action;
    }

    public static IEnumerable<TargetSelector> Values
    {
        get
        {
            yield return Every;
            yield return randomOne;
            yield return first;
        }
    }

    // Custom save/load logic
    public static void Look(ref TargetSelector value, string label)
    {
        var id = value.id;
        Scribe_Values.Look(ref id, label, 0, true);
        var found = Values.FirstOrDefault(sel => sel.id == id);
        if (found == null)
        {
            found = Every; // Default value
        }

        value = found;
    }

    public void RunOn(IEnumerable<IIncidentTarget> targets, Action<IIncidentTarget> incidentTarget)
    {
        action(targets, incidentTarget);
    }
}