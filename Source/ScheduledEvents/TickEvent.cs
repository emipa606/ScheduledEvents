﻿using System.Collections.Generic;

namespace ScheduledEvents;

public class TickEvent
{
    public readonly ScheduledEvent E;
    public readonly int Tick;

    private TickEvent(int tick, ScheduledEvent e)
    {
        Tick = tick;
        E = e;
    }

    // Adds the scheduled event to the list sorted
    public static void AddToList(List<TickEvent> list, int tick, ScheduledEvent e)
    {
        for (var i = 0; i < list.Count; i++)
        {
            var o = list[i];
            if (tick >= o.Tick)
            {
                continue;
            }

            list.Insert(i, new TickEvent(tick, e));
            return;
        }

        // If it wasn't inserted in for loop, do it here
        list.Add(new TickEvent(tick, e));
    }
}