using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Energy_Platform
{
    internal static class Extensions
    {
        public static IEnumerable<IEnumerable<int>> SplitByMonth(this IEnumerable<object> days)
        {
            int prev = -1;

            var ex = days.Where(x => ((string)x) != "").Select(x => (int)Convert.ChangeType(x, typeof(int)));
    
            using (var h = ex.GetEnumerator())
            {
                var list = new List<int>();
                while (h.MoveNext())
                {
                    var cur = h.Current;
                    if (cur < prev)
                    {
                        yield return list;
                        list = new List<int>();
                    }
                    list.Add(cur);
                    prev = cur;
                }
                yield return list;
            }

        }

        public static bool AreAllEmpty(this ICollection<object> coll)
        {
            return coll.All(x => (string)x == "");
        }
        public static IDictionary<string, object> GetCurrent(this IEnumerator enu)
        {
            return (IDictionary<string, object>)enu.Current;
        }
    }
}