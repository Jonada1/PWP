using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Limping.Api.Models;

namespace Limping.Api.Dtos.TestAnalysisDtos
{
    public class ReplaceTestAnalysisDto
    {
        [Required]
        public Double EndValue { get; set; }
        public string Description { get; set; }
        [Required]
        public LimpingSeverityEnum LimpingSeverity { get; set; }
    }
}
