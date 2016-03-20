using System;
using System.Diagnostics;

namespace Stash.@internal
{
    /// <summary>
    ///     Static convenience methods that help a method or constructor check whether it was invoked
    ///     correctly (whether its <i>preconditions</i> have been met). These methods generally accept a
    ///     <see cref="bool" /> expression which is expected to be <see langword="true" />
    ///     (or in the case of <see cref="M:CheckNotNull" />, an object reference which is expected
    ///     to be non-null). When <see langword="false" /> (or <see langword="null" />) is passed instead,
    ///     the method throws an unchecked exception, which helps the calling method communicate that the
    ///     caller has made a mistake.
    ///     <example>
    ///         As an example:
    ///         <code>
    ///             // Returns the positive square root of a given value.
    ///             public static double sqrt(double value)
    ///             {
    ///                 Preconditions.CheckArgument(value &gt;= 0.0, "negative value: {0}", value);
    ///                 // Calculate the square root.
    ///             }
    /// 
    ///             void ExampleBadCaller()
    ///             {
    ///                 var d = sqrt(-1.0);
    ///             }
    ///         </code>
    ///         In this example, <see cref="M:CheckArgument" /> throws an <see cref="ArgumentException" /> to
    ///         indicate that <c>ExampleBadCaller</c> made an error in its call to <c>sqrt</c>.
    ///     </example>
    ///     <h3>Warning about performance:</h3>
    ///     <para>
    ///         The goal of this class is to improve readability of code, but in some circumstances this may
    ///         come at a significant performance cost. Remember that parameter values for message construction
    ///         must all be computed eagerly, and autoboxing and varargs array creation may happen as well, even
    ///         when the precondition check then succeeds(as it should almost always do in production). In some
    ///         circumstances these wasted CPU cycles and allocations can add up to a real problem.
    ///         Performance-sensitive precondition checks can always be converted to the customary form:
    ///     </para>
    ///     <code>
    ///         if (value &lt; 0.0)
    ///         {
    ///             throw new ArgumentException("negative value: " + value);
    ///         }
    ///     </code>
    /// </summary>
    //[DebuggerStepThrough]
    internal static class Preconditions
    {
        /// <summary>
        ///     Ensures that an object reference passed as a parameter to the calling method is not null.
        /// </summary>
        /// <param name="reference">An object reference.</param>
        /// <param name="argumentName">The name of the argument to display in an exception, if one is thrown.</param>
        /// <returns>The non-null reference that was validated.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="reference" /> is null.</exception>
        public static T CheckNotNull<T>(T reference, string argumentName = null)
        {
            if (reference != null) return reference;

            throw new ArgumentNullException(argumentName);
        }

        /// <summary>
        ///     Ensures the truth of an expression involving one or more parameters to the calling method.
        /// </summary>
        /// <param name="expression">A boolean expression.</param>
        /// <exception cref="ArgumentException">If <paramref name="expression" /> is false.</exception>
        public static void CheckArgument(bool expression)
        {
            CheckArgument(expression, null, null);
        }

        /// <summary>
        ///     Ensures the truth of an expression involving one or more parameters to the calling method.
        /// </summary>
        /// <param name="expression">A boolean expression.</param>
        /// <param name="errorMessageTemplate">A template for the exception message should the check fail.</param>
        /// <param name="errorMessageArgs">The arguments to be substituted into the message template.</param>
        /// <exception cref="ArgumentException">If <paramref name="expression" /> is false.</exception>
        public static void CheckArgument(bool expression, string errorMessageTemplate, params object[] errorMessageArgs)
        {
            if (expression) return;

            if (string.IsNullOrEmpty(errorMessageTemplate))
                throw new ArgumentException("Invalid argument: expression is false.");

            throw new ArgumentException(string.Format(errorMessageTemplate, errorMessageArgs));
        }

        /// <summary>
        ///     Ensures the truth of an expression involving the state of the calling instance,
        ///     but not involving any parameters to the calling method.
        /// </summary>
        /// <param name="expression">A boolean expression.</param>
        /// <exception cref="Exception">If <paramref name="expression" /> is false.</exception>
        public static void CheckState(bool expression)
        {
            CheckState(expression, null, null);
        }

        /// <summary>
        ///     Ensures the truth of an expression involving the state of the calling instance,
        ///     but not involving any parameters to the calling method.
        /// </summary>
        /// <param name="expression">A boolean expression.</param>
        /// <param name="errorMessageTemplate">A template for the exception message should the check fail.</param>
        /// <param name="errorMessageArgs">The arguments to be substituted into the message template.</param>
        /// <exception cref="Exception">If <paramref name="expression" /> is false.</exception>
        public static void CheckState(bool expression, string errorMessageTemplate, params object[] errorMessageArgs)
        {
            if (expression) return;

            if (string.IsNullOrEmpty(errorMessageTemplate))
                throw new ArgumentException("Invalid state: expression is false.");

            throw new ArgumentException(string.Format(errorMessageTemplate, errorMessageArgs));
        }
    }
}