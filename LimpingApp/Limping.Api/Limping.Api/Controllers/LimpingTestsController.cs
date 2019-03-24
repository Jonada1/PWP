using System;
using System.Linq;
using System.Threading.Tasks;
using Limping.Api.Dtos.LimpingTestDtos;
using Limping.Api.Models;
using Limping.Api.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Limping.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LimpingTestsController : ControllerBase
    {
        private readonly LimpingDbContext _context;
        private readonly ILimpingTestsService _limpingTestsService;
        public LimpingTestsController(LimpingDbContext context, ILimpingTestsService limpingTestsService)
        {
            _context = context;
            _limpingTestsService = limpingTestsService;
        }

        [HttpGet("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LimpingTestDto))]
        public async Task<IActionResult> GetAll()
        {
            var allTests = await _limpingTestsService.GetAll();
            var response = allTests.Select(test => new LimpingTestDto(test));
            return Ok(response);
        }

        [HttpGet("[action]/{userId}")]
        public async Task<IActionResult> GetForUser([FromRoute] string userId)
        {
            var userExists = await _context.AppUsers.AnyAsync(user => user.Id == userId);
            if (!userExists)
            {
                return NotFound("The user was not found");
            }

            var userTests = await _limpingTestsService.GetUserTests(userId);
            var response = userTests.Select(test => new LimpingTestDto(test));
            return Ok(response);
        }

        [HttpGet("[action]/{limpingTestId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LimpingTestDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById([FromRoute] Guid limpingTestId)
        {
            var exists = await _context.LimpingTests.AnyAsync(lt => lt.Id == limpingTestId);
            if (!exists)
            {
                return NotFound();
            }
            var limpingTest = await _limpingTestsService.GetById(limpingTestId);
            var response = new LimpingTestDto(limpingTest);
            return Ok(response);
        }

        [HttpPost("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LimpingTestDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

            var createdTest = await _limpingTestsService.InsertTest(createDto.AppUserId, createDto.TestData, createDto.TestAnalysis);
            var response = new LimpingTestDto(createdTest);
            return Ok(response);
        }

        [HttpPatch("[action]/{testId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LimpingTestDto))]
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

            var edited = await _limpingTestsService.EditTest(testId, editTestDto.TestData, editTestDto.TestAnalysis);
            var response = new LimpingTestDto(edited);
            return Ok(response);
        }
        [HttpDelete("[action]/{limpingTestId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LimpingTestDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute] Guid limpingTestId)
        {
            var exists = await _context.LimpingTests.AnyAsync(lt => lt.Id == limpingTestId);
            if (!exists)
            {
                return NotFound();
            }

            await _limpingTestsService.DeleteTest(limpingTestId);
            return NoContent();
        }
    }
}