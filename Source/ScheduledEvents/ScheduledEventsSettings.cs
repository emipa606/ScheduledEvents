using System.Collections.Generic;
using Verse;

namespace ScheduledEvents;

public class ScheduledEventsSettings : ModSettings
{
    public static bool LOGDebug;
    public static List<ScheduledEvent> Events = [];

    public override void ExposeData()
    {
        Scribe_Values.Look(ref LOGDebug, "logDebug");
        Utils.ScribeCustomList(ref Events, "events", e =>
        {
            var incidentName = e?.IncidentName;
            var incidentTarget = e?.IncidentTarget;
            Scribe_Values.Look(ref incidentName, "incident");
            IncidentTarget.Look(ref incidentTarget, "incidentTarget");
            e?.Scribe();
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
        Events.RemoveAll(e => e == null); // Remove all nulls
        if (Scribe.mode == LoadSaveMode.Saving && Current.Game != null)
        {
            // We should try and find out game component and run an update on it
            var comp = Current.Game.GetComponent<SEGameComponent>();
            comp?.ReloadEvents();
        }

        base.ExposeData();
    }
}