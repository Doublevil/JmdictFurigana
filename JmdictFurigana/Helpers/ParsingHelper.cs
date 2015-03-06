using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JmdictFurigana.Helpers
{
    public static class ParsingHelper
    {
        #region Constants

        /// <summary>
        /// Defines how the numbers are parsed.
        /// </summary>
        private static readonly NumberStyles NumberStyles = NumberStyles.Number;

        /// <summary>
        /// Defines the culture used to parse numbers.
        /// </summary>
        public static readonly CultureInfo DefaultCulture =
            CultureInfo.CreateSpecificCulture("en-US");

        #endregion

        #region Methods

        /// <summary>
        /// Tries to parse the given input string as a short.
        /// </summary>
        /// <param name="input">Input string.</param>
        /// <returns>Value parsed if successful. Null otherwise.</returns>
        public static short? ParseShort(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return null;
            }

            short output = 0;
            if (short.TryParse(input, NumberStyles, DefaultCulture, out output))
            {
                return output;
            }

            return null;
        }

        /// <summary>
        /// Tries to parse the given input string as a short.
        /// If the value cannot be parsed, throws a failure exception.
        /// </summary>
        /// <param name="input">Input string.</param>
        /// <param name="failureException">Exception to throw when the parse fails.</param>
        /// <returns>Value parsed.</returns>
        public static short ForceShort(string input, Exception failureException = null)
        {
            short? output = ParseShort(input);
            if (output.HasValue)
            {
                return output.Value;
            }
            else
            {
                failureException = failureException
                    ?? new Exception(string.Format("Cannot parse \"{0}\" as a short.", input));

                throw failureException;
            }
        }

        /// <summary>
        /// Tries to parse the given input string as an integer.
        /// </summary>
        /// <param name="input">Input string.</param>
        /// <returns>Value parsed if successful. Null otherwise.</returns>
        public static int? ParseInt(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return null;
            }

            int output = 0;
            if (int.TryParse(input, NumberStyles, DefaultCulture, out output))
            {
                return output;
            }

            return null;
        }

        /// <summary>
        /// Tries to parse the given input string as an integer.
        /// If the value cannot be parsed, throws a failure exception.
        /// </summary>
        /// <param name="input">Input string.</param>
        /// <param name="failureException">Exception to throw when the parse fails.</param>
        /// <returns>Value parsed.</returns>
        public static int ForceInt(string input, Exception failureException = null)
        {
            int? output = ParseInt(input);
            if (output.HasValue)
            {
                return output.Value;
            }
            else
            {
                failureException = failureException
                    ?? new Exception(string.Format("Cannot parse \"{0}\" as an integer.", input));

                throw failureException;
            }
        }

        /// <summary>
        /// Tries to parse the given input string as a long.
        /// </summary>
        /// <param name="input">Input string.</param>
        /// <returns>Value parsed if successful. Null otherwise.</returns>
        public static long? ParseLong(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return null;
            }

            long output = 0;
            if (long.TryParse(input, NumberStyles, DefaultCulture, out output))
            {
                return output;
            }

            return null;
        }

        /// <summary>
        /// Tries to parse the given input string as a long.
        /// If the value cannot be parsed, throws a failure exception.
        /// </summary>
        /// <param name="input">Input string.</param>
        /// <param name="failureException">Exception to throw when the parse fails.</param>
        /// <returns>Value parsed.</returns>
        public static long ForceLong(string input, Exception failureException = null)
        {
            long? output = ParseLong(input);
            if (output.HasValue)
            {
                return output.Value;
            }
            else
            {
                failureException = failureException
                    ?? new Exception(string.Format("Cannot parse \"{0}\" as a long.", input));

                throw failureException;
            }
        }

        /// <summary>
        /// Tries to parse the given input string as a float.
        /// </summary>
        /// <param name="input">Input string.</param>
        /// <returns>Value parsed if successful. Null otherwise.</returns>
        public static float? ParseFloat(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return null;
            }

            float output = 0;
            if (float.TryParse(input, NumberStyles, DefaultCulture, out output))
            {
                return output;
            }

            return null;
        }

        /// <summary>
        /// Tries to parse the given input string as a float.
        /// If the value cannot be parsed, throws a failure exception.
        /// </summary>
        /// <param name="input">Input string.</param>
        /// <param name="failureException">Exception to throw when the parse fails.</param>
        /// <returns>Value parsed.</returns>
        public static float ForceFloat(string input, Exception failureException = null)
        {
            float? output = ParseFloat(input);
            if (output.HasValue)
            {
                return output.Value;
            }
            else
            {
                failureException = failureException
                    ?? new Exception(string.Format("Cannot parse \"{0}\" as a float.", input));

                throw failureException;
            }
        }

        /// <summary>
        /// Tries to parse the given input string as a double.
        /// </summary>
        /// <param name="input">Input string.</param>
        /// <returns>Value parsed if successful. Null otherwise.</returns>
        public static double? ParseDouble(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return null;
            }

            double output = 0;
            if (double.TryParse(input, NumberStyles, DefaultCulture, out output))
            {
                return output;
            }

            return null;
        }

        /// <summary>
        /// Tries to parse the given input string as a double.
        /// If the value cannot be parsed, throws a failure exception.
        /// </summary>
        /// <param name="input">Input string.</param>
        /// <param name="failureException">Exception to throw when the parse fails.</param>
        /// <returns>Value parsed.</returns>
        public static double ForceDouble(string input, Exception failureException = null)
        {
            double? output = ParseDouble(input);
            if (output.HasValue)
            {
                return output.Value;
            }
            else
            {
                failureException = failureException
                    ?? new Exception(string.Format("Cannot parse \"{0}\" as a double.", input));

                throw failureException;
            }
        }

        /// <summary>
        /// Tries to parse the given input string as a boolean.
        /// </summary>
        /// <param name="input">Input string.</param>
        /// <returns>Value parsed if successful. Null otherwise.</returns>
        /// <remarks>
        /// An integer value of 0 will be parsed as the <c>false</c> value.
        /// An integer value of 1 will be parsed as the <c>true</c> value.
        /// </remarks>
        public static bool? ParseBool(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return null;
            }

            bool output = false;
            if (bool.TryParse(input, out output))
            {
                return output;
            }

            int? intVal = ParseInt(input);
            if (intVal.HasValue && (intVal == 0 || intVal == 1))
            {
                return intVal == 1;
            }

            return null;
        }

        /// <summary>
        /// Tries to parse the given input string as a boolean.
        /// If the value cannot be parsed, throws a failure exception.
        /// </summary>
        /// <param name="input">Input string.</param>
        /// <param name="failureException">Exception to throw when the parse fails.</param>
        /// <returns>Value parsed.</returns>
        public static bool ForceBool(string input, Exception failureException = null)
        {
            bool? output = ParseBool(input);
            if (output.HasValue)
            {
                return output.Value;
            }
            else
            {
                failureException = failureException
                    ?? new Exception(string.Format("Cannot parse \"{0}\" as a boolean.", input));

                throw failureException;
            }
        }

        /// <summary>
        /// Tries to parse the given input string as a value of the specified enumeration.
        /// </summary>
        /// <typeparam name="T">Output enumeration type.</typeparam>
        /// <param name="input">Input string.</param>
        /// <returns>Value parsed as a member of the output enumeration if successful.
        /// Null otherwise.</returns>
        public static T? ParseEnum<T>(string input) where T : struct
        {
            if (string.IsNullOrEmpty(input))
            {
                return null;
            }

            // Get enum type
            Type enumType = typeof(T);
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("The type must be an enum type.", "T");
            }

            // Compare the input string to each possible value
            foreach (string enumVal in Enum.GetNames(enumType))
            {
                if (enumVal == input)
                {
                    // If a value matches the input string, return it
                    return (T)Enum.Parse(enumType, enumVal);
                }
            }

            // No values matched the input string.
            return null;
        }

        /// <summary>
        /// Tries to parse the given input string as a value of the specified enumeration.
        /// If the value cannot be parsed, throws a failure exception.
        /// </summary>
        /// <param name="input">Input string.</param>
        /// <param name="failureException">Exception to throw when the parse fails.</param>
        /// <returns>Value parsed.</returns>
        public static T? ForceEnum<T>(string input, Exception failureException = null)
            where T : struct
        {
            T? output = ParseEnum<T>(input);
            if (output.HasValue)
            {
                return output.Value;
            }
            else
            {
                failureException = failureException
                    ?? new Exception(string.Format(
                        "Cannot parse \"{0}\" as a {1} value.",
                        input, typeof(T).Name));

                throw failureException;
            }
        }

        /// <summary>
        /// Tries to parse the given input string as a DateTime.
        /// </summary>
        /// <param name="input">Input string.</param>
        /// <param name="format">Date format string.</param>
        /// <returns>Value parsed if successful. Null otherwise.</returns>
        public static DateTime? ParseDateTime(string input, string format = "u")
        {
            if (string.IsNullOrEmpty(input))
            {
                return null;
            }

            DateTime output = new DateTime();
            if (DateTime.TryParseExact(input, format, DefaultCulture, DateTimeStyles.None, out output))
            {
                return output;
            }

            return null;
        }

        /// <summary>
        /// Tries to parse the given input string as a DateTime.
        /// If the value cannot be parsed, throws a failure exception.
        /// </summary>
        /// <param name="input">Input string.</param>
        /// <param name="format">Date format string.</param>
        /// <param name="failureException">Exception to throw when the parse fails.</param>
        /// <returns>Value parsed.</returns>
        public static DateTime ForceDateTime(string input, string format = "u",
            Exception failureException = null)
        {
            DateTime? output = ParseDateTime(input, format);
            if (output.HasValue)
            {
                return output.Value;
            }
            else
            {
                failureException = failureException
                    ?? new Exception(string.Format("Cannot parse \"{0}\" as a DateTime.", input));

                throw failureException;
            }
        }

        #endregion
    }
}
