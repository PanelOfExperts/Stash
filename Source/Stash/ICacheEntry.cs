namespace Stash
{
    public interface ICacheEntry
    {
        string Key { get; }
        object Value { get; }
    }
}