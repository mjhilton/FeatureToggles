using System;
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
            var featureContext = Substitute.For<IFeaturesContext>();
            featureContext.Features.Returns(new TestDbSet<Feature>(new[]
            {
                new Feature { Name = "Test1" },
                new Feature { Name = "Test2" }
            }));
            var pollingFeatureCache = new PollingFeatureCache(featureContext, TimeSpan.FromMilliseconds(100));
            Assert.IsNull(pollingFeatureCache.Get("Test3"));
            featureContext.Features.Add(new Feature { Name = "Test3" });
            
            Thread.Sleep(200);

            Assert.IsNotNull(pollingFeatureCache.Get("Test3"));
        }

        [TestMethod]
        public void AfterUpdate_SchedulesNextUpdate()
        {
            var featureContext = Substitute.For<IFeaturesContext>();
            featureContext.Features.Returns(new TestDbSet<Feature>(new[]
            {
                new Feature { Name = "Test1" },
                new Feature { Name = "Test2" }
            }));
            var pollingFeatureCache = new PollingFeatureCache(featureContext, TimeSpan.FromMilliseconds(100));
            Assert.IsNull(pollingFeatureCache.Get("Test3"));
            Thread.Sleep(200);
            featureContext.Features.Add(new Feature { Name = "Test3" });

            Thread.Sleep(200);

            Assert.IsNotNull(pollingFeatureCache.Get("Test3"));
        }
    }
}
