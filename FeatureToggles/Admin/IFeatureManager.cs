using System;
using System.Collections.Generic;

namespace FeatureToggles.Admin
{
    public interface IFeatureManager
    {
        IEnumerable<FeatureSchedule> GetAll();
        TryResult Add(string name, bool defaultValue);
        TryResult Update(string name, bool newValue);
        TryResult Schedule(string name, DateTimeOffset effectiveTime, bool newValue);
        TryResult Deschedule(string name, DateTimeOffset effectiveTime);
        TryResult Remove(string name);
    }
}
