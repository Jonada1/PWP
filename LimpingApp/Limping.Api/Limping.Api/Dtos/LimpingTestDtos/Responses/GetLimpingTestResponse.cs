using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Halcyon.HAL;
using Limping.Api.Constants;
using Limping.Api.Extensions;
using Limping.Api.Models;
using Limping.Api.Utils;

namespace Limping.Api.Dtos.LimpingTestDtos.Responses
{
    public class GetLimpingTestResponse: HALResponse
    {
        public GetLimpingTestResponse(LimpingTest test, Link selfLink = null) : base(new LimpingTestDto(test))
        {
            if (selfLink == null)
            {
                this.AddLinks(
                    LinkGenerator.LimpingTests.GetSingle(test.Id.ToString(), "self")
                );
            }
            else
            {
                this.AddLinks(selfLink);
            }

            this.AddLinks(
                LinkGenerator.LimpingTests.GetSingle(test.Id.ToString()),
                LinkGenerator.LimpingTests.Edit(test.Id.ToString()),
                LinkGenerator.LimpingTests.Delete(test.Id.ToString()),
                LinkGenerator.LimpingTests.GetAll(),
                LinkGenerator.Analysis.GetSingle(test.TestAnalysisId.ToString()),
                LinkGenerator.Users.GetSingle(test.AppUserId)
            );
        }

        public class GetLimpingTestResponseProduces: LimpingTestDto
        {
            public List<Link> _links { get; set; }
        }

    }
}
