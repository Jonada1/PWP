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
using Limping.Api.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Limping.Api.Controllers
{
    /// <summary>
    /// The controller for the limping tests
    /// </summary>
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

        /// <summary>
        /// Gets all the limping tests
        /// </summary>
        /// <returns>A HAL request with all limping tests embedded</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetMultipleLimpingTestResponseProduces))]
        public async Task<IActionResult> GetAll()
        {
            var allTests = await _limpingTestsService.GetAll();
            var response = new GetMultipleLimpingTestsResponse(allTests);
            return Ok(response);
        }

        /// <summary>
        /// Gets all the limping tests for a user
        /// </summary>
        /// <param name="userId">The user id for which the limping tests will be fetched</param>
        /// <returns>
        /// Not Found if user doesn't exist
        /// Hal Response with embedded limping tests otherwise
        /// </returns>
        [HttpGet("user/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetMultipleLimpingTestResponseProduces))]
        public async Task<IActionResult> GetForUser([FromRoute] string userId)
        {
            var userExists = await _context.AppUsers.AnyAsync(user => user.Id == userId);
            if (!userExists)
            {
                return NotFound("The user was not found");
            }

            var userTests = await _limpingTestsService.GetUserTests(userId);
            // Create the HAL response with links
            var response = new GetMultipleLimpingTestsResponse(
                userTests,
                LinkGenerator.LimpingTests.GetForUser(userId, "self"),
                LinkGenerator.Users.GetSingle(userId)
            );
            return Ok(response);
        }

        /// <summary>
        /// Gets a single limping test by its id
        /// </summary>
        /// <param name="limpingTestId">The id of the limping test</param>
        /// <returns>
        /// Not found if the id doesn't exist
        /// The HAL response with the Limping Test otherwise
        /// </returns>
        [HttpGet("{limpingTestId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetLimpingTestResponse.GetLimpingTestResponseProduces))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById([FromRoute] Guid limpingTestId)
        {
            var exists = await _context.LimpingTests.AnyAsync(lt => lt.Id == limpingTestId);
            if (!exists)
            {
                return NotFound();
            }

            // Create the HAL response with links
            var limpingTest = await _limpingTestsService.GetById(limpingTestId);
            var response = new GetLimpingTestResponse(limpingTest);
            return Ok(response);
        }

        /// <summary>
        /// Create a limping test
        /// </summary>
        /// <param name="createDto">The dto needed to create a limping test</param>
        /// <returns>
        /// Bad request if invalid request
        /// Not found if the user for which needed to create doesn't exist
        /// The HAL response with the created limping test otherwise
        /// </returns>
        [HttpPost]
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

            // Convert it to database model
            var testAnalysis = new TestAnalysis
            {
                Description = testAnalysisDto.Description,
                EndValue = testAnalysisDto.EndValue.GetValueOrDefault(),
                LimpingSeverity = testAnalysisDto.LimpingSeverity.GetValueOrDefault(),
            };
            // Save it
            var createdTest = await _limpingTestsService.InsertTest(createDto.AppUserId, createDto.TestData, testAnalysis);

            // Create the HAL response
            var response = new GetLimpingTestResponse(createdTest, selfLink: LinkGenerator.LimpingTests.Create("self"));
            return Ok(response);
        }

        /// <summary>
        /// Edits the limping test
        /// </summary>
        /// <param name="testId">The test to be edited</param>
        /// <param name="editTestDto">The dto for the patch request</param>
        /// <returns>
        /// Bad request if the request is invalid
        /// Not Found if it the test doesn't exist
        /// The HAL response with edited test otherwise
        /// </returns>
        [HttpPatch("{testId}")]
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
            // Convert it to database model
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

            // Save it
            var edited = await _limpingTestsService.EditTest(testId, editTestDto.TestData, testAnalysis);

            // Create HAL response
            var response = new GetLimpingTestResponse(edited, selfLink: LinkGenerator.LimpingTests.Edit(edited.Id.ToString(), "self"));
            return Ok(response);
        }

        /// <summary>
        /// Deletes the limping test
        /// </summary>
        /// <param name="limpingTestId">The limping test that will be deleted</param>
        /// <returns>
        /// Not Found if it doesn't exist
        /// A HAL Response with links only otherwsie
        /// </returns>
        [HttpDelete("{limpingTestId}")]
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
                LinkGenerator.LimpingTests.Delete(limpingTestId.ToString(), "self"),
                LinkGenerator.LimpingTests.GetAll(),
                LinkGenerator.LimpingTests.GetForUser(limpingTest.AppUserId),
                LinkGenerator.Users.GetSingle(limpingTest.AppUserId)                
            );
            return Ok(response);
        }
    }
}