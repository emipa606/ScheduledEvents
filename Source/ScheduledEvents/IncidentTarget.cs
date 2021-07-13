using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace ScheduledEvents
{
    public class IncidentTarget
    {
        private static readonly IncidentTarget MAP = new IncidentTarget(0, "fair.ScheduledEvents.MapEvent",
            "Map_PlayerHome", true, _ =>
            {
                return Find.Maps.Select(m => (IIncidentTarget) m);
                //return Enumerable.Repeat<IIncidentTarget>(Find.AnyPlayerHomeMap, 1);
            });

        private static readonly IncidentTarget WORLD = new IncidentTarget(1, "fair.ScheduledEvents.WorldEvent", "World",
            false, _ => Enumerable.Repeat<IIncidentTarget>(Find.World, 1));

        private static readonly IncidentTarget CARAVAN = new IncidentTarget(2, "fair.ScheduledEvents.CaravanEvent",
            "Caravan", true, _ => { return Find.WorldObjects.Caravans.Select(c => (IIncidentTarget) c); });

        public readonly bool hasTargetSelector;

        private readonly int id;
        public readonly string label;
        private readonly string targetDefName;
        private readonly Func<ScheduledEvent, IEnumerable<IIncidentTarget>> targetGetter;

        private IncidentTarget(int id, string label, string targetDefName, bool hasTargetSelector,
            Func<ScheduledEvent, IEnumerable<IIncidentTarget>> targetGetter)
        {
            this.id = id;
            this.label = label;
            this.targetDefName = targetDefName;
            this.targetGetter = targetGetter;
            this.hasTargetSelector = hasTargetSelector;
        }

        public static IEnumerable<IncidentTarget> Values
        {
            get
            {
                yield return MAP;
                yield return WORLD;
                yield return CARAVAN;
            }
        }

        // Custom save/load logic
        public static void Look(ref IncidentTarget value, string label)
        {
            var id = value?.id ?? -1;
            Scribe_Values.Look(ref id, label, default, true);
            var found = Values.FirstOrDefault(target => target.id == id);
            value = found;
        }

        private IncidentTargetTagDef GetTargetTag()
        {
            return DefDatabase<IncidentTargetTagDef>.AllDefs.FirstOrDefault(e => e.defName.Equals(targetDefName));
        }

        public IEnumerable<IncidentDef> GetAllIncidentDefs()
        {
            var targetDef = GetTargetTag();
            if (targetDef == null)
            {
                return Enumerable.Empty<IncidentDef>();
            }

            return DefDatabase<IncidentDef>.AllDefs.Where(e => e.TargetTagAllowed(targetDef));
        }

        public IEnumerable<IIncidentTarget> GetCurrentTarget(ScheduledEvent e)
        {
            return targetGetter(e);
        }
    }
}