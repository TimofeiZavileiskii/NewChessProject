using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewChessProject
{
    public class Map<T1, T2>
    {
        private Dictionary<T1, T2> forward;
        private Dictionary<T2, T1> reverse;

        public Map()
        {
            forward = new Dictionary<T1, T2>();
            reverse = new Dictionary<T2, T1>();
        }

        public void Add(T1 t1, T2 t2)
        {
            forward.Add(t1, t2);
            reverse.Add(t2, t1);
        }
        public T2 this[T1 t1]
        {
            get
            {
                return forward[t1];
            }
        }

        public T1 this[T2 t2]
        {
            get
            {
                return reverse[t2];
            }
        }

    }
}