using System;
using System.Collections.Generic;

namespace Lab2
{
    public class Track
    {
        public static List<string> TrackedTactics { get; private set; } = new List<string>();


        public static bool CheckIfTracked(string tactic)
        {
            return TrackedTactics.Contains(tactic);
        }

        public static void AddTrack(string tactic)
        {
            if (!CheckIfTracked(tactic))
            {
                TrackedTactics.Add(tactic);
            }
        }

        public static void RemoveTrack(string tactic)
        {
            if (CheckIfTracked(tactic)) TrackedTactics.Remove(tactic);
        }
    }
}
