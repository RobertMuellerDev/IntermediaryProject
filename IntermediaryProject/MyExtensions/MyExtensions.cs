using System.Collections.Generic;

namespace ExtensionMethods {
    public static class MyExtensions {
        /// <summary>
        /// <br>Removes the element at the given index and returns it.</br>
        /// <br>If no index is provided the last element will be removed.</br>
        /// </summary>
        /// <param name="indexToPop">index of the element to remove</param>
        /// <returns>The removed element.</returns>
        public static T Pop<T>(this List<T> list, int indexToPop = -1) {
            if (indexToPop < 0) {
                indexToPop = list.Count - 1;
            }
            var element = list[indexToPop];
            list.RemoveAt(indexToPop);
            return element;

        }
    }
}
