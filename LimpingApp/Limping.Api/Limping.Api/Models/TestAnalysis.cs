using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Limping.Api.Models
{
    public class TestAnalysis
    {
        [Key]
        public Guid Id { get; set; }
        public Double EndValue { get; set; }
        public string Description { get; set; }
        public LimpingSeverityEnum LimpingSeverity { get; set; }
    }

    public enum LimpingSeverityEnum
    {
        Low,
        Medium,
        High
    }
}
