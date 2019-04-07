using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Limping.Api.Dtos.TestAnalysisDtos;
using Limping.Api.Models;

namespace Limping.Api.Dtos.LimpingTestDtos
{
    public class EditLimpingTestDto
    {
        [Required]
        public string TestData { get; set; }
        public ReplaceTestAnalysisDto TestAnalysis { get; set; }
    }
}
