using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Limping.Api.Models
{
    public class TestComparison
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime ComparisonDate { get; set; }
        [DataType("jsonb")]
        public string ComparisonResults { get; set; }
        public virtual List<LimpingTestTestComparison> LimpingTestTestComparisons { get; set; }
    }
}
