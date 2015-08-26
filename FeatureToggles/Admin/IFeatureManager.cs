using System;
using System.Collections.Generic;

namespace FeatureToggles.Admin
{
    public interface IFeatureManager
    {
        IEnumerable<FeatureSchedule> GetAll();
        void Add(string name, bool defaultValue);
        void Update(string name, bool newValue);
        void Schedule(string name, DateTimeOffset effectiveTime, bool newValue);
        void Deschedule(string name, DateTimeOffset effectiveTime);
        void Remove(string name);
    }
}
