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
        private readonly WordCacheService wordCacheService;
        private static readonly Random _rng = new();

        public WordsController(WordCacheService _wordCacheService)
        {
            wordCacheService = _wordCacheService;
        }

        // Helper functions
        static List<WordTestObject> convertToWordTestObjects(List<char> strs)
        {
            List<WordTestObject> wordTestObjects = new List<WordTestObject>();
            List<char> currentWordChars = [];
            int startIndex = 0;

            for (int i = 0; i < strs.Count; i++)
            {

                if (strs[i] == ' ')
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
                    currentWordChars.Add(strs[i]);
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
            var randomWords = wordCacheService.GetRandomWords(200);
            int totalWordCount = 0;
            List<char> selectedWords = [];

            foreach (string word in randomWords)
            {
                string workingWord = word + " ";
                selectedWords.AddRange(workingWord.Select(c => c));
                totalWordCount++;

                if (totalWordCount == 50)
                {
                    break;
                }
            }

            // Return WordTestObjects list
            return Ok(convertToWordTestObjects(selectedWords));

        }

        [HttpGet("RandomCaps")]
        public IActionResult getRandomCapsWords()
        {
            var randomWords = wordCacheService.GetRandomWords(200);
            int totalWordCount = 0;
            List<char> selectedWords = [];

            foreach (string word in randomWords)
            {
                string workingWord = (char.ToUpper(word[0]) + word.Substring(1)) + " ";
                selectedWords.AddRange(workingWord.Select(c => c));

                if (totalWordCount == 50)
                {
                    break;
                }
            }

            // Return WordTestObjects list
            return Ok(convertToWordTestObjects(selectedWords));

        }

        [HttpGet("RandomNumbers")]
        public IActionResult getRandomWordsAndNumbers()
        {
            int totalWordCount = 0;
            string numString = string.Empty;
            List<char> selectedWords = [];

            var randomWords = wordCacheService.GetRandomWords(200);

            foreach (string word in randomWords)
            {
                // 33% chance to populate a num
                if(_rng.Next(0, 3) == 2)
                {
                    // Decide numString length (1 - 5)
                    int numStrLen = _rng.Next(1, 6);
                    
                    // Create numString
                    switch (numStrLen)
                    {
                        case 1:
                            numString = _rng.Next(1, 10).ToString() + " ";
                            break;
                        case 2:
                            numString = _rng.Next(1, 100).ToString() + " ";
                            break;
                        case 3:
                            numString = _rng.Next(1, 1000).ToString() + " ";
                            break;
                        case 4:
                            numString = _rng.Next(1, 10000).ToString() + " ";
                            break;
                        case 5:
                            numString = _rng.Next(1, 100000).ToString() + " ";
                            break;
                    }
                }

                string workingWord = word + " ";
                selectedWords.AddRange(workingWord.Select(c => c));
                totalWordCount++;

                if(numString != string.Empty && totalWordCount != 50)
                {
                    selectedWords.AddRange(numString.Select(c => c));
                    totalWordCount++;
                    numString = string.Empty;
                }

                if (totalWordCount == 50)
                {
                    break;
                }
            }

            // Return WordTestObjects list
            return Ok(convertToWordTestObjects(selectedWords));

        }
    }
}
