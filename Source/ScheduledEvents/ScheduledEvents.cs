using System.Collections.Generic;
using System.Linq;
using Mlie;
using RimWorld;
using UnityEngine;
using Verse;

namespace ScheduledEvents;

public class ScheduledEvents : Mod
{
    private static string currentVersion;
    private static bool sortByTime;
    private Vector2 settingsScrollPos;

    public ScheduledEvents(ModContentPack content) : base(content)
    {
        // Load settings?
        GetSettings<ScheduledEventsSettings>();
        currentVersion =
            VersionFromManifest.GetVersionFromModMetaData(content.ModMetaData);
    }

    public override void DoSettingsWindowContents(Rect inRect)
    {
        GUI.BeginGroup(inRect);

        var y = 10;
        const int labelHeight = 25;
        const int entryHeight = 30;
        const int textWidth = 300;
        const int entryWidth = 100;
        const int scaleWidth = 100;

        if (currentVersion != null)
        {
            GUI.contentColor = Color.gray;
            Widgets.Label(new Rect(inRect.x + (inRect.width / 3 * 2), inRect.y, inRect.width / 3, 25f),
                "fair.ScheduledEvents.modversion".Translate(currentVersion));
            GUI.contentColor = Color.white;
        }

        var outRect = new Rect(0, 0, inRect.width, inRect.height);
        const int rowHeight = labelHeight + 35 + ((entryHeight + 5) * 2) + 10;
        var scrollView = new Rect(0, 0, inRect.width - 20,
            10 + (rowHeight * ScheduledEventsSettings.Events.Count) + 35);
        Widgets.BeginScrollView(outRect, ref settingsScrollPos, scrollView);
        if (Widgets.RadioButtonLabeled(new Rect(0, y, inRect.width / 3, 30), "fair.ScheduledEvents.Name".Translate(),
                !sortByTime))
        {
            sortByTime = false;
        }

        if (Widgets.RadioButtonLabeled(new Rect(inRect.width / 2, y, inRect.width / 3, 30),
                "fair.ScheduledEvents.Time".Translate(), sortByTime, Current.Game == null))
        {
            sortByTime = true;
        }

        List<ScheduledEvent> sortedEvents;
        if (sortByTime && Current.Game != null)
        {
            sortedEvents = ScheduledEventsSettings.Events
                .OrderBy(scheduledEvent => scheduledEvent.GetNextEventTick(GenTicks.TicksGame))
                .ToList();
        }
        else
        {
            sortedEvents = ScheduledEventsSettings.Events.OrderBy(scheduledEvent => scheduledEvent.IncidentName)
                .ToList();
        }

        y += labelHeight * 2;
        for (var i = 0; i < sortedEvents.Count; i++)
        {
            var e = sortedEvents[i];
            var headerLabel = new Rect(0, y, textWidth, labelHeight);
            Widgets.Label(headerLabel,
                "fair.ScheduledEvents.SettingLabel".Translate(e.IncidentName, e.IncidentTarget.Label.Translate()));
            if (Current.Game != null)
            {
                Widgets.Label(new Rect(inRect.width / 3 * 2, y, inRect.width / 4, labelHeight),
                    "fair.ScheduledEvents.NextEventIn".Translate(
                        (e.GetNextEventTick(GenTicks.TicksGame) - GenTicks.TicksGame).ToStringTicksToPeriod()));
            }

            if (e.IncidentTarget.HasTargetSelector)
            {
                var selectorButton = new Rect(textWidth, y, 200, labelHeight);
                if (Widgets.ButtonText(selectorButton,
                        e.TargetSelector.Label.Translate(e.IncidentTarget.Label.Translate())))
                {
                    var list = new List<FloatMenuOption>();
                    foreach (var sel in TargetSelector.Values)
                    {
                        list.Add(new FloatMenuOption(sel.Label.Translate(e.IncidentTarget.Label.Translate()),
                            delegate
                            {
                                e.TargetSelector = sel;
                                base.WriteSettings();
                                base.DoSettingsWindowContents(scrollView); // Update button text
                            }));
                    }

                    Find.WindowStack.Add(new FloatMenu(list));
                }
            }

            y += labelHeight;

            Utils.DrawScaleSetting(0, y, textWidth, entryWidth, entryHeight, scaleWidth,
                "fair.ScheduledEvents.SettingRunEvery".Translate(), e.IntervalScale.Label.Translate(),
                ref e.interval, 1, scale =>
                {
                    e.IntervalScale = scale;
                    base.WriteSettings();
                    base.DoSettingsWindowContents(scrollView); // Update button text
                });
            y += entryHeight + 5;

            Utils.DrawScaleSetting(0, y, textWidth, entryWidth, entryHeight, scaleWidth,
                "fair.ScheduledEvents.SettingOffsetBy".Translate(), e.OffsetScale.Label.Translate(), ref e.Offset,
                0, scale =>
                {
                    e.OffsetScale = scale;
                    base.WriteSettings();
                    base.DoSettingsWindowContents(scrollView); // Update button text
                });
            y += entryHeight + 5;

            var removeButton = new Rect(0, y, 200, 30);
            var oldColor = GUI.color;
            GUI.color = new Color(1f, 0.3f, 0.35f);
            if (Widgets.ButtonText(removeButton, "fair.ScheduledEvents.RemoveEvent".Translate()))
            {
                ScheduledEventsSettings.Events.RemoveAt(i);
                base.WriteSettings();
                base.DoSettingsWindowContents(scrollView); // Update window
            }

            GUI.color = oldColor;
            y += 35;

            Widgets.DrawLineHorizontal(0, y, scrollView.width);
            y += 10;
        }

        var addButtonX = 0;

        foreach (var target in IncidentTarget.Values)
        {
            var addButton = new Rect(addButtonX, y, 200, 30);
            if (Widgets.ButtonText(addButton, "fair.ScheduledEvents.AddEvent".Translate(target.Label.Translate())))
            {
                var list = new List<FloatMenuOption>();

                //Utils.LogMessage("Events: " + DefDatabase<IncidentDef>.AllDefs.Count());

                foreach (var incident in from d in target.GetAllIncidentDefs() orderby d.defName select d)
                {
                    list.Add(new FloatMenuOption(incident.defName, delegate
                    {
                        ScheduledEventsSettings.Events.Add(new ScheduledEvent(target, incident.defName));
                        Utils.LogDebug("Added scheduled " + target.Label.Translate() + " event");
                        base.WriteSettings();
                        base.DoSettingsWindowContents(inRect); // Update window contents
                    }));
                }

                Find.WindowStack.Add(new FloatMenu(list));
            }

            addButtonX += 205;
        }

        var debugLogging = new Rect(addButtonX, y, 200, 30);
        Widgets.CheckboxLabeled(debugLogging, "fair.ScheduledEvents.DebugLogging".Translate(),
            ref ScheduledEventsSettings.LOGDebug);

        Widgets.EndScrollView();

        GUI.EndGroup();
        base.DoSettingsWindowContents(inRect);
    }

    public override string SettingsCategory()
    {
        return "fair.ScheduledEvents.Title".Translate();
    }
}