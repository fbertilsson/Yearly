using System;

namespace YearlyBackend.Periodic
{
    public class TooFewEntriesException : Exception
    {
        public int MinEntries { get; }

        public TooFewEntriesException(int minEntries)
        {
            MinEntries = minEntries;
        }
    }
}