using System;
using System.Threading.Tasks;
using Limping.Api.Models;
using Limping.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Limping.Api.Services
{
    public class TestAnalysesService: ITestAnalysesService
    {
        private readonly LimpingDbContext _context;

        public TestAnalysesService(LimpingDbContext context)
        {
            _context = context;
        }

        public async Task ReplaceTest(Guid testId, TestAnalysis newTest)
        {
            var oldTest = await _context.TestAnalyses.FindAsync(testId);
            if (oldTest.LimpingTestId != newTest.LimpingTestId)
            {
                throw new Exception("The old test doesn't have the same LimpingTestId as the new test. Therefore cannot replace one another");
            }
            _context.TestAnalyses.Remove(oldTest);
            _context.TestAnalyses.Add(newTest);
            await _context.SaveChangesAsync();
        }

        public async Task EditTest(TestAnalysis editedTest)
        {
            _context.Entry(editedTest).State = EntityState.Modified;
            await _context.SaveChangesAsync();

        }
    }
}
