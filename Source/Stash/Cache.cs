using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stash
{
    public interface ICache
    {
        TValue Get<TValue>(string key, Func<TValue> getter);
    }

    public class CacheFactory
    {
        public virtual ICache GetNullCache()
        {
            return new NullCache();
        }
    }

    internal class NullCache : ICache
    {
        public TValue Get<TValue>(string key, Func<TValue> getter)
        {
            return getter();
        }
    }
}
