using System.Collections.Generic;
using System.Linq;
using Mlie;
using UnityEngine;
using Verse;

namespace ScheduledEvents;

public class ScheduledEvents : Mod
{
    private static string currentVersion;
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
        var labelHeight = 25;
        var entryHeight = 30;
        var textWidth = 300;
        var entryWidth = 100;
        var scaleWidth = 100;

        if (currentVersion != null)
        {
            GUI.contentColor = Color.gray;
            Widgets.Label(new Rect(inRect.x + (inRect.width / 3 * 2), inRect.y, inRect.width / 3, 25f),
                "fair.ScheduledEvents.modversion".Translate(currentVersion));
            GUI.contentColor = Color.white;
        }

        var outRect = new Rect(0, 0, inRect.width, inRect.height);
        var rowHeight = labelHeight + 35 + ((entryHeight + 5) * 2) + 10;
        var scrollView = new Rect(0, 0, inRect.width - 20,
            10 + (rowHeight * ScheduledEventsSettings.events.Count) + 35);
        Widgets.BeginScrollView(outRect, ref settingsScrollPos, scrollView);
        for (var i = 0; i < ScheduledEventsSettings.events.Count; i++)
        {
            var e = ScheduledEventsSettings.events[i];
            var headerLabel = new Rect(0, y, textWidth, labelHeight);
            Widgets.Label(headerLabel,
                "fair.ScheduledEvents.SettingLabel".Translate(e.incidentName, e.incidentTarget.label.Translate()));

            if (e.incidentTarget.hasTargetSelector)
            {
                var selectorButton = new Rect(textWidth, y, 200, labelHeight);
                if (Widgets.ButtonText(selectorButton,
                        e.targetSelector.label.Translate(e.incidentTarget.label.Translate())))
                {
                    var list = new List<FloatMenuOption>();
                    foreach (var sel in TargetSelector.Values)
                    {
                        list.Add(new FloatMenuOption(sel.label.Translate(e.incidentTarget.label.Translate()),
                            delegate
                            {
                                e.targetSelector = sel;
                                base.WriteSettings();
                                base.DoSettingsWindowContents(scrollView); // Update button text
                            }));
                    }

                    Find.WindowStack.Add(new FloatMenu(list));
                }
            }

            y += labelHeight;

            Utils.DrawScaleSetting(0, y, textWidth, entryWidth, entryHeight, scaleWidth,
                "fair.ScheduledEvents.SettingRunEvery".Translate(), e.intervalScale.label.Translate(),
                ref e.interval, 1, scale =>
                {
                    e.intervalScale = scale;
                    base.WriteSettings();
                    base.DoSettingsWindowContents(scrollView); // Update button text
                });
            y += entryHeight + 5;

            Utils.DrawScaleSetting(0, y, textWidth, entryWidth, entryHeight, scaleWidth,
                "fair.ScheduledEvents.SettingOffsetBy".Translate(), e.offsetScale.label.Translate(), ref e.offset,
                0, scale =>
                {
                    e.offsetScale = scale;
                    base.WriteSettings();
                    base.DoSettingsWindowContents(scrollView); // Update button text
                });
            y += entryHeight + 5;

            var removeButton = new Rect(0, y, 200, 30);
            var oldColor = GUI.color;
            GUI.color = new Color(1f, 0.3f, 0.35f);
            if (Widgets.ButtonText(removeButton, "fair.ScheduledEvents.RemoveEvent".Translate()))
            {
                ScheduledEventsSettings.events.RemoveAt(i);
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
            if (Widgets.ButtonText(addButton, "fair.ScheduledEvents.AddEvent".Translate(target.label.Translate())))
            {
                var list = new List<FloatMenuOption>();

                //Utils.LogMessage("Events: " + DefDatabase<IncidentDef>.AllDefs.Count());

                foreach (var incident in from d in target.GetAllIncidentDefs() orderby d.defName select d)
                {
                    list.Add(new FloatMenuOption(incident.defName, delegate
                    {
                        ScheduledEventsSettings.events.Add(new ScheduledEvent(target, incident.defName));
                        Utils.LogDebug("Added scheduled " + target.label.Translate() + " event");
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
            ref ScheduledEventsSettings.logDebug);

        Widgets.EndScrollView();

        GUI.EndGroup();
        base.DoSettingsWindowContents(inRect);
    }

    public override string SettingsCategory()
    {
        return "fair.ScheduledEvents.Title".Translate();
    }
}