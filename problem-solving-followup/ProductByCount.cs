using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace problem_solving_followup
{
    public class ItemByCount : IEquatable<ItemByCount>
    {
        public string item;
        public int count;

        public ItemByCount(string i, int c)
        {
            item = i;
            count = c;
        }

        public bool Equals(ItemByCount ic)
        {
            if (ic == null)
            {
                return false;
            }

            return ic.item.Equals(this.item) && ic.count == this.count;
        }

        public bool Equals(object o)
        {
            if (o == null)
            {
                return false;
            }

            return this.Equals((ItemByCount)o);
        }

        public override int GetHashCode()
        {
            int hashItem = item.GetHashCode();
            int hashCount = count.GetHashCode();

            return hashItem ^ hashCount;
        }
    }
}