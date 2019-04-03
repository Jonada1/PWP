using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Halcyon.HAL;
using Limping.Api.Constants;
using Limping.Api.Models;

namespace Limping.Api.Dtos.LimpingTestDtos.Responses
{
    public class GetMultipleLimpingTestsResponse : HALResponse
    {
        public GetMultipleLimpingTestsResponse(List<LimpingTest> tests, Link selfLink = null, params Link[] links) : base(null)
        {
            this.AddLinks(selfLink ?? new Link("self", $"{ControllerUrls.LimpingTests}GetAll", "Get all limping tests", LinkMethods.GET));

            this.AddLinks(links);

            this.AddEmbeddedCollection("tests", tests.Select(test => new GetLimpingTestResponse(test)));
        }
    }

    public class GetMultipleLimpingTestResponseProduces
    {
        public List<Link> _links { get; set; }
        public List<GetLimpingTestResponse.GetLimpingTestResponseProduces> _embedded { get; set; }
    }
}
