using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Limping.Api.Dtos.TestAnalysisDtos;
using Limping.Api.Dtos.TestAnalysisDtos.Responses;
using Limping.Api.Models;
using Limping.Api.Services.Interfaces;
using Limping.Api.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Limping.Api.Controllers
{
    /// <summary>
    /// Creates the analysis for a test
    /// </summary>
    [Route("api/[controller]")]
    public class AnalysisController : LimpingControllerBase
    {
        private readonly LimpingDbContext _context;
        private readonly ITestAnalysesService _testAnalysesService;
        public AnalysisController(LimpingDbContext context, ITestAnalysesService testAnalysesService)
        {
            _context = context;
            _testAnalysesService = testAnalysesService;
        }

        /// <summary>
        /// Gets a single analysis by its id
        /// </summary>
        /// <param name="analysisId">The id of the analysis</param>
        /// <returns>
        /// Not Found if it doesn't exist
        /// The HAL response with the analysis otherwise
        /// </returns>
        [HttpGet("{analysisId}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetTestAnalysisProduces))]
        public async Task<IActionResult> GetById([FromRoute] Guid analysisId)
        {
            var exists = await _context.TestAnalyses.AnyAsync(x => x.Id == analysisId);
            if (!exists)
            {
                return NotFound();
            }

            var found = await _context.TestAnalyses.FindAsync(analysisId);

            // Create HAL Reponse
            var response = new GetTestAnalysisResponse(found);
            return Ok(response);
        }

        /// <summary>
        /// Insert or update a test analysis. Insert if doesn't exist, update otherwise
        /// </summary>
        /// <param name="testId">The id of the test for which we are upserting the analysis</param>
        /// <param name="testAnalysisDto">The analysis dto</param>
        /// <returns>
        /// Not found if the test we are upserting for doesn't exist
        /// 
        /// </returns>
        [HttpPut("{testId}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetTestAnalysisProduces))]
        public async Task<IActionResult> UpsertTestAnalysis([FromRoute]Guid testId, [FromBody]ReplaceTestAnalysisDto testAnalysisDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var testExists = await _context.LimpingTests.AnyAsync(test => test.Id == testId);
            if (!testExists)
            {
                return NotFound();
            }

            // Convert it to db model
            var testAnalysis = new TestAnalysis
            {
                Description = testAnalysisDto.Description,
                EndValue = testAnalysisDto.EndValue.GetValueOrDefault(),
                LimpingSeverity = testAnalysisDto.LimpingSeverity.GetValueOrDefault(),
                LimpingTestId = testId,
            };

            // Replace it
            await _testAnalysesService.ReplaceTestAnalysis(testId, testAnalysis);

            // Convert it to HAL
            var response = new GetTestAnalysisResponse(testAnalysis, LinkGenerator.Analysis.Edit(testAnalysis.Id.ToString(), "self"));

            return Ok(response);
        }

        /// <summary>
        /// Replaces a test analysis
        /// </summary>
        /// <param name="testAnalysisId">The analysis which will be replaced</param>
        /// <param name="testAnalysisDto">The dto of the analysis which will replace it</param>
        /// <returns>The replaced analysis</returns>
        [HttpPut("replace/{testAnalysisId}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetTestAnalysisProduces))]
        public async Task<IActionResult> EditTestAnalysis([FromRoute] Guid testAnalysisId,
            [FromBody] ReplaceTestAnalysisDto testAnalysisDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var testExists = await _context.TestAnalyses.AnyAsync(analysis => analysis.Id == testAnalysisId);
            if (!testExists)
            {
                return NotFound();
            }

            var testAnalysis = await _context.TestAnalyses.AsNoTracking().SingleAsync(x => x.Id == testAnalysisId);
            testAnalysis.Description = testAnalysisDto.Description;
            testAnalysis.EndValue = testAnalysisDto.EndValue.GetValueOrDefault();
            testAnalysis.LimpingSeverity = testAnalysisDto.LimpingSeverity.GetValueOrDefault();
            await _testAnalysesService.EditTestAnalysis(testAnalysis);
            var response = new GetTestAnalysisResponse(testAnalysis, LinkGenerator.Analysis.Replace(testAnalysis.Id.ToString(), "self"));
            return Ok(response);
        }

    }
}