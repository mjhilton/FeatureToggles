using System;
using System.Collections.Generic;

namespace FeatureToggles.Admin
{
    public class FeatureSchedule
    {
        public string Name { get; set; }
        public KeyValuePair<DateTimeOffset, bool> CurrentValue { get; set; }
        public IList<KeyValuePair<DateTimeOffset, bool>> ScheduledValues { get; set; }
    }
}
