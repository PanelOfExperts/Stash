namespace Stash.fluent
{
    public interface IPartOfSpeech
    {
        ICache Cache { get; }
    }

    public interface IPrepositionOrConjunction : IPartOfSpeech
    {
    }

    public interface IPronounOrConjunction : IPartOfSpeech
    {
    }

    public interface IPronoun : IPronounOrConjunction
    {
    }

    public interface IPreposition : IPrepositionOrConjunction
    {
    }

    public interface IConjunction : IPrepositionOrConjunction, IPronounOrConjunction
    {
    }

    public class PartOfSpeech : IPartOfSpeech
    {
        public PartOfSpeech(ICache cacheToModify)
        {
            Cache = cacheToModify;
        }

        public ICache Cache { get; }
    }
    
    // Which
    // A pronoun takes the place of a noun
    public class Pronoun : PartOfSpeech, IPronoun
    {
        public Pronoun(ICache cacheToModify) : base(cacheToModify)
        {
        }
    }

    // With
    // A preposition "stands before"
    public class Preposition : PartOfSpeech, IPreposition
    {
        public Preposition(ICache cacheToModify) : base(cacheToModify)
        {
        }
    }

    // And
    // A conjunction joins words together
    public class Conjunction : PartOfSpeech, IConjunction
    {
        public Conjunction(ICache cacheToModify) : base(cacheToModify)
        {
        }
    }
}