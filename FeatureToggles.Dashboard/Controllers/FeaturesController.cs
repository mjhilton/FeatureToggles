using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using FeatureToggles.Infrastructure;
using FeatureToggles.Infrastructure.EntityFramework;

namespace FeatureToggles.Dashboard.Controllers
{
    [Authorize]
    public class FeaturesController : ApiController
    {
        private readonly IFeaturesContext _featuresContext;

        public FeaturesController(IFeaturesContext featuresContext)
        {
            _featuresContext = featuresContext;
        }

        // GET api/features
        public IEnumerable<Feature> Get()
        {
            return _featuresContext.Features.ToList();
        }

        // GET api/features/{id:Guid}
        public Feature Get(Guid id)
        {
            return _featuresContext.Features.Single(f => f.Id == id);
        }

        // GET api/features/{featureName:string}
        public IEnumerable<Feature> Get(string featureName)
        {
            return _featuresContext.Features.Where(f => f.Name == featureName).ToList();
        }

        // POST api/features
        public void Post([FromBody]Feature feature)
        {
            _featuresContext.Features.Add(feature);
        }

        // PUT api/features/{id:Guid}
        public void Put(Guid id, [FromBody]Feature feature)
        {
            var existingFeature = _featuresContext.Features.Single(f => f.Id == id);
            
            existingFeature.EffectiveAt = feature.EffectiveAt;
            existingFeature.Enabled = feature.Enabled;
        }

        // DELETE api/features/{id:Guid}
        public void Delete(Guid id)
        {
            _featuresContext.Features.Remove(_featuresContext.Features.Single(f => f.Id == id));
        }
    }
}
