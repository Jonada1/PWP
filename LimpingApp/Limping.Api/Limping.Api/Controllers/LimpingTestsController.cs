using System;
using System.Linq;
using System.Threading.Tasks;
using Halcyon.HAL;
using Limping.Api.Constants;
using Limping.Api.Dtos;
using Limping.Api.Dtos.LimpingTestDtos;
using Limping.Api.Dtos.LimpingTestDtos.Responses;
using Limping.Api.Models;
using Limping.Api.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Limping.Api.Controllers
{
    [Route("api/[controller]")]
    public class LimpingTestsController : LimpingControllerBase
    {
        private readonly LimpingDbContext _context;
        private readonly ILimpingTestsService _limpingTestsService;
        public LimpingTestsController(LimpingDbContext context, ILimpingTestsService limpingTestsService)
        {
            _context = context;
            _limpingTestsService = limpingTestsService;
        }

        [HttpGet("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetMultipleLimpingTestResponseProduces))]
        public async Task<IActionResult> GetAll()
        {
            var allTests = await _limpingTestsService.GetAll();
            var response = new GetMultipleLimpingTestsResponse(allTests);
            return Ok(response);
        }

        [HttpGet("[action]/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetMultipleLimpingTestResponseProduces))]
        public async Task<IActionResult> GetForUser([FromRoute] string userId)
        {
            var userExists = await _context.AppUsers.AnyAsync(user => user.Id == userId);
            if (!userExists)
            {
                return NotFound("The user was not found");
            }

            var userTests = await _limpingTestsService.GetUserTests(userId);
            var response = new GetMultipleLimpingTestsResponse(
                userTests,
                new Link("getForUser", $"{ControllerUrls.LimpingTests}GetForUser/{userId}",
                    "Get limping tests for users", LinkMethods.GET),
                new Link($"{ControllerUrls.AppUsers}GetById/{userId}", "Get user", LinkMethods.GET)
            );
            return Ok(response);
        }

        [HttpGet("[action]/{limpingTestId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetLimpingTestResponse.GetLimpingTestResponseProduces))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById([FromRoute] Guid limpingTestId)
        {
            var exists = await _context.LimpingTests.AnyAsync(lt => lt.Id == limpingTestId);
            if (!exists)
            {
                return NotFound();
            }
            var limpingTest = await _limpingTestsService.GetById(limpingTestId);
            var response = new GetLimpingTestResponse(limpingTest);
            return Ok(response);
        }

        [HttpPost("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetLimpingTestResponse.GetLimpingTestResponseProduces))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateLimpingTestDto createDto)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(createDto.TestData))
            {
                return BadRequest(ModelState);
            }
            var userExists = await _context.AppUsers.AnyAsync(user => user.Id == createDto.AppUserId);
            if (!userExists)
            {
                return NotFound("The user was not found");
            }

            var testAnalysisDto = createDto.TestAnalysis;
            var testAnalysis = new TestAnalysis
            {
                Description = testAnalysisDto.Description,
                EndValue = testAnalysisDto.EndValue.GetValueOrDefault(),
                LimpingSeverity = testAnalysisDto.LimpingSeverity.GetValueOrDefault(),
            };
            var createdTest = await _limpingTestsService.InsertTest(createDto.AppUserId, createDto.TestData, testAnalysis);
            var response = new GetLimpingTestResponse(createdTest, selfLink: new Link("self", $"{ControllerUrls.LimpingTests}Create","Create limping test", LinkMethods.POST));
            return Ok(response);
        }

        [HttpPatch("[action]/{testId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetLimpingTestResponse.GetLimpingTestResponseProduces))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit([FromRoute] Guid testId, [FromBody] EditLimpingTestDto editTestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var exists = await _context.LimpingTests.AnyAsync(lt => lt.Id == testId);
            if (!exists)
            {
                return NotFound();
            }
            var testAnalysisDto = editTestDto.TestAnalysis;
            TestAnalysis testAnalysis = null;
            if (testAnalysisDto != null)
            {
                testAnalysis = new TestAnalysis
                {
                    Description = testAnalysisDto.Description,
                    EndValue = testAnalysisDto.EndValue.GetValueOrDefault(),
                    LimpingSeverity = testAnalysisDto.LimpingSeverity.GetValueOrDefault(),
                    LimpingTestId = testId
                };
            }
            var edited = await _limpingTestsService.EditTest(testId, editTestDto.TestData, testAnalysis);
            var response = new GetLimpingTestResponse(edited, selfLink: new Link("self", $"{ControllerUrls.LimpingTests}Edit/{testId}", "Edit limping test", LinkMethods.PATCH));
            return Ok(response);
        }
        [HttpDelete("[action]/{limpingTestId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseWithLinksOnly))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute] Guid limpingTestId)
        {
            var exists = await _context.LimpingTests.AnyAsync(lt => lt.Id == limpingTestId);
            if (!exists)
            {
                return NotFound();
            }

            var limpingTest = await _limpingTestsService.GetById(limpingTestId);
            _context.Entry(limpingTest).State = EntityState.Detached;
            await _limpingTestsService.DeleteTest(limpingTestId);
            var response = new HALResponse(null).AddLinks(
                new Link("self", $"{ControllerUrls.LimpingTests}Delete/{limpingTestId}", "Delete limping test", LinkMethods.DELETE),
                new Link("getAll", $"{ControllerUrls.LimpingTests}GetAll/{limpingTest.Id}", "Get all limping tests", LinkMethods.GET),
                new Link("getForUser", $"{ControllerUrls.LimpingTests}GetForUser/{limpingTest.Id}", "Get for user", LinkMethods.GET),
                new Link("user", $"{ControllerUrls.AppUsers}GetById/{limpingTest.AppUserId}", "Get user", LinkMethods.GET)                
            );
            return Ok(response);
        }
    }
}