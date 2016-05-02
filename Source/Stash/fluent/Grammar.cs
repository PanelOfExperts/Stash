namespace Stash.fluent
{
    public static class Grammar
    {
        /// <summary>
        ///     Creates a Pronoun for the given cache, enabling the user to
        ///     wrap the object via fluent extension.
        /// </summary>
        /// <para>
        ///     Any member of a small class of words found in many languages that are used as replacements
        ///     or substitutes for nouns and noun phrases, and that have very general reference, as I, you,
        ///     he, this, who, what. Pronouns are sometimes formally distinguished from nouns, as in English
        ///     by the existence of special objective forms, as him for he or me for I, and by nonoccurrence
        ///     with an article or adjective.
        /// </para>
        public static IPronoun Which(this ICache cacheToModify)
        {
            return new Pronoun(cacheToModify);
        }
        
        /// <summary>
        ///     Creates a Conjunction for the given cache, enabling the user to
        ///     chain phrases of fluent extension.
        /// </summary>
        /// <para>
        ///     Any member of a small class of words distinguished in many languages by their function as
        ///     connectors between words, phrases, clauses, or sentences, as and, because, but, however.
        /// </para>
        public static IConjunction And(this ICache cacheToModify)
        {
            return new Conjunction(cacheToModify);
        }

        ///// <summary>
        /////     Creates a Preposition for the given cache, enabling the user to
        /////     modify properties of the object via fluent extension.
        ///// </summary>
        ///// <para>
        /////     any member of a class of words found in many languages that are used before nouns, pronouns,
        /////     or other substantives to form phrases functioning as modifiers of verbs, nouns, or adjectives,
        /////     and that typically express a spatial, temporal, or other relationship, as in, on, by, to, since.
        ///// </para>
        //public static IPreposition With(this ICache cacheToModify)
        //{
        //    return new Preposition(cacheToModify);
        //}
    }
}