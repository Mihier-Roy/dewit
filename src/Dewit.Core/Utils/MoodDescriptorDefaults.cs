using Dewit.Core.Entities;
using Dewit.Core.Enums;
using Dewit.Core.Interfaces;

namespace Dewit.Core.Utils
{
    public static class MoodDescriptorDefaults
    {
        internal static readonly Dictionary<Mood, string> Defaults = new()
        {
            [Mood.VeryHappy]  = "inspired,valued,grateful,energized,confident,creative,loved,motivated,joyful,accomplished",
            [Mood.Happy]      = "content,optimistic,relaxed,appreciated,hopeful,calm,focused,productive,cheerful,connected",
            [Mood.Meh]        = "indifferent,tired,bored,distracted,unmotivated,neutral,restless,uncertain,disconnected,sluggish",
            [Mood.Down]       = "stressed,anxious,frustrated,overwhelmed,lonely,disappointed,irritable,drained,worried,sad",
            [Mood.ExtraDown]  = "hopeless,defeated,exhausted,depressed,empty,isolated,despairing,numb,worthless,broken",
        };

        /// <summary>Seeds default descriptors into the MoodDescriptors table if it is empty.</summary>
        public static void SeedIfMissing(IRepository<MoodDescriptorItem> repo)
        {
            if (repo.List().Any()) return;

            foreach (var (mood, descriptors) in Defaults)
            {
                repo.Add(new MoodDescriptorItem
                {
                    Mood = mood.ToString(),
                    Descriptors = descriptors
                });
            }
        }
    }
}
