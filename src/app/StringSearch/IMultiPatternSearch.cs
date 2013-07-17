using System;
using System.Collections.Generic;

namespace StringSearch
{
    /// <summary>
    /// This interface defines a contract for string searching strategies
    /// on multiple string patterns of variable length
    /// </summary>
    public interface IMultiPatternSearch
    {
        /// <summary>
        /// Build an internal data structure
        /// </summary>
        /// <param name="patterns">patterns to search for with exact match 
        /// of the whole pattern</param>
        void Init(List<string> patterns);

        /// <summary>
        /// Search input text for patterns matching
        /// </summary>
        /// <param name="input">Text where search shall be performed</param>
        /// <returns>Positions of the found matches and the pattern value</returns>
        IEnumerable<Tuple<int, string>> FindAll(string input);
    }
}