using Dewit.Core.Enums;
using Dewit.Core.Interfaces;

namespace Dewit.Core.Utils
{
    public static class MoodDescriptorDefaults
    {
        private static readonly Dictionary<Mood, string> Defaults = new()
        {
            [Mood.VeryHappy]  = "inspired,valued,grateful,energized,confident,creative,loved,motivated,joyful,accomplished",
            [Mood.Happy]      = "content,optimistic,relaxed,appreciated,hopeful,calm,focused,productive,cheerful,connected",
            [Mood.Meh]        = "indifferent,tired,bored,distracted,unmotivated,neutral,restless,uncertain,disconnected,sluggish",
            [Mood.Down]       = "stressed,anxious,frustrated,overwhelmed,lonely,disappointed,irritable,drained,worried,sad",
            [Mood.ExtraDown]  = "hopeless,defeated,exhausted,depressed,empty,isolated,despairing,numb,worthless,broken",
        };

        /// <summary>Seeds default descriptors into config if the keys do not already exist.</summary>
        public static void SeedIfMissing(IConfigurationService config)
        {
            foreach (var (mood, descriptors) in Defaults)
            {
                var key = mood.ToConfigKey();
                if (!config.KeyExists(key))
                    config.SetValue(key, descriptors);
            }
        }
    }
}
