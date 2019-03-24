using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Limping.Api.Models;

namespace Limping.Api.Dtos.LimpingTestDtos
{
    public class LimpingTestDto
    {
        public LimpingTestDto() { }

        public LimpingTestDto(LimpingTest test)
        {
            Id = test.Id;
            Date = test.Date;
            TestData = test.TestData;
            AppUserId = test.AppUserId;
        }
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public string TestData { get; set; }
        public string AppUserId { get; set; }
    }
}
