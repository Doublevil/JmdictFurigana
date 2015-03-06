using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JmdictFurigana.Extensions
{
    public static class ListExtensions
    {
        /// <summary>
        /// Clones a list of cloneable objects.
        /// </summary>
        /// <typeparam name="T">List type.</typeparam>
        /// <param name="list">List to clone.</param>
        /// <returns>List containing cloned instances of the input.</returns>
        public static List<T> Clone<T>(this List<T> list) where T: ICloneable
        {
            List<T> output = new List<T>(list.Count);
            foreach (T item in list)
            {
                output.Add((T)item.Clone());
            }

            return output;
        }
    }
}
