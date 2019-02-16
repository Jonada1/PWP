using Limping.Api.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Limping.Api.Services.Interfaces;

namespace Limping.Api.Services
{
    public class LimpingTestsService: ILimpingTestsService
    {
        private readonly LimpingDbContext _context;
        public LimpingTestsService(LimpingDbContext context)
        {
            _context = context;
        }

        public async Task<LimpingTest> GetById(Guid id)
        {
            var limpingTest = await _context.LimpingTests.FindAsync(id);
            return limpingTest;
        }

        public async Task<List<LimpingTest>> GetUserTests(string userId)
        {
            var limpingTests = await _context.LimpingTests.Where(lt => lt.AppUserId == userId).ToListAsync();
            return limpingTests;
        }

        public async Task<LimpingTest> InsertTest(string userId, string testData, TestAnalysis testAnalysis)
        {
            var limpingTest = new LimpingTest
            {
                AppUserId = userId,
                TestData = testData,
                Date = DateTime.Now,
                TestAnalysis = testAnalysis
            };
            _context.LimpingTests.Add(limpingTest);
            await _context.SaveChangesAsync();
            return limpingTest;
        }

        public async Task<LimpingTest> EditTest(Guid testId, string testData, TestAnalysis testAnalysis)
        {
            var limpingTest = await _context.LimpingTests.FindAsync(testId);
            _context.Entry(limpingTest).State = EntityState.Modified;
            limpingTest.TestData = testData;
            limpingTest.TestAnalysis = testAnalysis;
            limpingTest.Date = DateTime.Now;
            await _context.SaveChangesAsync();
            return limpingTest;
        }

        public async Task DeleteTest(Guid testId)
        {
            var limpingTest = await _context.LimpingTests.FindAsync(testId);
            _context.LimpingTests.Remove(limpingTest);
            await _context.SaveChangesAsync();
        }
    }
}
