using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TypicalTypistAPI.Models;

namespace TypicalTypistAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserStatsController : ControllerBase
    {
        private TypicalTypistDbContext dbContext = new TypicalTypistDbContext();

        // DTO Converstions

        static UserTypingTestDTO convertToTestDTO(UserTypingTest test)
        {
            return new UserTypingTestDTO
            {
                userId = test.UserId,
                date = test.TestDate,
                CharCount = test.CharCount,
                incorrectCount = test.IncorrectCount,
                Mode = test.Mode,
                Speed = test.Wpm,
                Accuracy = test.Accuracy

            };
        }

        static UserKeyStatDTO convertToKeyStatDTO(UserKeyStat stat)
        {
            return new UserKeyStatDTO
            {
                UserId = stat.UserId,
                Key = stat.Key,
                TotalTyped = stat.TotalTyped,
                Accuracy = stat.Accuracy,
                Speed = stat.Speed
            };
        }

        // HTTP calls

        [HttpPost("stats")]
        public async Task<IActionResult> sendStatResults(UserStatDTO statDTO)
        {

            if (statDTO == null)
            {
                return BadRequest(new { Message = "Invalid stat data." });
            }

            var existingStat = await dbContext.UserStats.FirstOrDefaultAsync(u => u.UserId == statDTO.UserId);

            if (existingStat == null)
            {
                UserStat userStat = new UserStat();

                userStat.UserId = statDTO.UserId;
                userStat.CharsTyped = statDTO.CharsTyped;
                userStat.TimeTyped = statDTO.TimeTyped;
                userStat.Wpm = statDTO.WPM;
                userStat.TopWpm = statDTO.WPM;
                userStat.Cpm = 0;                           // setting CPM stats to zero for now... depends on if I implement char mode later.
                userStat.TopCpm = 0;
                userStat.Accuracy = statDTO.Accuracy;
                userStat.TopAccuracy = statDTO.Accuracy;

                dbContext.UserStats.Add(userStat);

            }

            else
            {
                existingStat.CharsTyped += statDTO.CharsTyped;
                existingStat.TimeTyped += statDTO.TimeTyped;
                existingStat.Accuracy = (existingStat.Accuracy + statDTO.Accuracy) / 2;
                existingStat.Wpm = (existingStat.Wpm + statDTO.WPM) / 2;
                if (statDTO.Accuracy > existingStat.TopAccuracy)
                {
                    existingStat.TopAccuracy = statDTO.Accuracy;
                }
                if (statDTO.WPM > existingStat.TopWpm)
                {
                    existingStat.TopWpm = statDTO.WPM;
                }
            }


            await dbContext.SaveChangesAsync();

            return Ok();

        }

        [HttpPost("tests")]
        public async Task<IActionResult> sendTestResults(UserTypingTestDTO testDTO)
        {

            if (testDTO == null)
            {
                return BadRequest(new { Message = "Invalid test data." });
            }

            // Check how many tests user has
            var userTests = dbContext.UserTypingTests
                                     .Where(test => test.UserId == testDTO.userId)
                                     .OrderBy(test => test.TestDate)
                                     .ToList();

            // If >10, remove oldest one
            if (userTests.Count >= 10)
            {
                var oldestTest = userTests.First();
                dbContext.UserTypingTests.Remove(oldestTest);
            }

            UserTypingTest typingTest = new UserTypingTest();

            typingTest.UserId = testDTO.userId;
            typingTest.CharCount = testDTO.CharCount;
            typingTest.IncorrectCount = testDTO.incorrectCount;
            typingTest.Mode = testDTO.Mode;
            typingTest.Wpm = testDTO.Speed;
            typingTest.Accuracy = testDTO.Accuracy;

            dbContext.UserTypingTests.Add(typingTest);
            await dbContext.SaveChangesAsync();

            return Ok();

        }

        [HttpPost("bigraphs")]
        public async Task<IActionResult> sendBigraphResults(UserBigraphStatDTO[] statArr)
        {

            if (statArr == null)
            {
                return BadRequest(new { Message = "Invalid bigraph data." });
            }

            foreach (var bigraphStatDTO in statArr)
            {
                var existingStat = await dbContext.UserBigraphStats.FirstOrDefaultAsync(stat =>
                stat.UserId == bigraphStatDTO.UserId && stat.Bigraph == bigraphStatDTO.Bigraph);

                if (existingStat == null)
                {
                    UserBigraphStat userBigraph = new UserBigraphStat();

                    userBigraph.UserId = bigraphStatDTO.UserId;
                    userBigraph.StartingKey = bigraphStatDTO.Bigraph[0].ToString();
                    userBigraph.Bigraph = bigraphStatDTO.Bigraph;
                    userBigraph.TotalTyped = bigraphStatDTO.Quantity;
                    userBigraph.Accuracy = bigraphStatDTO.Accuracy;
                    userBigraph.Speed = bigraphStatDTO.Speed;

                    dbContext.Add(userBigraph);

                }
                else
                {
                    existingStat.TotalTyped += bigraphStatDTO.Quantity;
                    existingStat.Accuracy = (existingStat.Accuracy + bigraphStatDTO.Accuracy) / 2;
                    existingStat.Speed = (existingStat.Speed + bigraphStatDTO.Speed) / 2;
                }

            }

            await dbContext.SaveChangesAsync();
            return Ok();

        }

        [HttpPost("keys")]
        public async Task<IActionResult> sendKeyResults(UserKeyStatDTO[] statArr)
        {

            if (statArr == null)
            {
                return BadRequest(new { Message = "Inavlid key data." });
            }

            foreach (var userKeyStatDTO in statArr)
            {
                var existingStat = await dbContext.UserKeyStats.FirstOrDefaultAsync(stat =>
                stat.UserId == userKeyStatDTO.UserId && stat.Key == userKeyStatDTO.Key);

                if (existingStat == null)
                {
                    UserKeyStat keyStat = new UserKeyStat();

                    // keyStat.UserId = userKeyStatDTO.UserId;
                    keyStat.Key = userKeyStatDTO.Key;
                    keyStat.TotalTyped = userKeyStatDTO.TotalTyped;
                    keyStat.Accuracy = userKeyStatDTO.Accuracy;
                    keyStat.Speed = userKeyStatDTO.Speed;

                    dbContext.Add(keyStat);

                }
                else
                {
                    existingStat.TotalTyped += userKeyStatDTO.TotalTyped;
                    existingStat.Accuracy = (existingStat.Accuracy + userKeyStatDTO.Accuracy) / 2;
                    existingStat.Speed = (existingStat.Speed + userKeyStatDTO.Speed) / 2;
                }

            }

            await dbContext.SaveChangesAsync();
            return Ok();

        }

        [HttpGet("UserTests")]
        public async Task<IActionResult> getTestList(int userId)
        {
            List<UserTypingTest> result = await dbContext.UserTypingTests.Where(t => t.UserId == userId).ToListAsync();

            if (result.Count == 0)
            {
                return NotFound(new { Message = "No typing tests found for user." });
            }

            List<UserTypingTestDTO> formattedResult = result.Select(t => convertToTestDTO(t)).ToList();

            return Ok(formattedResult);

        }

        [HttpGet("UserBigraphs")]
        public async Task<IActionResult> getBigraphStats(int userId)
        {
            List<UserBigraphStat> result = await dbContext.UserBigraphStats.Where(t => t.UserId == userId).ToListAsync();

            if (result.Count == 0)
            {
                return NotFound(new { Message = "No bigraph data found for user." });
            }

            return Ok(result);

        }

        [HttpGet("UserSpecificBigraphs")]
        public async Task<IActionResult> getUserSpecificBigraphs(int userId, string key)
        {
            if (key.Length != 1)
            {
                return BadRequest(new { Message = "The key must be a single character." });
            }

            char keyChar = key[0];

            List<UserBigraphStat> result = await dbContext.UserBigraphStats
                .Where(t => t.UserId == userId && (t.StartingKey == key || t.Bigraph.Substring(1, 1) == key))
                .ToListAsync();

            if (result.Count == 0)
            {
                return NotFound(new { Message = "No bigraph data found for given parameters." });
            }

            return Ok(result);

        }

        [HttpGet("UserKeys")]
        public async Task<IActionResult> getKeyStats(int userId)
        {
            List<UserKeyStat> result = await dbContext.UserKeyStats.Where(t => t.UserId == userId).ToListAsync();

            if (result.Count == 0)
            {
                return NotFound(new { Message = "No bigraph data found for user." });
            }

            List<UserKeyStatDTO> formattedResult = result.Select(t => convertToKeyStatDTO(t)).ToList();

            return Ok(result);

        }

        [HttpGet("UserStats")]
        public async Task<IActionResult> getUserStats(int userId)
        {
            UserStat result = await dbContext.UserStats.FirstOrDefaultAsync(t => t.UserId == userId);

            if (result == null)
            {
                return NotFound(new { Message = "No stat data found for user." });
            }

            return Ok(result);

        }
    }
}
