using System;
using Stash.rules;

namespace Stash.caches
{
    public class Ticket : ICacheEntry
    {
        private readonly ExpirationRules _rules;
        private object _value;

        public Ticket(string key, ExpirationRules rules)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException(Strings.EXCEPTION_KeyCannotBeNull);
            if (rules==null)
                throw new ArgumentException(Strings.EXCEPTION_RulesCannotBeNull);

            Key = key;
            Created = DateTime.UtcNow;
            LastAccessed = DateTime.UtcNow;
            _rules = rules;
        }

        public string Key { get; set; }

        public DateTime Created { get; }

        public DateTime LastAccessed { get; private set; }

        public DateTime Expiration { get; set; }

        public object Value
        {
            get
            {
                UpdateExpiration();
                return _value;
            }
            set
            {
                UpdateExpiration();
                _value = value;
            }
        }

        private void UpdateExpiration()
        {
            LastAccessed = DateTime.UtcNow;
            Expiration = _rules.GetNewExpiration(LastAccessed);
        }
    }
}