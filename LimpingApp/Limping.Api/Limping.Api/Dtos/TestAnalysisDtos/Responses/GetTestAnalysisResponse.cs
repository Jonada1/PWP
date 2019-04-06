using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Halcyon.HAL;
using Limping.Api.Constants;

namespace Limping.Api.Dtos.TestAnalysisDtos.Responses
{
    public class GetTestAnalysisResponse: HALResponse
    {
        public GetTestAnalysisResponse(TestAnalysisDto analysis, Link selfLink = null) : base(analysis)
        {
            if (selfLink == null)
            {
                this.AddLinks(
                    new Link("self", $"{ControllerUrls.Analysis}GetById/{analysis.Id}", "Get analysis", LinkMethods.GET)
                );
            }
            else
            {
                this.AddLinks(selfLink);
            }
            this.AddLinks(
                new Link("edit", $"{ControllerUrls.Analysis}EditTestAnalysis/{analysis.Id}", "Edit analysis", LinkMethods.PUT),
                new Link("limpingTest", $"{ControllerUrls.LimpingTests}GetById/{analysis.LimpingTestId}", "Get limping test", LinkMethods.GET)
            );
        }
    }

    public class GetTestAnalysisProduces : TestAnalysisDto
    {
        public List<Link> _links { get; set; }
    }
}
