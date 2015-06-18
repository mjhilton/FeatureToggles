using System;
using System.Collections.Generic;
using System.Threading;
using FeatureToggles.Infrastructure;
using FeatureToggles.Infrastructure.EntityFramework;
using FeatureToggles.Tests.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace FeatureToggles.Tests
{
    [TestClass]
    public class PollingFeatureCacheTests
    {
        [TestMethod]
        public void OnCreation_ImmediatelyRetrievesFeatureValues()
        {
            var featureContext = Substitute.For<IFeaturesContext>();
            featureContext.Features.Returns(new TestDbSet<Feature>(new[]
            {
                new Feature { Name = "Test1" },
                new Feature { Name = "Test2" }
            }));

            var pollingFeatureCache = new PollingFeatureCache(featureContext);

            Assert.IsNotNull(pollingFeatureCache.Get("Test1"));
        }

        [TestMethod]
        public void OnCreation_SchedulesNextUpdate()
        {
            var featureContext = CreateContext(new[]
            {
                new Feature { Name = "Test1" },
                new Feature { Name = "Test2" }
            });
            var pollingFeatureCache = new PollingFeatureCache(featureContext, TimeSpan.FromMilliseconds(100));
            Assert.IsNull(pollingFeatureCache.Get("Test3"));
            featureContext.Features.Add(new Feature { Name = "Test3" });
            
            Thread.Sleep(200);

            Assert.IsNotNull(pollingFeatureCache.Get("Test3"));
        }

        [TestMethod]
        public void AfterUpdate_SchedulesNextUpdate()
        {
            var featureContext = CreateContext(new[]
            {
                new Feature {Name = "Test1"},
                new Feature {Name = "Test2"}
            });
            var pollingFeatureCache = new PollingFeatureCache(featureContext, TimeSpan.FromMilliseconds(100));
            Assert.IsNull(pollingFeatureCache.Get("Test3"));
            Thread.Sleep(200);
            featureContext.Features.Add(new Feature { Name = "Test3" });

            Thread.Sleep(200);

            Assert.IsNotNull(pollingFeatureCache.Get("Test3"));
        }

        [TestMethod]
        public void WhenMultipleScheduledValuesExist_ReturnsCurrentlyActiveValue()
        {
            var featureContext = CreateContext(new[]
            {
                new Feature { Name = "Feature1", EffectiveAt = new DateTimeOffset(2015, 01, 01, 12, 00, 00, TimeSpan.Zero), Enabled = true },
                new Feature { Name = "Feature1", EffectiveAt = new DateTimeOffset(2015, 01, 01, 17, 00, 00, TimeSpan.Zero), Enabled = false }
            });
            var pollingFeatureCache = new PollingFeatureCache(featureContext);

            Clock.Freeze(new DateTime(2015, 01, 01, 12, 00, 00));
            Assert.IsTrue(pollingFeatureCache.Get("Feature1").Enabled);

            Clock.Freeze(new DateTime(2015, 01, 01, 16, 59, 59, 999));
            Assert.IsTrue(pollingFeatureCache.Get("Feature1").Enabled);

            Clock.Freeze(new DateTime(2015, 01, 01, 17, 0, 0));
            Assert.IsFalse(pollingFeatureCache.Get("Feature1").Enabled);
        }

        private static IFeaturesContext CreateContext(IEnumerable<Feature> features)
        {
            var featureContext = Substitute.For<IFeaturesContext>();
            featureContext.Features.Returns(new TestDbSet<Feature>(features));
            return featureContext;
        }
    }
}
