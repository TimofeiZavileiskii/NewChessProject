using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewChessProject
{
    class Map<Type1, Type2>
    {
        Dictionary<Type1, Type2> forward;
        Dictionary<Type2, Type1> reverse;

        public Map()
        {
            forward = new Dictionary<Type1, Type2>();
            reverse = new Dictionary<Type2, Type1>();
        }

        public void Add(Type1 item1, Type2 item2)
        {
            forward.Add(item1, item2);
            reverse.Add(item2, item1);
        }

        public bool Contains(Type1 item1)
        {
            return forward.ContainsKey(item1);
        }

        public bool Contains(Type2 item2)
        {
            return reverse.ContainsKey(item2);
        }

        public Type2 this[Type1 item1]
        {
            get
            {
                return forward[item1];
            }
        }

        public Type1 this[Type2 item2]
        {
            get
            {
                return reverse[item2];
            }
        }

    }
}