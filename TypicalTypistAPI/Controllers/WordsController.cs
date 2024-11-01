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
        static List<WordTestObject> convertToWordTestObjects(List<string> strs)
        {
            List<WordTestObject> wordTestObjects = new List<WordTestObject>();
            List<char> currentWordChars = new List<char>();
            int startIndex = 0;

            for (int i = 0; i < strs.Count; i++)
            {
                char s = char.Parse(strs[i]);

                if (s == ' ')
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
                string workingWord = word + " ";
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

        [HttpGet("RandomCaps")]
        public IActionResult getRandomCapsWords()
        {
            int desiredChars = 144;
            int totalCharCount = 0;
            List<string> selectedWords = [];

            var randomWords = wordCacheService.GetRandomWords(200);

            foreach (string word in randomWords)
            {
                string workingWord = (char.ToUpper(word[0]) + word.Substring(1)) + " ";
                if (totalCharCount + workingWord.Length <= desiredChars)
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

        [HttpGet("RandomNumbers")]
        public IActionResult getRandomWordsAndNumbers()
        {
            int desiredChars = 144;
            int totalCharCount = 0;
            string numString = string.Empty;
            List<string> selectedWords = [];

            var randomWords = wordCacheService.GetRandomWords(200);

            foreach (string word in randomWords)
            {
                // 33% chance to populate a num
                if(_rng.Next(0, 3) == 2)
                {
                    // Decide string length
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
                if (totalCharCount + workingWord.Length <= desiredChars)
                {
                    totalCharCount += workingWord.Length;
                    selectedWords.AddRange(workingWord.Select(c => c.ToString()));
                }

                if(numString != string.Empty)
                {
                    if(totalCharCount + numString.Length <= desiredChars)
                    {
                        totalCharCount += numString.Length;
                        selectedWords.AddRange(numString.Select(c => c.ToString()));
                    }
                    numString = string.Empty;
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
