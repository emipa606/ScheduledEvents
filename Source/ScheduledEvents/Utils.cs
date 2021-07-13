using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using Verse;

namespace ScheduledEvents
{
    public static class Utils
    {
        public static void LogMessage(string message)
        {
            Log.Message("[ScheduledEvents]: " + message);
        }

        public static void LogWarning(string message)
        {
            Log.Warning("[ScheduledEvents]: " + message);
        }

        public static void LogDebug(string message)
        {
            if (ScheduledEventsSettings.logDebug)
            {
                Log.Message("[ScheduledEvents DEBUG]: " + message);
            }
        }

        public static void LogDebugWarning(string message)
        {
            if (ScheduledEventsSettings.logDebug)
            {
                Log.Warning("[ScheduledEvents DEBUG]: " + message);
            }
        }

        public static void ScribeCustomList<T>(ref List<T> list, string label, Action<T> saver, Func<T> loader,
            IExposable caller)
        {
            Scribe.EnterNode("events");
            try
            {
                if (Scribe.mode == LoadSaveMode.Saving)
                {
                    foreach (var e in list)
                    {
                        Scribe.EnterNode("li");
                        try
                        {
                            saver(e);
                        }
                        finally
                        {
                            Scribe.ExitNode();
                        }
                    }
                }
                else if (Scribe.mode == LoadSaveMode.LoadingVars)
                {
                    var curXmlParent = Scribe.loader.curXmlParent;
                    list = new List<T>();
                    foreach (var obj in curXmlParent.ChildNodes)
                    {
                        var subNode = (XmlNode) obj;
                        var oldXmlParent = Scribe.loader.curXmlParent;
                        var oldParent = Scribe.loader.curParent;
                        var oldPathRelToParent = Scribe.loader.curPathRelToParent;
                        Scribe.loader.curPathRelToParent = null;
                        Scribe.loader.curParent = caller;
                        Scribe.loader.curXmlParent = subNode;
                        try
                        {
                            list.Add(loader());
                        }
                        finally
                        {
                            Scribe.loader.curXmlParent = oldXmlParent;
                            Scribe.loader.curParent = oldParent;
                            Scribe.loader.curPathRelToParent = oldPathRelToParent;
                        }
                    }
                }
            }
            finally
            {
                Scribe.ExitNode();
            }
        }

        public static void DrawScaleSetting(int x, int y, int textWidth, int entryWidth, int entryHeight,
            int scaleWidth, string label, string scaleLabel, ref int value, int minValue,
            Action<IntervalScale> setScale)
        {
            var labelRect = new Rect(0, y + 5, textWidth, entryHeight);
            Widgets.Label(labelRect, label);

            var fieldRect = new Rect(textWidth, y, entryWidth, entryHeight);
            string fieldBuffer = null; // Don't need string buffer for this
            Widgets.TextFieldNumeric(fieldRect, ref value, ref fieldBuffer, minValue, 1000);

            var scaleButton = new Rect(textWidth + entryWidth, y, scaleWidth, entryHeight);
            if (!Widgets.ButtonText(scaleButton, scaleLabel))
            {
                return;
            }

            var list = new List<FloatMenuOption>();
            foreach (var scale in IntervalScale.Values)
            {
                list.Add(new FloatMenuOption(scale.label.Translate(), delegate { setScale(scale); }));
            }

            Find.WindowStack.Add(new FloatMenu(list));
        }
    }
}