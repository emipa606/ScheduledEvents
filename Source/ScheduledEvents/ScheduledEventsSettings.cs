using System.Collections.Generic;
using Verse;

namespace ScheduledEvents;

public class ScheduledEventsSettings : ModSettings
{
    public static bool logDebug;
    public static List<ScheduledEvent> events = new List<ScheduledEvent>();

    public override void ExposeData()
    {
        Scribe_Values.Look(ref logDebug, "logDebug",
            true); // TODO: Set this to false on release, make a setting for it?
        Utils.ScribeCustomList(ref events, "events", e =>
        {
            var incidentName = e.incidentName;
            var incidentTarget = e.incidentTarget;
            Scribe_Values.Look(ref incidentName, "incident");
            IncidentTarget.Look(ref incidentTarget, "incidentTarget");
            e.Scribe();
        }, () =>
        {
            string incidentName = null;
            IncidentTarget incidentTarget = null;
            Scribe_Values.Look(ref incidentName, "incident");
            IncidentTarget.Look(ref incidentTarget, "incidentTarget");
            if (incidentName == null || incidentTarget == null)
            {
                Utils.LogDebug("Found invalid incident in saved events");
                return null;
            }

            var e = new ScheduledEvent(incidentTarget, incidentName);
            e.Scribe();
            return e;
        }, this);
        events.RemoveAll(e => e == null); // Remove all nulls
        if (Scribe.mode == LoadSaveMode.Saving)
        {
            // We should try and find out game component and run an update on it
            if (Current.Game != null)
            {
                var comp = Current.Game.GetComponent<SEGameComponent>();
                comp?.ReloadEvents();
            }
        }

        base.ExposeData();
    }
}