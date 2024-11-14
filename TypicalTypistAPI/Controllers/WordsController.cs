using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
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
        private static readonly StringBuilder sb = new StringBuilder(1, 6);
        private static readonly char[] LegalSymbols = new char[] { '.', ',', '\'', '"', '?', '!', '*', '=', '+', '-', '/', '\\', '<', '>', '(', ')', '{', '}',
            '[', ']', '^', '~', '%', '$', '#', '@', '`', '&', '|' };

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
        [HttpGet("Random/{x}")]
        public IActionResult GetRandomWords(int x)
        {
            var randomWords = wordCacheService.GetRandomWords(200);
            int totalWordCount = 0;
            List<char> selectedWords = [];

            foreach (string word in randomWords)
            {
                string workingWord = word + " ";
                selectedWords.AddRange(workingWord.Select(c => c));
                totalWordCount++;

                if (totalWordCount == x)
                {
                    break;
                }
            }
            return Ok(convertToWordTestObjects(selectedWords));
        }

        [HttpGet("RandomNums/{y}")]
        public IActionResult GetRandomWordsAndNums(int y)
        {
            int totalWordCount = 0;
            List<char> selectedWords = [];
            var randomWords = wordCacheService.GetRandomWords(200);

            foreach (string word in randomWords)
            {
                string workingWord = word + " ";
                selectedWords.AddRange(workingWord.Select(c => c));
                totalWordCount++;

                if (totalWordCount == y)
                {
                    break;
                }

                // 33% chance to populate a num string
                if (_rng.Next(0, 3) == 2)
                {
                    int numStrLen = _rng.Next(1, 6);
                    switch (numStrLen)
                    {
                        case 1:
                            sb.Append(_rng.Next(1, 10));
                            break;
                        case 2:
                            sb.Append(_rng.Next(1, 100));
                            break;
                        case 3:
                            sb.Append(_rng.Next(1, 1000));
                            break;
                        case 4:
                            sb.Append(_rng.Next(1, 10000));
                            break;
                        case 5:
                            sb.Append(_rng.Next(1, 100000));
                            break;
                    }

                    sb.Append(' ');
                    selectedWords.AddRange(sb.ToString().Select(c => c));
                    totalWordCount++;
                    sb.Clear();

                    if (totalWordCount == y)
                    {
                        break;
                    }
                }
            }
            return Ok(convertToWordTestObjects(selectedWords));
        }

        [HttpGet("RandomSymbols/{z}")]
        public IActionResult GetRandomWordsAndSymbols(int z)
        {
            int totalWordCount = 0;
            List<char> selectedWords = [];
            var randomWords = wordCacheService.GetRandomWords(200);

            foreach (string word in randomWords)
            {
                string workingWord = word + " ";
                selectedWords.AddRange(workingWord.Select(c => c));
                totalWordCount++;

                if (totalWordCount == z)
                {
                    break;
                }

                // 33% chance to populate symbol string
                if (_rng.Next(0, 3) == 2)
                {
                    int symStrLen = _rng.Next(1, 6);
                    switch (symStrLen)
                    {
                        case 1:
                            while(sb.Length < 1)
                            {
                                sb.Append(LegalSymbols[_rng.Next(0, LegalSymbols.Length)]);
                            }
                            break;
                        case 2:
                            while (sb.Length < 2)
                            {
                                sb.Append(LegalSymbols[_rng.Next(0, LegalSymbols.Length)]);
                            }
                            break;
                        case 3:
                            while (sb.Length < 3)
                            {
                                sb.Append(LegalSymbols[_rng.Next(0, LegalSymbols.Length)]);
                            }
                            break;
                        case 4:
                            while (sb.Length < 4)
                            {
                                sb.Append(LegalSymbols[_rng.Next(0, LegalSymbols.Length)]);
                            }
                            break;
                        case 5:
                            while (sb.Length < 5)
                            {
                                sb.Append(LegalSymbols[_rng.Next(0, LegalSymbols.Length)]);
                            }
                            break;
                    }

                    sb.Append(' ');
                    selectedWords.AddRange(sb.ToString().Select(c => c));
                    totalWordCount++;
                    sb.Clear();

                    if (totalWordCount == z)
                    {
                        break;
                    }
                }
            }
            return Ok(convertToWordTestObjects(selectedWords));
        }

        [HttpGet("RandomNumsAndSymbols/{ax}")]
        public IActionResult GetRandomWordsNumsAndSymbols(int ax)
        {
            int totalWordCount = 0;
            List<char> selectedWords = [];
            var randomWords = wordCacheService.GetRandomWords(200);

            foreach (string word in randomWords)
            {
                string workingWord = word + " ";
                selectedWords.AddRange(workingWord.Select(c => c));
                totalWordCount++;

                if (totalWordCount == ax)
                {
                    break;
                }

                // 50% chance to populate symbol or number string
                if (_rng.Next(2) == 1)
                {
                    // 50% chance of symbol string
                    if (_rng.Next(2) == 0)
                    {
                        int symStrLen = _rng.Next(1, 6);
                        switch (symStrLen)
                        {
                            case 1:
                                while (sb.Length < 1)
                                {
                                    sb.Append(LegalSymbols[_rng.Next(0, LegalSymbols.Length)]);
                                }
                                break;
                            case 2:
                                while (sb.Length < 2)
                                {
                                    sb.Append(LegalSymbols[_rng.Next(0, LegalSymbols.Length)]);
                                }
                                break;
                            case 3:
                                while (sb.Length < 3)
                                {
                                    sb.Append(LegalSymbols[_rng.Next(0, LegalSymbols.Length)]);
                                }
                                break;
                            case 4:
                                while (sb.Length < 4)
                                {
                                    sb.Append(LegalSymbols[_rng.Next(0, LegalSymbols.Length)]);
                                }
                                break;
                            case 5:
                                while (sb.Length < 5)
                                {
                                    sb.Append(LegalSymbols[_rng.Next(0, LegalSymbols.Length)]);
                                }
                                break;
                        }
                        sb.Append(' ');
                    }

                    // 50% chance of num string
                    else
                    {
                        int numStrLen = _rng.Next(1, 6);
                        switch (numStrLen)
                        {
                            case 1:
                                sb.Append(_rng.Next(1, 10));
                                break;
                            case 2:
                                sb.Append(_rng.Next(1, 100));
                                break;
                            case 3:
                                sb.Append(_rng.Next(1, 1000));
                                break;
                            case 4:
                                sb.Append(_rng.Next(1, 10000));
                                break;
                            case 5:
                                sb.Append(_rng.Next(1, 100000));
                                break;
                        }
                        sb.Append(' ');
                    }

                    selectedWords.AddRange(sb.ToString().Select(c => c));
                    totalWordCount++;
                    sb.Clear();

                    if (totalWordCount == ax)
                    {
                        break;
                    }
                }
            }
            return Ok(convertToWordTestObjects(selectedWords));
        }

        [HttpGet("RandomCaps/{ay}")]
        public IActionResult GetRandomCaps(int ay)
        {
            var randomWords = wordCacheService.GetRandomWords(200);
            int totalWordCount = 0;
            List<char> selectedWords = [];

            foreach (string word in randomWords)
            {
                string workingWord = (char.ToUpper(word[0]) + word[1..]) + " ";
                selectedWords.AddRange(workingWord.Select(c => c));
                totalWordCount++;

                if (totalWordCount == ay)
                {
                    break;
                }
            }
            return Ok(convertToWordTestObjects(selectedWords));
        }

        [HttpGet("RandomCapsNums/{az}")]
        public IActionResult GetRandomCapsAndNums(int az)
        {
            int totalWordCount = 0;
            List<char> selectedWords = [];
            var randomWords = wordCacheService.GetRandomWords(200);

            foreach (string word in randomWords)
            {
                string workingWord = (char.ToUpper(word[0]) + word.Substring(1)) + " ";
                selectedWords.AddRange(workingWord.Select(c => c));
                totalWordCount++;

                if (totalWordCount == az)
                {
                    break;
                }

                // 33% chance to populate num string
                if (_rng.Next(0, 3) == 2)
                {
                    int numStrLen = _rng.Next(1, 6);
                    switch (numStrLen)
                    {
                        case 1:
                            sb.Append(_rng.Next(1, 10));
                            break;
                        case 2:
                            sb.Append(_rng.Next(1, 100));
                            break;
                        case 3:
                            sb.Append(_rng.Next(1, 1000));
                            break;
                        case 4:
                            sb.Append(_rng.Next(1, 10000));
                            break;
                        case 5:
                            sb.Append(_rng.Next(1, 100000));
                            break;
                    }

                    sb.Append(' ');
                    selectedWords.AddRange(sb.ToString().Select(c => c));
                    totalWordCount++;
                    sb.Clear();

                    if (totalWordCount == az)
                    {
                        break;
                    }
                }
            }
            return Ok(convertToWordTestObjects(selectedWords));
        }

        [HttpGet("RandomCapsSymbols/{bx}")]
        public IActionResult GetRandomCapsAndSymbols(int bx)
        {
            int totalWordCount = 0;
            List<char> selectedWords = [];

            var randomWords = wordCacheService.GetRandomWords(200);

            foreach (string word in randomWords)
            {
                string workingWord = (char.ToUpper(word[0]) + word[1..]) + " ";
                selectedWords.AddRange(workingWord.Select(c => c));
                totalWordCount++;

                if (totalWordCount == bx)
                {
                    break;
                }

                // 33% chance to populate symbol string
                if (_rng.Next(0, 3) == 2)
                {
                    int symStrLen = _rng.Next(1, 6);
                    switch (symStrLen)
                    {
                        case 1:
                            while (sb.Length < 1)
                            {
                                sb.Append(LegalSymbols[_rng.Next(0, LegalSymbols.Length)]);
                            }
                            break;
                        case 2:
                            while (sb.Length < 2)
                            {
                                sb.Append(LegalSymbols[_rng.Next(0, LegalSymbols.Length)]);
                            }
                            break;
                        case 3:
                            while (sb.Length < 3)
                            {
                                sb.Append(LegalSymbols[_rng.Next(0, LegalSymbols.Length)]);
                            }
                            break;
                        case 4:
                            while (sb.Length < 4)
                            {
                                sb.Append(LegalSymbols[_rng.Next(0, LegalSymbols.Length)]);
                            }
                            break;
                        case 5:
                            while (sb.Length < 5)
                            {
                                sb.Append(LegalSymbols[_rng.Next(0, LegalSymbols.Length)]);
                            }
                            break;
                    }

                    sb.Append(' ');
                    selectedWords.AddRange(sb.ToString().Select(c => c));
                    totalWordCount++;
                    sb.Clear();

                    if (totalWordCount == bx)
                    {
                        break;
                    }
                }
            }
            return Ok(convertToWordTestObjects(selectedWords));
        }

        [HttpGet("RandomCapsNumsAndSymbols/{by}")]
        public IActionResult GetRandomCapsNumsAndSymbols(int by)
        {
            int totalWordCount = 0;
            List<char> selectedWords = [];
            var randomWords = wordCacheService.GetRandomWords(200);

            foreach (string word in randomWords)
            {
                string workingWord = (char.ToUpper(word[0]) + word[1..]) + " ";
                selectedWords.AddRange(workingWord.Select(c => c));
                totalWordCount++;

                if (totalWordCount == by)
                {
                    break;
                }

                // 50% chance to populate symbol or number string
                if (_rng.Next(2) == 1)
                {
                    // 50% chance of symbol string
                    if (_rng.Next(2) == 0)
                    {
                        int symStrLen = _rng.Next(1, 6);
                        switch (symStrLen)
                        {
                            case 1:
                                while (sb.Length < 1)
                                {
                                    sb.Append(LegalSymbols[_rng.Next(0, LegalSymbols.Length)]);
                                }
                                break;
                            case 2:
                                while (sb.Length < 2)
                                {
                                    sb.Append(LegalSymbols[_rng.Next(0, LegalSymbols.Length)]);
                                }
                                break;
                            case 3:
                                while (sb.Length < 3)
                                {
                                    sb.Append(LegalSymbols[_rng.Next(0, LegalSymbols.Length)]);
                                }
                                break;
                            case 4:
                                while (sb.Length < 4)
                                {
                                    sb.Append(LegalSymbols[_rng.Next(0, LegalSymbols.Length)]);
                                }
                                break;
                            case 5:
                                while (sb.Length < 5)
                                {
                                    sb.Append(LegalSymbols[_rng.Next(0, LegalSymbols.Length)]);
                                }
                                break;
                        }
                        sb.Append(' ');
                    }

                    // 50% chance of num string
                    else
                    {
                        int numStrLen = _rng.Next(1, 6);
                        switch (numStrLen)
                        {
                            case 1:
                                sb.Append(_rng.Next(1, 10));
                                break;
                            case 2:
                                sb.Append(_rng.Next(1, 100));
                                break;
                            case 3:
                                sb.Append(_rng.Next(1, 1000));
                                break;
                            case 4:
                                sb.Append(_rng.Next(1, 10000));
                                break;
                            case 5:
                                sb.Append(_rng.Next(1, 100000));
                                break;
                        }
                        sb.Append(' ');
                    }

                    selectedWords.AddRange(sb.ToString().Select(c => c));
                    totalWordCount++;
                    sb.Clear();

                    if (totalWordCount == by)
                    {
                        break;
                    }
                }
            }
            return Ok(convertToWordTestObjects(selectedWords));
        }

        [HttpGet("Chaotic/{bz}")]
        public IActionResult GetChaotic(int bz)
        {
            int totalWordCount = 0;
            StringBuilder capsBuilder = new(1, 11);
            List<char> selectedWords = [];

            var randomWords = wordCacheService.GetRandomWords(200);

            foreach (string word in randomWords)
            {
                // Handle random caps
                string workingWord = word + " ";
                foreach(char c in workingWord)
                {
                    capsBuilder.Append(_rng.Next(2) == 0 ? char.ToUpper(c) : char.ToLower(c));
                }
                selectedWords.AddRange(capsBuilder.ToString().Select(c => c));
                capsBuilder.Clear();
                totalWordCount++;

                if (totalWordCount == bz)
                {
                    break;
                }

                // 50% chance to populate symbol or number string
                if (_rng.Next(2) == 1)
                {
                    // 50% chance of symbol string
                    if(_rng.Next(2) == 0)
                    {
                        int symStrLen = _rng.Next(1, 6);
                        switch (symStrLen)
                        {
                            case 1:
                                while (sb.Length < 1)
                                {
                                    sb.Append(LegalSymbols[_rng.Next(0, LegalSymbols.Length)]);
                                }
                                break;
                            case 2:
                                while (sb.Length < 2)
                                {
                                    sb.Append(LegalSymbols[_rng.Next(0, LegalSymbols.Length)]);
                                }
                                break;
                            case 3:
                                while (sb.Length < 3)
                                {
                                    sb.Append(LegalSymbols[_rng.Next(0, LegalSymbols.Length)]);
                                }
                                break;
                            case 4:
                                while (sb.Length < 4)
                                {
                                    sb.Append(LegalSymbols[_rng.Next(0, LegalSymbols.Length)]);
                                }
                                break;
                            case 5:
                                while (sb.Length < 5)
                                {
                                    sb.Append(LegalSymbols[_rng.Next(0, LegalSymbols.Length)]);
                                }
                                break;
                        }
                        sb.Append(' ');
                    }

                    // 50% chance of num string
                    else
                    {
                        int numStrLen = _rng.Next(1, 6);
                        switch (numStrLen)
                        {
                            case 1:
                                sb.Append(_rng.Next(1, 10));
                                break;
                            case 2:
                                sb.Append(_rng.Next(1, 100));
                                break;
                            case 3:
                                sb.Append(_rng.Next(1, 1000));
                                break;
                            case 4:
                                sb.Append(_rng.Next(1, 10000));
                                break;
                            case 5:
                                sb.Append(_rng.Next(1, 100000));
                                break;
                        }
                        sb.Append(' ');
                    }           
                    
                    selectedWords.AddRange(sb.ToString().Select(c => c));
                    totalWordCount++;
                    sb.Clear();
                    
                    if (totalWordCount == bz)
                    {
                        break;
                    }
                }
            }
            return Ok(convertToWordTestObjects(selectedWords));
        }
    }
}
