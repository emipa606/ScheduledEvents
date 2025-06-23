using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace ScheduledEvents;

public class SEGameComponent(Game game) : GameComponent
{
    private readonly List<TickEvent> events = [];

    private readonly List<NextEvent> nextEvents = [];

    public override void FinalizeInit()
    {
        // For some reason, currentTick during this is not correct
        ReloadEvents();
        base.FinalizeInit();
    }

    public void ReloadEvents()
    {
        events.Clear();
        var currentTick = game.tickManager.TicksAbs;
        Utils.LogDebug("Loading scheduled events...");
        foreach (var e in ScheduledEventsSettings.Events)
        {
            var nextEventTick = e.GetNextEventTick(currentTick);
            if (nextEventTick <= 0)
            {
                Utils.LogDebug($"{e.IncidentName} event has invalid next tick");
                continue;
            }

            Utils.LogDebug(
                $"Event {e.IncidentName} will happen on {GenDate.HourOfDay(nextEventTick, 0)}h, {GenDate.DateFullStringAt(nextEventTick, Vector2.zero)}");
            TickEvent.AddToList(events, nextEventTick, e);
        }
    }

    public override void GameComponentTick()
    {
        var currentTick = game.tickManager.TicksAbs;
        var nextEvent = events.FirstOrDefault();
        if (nextEvent != null && nextEvent.Tick <= currentTick)
        {
            Utils.LogDebug($"Firing scheduled {nextEvent.E.IncidentName} event!");
            // Remove from list
            events.Remove(nextEvent);
            var nextEventTick = nextEvent.E.GetNextEventTick(currentTick);

            var incident = nextEvent.E.GetIncident();
            if (incident != null)
            {
                var targets = nextEvent.E.IncidentTarget.GetCurrentTarget(nextEvent.E);
                var incidentTargets = targets as IIncidentTarget[] ?? targets.ToArray();
                if (incidentTargets.Any())
                {
                    nextEvent.E.TargetSelector.RunOn(incidentTargets,
                        target => nextEvents.Add(new NextEvent(incident, target)));
                }
                else
                {
                    Utils.LogDebugWarning("Event found 0 targets");
                }
            }
            else
            {
                Utils.LogWarning("Could not fire event, since it could not find an IncidentDef");
            }

            Utils.LogDebug(
                $"Next event will happen on {GenDate.HourOfDay(nextEventTick, 0)}h, {GenDate.DateFullStringAt(nextEventTick, Vector2.zero)}");
            //Utils.LogDebug($"Hours until: {(nextEventTick - currentTick) / GenDate.TicksPerHour}");
            TickEvent.AddToList(events, nextEventTick, nextEvent.E);
        }

        if (nextEvents.Count > 0)
        {
            nextEvents.First().Execute();
            nextEvents.RemoveAt(0);
        }

        //Current.Game.storyteller.TryFire()
        base.GameComponentTick();
    }

    private class NextEvent(IncidentDef incident, IIncidentTarget target)
    {
        public void Execute()
        {
            if (incident.TargetAllowed(target))
            {
                // This is basically taken from Dialog_DebugActionMenu (debug menu source)
                var parms = StorytellerUtility.DefaultParmsNow(incident.category, target);
                if (incident.pointsScaleable)
                {
                    var stComp = Find.Storyteller.storytellerComps.First(x =>
                        x is StorytellerComp_OnOffCycle or StorytellerComp_RandomMain);
                    parms = stComp.GenerateParms(incident.category, parms.target);
                }

                incident.Worker.TryExecute(parms);
            }
            else
            {
                Utils.LogDebugWarning("Event target was invalid");
            }
        }
    }
}