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
         
            int minChars = 142;
            int maxChars = 144;
            int totalCharCount = 0;
            List<string> selectedWords = [];
            //List<string> randomWords = await dbContext.Words
            //    .OrderBy(w => Guid.NewGuid())
            //    .Select(w => w.Word1)
            //    .Take(200)
            //    .ToListAsync();

            var randomWords = wordCacheService.GetRandomWords(200);

            foreach (string word in randomWords)
            {
                // Add 1 to denote spaces
                string workingWord = word + "1";
                totalCharCount += workingWord.Length;

                if (totalCharCount > maxChars)
                {
                    break;
                }

                selectedWords.AddRange(workingWord.Select(c => c.ToString()));

                if (totalCharCount >= minChars && totalCharCount <= maxChars)
                {
                    break;
                }
            }

            // Return WordTestObjects list
            return Ok(convertToWordTestObjects(selectedWords));
            

        }
    }
}
