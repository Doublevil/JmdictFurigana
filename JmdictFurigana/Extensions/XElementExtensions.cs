using JmdictFurigana.Helpers;
using System.Linq;
using System.Xml.Linq;

namespace JmdictFurigana.Extensions
{
    public static class XElementExtensions
    {
        #region Node read methods

        /// <summary>
        /// Returns a value indicating whether this element has at least one direct child node
        /// matching the given name.
        /// </summary>
        /// <param name="root">Target root element.</param>
        /// <param name="node">Name of the nodes to search.</param>
        /// <returns><c>True</c> if the root node has at least one direct child node with the
        /// given name. <c>False</c> otherwise.</returns>
        public static bool HasElement(this XElement root, XName node)
        {
            return root.Elements(node).Any();
        }

        /// <summary>
        /// Returns a value indicating whether this element has at least one child or sub-child
        /// node matching the given name.
        /// </summary>
        /// <param name="root">Target root element.</param>
        /// <param name="node">Name of the nodes to search.</param>
        /// <returns><c>True</c> if the root node has at least one child or sub-child node with
        /// the given name. <c>False</c> otherwise.</returns>
        public static bool HasDescendant(this XElement root, XName node)
        {
            return root.Descendants(node).Any();
        }

        #endregion

        #region Attribute read methods

        /// <summary>
        /// Reads the given attribute as a string value.
        /// </summary>
        /// <param name="root">Node carrying the target attribute.</param>
        /// <param name="attribute">Target attribute name.</param>
        /// <returns>The value as a string, if it was successfuly read, or
        /// <c>null</c> if the attribute does not exist.</returns>
        public static string ReadAttributeString(this XElement root, XName attribute)
        {
            if (root.Attribute(attribute) != null)
            {
                return root.Attribute(attribute).Value;
            }
            return null;
        }

        /// <summary>
        /// Reads the given attribute as a short value.
        /// </summary>
        /// <param name="root">Node carrying the target attribute.</param>
        /// <param name="attribute">Target attribute name.</param>
        /// <returns>The value as a short, if it was successfuly read, or
        /// <c>null</c> if the attribute does not exist or the value is not a short.</returns>
        public static short? ReadAttributeShort(this XElement root, XName attribute)
        {
            if (root.Attribute(attribute) != null)
            {
                return ParsingHelper.ParseShort(root.Attribute(attribute).Value);
            }
            return null;
        }

        /// <summary>
        /// Reads the given attribute as an integer value.
        /// </summary>
        /// <param name="root">Node carrying the target attribute.</param>
        /// <param name="attribute">Target attribute name.</param>
        /// <returns>The value as an integer, if it was successfuly read, or
        /// <c>null</c> if the attribute does not exist or the value is not an integer.</returns>
        public static int? ReadAttributeInt(this XElement root, XName attribute)
        {
            if (root.Attribute(attribute) != null)
            {
                return ParsingHelper.ParseInt(root.Attribute(attribute).Value);
            }
            return null;
        }

        /// <summary>
        /// Reads the given attribute as a long value.
        /// </summary>
        /// <param name="root">Node carrying the target attribute.</param>
        /// <param name="attribute">Target attribute name.</param>
        /// <returns>The value as a long, if it was successfuly read, or
        /// <c>null</c> if the attribute does not exist or the value is not a long.</returns>
        public static long? ReadAttributeLong(this XElement root, XName attribute)
        {
            if (root.Attribute(attribute) != null)
            {
                return ParsingHelper.ParseLong(root.Attribute(attribute).Value);
            }
            return null;
        }

        /// <summary>
        /// Reads the given attribute as a boolean value.
        /// </summary>
        /// <param name="root">Node carrying the target attribute.</param>
        /// <param name="attribute">Target attribute name.</param>
        /// <returns>The value as a boolean, if it was successfuly read, or
        /// <c>null</c> if the attribute does not exist or the value is not a boolean.</returns>
        /// <remarks>
        /// <para>Correct values for <c>True</c> are "1" and "true" in any case.</para>
        /// <para>Correct values for <c>False</c> are "0" and "false" in any case.</para>
        /// </remarks>
        public static bool? ReadAttributeBoolean(this XElement root, XName attribute)
        {
            if (root.Attribute(attribute) != null)
            {
                return ParsingHelper.ParseBool(root.Attribute(attribute).Value);
            }
            return null;
        }

        /// <summary>
        /// Reads the given attribute as a float value.
        /// </summary>
        /// <param name="root">Node carrying the target attribute.</param>
        /// <param name="attribute">Target attribute name.</param>
        /// <returns>The value as a float, if it was successfuly read, or <c>null</c>
        /// if the attribute does not exist or the value is not a float.</returns>
        public static float? ReadAttributeFloat(this XElement root, XName attribute)
        {
            if (root.Attribute(attribute) != null)
            {
                return ParsingHelper.ParseFloat(root.Attribute(attribute).Value);
            }
            return null;
        }

        /// <summary>
        /// Reads the given attribute as a double value.
        /// </summary>
        /// <param name="root">Node carrying the target attribute.</param>
        /// <param name="attribute">Target attribute name.</param>
        /// <returns>The value as a double, if it was successfuly read, or <c>null</c>
        /// if the attribute does not exist or the value is not a double.</returns>
        public static double? ReadAttributeDouble(this XElement root, XName attribute)
        {
            if (root.Attribute(attribute) != null)
            {
                return ParsingHelper.ParseDouble(root.Attribute(attribute).Value);
            }
            return null;
        }

        /// <summary>
        /// Reads the given attribute as a value in a specified enumeration.
        /// </summary>
        /// <typeparam name="T">Enumeration type.</typeparam>
        /// <param name="root">Node carrying the target attribute.</param>
        /// <param name="attribute">Target attribute name.</param>
        /// <returns>The value as a value of the specified enumeration, if it was
        /// successfuly read, or <c>null</c> if the attribute does not exist or the value
        /// does not match any value in the specified enumeration.</returns>
        public static T? ReadAttributeEnum<T>(this XElement root, XName attribute) where T : struct
        {
            if (root.Attribute(attribute) != null)
            {
                return ParsingHelper.ParseEnum<T>(root.Attribute(attribute).Value);
            }
            return null;
        }

        #endregion
    }
}
