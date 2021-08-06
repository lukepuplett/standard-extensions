using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    /// <summary>
    /// Contains extension methods for types in the System namespace.
    /// </summary>
    /// <remarks>
    /// The class is named to prevent clashes with similarly named extension method
    /// classes in other projects.
    /// </remarks>
    public static class StandardExtensions_SystemExtensions
    {
        #region Object

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <param name="nullAsEmpty">if set to <c>true</c> [null as empty].</param>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes")]
        public static string ToString(this object o, bool nullAsEmpty)
        {
            if (o != null)
            {
                return o.ToString();
            }
            else
            {
                if (nullAsEmpty)
                    return String.Empty;

                throw new NullReferenceException("ToString called on a null instance.");
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="nullValue">The value to return if the string is null.</param>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        /// <param name="s">Instance</param>
        public static string ToString(this object s, string nullValue)
        {
            if (s == null)
                return nullValue;

            return s.ToString();
        }

        #endregion

        #region Boolean

        /// <summary>
        /// Returns one of two different strings depending on the value.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> as specified.
        /// </returns>
        /// <param name="trueString">The string to return if the value is true.</param>
        /// <param name="falseString">The string to return if the value is false.</param>
        public static string ToString(this bool value, string trueString, string falseString)
        {
            return value ? trueString : falseString;
        }

        #endregion

        #region DateTimeOffset

        /// <summary>
        /// Gets the difference between this instant and now, positive if instant has passed.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>A TimeSpan instance.</returns>
        public static TimeSpan DiffNow(this DateTimeOffset instance)
        {
            return DateTimeOffset.Now - instance;
        }

        /// <summary>
        /// Compares this DateTimeOffset with another for equality within a specified tolerance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="value">A DateTimeOffset instance to compare to.</param>
        /// <param name="milliseconds">The number of ms that are deemed within acceptable range either side.</param>
        /// <returns></returns>
        public static bool AlmostEquals(this DateTimeOffset instance, DateTimeOffset value, int milliseconds)
        {
            if (instance.ToString() != value.ToString()) { return false; }

            long absolute = (long)Math.Abs(instance.Millisecond - value.Millisecond);
            return (absolute < milliseconds);
        }

        /// <summary>
        /// Compares this DateTimeOffset with another for equality within a specified tolerance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="value">A DateTimeOffset instance to compare to.</param>
        /// <param name="tolerance">A TimeSpan deemed within acceptable range either side.</param>
        /// <returns></returns>
        public static bool AlmostEquals(this DateTimeOffset instance, DateTimeOffset value, TimeSpan tolerance)
        {
            bool result;
            TimeSpan difference = instance.Subtract(value);
            if (value > instance)
            {
                result = (difference.Negate() <= tolerance);
            }
            else
            {
                result = (difference <= tolerance);
            }
            return result;
        }

        /// <summary>
        /// Returns whether or not the date is on a weekend.
        /// </summary>
        public static bool IsWeekend(this DateTimeOffset instance)
        {
            return ((instance.DayOfWeek == DayOfWeek.Saturday) || (instance.DayOfWeek == DayOfWeek.Sunday));
        }

        /// <summary>
        /// Returns a new DateTimeOffset rounded back to the nearest specified division of time.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="grain">A TimeSpan representing one period of granularity, for example: an hour.</param>
        /// <returns></returns>
        public static DateTimeOffset RoundBack(this DateTimeOffset instance, TimeSpan grain)
        {
            // 18 round back to chunks of 5: 18 divided by 5 = 3 wholes, 3 * 5 = 15.
            //
            long wholeChunks = instance.Ticks / grain.Ticks; // Int64, so loses decimal precision, so always less (rounded back).
            var ticks = wholeChunks * grain.Ticks;
            return new DateTimeOffset(ticks, instance.Offset);
        }

        /// <summary>
        /// Returns a new DateTimeOffset rounded forward to the nearest specified division of time.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="grain">A TimeSpan representing one period of granularity, for example: an hour.</param>
        /// <returns></returns>
        public static DateTimeOffset RoundForward(this DateTimeOffset instance, TimeSpan grain)
        {
            return RoundBack(instance, grain) + grain;
        }

        /// <summary>
        /// Returns true if the DateTimeOffset instance represents a moment in the future.
        /// </summary>
        /// <returns>True if the DateTimeOffset instance is greater than now using UTC.</returns>
        public static bool IsInTheFuture(this DateTimeOffset instance)
        {
            return DateTimeOffset.UtcNow.CompareTo(instance.ToUniversalTime()) < 0;
        }

        /// <summary>
        /// Returns a user-friendly string representation of the DateTimeOffset.
        /// </summary>
        public static string ToCaption(this DateTimeOffset instance)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder(3);
            if (DateTimeOffset.UtcNow.AlmostEquals(instance, TimeSpan.FromSeconds(30)))
            {
                sb.Append("Now");
            }
            else if ((!instance.IsInTheFuture()) & DateTimeOffset.UtcNow.AlmostEquals(instance, TimeSpan.FromSeconds(59)))
            {
                TimeSpan dts = DateTimeOffset.Now - instance;

                sb.Append((int)dts.TotalSeconds);
                sb.Append(" seconds ago");
            }
            else if ((!instance.IsInTheFuture()) & DateTimeOffset.UtcNow.AlmostEquals(instance, TimeSpan.FromMinutes(59)))
            {
                TimeSpan dts = DateTimeOffset.Now - instance;

                sb.Append((int)dts.TotalMinutes);
                if ((int)dts.TotalMinutes == 1)
                    sb.Append(" minute ago");
                else
                    sb.Append(" minutes ago");
            }
            else if ((!instance.IsInTheFuture()) & DateTimeOffset.UtcNow.AlmostEquals(instance, TimeSpan.FromHours(6)))
            {
                TimeSpan dts = DateTimeOffset.Now - instance;

                sb.Append((int)dts.TotalHours);
                if ((int)dts.TotalHours == 1)
                    sb.Append(" hour ago");
                else
                    sb.Append(" hours ago");
            }
            else if (DateTimeOffset.UtcNow.DayOfYear == instance.DayOfYear)
            {
                sb.Append(instance.ToString("h:mm tt"));
                sb.Append(" today");
            }
            else if ((DateTimeOffset.UtcNow.DayOfYear - instance.DayOfYear) == 1)
            {
                sb.Append(instance.ToString("h:mm tt"));
                sb.Append(" yesterday");
            }
            else if ((!instance.IsInTheFuture()) && (DateTimeOffset.UtcNow - instance).TotalDays < 7)
            {
                sb.Append(instance.ToString("h:mm tt"));
                sb.Append(" ");
                sb.Append(instance.ToString("dddd"));
                sb.Append(" gone");
            }
            else if ((instance - DateTimeOffset.UtcNow).TotalDays < 7)
            {
                sb.Append(instance.ToString("h:mm tt"));
                sb.Append(" this ");
                sb.Append(instance.ToString("dddd"));
            }
            else if (DateTimeOffset.UtcNow.Year == instance.Year)
            {
                sb.Append(instance.ToString("h:mm tt mm MMM"));
            }
            else
            {
                sb.Append(instance.ToString("h:mm tt mm MMM yyyy"));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Returns the DateTimeOffset formatted as an ISO-8601 string.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>
        /// An ISO 8601 compliant string
        /// </returns>
        public static string ToISO8601String(this DateTimeOffset instance)
        {
            return ToISO8601String(instance, false);
        }

        /// <summary>
        /// Returns the DateTimeOffset formatted as an ISO-8601 string.
        /// </summary>
        /// <remarks>
        /// If the value is equivalent to UTC, that is, it has an offset of zero, then the string will return in
        /// UTC, that is, with a Z indicator. For all other values, the offset is returned in the string as per
        /// the ISO8601 standard.
        /// </remarks>
        /// <param name="instance">The instance.</param>
        /// <param name="includeMilliseconds">When set to <c>true</c> include milliseconds as a fraction of the
        /// seconds. Note that this will return the offset, even if the value happens to be UTC.</param>
        /// <returns>An ISO 8601 compliant string</returns>
        public static string ToISO8601String(this DateTimeOffset instance, bool includeMilliseconds)
        {
            if (includeMilliseconds)
            {
                return instance.ToString("o");
            }
            else
            {
                if (instance.Offset.TotalSeconds == 0)
                {
                    return instance.DateTime.ToString("yyyy-MM-ddTHH:mm:ssZ");
                }
                else
                {
                    return instance.ToString("yyyy-MM-ddTHH:mm:ssK");
                }
            }
        }

        /// <summary>
        /// Returns a <see cref="DateTime"/> with the offset dropped and the <see cref="DateTimeKind"/> set as most appropriate.
        /// </summary>
        /// <remarks>
        /// When the offset is zero, returns as UTC. When the offset matches that of the current local offset then returns
        /// as local kind. Otherwise the DateTime returned if of the unspecified kind.
        /// </remarks>
        /// <param name="instance"></param>
        /// <returns>A DateTime instance.</returns>        
        public static DateTime ToDateTime(this DateTimeOffset instance)
        {
            if (instance.Offset.Equals(TimeSpan.Zero))
            {
                return instance.UtcDateTime;
            }
            else if (instance.Offset.Equals(TimeZoneInfo.Local.GetUtcOffset(instance.DateTime)))
            {
                return DateTime.SpecifyKind(instance.DateTime, DateTimeKind.Local);
            }
            else
            {
                return instance.DateTime;
            }
        }

        /// <summary>
        /// Returns a new instance forwarded to the start of next month.
        /// </summary>
        /// <returns>A new DateTimeOffset instance.</returns>
        public static DateTimeOffset ToStartOfNextMonth(this DateTimeOffset instance)
        {
            if (instance.Month < 12)
            {
                return new DateTimeOffset(instance.Year, instance.Month + 1, 1, 0, 0, 0, 0, instance.Offset);
            }
            else
            {
                return new DateTimeOffset(instance.Year + 1, 1, 1, 0, 0, 0, 0, instance.Offset);
            }
        }

        #endregion

        #region DateTime

        /// <summary>
        /// Compares this DateTime with another for equality within a specified tolerance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="value">A DateTime instance to compare to.</param>
        /// <param name="milliseconds">The number of ms that are deemed within acceptable range either side.</param>
        /// <returns></returns>
        public static bool AlmostEquals(this DateTime instance, DateTime value, int milliseconds)
        {
            if (instance.ToString() != value.ToString()) { return false; }

            long absolute = (long)Math.Abs(instance.Millisecond - value.Millisecond);
            return (absolute < milliseconds);
        }

        /// <summary>
        /// Compares this DateTime with another for equality within a specified tolerance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="value">A DateTime instance to compare to.</param>
        /// <param name="tolerance">A TimeSpan deemed within acceptable range either side.</param>
        /// <returns></returns>
        public static bool AlmostEquals(this DateTime instance, DateTime value, TimeSpan tolerance)
        {
            bool result;
            TimeSpan difference = instance.Subtract(value);
            if (value > instance)
            {
                result = (difference.Negate() <= tolerance);
            }
            else
            {
                result = (difference <= tolerance);
            }
            return result;
        }

        /// <summary>
        /// Returns whether or not the date is on a weekend.
        /// </summary>
        public static bool IsWeekend(this DateTime instance)
        {
            return ((instance.DayOfWeek == DayOfWeek.Saturday) || (instance.DayOfWeek == DayOfWeek.Sunday));
        }

        /// <summary>
        /// Returns a new DateTime rounded back to the nearest specified division of time.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="grain">A TimeSpan representing one period of granularity, for example: an hour.</param>
        /// <returns></returns>
        public static DateTime RoundBack(this DateTime instance, TimeSpan grain)
        {
            // 18 round back to chunks of 5: 18 divided by 5 = 3 wholes, 3 * 5 = 15.
            //
            var wholeChunks = instance.Ticks / grain.Ticks;
            var dt = new DateTime(wholeChunks * grain.Ticks);
            return DateTime.SpecifyKind(dt, instance.Kind);
        }

        /// <summary>
        /// Returns true if the DateTime instance represents a moment in the future.
        /// </summary>
        /// <returns>True if the DateTime instance is greater than now using UTC.</returns>
        public static bool IsInTheFuture(this DateTime instance)
        {
            return DateTime.UtcNow.CompareTo(instance.ToUniversalTime()) < 0;
        }

        /// <summary>
        /// Returns a user-friendly string representation of the DateTime.
        /// </summary>
        public static string ToCaption(this DateTime instance)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder(3);
            if (DateTime.UtcNow.AlmostEquals(instance, TimeSpan.FromSeconds(30)))
            {
                sb.Append("Now");
            }
            else if ((!instance.IsInTheFuture()) & DateTime.UtcNow.AlmostEquals(instance, TimeSpan.FromSeconds(59)))
            {
                TimeSpan dts = DateTime.Now - instance;

                sb.Append((int)dts.TotalSeconds);
                sb.Append(" seconds ago");
            }
            else if ((!instance.IsInTheFuture()) & DateTime.UtcNow.AlmostEquals(instance, TimeSpan.FromMinutes(59)))
            {
                TimeSpan dts = DateTime.Now - instance;

                sb.Append((int)dts.TotalMinutes);
                if ((int)dts.TotalMinutes == 1)
                    sb.Append(" minute ago");
                else
                    sb.Append(" minutes ago");
            }
            else if ((!instance.IsInTheFuture()) & DateTime.UtcNow.AlmostEquals(instance, TimeSpan.FromHours(6)))
            {
                TimeSpan dts = DateTime.Now - instance;

                sb.Append((int)dts.TotalHours);
                if ((int)dts.TotalHours == 1)
                    sb.Append(" hour ago");
                else
                    sb.Append(" hours ago");
            }
            else if (DateTime.UtcNow.DayOfYear == instance.DayOfYear)
            {
                sb.Append(instance.ToString("h:mm tt"));
                sb.Append(" today");
            }
            else if ((DateTime.UtcNow.DayOfYear - instance.DayOfYear) == 1)
            {
                sb.Append(instance.ToString("h:mm tt"));
                sb.Append(" yesterday");
            }
            else if ((!instance.IsInTheFuture()) && (DateTime.UtcNow - instance).TotalDays < 7)
            {
                sb.Append(instance.ToString("h:mm tt"));
                sb.Append(" ");
                sb.Append(instance.ToString("dddd"));
                sb.Append(" gone");
            }
            else if ((instance - DateTime.UtcNow).TotalDays < 7)
            {
                sb.Append(instance.ToString("h:mm tt"));
                sb.Append(" this ");
                sb.Append(instance.ToString("dddd"));
            }
            else if (DateTime.UtcNow.Year == instance.Year)
            {
                sb.Append(instance.ToString("h:mm tt mm MMM"));
            }
            else
            {
                sb.Append(instance.ToString("h:mm tt mm MMM yyyy"));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Returns the current DateTime as a DateTimeOffset that represents the original captured moment.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="utcOffset">The UTC offset.</param>
        /// <returns>DateTimeOffset.</returns>
        /// <exception cref="System.ArgumentException">The date time is not represented in UTC.</exception>
        public static DateTimeOffset ToDateTimeOffset(this DateTime instance, TimeSpan utcOffset)
        {
            if (instance.Kind != DateTimeKind.Utc)
                throw new ArgumentException("The date time is not represented in UTC.");

            DateTime originalTime = DateTime.SpecifyKind(instance.Add(utcOffset), DateTimeKind.Unspecified);
            DateTimeOffset dto = new DateTimeOffset(originalTime, utcOffset);

            return dto;
        }

        #endregion

        #region TimeSpan

        /// <summary>
        /// Compares this TimeSpan with another to see if they're almost the same to within a specified tolerance.
        /// </summary>        
        /// <param name="compared">A TimeSpan to compare to.</param>
        /// <param name="tolerance">A TimeSpan serving as the max distance either way.</param>
        /// <returns>True if the TimeSpan is almost equal within the tolerance supplied.</returns>
        /// <param name="instance">Instance</param>
        public static bool AlmostEquals(this TimeSpan instance, TimeSpan compared, TimeSpan tolerance)
        {
            var residue = Math.Abs(instance.Ticks - compared.Ticks);
            return (residue < tolerance.Ticks);
        }

        /// <summary>
        /// Returns a friendly string representation of the TimeSpan in English.
        /// </summary>
        public static string ToCaption(this TimeSpan instance)
        {
            if (Math.Round(instance.TotalDays, 0) == 1)
            {
                return "1 day";
            }
            else if ((int)instance.TotalDays > 0)
            {
                if (instance.TotalDays % 1 > 0)
                {
                    return (String.Concat((int)instance.TotalDays, " days ", (int)instance.Hours, " hours"));
                }

                return (String.Concat((int)instance.TotalDays, " days"));
            }
            else if ((int)instance.TotalHours > 0)
            {
                if (instance.TotalHours % 1 > 0)
                {
                    if ((int)instance.TotalHours == 1)
                    {
                        return (String.Concat((int)instance.TotalHours, " hour ", (int)instance.Minutes, " minutes"));
                    }
                    else
                    {
                        return (String.Concat((int)instance.TotalHours, " hours ", (int)instance.Minutes, " minutes"));
                    }
                }
                return (String.Concat((int)instance.TotalHours, " hours"));
            }
            else if ((int)instance.TotalMinutes > 0)
            {
                if (instance.TotalMinutes % 1 > 0)
                {
                    if (instance.TotalMinutes == 1)
                    {
                        return (String.Concat((int)instance.TotalMinutes, " minute ", (int)instance.Seconds, " seconds"));
                    }
                    else
                    {
                        return (String.Concat((int)instance.TotalMinutes, " minutes ", (int)instance.Seconds, " seconds"));
                    }

                }

                return (String.Concat((int)instance.TotalMinutes, " minutes"));
            }
            else if ((int)instance.TotalSeconds > 0)
            {
                return (String.Concat((int)instance.TotalSeconds, " seconds"));
            }
            else if ((int)instance.TotalMilliseconds > 0)
            {
                return (String.Concat((int)instance.TotalMilliseconds, "ms"));
            }
            else
            {
                return ("<1ms");
            }
        }

        #endregion

        #region String

        /// <summary>
        /// Breaks a string up into words.
        /// </summary>
        public static IEnumerable<string> SplitWords(this string instance)
        {
            var space = new string[] { " " };
            return (instance + " ").Split(space, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// Convenience for String.Split which accepts delimiter not as an array.
        /// </summary>
        public static string[] Split(this string instance, string delimiter)
        {
            return Split(instance, delimiter, StringSplitOptions.None);
        }

        /// <summary>
        /// Convenience for String.Split which accepts delimiter not as an array.
        /// </summary>
        public static string[] Split(this string instance, string delimiter, StringSplitOptions options)
        {
            return instance.Split(new string[] { delimiter }, options);
        }

        /// <summary>
        /// Converts a string to an array of bytes encoded in UTF8.
        /// </summary>        
        /// <returns>A byte array.</returns>
        public static byte[] ToUTF8ByteArray(this string instance)
        {
            return System.Text.Encoding.UTF8.GetBytes(instance);
        }

        /// <summary>
        /// Turns a StringThatLooksLikeThis into a String That Looks Like This.
        /// </summary>        
        /// <returns>A string.</returns>
        public static string ToCasePhrase(this string instance)
        {
            var chars = instance.ToCharArray();
            var sb = new StringBuilder();

            foreach (var ch in chars)
            {
                bool isUpper = Char.IsUpper(ch);
                if (isUpper)
                    sb.Append(" ");
                sb.Append(ch);

            }

            return sb.ToString().Trim();
        }


        /// <summary>
        /// Turns a string That loOks like THIS into a String That Looks Like This.
        /// </summary>
        /// <remarks>
        /// Also normalizes the spacing to single spaces.
        /// </remarks> 
        /// <returns>A string.</returns>
        public static string ToTitleCase(this string instance)
        {
            return String.Join(" ", instance.ToUpper().SplitWords().Select(w => w.ToLowerTail()));
        }

        /// <summary>
        /// Capitalizes just the first letter of the string and leaves the others as is.
        /// </summary>        
        /// <returns>Returns 'a string that goes LIKE this' as 'A string that goes LIKE this'.</returns>
        public static string ToInitialCap(this string instance)
        {
            if (String.IsNullOrEmpty(instance))
                return instance;

            var tail = instance.Substring(1, instance.Length - 1);
            var initial = instance.Substring(0, 1).ToUpper();

            return string.Concat(initial, tail);
        }

        /// <summary>
        /// Makes all characters after the first, lowercase but doesn't necessarily make the first character uppercase.
        /// </summary>
        /// <returns>Returns 'a string Like this' as 'a string like this'.</returns>
        public static string ToLowerTail(this string instance)
        {
            if (String.IsNullOrEmpty(instance))
                return instance;

            var tail = instance.Substring(1, instance.Length - 1).ToLower();
            var initial = instance.Substring(0, 1);

            return string.Concat(initial, tail);
        }

        /// <summary>
        /// Returns a substring of the string starting from the left and doesn't throw when the length is longer than the string.
        /// </summary>
        public static string Left(this string instance, int length)
        {
            if (length == 0)
                return String.Empty;

            return instance.Substring(0, Math.Min(instance.Length, length));
        }

        /// <summary>
        /// Returns the string on the left-side of the delimiter or the whole string if no occurrences are found.
        /// </summary>
        /// <param name="delimiter">The first occurrence of a string to use as a delimiter.</param>
        /// <returns>A string</returns>
        /// <param name="instance">Instance</param>
        public static string LeftUntil(this string instance, string delimiter)
        {
            if (instance.Length == 0)
                return String.Empty;

            int i = instance.IndexOf(delimiter);
            if (i == -1)
                return instance;

            return instance.Left(i);
        }

        /// <summary>
        /// Returns a substring of the right-most characters of the specified length and doesn't throw when the length is longer than the string.
        /// </summary>
        public static string Right(this string instance, int length)
        {
            if (instance.Length == 0 || length < 1)
                return String.Empty;

            length = Math.Min(length, instance.Length);

            int s = instance.Length - length;

            return instance.Substring(s, length);
        }

        /// <summary>
        /// Returns the text after the specified delimiter.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="delimiter">The delimiter.</param>
        /// <returns></returns>
        public static string RightAfter(this string instance, string delimiter)
        {
            if (instance.Length == 0)
                return String.Empty;

            if (delimiter.Length == 0)
                return instance;

            int i = instance.IndexOf(delimiter);
            if (i == -1)
                return String.Empty;

            if (i == 0 && instance.Length == 1)
                return String.Empty;

            return instance.Mid(i + delimiter.Length, instance.Length);
        }

        /// <summary>
        /// Makes String.IsNullOrEmpty more betterer.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>
        ///   <c>true</c> if [is not null or empty] [the specified instance]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNotNullOrWhitespace(this string instance)
        {
            return !String.IsNullOrWhiteSpace(instance);
        }

        /// <summary>
        /// Returns an empty string if the string is null.
        /// </summary>
        public static string NullAsEmpty(this string instance)
        {
            if (instance == null)
                return String.Empty;

            return instance;
        }

        /// <summary>
        /// Retrieves a substring from this instance. The substring starts at a specified
        /// character position and reads up to but not including the end position.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="startIndex">The start index, inclusive.</param>
        /// <param name="endIndex">The end index, exclusive.</param>
        /// <returns></returns>
        public static string Mid(this string s, long startIndex, long endIndex)
        {
            if (startIndex == endIndex)
            {
                return String.Empty;
            }

            if (startIndex < endIndex)
            {
                return s.Substring((int)startIndex, (int)(endIndex - startIndex));
            }
            else
            {
                return s.Substring((int)endIndex, (int)(startIndex - endIndex));
            }
        }

        /// <summary>
        /// Removes characters that are neither digits, letters or exist in the specified array.
        /// </summary>
        public static string CharsOnly(this string s)
        {
            return CharsOnly(s, new Char[] { });
        }

        /// <summary>
        /// Removes characters that are neither digits, letters or exist in the specified array.
        /// </summary>
        /// <param name="butRetain">A string of characters to also retain.</param>
        /// <param name="s">Instance</param>
        public static string CharsOnly(this string s, string butRetain)
        {
            return CharsOnly(s, butRetain.ToCharArray());
        }

        /// <summary>
        /// Removes characters that are neither digits, letters or exist in the specified array.
        /// </summary>
        /// <param name="butRetain">An array of characters to also retain.</param>
        /// <param name="s">Instance</param>
        public static string CharsOnly(this string s, Char[] butRetain)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Char c in s)
            {
                if (Char.IsLetterOrDigit(c) || butRetain.Contains(c))
                    sb.Append(c);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Wraps a string in some other string.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="value">The string to insert before and after the current string.</param>
        /// <returns>A string</returns>
        public static string SurroundWith(this string s, string value)
        {
            return $"{value}{s}{value}";
        }

        /// <summary>
        /// Wraps a string in some other string.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="character">The string to insert before and after the current string.</param>
        /// <returns>A string</returns>
        public static string SurroundWith(this string s, char character)
        {
            return $"{character}{s}{character}";
        }

        /// <summary>
        /// Wraps a string in quotes.
        /// </summary>
        /// <param name="doubleQuotes">True for double quotes, false for single.</param>
        /// <returns>A string</returns>
        public static string SurroundWith(this string s, bool doubleQuotes)
        {
            if (doubleQuotes)
            {
                return $"\"{s}\"";
            }
            else
            {
                return $"\'{s}\'";
            }
        }

        #endregion

        #region Byte

        /// <summary>
        /// Returns whether a value is almost equal to another value to within a specified tolerance.
        /// </summary>
        public static bool AlmostEquals(this byte instance, byte value, byte tolerance)
        {
            return (System.Math.Abs(instance - value) <= tolerance);
        }

        /// <summary>
        /// Determines whether this value is even.
        /// </summary>
        /// <param name="i">The i.</param>
        /// <returns></returns>
        public static bool IsEven(this byte i)
        {
            return (i % 2 == 0);
        }

        #endregion

        #region Byte[]

        /// <summary>
        /// Turns the byte array into its Hex representation.
        /// </summary>
        public static string ToHex(this byte[] instance)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in instance)
            {
                sb.Append(b.ToString("X").PadLeft(2, "0"[0]));
            }
            return sb.ToString();
        }

        #endregion

        #region Int

        /// <summary>
        /// Returns whether a value is almost equal to another value to within a specified tolerance.
        /// </summary>
        public static bool AlmostEquals(this int instance, int value, int tolerance)
        {
            return (System.Math.Abs(instance - value) <= tolerance);
        }

        /// <summary>
        /// Determines whether this value is even.
        /// </summary>
        /// <param name="i">The i.</param>
        /// <returns></returns>
        public static bool IsEven(this int i)
        {
            return (i % 2 == 0);
        }

        #endregion

        #region Guid

        /// <summary>
        /// Determines whether this GUID is empty.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        /// <returns></returns>
        public static bool IsEmpty(this Guid guid)
        {
            return Guid.Empty == guid;
        }

        /// <summary>
        /// Determines whether this GUID is empty and if it is, then returns an alternative value.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        /// <param name="fallback">The value to return if this instance is empty.</param>
        /// <returns>A Guid to return if the value is empty.</returns>
        public static Guid IfEmpty(this Guid guid, Guid fallback)
        {
            if (guid.IsEmpty())
            {
                return fallback;
            }
            else
            {
                return guid;
            }
        }

        #endregion

        #region Double

        /// <summary>
        /// Returns the specified value if it is 'normal' else returns the fallback value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="fallback">The fallback value.</param>        
        /// <returns>The current value or the fallback value.</returns>
        public static double Else(this double value, double fallback)
        {
            return Else(value, fallback, false);
        }

        /// <summary>
        /// Returns the specified value if it is 'normal' else returns the fallback value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="fallback">The fallback value.</param>
        /// <param name="allowExtremities">Set true to consider infinity and max and min values as normal.</param>
        /// <returns>The current value or the fallback value.</returns>
        public static double Else(this double value, double fallback, bool allowExtremities)
        {
            if (value == 0)
                return fallback;

            if (Double.IsNaN(value))
                return fallback;

            if (!allowExtremities)
            {
                if (Double.MaxValue == value)
                    return fallback;

                if (Double.MinValue == value)
                    return fallback;

                if (Double.IsInfinity(value))
                    return fallback;
            }

            return value;
        }

        /// <summary>
        /// Returns whether a value is almost equal to another value to within a specified tolerance.
        /// </summary>
        public static bool AlmostEquals(this double instance, double value, double tolerance)
        {
            return (System.Math.Abs(instance - value) <= tolerance);
        }

        /// <summary>
        /// Returns the value rounded to the nearest decimal.
        /// </summary>
        public static double RoundedTo(this double instance, int decimalPlaces)
        {
            return (System.Math.Round(instance, decimalPlaces));
        }

        #endregion

        #region Random

        /// <summary>
        /// Runs a specified random action.
        /// </summary>
        /// <param name="random">The random.</param>
        /// <param name="actions">The actions that can run.</param>
        /// <returns>T.</returns>
        /// <exception cref="System.ArgumentException">Cannot run anything. No actions were specified.;functions</exception>
        public static void Run(this Random random, params Action[] actions)
        {
            if (actions != null && actions.Length > 0)
            {
                actions[random.Next(0, actions.Length)]();
            }
            else
            {
                throw new ArgumentException("Cannot run anything. No actions were specified.", "functions");
            }
        }

        /// <summary>
        /// Runs a specified random function.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="random">The random.</param>
        /// <param name="functions">The functions.</param>
        /// <returns>T.</returns>
        /// <exception cref="System.ArgumentException">Cannot run anything. No functions were specified.;functions</exception>
        public static T Run<T>(this Random random, params Func<T>[] functions)
        {
            if (functions != null && functions.Length > 0)
            {
                return functions[random.Next(0, functions.Length)]();
            }
            else
            {
                throw new ArgumentException("Cannot run anything. No functions were specified.", "functions");
            }
        }

        /// <summary>
        /// Returns a random element from a sequence.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="random">The random.</param>
        /// <param name="sequence">A sequence of elements.</param>
        /// <returns>T.</returns>
        /// <exception cref="System.ArgumentException">Cannot return a value. No element choices were specified.;elements</exception>
        public static T Choice<T>(this Random random, params T[] sequence)
        {
            if (sequence != null && sequence.Length > 0)
            {
                return sequence[random.Next(0, sequence.Length)];
            }
            else
            {
                throw new ArgumentException("Cannot return a value. No sequence was specified.", "sequence");
            }
        }

        #endregion

        #region Exception

        /// <summary>
        /// Returns whether the exception is not one of the critical runtime exceptions.
        /// </summary>
        /// <seealso cref="https://stackoverflow.com/questions/5507836/which-types-of-exception-not-to-catch/5508733#5508733"/>
        /// <param name="exception">The exception.</param>
        /// <returns>True if the exception is not one of the critical runtime exceptions that should never be handled in user code.</returns>
        [Obsolete("This method is intended to be used to make existing badly writtern catch-all exception code, less bad.", false)]
        public static bool IsSafeToHandle(this Exception exception)
        {
            var unsafeExceptions = new[]
            {
                typeof(NullReferenceException),
                typeof(StackOverflowException),
                typeof(OutOfMemoryException),
                typeof(Threading.ThreadAbortException),
                typeof(ExecutionEngineException),
                typeof(IndexOutOfRangeException),
                typeof(AccessViolationException)
            };

            return !unsafeExceptions.Contains(exception.GetType());
        }

        #endregion
    }
}