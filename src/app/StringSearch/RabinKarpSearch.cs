using System;
using System.Collections.Generic;

namespace StringSearch
{
    /// <summary>
    /// Implementation of a Rabin-Karp multi pattern match algorithm. 
    /// String patterns can be of variable length.
    /// This class is not thread safe
    /// </summary>
    public class RabinKarpSearch : IMultiPatternSearch
    {
        // magic values used for rolling hashing
        private const int B = 257;
        private const int M = 8300009;
        // where e depends on min length of all patterns and 
        // is calculated as e = B^{m-1} mod M
        private int e;

        // start position of a match
        private int matchIndex;
        // pattern that matches
        private string matchPattern;
        // min length of all patterns
        private int minPatternLength;
        // checks if Init method was called before FindAll
        private bool wasInitialised;
        // contains rolling hash code and pattern to match
        private readonly Dictionary<int, string> patternDictionary;
        // cache of the text that is searched
        private char[] cachedTextToSearch;

        /// <summary>
        /// Constructor creates a new instance of a class
        /// </summary>
        public RabinKarpSearch()
        {
            e = 1;
            matchIndex = -1;
            wasInitialised = false;
            minPatternLength = Int32.MaxValue;
            patternDictionary = new Dictionary<int, string>();
        }

        /// <summary>
        /// Build an internal data structure that contains search patterns
        /// </summary>
        /// <param name="patterns">strings to search for</param>
        public void Init(List<string> patterns)
        {
            if (patterns == null)
                throw new ArgumentNullException("patterns");
            if (patterns.Count == 0)
                throw new ArgumentException(
                    "patterns shall not be an empty list");

            // find pattern with min length
            foreach (string pattern in patterns)
            {
                if (string.IsNullOrEmpty(pattern))
                    throw new ArgumentException(
                        "single pattern in a list cannot be null or empty");

                if (pattern.Length < minPatternLength)
                {
                    minPatternLength = pattern.Length;
                }
            }

            // build an internal dictionary with rolling hashes and patterns 
            foreach (string pattern in patterns)
            {
                int rollingHashCode = GetRollingHashCode(pattern);
                patternDictionary.Add(rollingHashCode, pattern);
            }

            // e = B^{m-1} mod M
            for (int i = 0; i < minPatternLength - 1; i++)
            {
                e = (e * B) % M;
            }

            wasInitialised = true;
        }

        /// <summary>
        /// Search input text for patterns matching
        /// </summary>
        /// <param name="input">Text where search shall be performed</param>
        /// <returns>Positions of the matches and the pattern that matched</returns>
        public IEnumerable<Tuple<int, string>> FindAll(string input)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentNullException("input");
            if (!wasInitialised)
                throw new InvalidOperationException(
                    "The class has not been initialised. " +
                    "Maybe you forgot to call an Init method?");

            int position = -1;

            // enumerate all matches as a C# iterator
            while (HasMatch(input, position + 1))
            {
                position = matchIndex;
                var tuple = new Tuple<int, string>(matchIndex, matchPattern);
                yield return tuple;
            }
        }

        /// <summary>
        /// Entry private method for search
        /// </summary>
        /// <param name="text">String to search in</param>
        /// <param name="fromIndex">current index to start search from</param>
        /// <returns><value>true</value> if match found, 
        /// othervise <value>false</value></returns>
        private bool HasMatch(string text, int fromIndex)
        {
            // pattern is larger then the text chunk
            if (text.Length - fromIndex < minPatternLength)
            {
                return false;
            }

            // split the text into char array and cache it
            if (cachedTextToSearch == null)
            {
                cachedTextToSearch = text.ToCharArray();
            }

            // search cached text based on position
            return HasMatch(cachedTextToSearch, fromIndex);
        }

        /// <summary>
        /// Private method to search text starting from the specified position
        /// </summary>
        /// <param name="chars">text to search</param>
        /// <param name="fromIndex">position to start search from</param>
        /// <returns><value>true</value> if match found, 
        /// othervise <value>false</value></returns>
        private bool HasMatch(char[] chars, int fromIndex)
        {
            // initially no match
            matchIndex = -1;
            // get length of the chunk that is searched
            int len = chars.Length;

            // min pattern length is larger then the text chunk
            if (len - fromIndex < minPatternLength)
            {
                // all patterns are of greater length, no match
                return false;
            }

            // calculate the rolling hash code of the chunk
            int rollingHashCode = 0;
            for (int i = 0; i < minPatternLength; i++)
            {
                char c = chars[fromIndex + i];
                rollingHashCode = (rollingHashCode * B + c) % M;
            }

            // now we have hashcode, let's check if our internal dictionary 
            // contains this hash code
            if (patternDictionary.ContainsKey(rollingHashCode))
            {
                // there is a match
                // get the string value of a pattern
                string pattern = patternDictionary[rollingHashCode];

                // assert char-by-char match
                if (HasMatch(chars, fromIndex, pattern))
                {
                    // char-by-char match succeeded, we found it
                    return true;
                }
            }

            // iterate, start from current search position plus minimum pattern
            // length (because last search would check beyond the current 
            // index for the min pattern len) and then iterate until end
            for (int i = minPatternLength + fromIndex; i < len; i++)
            {
                // get chars that mark start and stop position of the min pattern length
                char c1 = chars[i - minPatternLength];
                char c2 = chars[i];

                // calculate rolling hash code
                rollingHashCode = (B * (rollingHashCode - ((c1 * e) % M)) + c2) % M;
                if (rollingHashCode < 0)
                {
                    rollingHashCode += M;
                }

                // now we have hashcode, let's check if our internal dictionary 
                // contains this hash code
                if (patternDictionary.ContainsKey(rollingHashCode))
                {
                    // there is a match
                    // get the string value of a pattern
                    string pattern = patternDictionary[rollingHashCode];

                    // assert char-by-char match
                    if (HasMatch(chars, 1 + i - minPatternLength, pattern))
                    {
                        // char-by-char match succeeded, we found it
                        return true;
                    }
                }
            }

            // no match found
            return false;
        }

        /// <summary>
        /// Private method to perform char-by-char comparison of the text with
        /// the pattern. This method is called when rolling hash codes match 
        /// to verify it's real match and not hash collision.
        /// </summary>
        /// <param name="text">String to search in</param>
        /// <param name="startIndex">current index to start search from</param>
        /// <param name="pattern">pattern that is suspected to match the text 
        /// (usually at start location)</param>
        /// <returns><value>true</value> if match found, 
        /// othervise <value>false</value></returns>
        private bool HasMatch(char[] text, int startIndex, string pattern)
        {
            int len = pattern.Length;

            // pattern is larger then the text chunk
            if (text.Length - startIndex < len)
            {
                // no match
                return false;
            }

            // start comparing char-by-char incrementally
            for (int i = 0; i < len; ++i)
            {
                char c = text[startIndex + i];
                if (c != pattern[i])
                {
                    // there was a difference, hence no match
                    return false;
                }
            }

            // successfully matched, remember the index and pattern
            matchIndex = startIndex;
            matchPattern = pattern;
            return true;
        }

        /// <summary>
        /// Calculates rolling hash code for a string
        /// </summary>
        /// <param name="pattern">String for wich rolling hash code 
        /// to be calculated</param>
        /// <returns>Hash code</returns>
        private int GetRollingHashCode(string pattern)
        {
            if (string.IsNullOrEmpty(pattern))
                throw new ArgumentNullException("pattern");

            // calculate the rolling hash code value of the pattern
            int rollingHashCode = 0;
            for (int i = 0; i < minPatternLength; i++)
            {
                int c = pattern[i];
                rollingHashCode = (rollingHashCode * B + c) % M;
            }

            return rollingHashCode;
        }
    }
}