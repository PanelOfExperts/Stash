using System.Collections;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Stash.@internal
{
    /// <summary>
    ///     Helper functions useful for implementing <see cref="object.ToString()" />.
    ///     Specification by example:
    ///     <para>
    ///         <code>
    ///             // Returns "ClassName{}"
    ///             ToStringHelper(this).ToString();
    ///         </code>
    ///         <code>
    ///             // Returns "ClassName{x=1}"
    ///             MoreObjects.ToStringHelper(this)
    ///             .Add("x", 1)
    ///             .ToString();
    ///         </code>
    ///         <code>
    ///             // Returns "MyObject{x=1}"
    ///             MoreObjects.ToStringHelper("MyObject")
    ///             .Add("x", 1)
    ///             .ToString();
    ///         </code>
    ///         <code>
    ///             // Returns "ClassName{x=1, y=foo}"
    ///             MoreObjects.ToStringHelper(this)
    ///             .Add("x", 1)
    ///             .Add("y", "foo")
    ///             .ToString();
    ///         </code>
    ///         <code>
    ///             // Returns "ClassName{x=1}"
    ///             MoreObjects.ToStringHelper(this)
    ///             .OmitNullValues()
    ///             .Add("x", 1)
    ///             .Add("y", null)
    ///             .ToString();
    ///         </code>
    ///     </para>
    ///     Note that class names may be obfuscated.
    /// </summary>
    public abstract class ToStringHelper
    {
        private ValueHolder _holderHead = new ValueHolder();
        private ValueHolder _holderTail = new ValueHolder();
        private bool _omitNullValues;

        /// <summary>
        ///     Creates an instance of <see cref="ToStringHelper" />.
        /// </summary>
        /// <param name="self">
        ///     The object to generate the string for (typically <see langword="this" />),
        ///     used only for its class name.
        /// </param>
        public static ToStringHelper GetInstance(object self)
        {
            return GetInstance(self.GetType().Name);
        }

        /// <summary>
        ///     Creates an instance of <see cref="ToStringHelper" /> in the same manner as
        ///     <see cref="GetInstance(object)" />, but using <paramref name="className" />
        ///     instead of using an instance's type.
        /// </summary>
        /// <param name="className">The name of the instance type.</param>
        public static ToStringHelper GetInstance(string className)
        {
            return new Implementation(className);
        }

        /// <summary>
        ///     Returns a string in the format specified by <see cref="ToStringHelper" />.
        ///     After calling this method, you can keep adding more properties to later
        ///     call <see cref="M:ToString()" /> again and get a more complete representation
        ///     of the same object; but properties cannot be removed, so this only allows
        ///     limited reuse of the helper instance. The helper allows duplication of
        ///     properties (multiple name/value pairs with the same name can be added).
        /// </summary>
        public new abstract string ToString();

        /// <summary>
        ///     Configures the <see cref="ToStringHelper" /> so
        ///     <see cref="ToString()" /> will ignore properties
        ///     with null value.  The order of calling this method,
        ///     relative to the <see cref="M:Add" />/<see cref="M:AddValue" />
        ///     methods, is not significant.
        /// </summary>
        public ToStringHelper OmitNullValues()
        {
            _omitNullValues = true;
            return this;
        }

        /// <summary>
        ///     Adds a name/value pair to the formatted output in
        ///     <code>name=value</code> format.  If <paramref name="value" />
        ///     is <see langword="null" />, the string "null" is used, unless
        ///     <see cref="OmitNullValues" /> is called, in which case
        ///     this name/value pair will not be added.
        /// </summary>
        public ToStringHelper Add(string name, object value = null)
        {
            return AddHolder(name, value);
        }

        /// <summary>
        ///     Adds a name/value pair to the formatted output in <code>name=value</code> format.
        /// </summary>
        public ToStringHelper Add(string name, bool value)
        {
            return AddHolder(name, value.ToString());
        }

        /// <summary>
        ///     Adds a name/value pair to the formatted output in <code>name=value</code> format.
        /// </summary>
        public ToStringHelper Add(string name, char value)
        {
            return AddHolder(name, value.ToString());
        }

        /// <summary>
        ///     Adds a name/value pair to the formatted output in <code>name=value</code> format.
        /// </summary>
        public ToStringHelper Add(string name, double value)
        {
            return AddHolder(name, value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        ///     Adds a name/value pair to the formatted output in <code>name=value</code> format.
        /// </summary>
        public ToStringHelper Add(string name, float value)
        {
            return AddHolder(name, value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        ///     Adds a name/value pair to the formatted output in <code>name=value</code> format.
        /// </summary>
        public ToStringHelper Add(string name, int value)
        {
            return AddHolder(name, value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        ///     Adds a name/value pair to the formatted output in <code>name=value</code> format.
        /// </summary>
        public ToStringHelper Add(string name, long value)
        {
            return AddHolder(name, value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        ///     Adds an unnamed value to the formatted output.
        ///     It is strongly encouraged to use <see cref="M:Add(string, object)" />
        ///     instead and give value a readable name.
        /// </summary>
        public ToStringHelper AddValue(object value)
        {
            return AddHolder(value);
        }

        /// <summary>
        ///     Adds an unnamed value to the formatted output.
        ///     It is strongly encouraged to use <see cref="M:Add(bool, object)" />
        ///     instead and give value a readable name.
        /// </summary>
        public ToStringHelper AddValue(bool value)
        {
            return AddHolder(value);
        }

        /// <summary>
        ///     Adds an unnamed value to the formatted output.
        ///     It is strongly encouraged to use <see cref="M:Add(char, object)" />
        ///     instead and give value a readable name.
        /// </summary>
        public ToStringHelper AddValue(char value)
        {
            return AddHolder(value);
        }

        /// <summary>
        ///     Adds an unnamed value to the formatted output.
        ///     It is strongly encouraged to use <see cref="M:Add(double, object)" />
        ///     instead and give value a readable name.
        /// </summary>
        public ToStringHelper AddValue(double value)
        {
            return AddHolder(value);
        }

        /// <summary>
        ///     Adds an unnamed value to the formatted output.
        ///     It is strongly encouraged to use <see cref="M:Add(float, object)" />
        ///     instead and give value a readable name.
        /// </summary>
        public ToStringHelper AddValue(float value)
        {
            return AddHolder(value);
        }

        /// <summary>
        ///     Adds an unnamed value to the formatted output.
        ///     It is strongly encouraged to use <see cref="M:Add(int, object)" />
        ///     instead and give value a readable name.
        /// </summary>
        public ToStringHelper AddValue(int value)
        {
            return AddHolder(value);
        }

        /// <summary>
        ///     Adds an unnamed value to the formatted output.
        ///     It is strongly encouraged to use <see cref="M:Add(long, object)" />
        ///     instead and give value a readable name.
        /// </summary>
        public ToStringHelper AddValue(long value)
        {
            return AddHolder(value);
        }

        private ValueHolder AddHolder()
        {
            var valueHolder = new ValueHolder();
            _holderTail = _holderTail.Next = valueHolder;
            return valueHolder;
        }

        private ToStringHelper AddHolder(object value)
        {
            var valueHolder = AddHolder();
            valueHolder.Value = value;
            return this;
        }

        private ToStringHelper AddHolder(string name, object value = null)
        {
            var valueHolder = AddHolder();
            valueHolder.Value = value;
            valueHolder.Name = Preconditions.CheckNotNull(name);
            return this;
        }

        private class Implementation : ToStringHelper
        {
            private readonly string _className;

            internal Implementation(string className)
            {
                _className = Preconditions.CheckNotNull(className);
                _holderHead = _holderTail;
            }

            public override string ToString()
            {
                // create a copy to keep it consistent in case value changes
                var omitNullValuesSnapshot = _omitNullValues;
                var nextSeparator = "";
                var builder = new StringBuilder(32)
                    .Append(_className)
                    .Append('{');

                for (var valueHolder = _holderHead.Next; valueHolder != null; valueHolder = valueHolder.Next)
                {
                    var value = valueHolder.Value;
                    if (!omitNullValuesSnapshot || value != null)
                    {
                        builder.Append(nextSeparator);
                        nextSeparator = ", ";

                        if (valueHolder.Name != null)
                        {
                            builder.Append(valueHolder.Name).Append('=');
                        }


                        if (value != null && value.GetType().IsArray)
                        {
                            builder.Append(ArrayToString(value));
                        }
                        else
                        {
                            builder.Append(value);
                        }
                    }
                }
                return builder.Append('}').ToString();
            }

            private string ArrayToString(object value)
            {
                var strings = ((IEnumerable) value)
                    .Cast<object>()
                    .Select(x => x?.ToString() ?? x)
                    .ToArray();

                return string.Join(", ", strings);
            }
        }

        private sealed class ValueHolder
        {
            public string Name { get; set; }
            public ValueHolder Next { get; set; }
            public object Value { get; set; }
        }
    }
}