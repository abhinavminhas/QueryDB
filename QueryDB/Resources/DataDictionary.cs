using System.Collections.Generic;

namespace QueryDB.Resources
{
    /// <summary>
    /// Data dictionary to hold table data.
    /// </summary>
    public class DataDictionary
    {
        /// <summary>
        /// Holds table data values.
        /// </summary>
        internal IDictionary<string, string> Data { get; private set; }

        /// <summary>
        /// Creates data dictionary to hold table data.
        /// </summary>
        public DataDictionary()
        {
            Data = new Dictionary<string, string>();
        }
    }
}
