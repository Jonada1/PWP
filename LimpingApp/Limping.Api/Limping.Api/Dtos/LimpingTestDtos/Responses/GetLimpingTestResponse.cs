using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Halcyon.HAL;
using Limping.Api.Constants;
using Limping.Api.Extensions;
using Limping.Api.Models;

namespace Limping.Api.Dtos.LimpingTestDtos.Responses
{
    public class GetLimpingTestResponse: HALResponse
    {
        public GetLimpingTestResponse(LimpingTest test, Link selfLink = null) : base(new LimpingTestDto(test))
        {
            if (selfLink == null)
            {
                this.AddLinks(
                    new Link("self", $"{ControllerUrls.LimpingTests}GetById/{test.Id}", "Get limping test", LinkMethods.GET)
                );
            }
            else
            {
                this.AddLinks(selfLink);
            }

            this.AddLinks(
                new Link("getById", $"{ControllerUrls.LimpingTests}GetById/{test.Id}", "Get limping test", LinkMethods.GET),
                new LinkExtended("create", $"{ControllerUrls.LimpingTests}Create/{test.Id}", "Create limping test", LinkMethods.POST, nameof(CreateLimpingTestDto)),
                new LinkExtended("edit", $"{ControllerUrls.LimpingTests}Edit/{test.Id}", "Edit limping test", LinkMethods.PATCH, nameof(EditLimpingTestDto)),
                new Link("delete", $"{ControllerUrls.LimpingTests}Delete/{test.Id}", "Delete limping test", LinkMethods.DELETE),
                new Link("getAll", $"{ControllerUrls.LimpingTests}GetAll/{test.Id}", "Get all limping tests", LinkMethods.GET),
                new Link("getForUser", $"{ControllerUrls.LimpingTests}GetForUser/{test.Id}","Get for user", LinkMethods.GET),
                new Link("user", $"{ControllerUrls.AppUsers}GetById/{test.AppUserId}", "Get user", LinkMethods.GET)
            );
            if (test.TestAnalysisId != null)
            {
                this.AddLinks(
                    new Link("analysis", $"{ControllerUrls.Analysis}GetById/{test.TestAnalysisId}", "Get analysis", LinkMethods.GET)
                );
            }
        }

        public class GetLimpingTestResponseProduces: LimpingTestDto
        {
            
        }

    }
}
