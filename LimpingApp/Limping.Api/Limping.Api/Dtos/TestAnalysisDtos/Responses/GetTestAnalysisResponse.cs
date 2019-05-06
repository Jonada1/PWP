using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Halcyon.HAL;
using Limping.Api.Constants;
using Limping.Api.Models;
using Limping.Api.Utils;

namespace Limping.Api.Dtos.TestAnalysisDtos.Responses
{
    public class GetTestAnalysisResponse: HALResponse
    {
        public GetTestAnalysisResponse(TestAnalysis analysis, Link selfLink = null) : base(analysis)
        {
            if (selfLink == null)
            {
                this.AddLinks(
                    LinkGenerator.Analysis.GetSingle(analysis.Id.ToString(), "self")
                );
            }
            else
            {
                this.AddLinks(selfLink);
            }
            this.AddLinks(
                LinkGenerator.Analysis.Edit(analysis.Id.ToString()),
                LinkGenerator.LimpingTests.GetSingle(analysis.LimpingTestId.ToString())
            );
        }
    }

    public class GetTestAnalysisProduces : TestAnalysisDto
    {
        public List<Link> _links { get; set; }
    }
}
