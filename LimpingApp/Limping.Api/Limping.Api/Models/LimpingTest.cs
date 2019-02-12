using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Limping.Api.Models
{
    public class LimpingTest
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        [DataType("jsonb")]
        public string TestData { get; set; }
        public string AppUserId { get; set; }
        public Guid TestAnalysisId { get; set; }
        public virtual AppUser AppUser { get; set; }
        public virtual TestAnalysis TestAnalysis { get; set; }
    }
}
