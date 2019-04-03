using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Limping.Api.Models;

namespace Limping.Api.Dtos.TestAnalysisDtos
{
    public class TestAnalysisDto
    {
        public TestAnalysisDto()
        {
            
        }

        public TestAnalysisDto(TestAnalysis analysis)
        {
            Id = analysis.Id;
            EndValue = analysis.EndValue;
            Description = analysis.Description;
            LimpingSeverity = analysis.LimpingSeverity;
            LimpingTestId = analysis.LimpingTestId;
        }
        public Guid Id { get; set; }
        public Double EndValue { get; set; }
        public string Description { get; set; }
        public LimpingSeverityEnum LimpingSeverity { get; set; }
        public Guid LimpingTestId { get; set; }
    }
}

