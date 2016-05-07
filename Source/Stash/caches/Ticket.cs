using System;

namespace Stash.caches
{
    public class Ticket
    {
        private readonly object _value;

        public Ticket(string key) :this(key, null)
        {

        }

        public Ticket(string key, object value)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException(Strings.EXCEPTION_KeyCannotBeNull);

            Key = key;
            Created = DateTime.UtcNow;
            LastAccessed = DateTime.UtcNow;
            _value = value;
        }

        public string Key { get; set; }

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