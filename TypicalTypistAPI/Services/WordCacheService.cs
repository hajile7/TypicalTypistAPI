using Microsoft.EntityFrameworkCore;
using TypicalTypistAPI.Models;

namespace TypicalTypistAPI.Services
{
    public class WordCacheService
    {
        private readonly List<string> cachedWords = [];
        private readonly object _lock = new();
        private static readonly Random _rng = new();

        public WordCacheService() 
        {
            cachedWords = [];
        }

        public async Task LoadWordsAsync(TypicalTypistDbContext dbContext)
        {

            lock (_lock)
            {
                if (cachedWords.Count != 0) return;
            }

            var words = await dbContext.Words
                .Select(w => w.Word1)
                .ToListAsync();

            lock (_lock)
            {
                cachedWords.AddRange(words);
            }

        }

        public List<string> GetRandomWords(int count)
        {
            lock ( _lock)
            {
                var wordsCopy = cachedWords.ToList();
                for(int i = wordsCopy.Count - 1; i > 0; i--)
                {
                    int j = _rng.Next(i + 1);
                    (wordsCopy[i], wordsCopy[j]) = (wordsCopy[j], wordsCopy[i]);
                }

                return wordsCopy.Take(count).ToList();

            }
        }


    }
}
