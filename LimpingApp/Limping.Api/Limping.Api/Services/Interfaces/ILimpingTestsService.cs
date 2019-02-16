using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Limping.Api.Models;

namespace Limping.Api.Services.Interfaces
{
    public interface ILimpingTestsService
    {
        Task<LimpingTest> GetById(Guid id);
        Task<List<LimpingTest>> GetUserTests(string userId);
        Task<LimpingTest> InsertTest(string userId, string testData, TestAnalysis testAnalysis);
        Task<LimpingTest> EditTest(Guid testId, string testData, TestAnalysis testAnalysis);
        Task DeleteTest(Guid testId);
    }

}
