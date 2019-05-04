﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Limping.Api.Dtos.TestAnalysisDtos;
using Limping.Api.Dtos.TestAnalysisDtos.Responses;
using Limping.Api.Models;
using Limping.Api.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Limping.Api.Controllers
{
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
            var response = new TestAnalysisDto(found);
            return Ok(response);
        }

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

            var testAnalysis = new TestAnalysis
            {
                Description = testAnalysisDto.Description,
                EndValue = testAnalysisDto.EndValue.GetValueOrDefault(),
                LimpingSeverity = testAnalysisDto.LimpingSeverity.GetValueOrDefault(),
                LimpingTestId = testId,
            };
            await _testAnalysesService.ReplaceTestAnalysis(testId, testAnalysis);
            var response = new TestAnalysisDto(testAnalysis);
            return Ok(response);
        }

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
            var response = new TestAnalysisDto(testAnalysis);
            return Ok(response);
        }

    }
}