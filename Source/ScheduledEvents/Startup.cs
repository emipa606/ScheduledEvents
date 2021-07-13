using Verse;

namespace ScheduledEvents
{
    [StaticConstructorOnStartup]
    public static class Startup
    {
        static Startup()
        {
            Utils.LogMessage("Loaded " + ScheduledEventsSettings.events.Count + " events from settings.");
        }
    }
}