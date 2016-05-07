using System;

namespace Stash.caches
{
    public class Ticket
    {
        private readonly object _value;

        public Ticket(object value)
        {
            Created = DateTime.UtcNow;
            LastAccessed = DateTime.UtcNow;
            _value = value;
        }

        public DateTime Created { get; }

        public DateTime LastAccessed { get; private set; }

        public object Value
        {
            get
            {
                LastAccessed = DateTime.UtcNow;
                return _value;
            }
        }
    }
}