using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TypicalTypistAPI.Models;
using TypicalTypistAPI.Services;

namespace TypicalTypistAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WordsController : ControllerBase
    {
        private TypicalTypistDbContext dbContext = new TypicalTypistDbContext();
        private readonly WordCacheService wordCacheService;
        private static readonly Random _rng = new();

        public WordsController(WordCacheService _wordCacheService)
        {
            wordCacheService = _wordCacheService;
        }

        // Helper functions
        static List<WordTestObject> convertToWordTestObjects(List<string> strs)
        {
            List<WordTestObject> wordTestObjects = new List<WordTestObject>();
            List<char> currentWordChars = new List<char>();
            int startIndex = 0;

            for (int i = 0; i < strs.Count; i++)
            {
                char s = char.Parse(strs[i]);

                if (s == '1')
                {
                    if (currentWordChars.Count > 0)
                    {
                        wordTestObjects.Add(new WordTestObject { chars = currentWordChars, startIndex = startIndex });
                        currentWordChars = new List<char>();
                    }
                    startIndex = i + 1;
                }
                else
                {
                    if (currentWordChars.Count == 0)
                    {
                        startIndex = i;
                    }
                    currentWordChars.Add(s);
                }
            }
            if (currentWordChars.Count > 0)
            {
                wordTestObjects.Add(new WordTestObject { chars = currentWordChars, startIndex = startIndex });
            }
            return wordTestObjects;
        }

        // HTTP calls
        [HttpGet("Random")]
        public IActionResult getRandomWords()
        {
            int desiredChars = 144;
            int totalCharCount = 0;
            List<string> selectedWords = [];

            var randomWords = wordCacheService.GetRandomWords(200);

            foreach (string word in randomWords)
            {
                // Add 1 to denote spaces... process out in frontend (ProcessCheckTestInstance)
                string workingWord = word + "1";
                if(totalCharCount + workingWord.Length <= desiredChars)
                {
                    totalCharCount += workingWord.Length;
                    selectedWords.AddRange(workingWord.Select(c => c.ToString()));
                }

                if (totalCharCount == desiredChars || totalCharCount >= 142)
                {
                    break;
                }
            }

            // Return WordTestObjects list
            return Ok(convertToWordTestObjects(selectedWords));

        }
    }
}
