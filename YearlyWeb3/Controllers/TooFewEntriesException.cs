using System;

namespace YearlyWeb3.Controllers
{
    public class TooFewEntriesException : Exception
    {
        public int MinEntries { get; private set; }

        public TooFewEntriesException(int minEntries)
        {
            MinEntries = minEntries;
        }
    }
}