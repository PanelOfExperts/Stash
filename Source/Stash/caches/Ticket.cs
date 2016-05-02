using System;

namespace Stash.caches
{
    public class Ticket
    {
        //internal readonly Func<DateTime> Now = () => DateTime.UtcNow;
        private object _value;

        public Ticket()
        {
            CreationDate = DateTime.UtcNow;
            LastAccessedDate = DateTime.UtcNow;
        }

        public object Value
        {
            get
            {
                LastAccessedDate = DateTime.UtcNow;
                return _value;
            }
            set
            {
                LastAccessedDate = DateTime.UtcNow;
                _value = value;
            }
        }

        public DateTime CreationDate { get; }

        public DateTime LastAccessedDate { get; private set; }
    }
}