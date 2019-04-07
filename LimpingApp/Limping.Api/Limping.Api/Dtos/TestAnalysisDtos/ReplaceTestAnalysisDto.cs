using System;
using System.ComponentModel.DataAnnotations;
using Limping.Api.Models;

namespace Limping.Api.Dtos.TestAnalysisDtos
{
    public class ReplaceTestAnalysisDto
    {
        [Required]
        public double? EndValue { get; set; }
        public string Description { get; set; }
        [Required]
        public LimpingSeverityEnum? LimpingSeverity { get; set; }
    }
}
