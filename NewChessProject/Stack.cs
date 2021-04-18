using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewChessProject
{

    class Stack<Type>
    {
        Type[] array;
        int top;
        int length;

        public int Count
        {
            get
            {
                return top;
            }
        }

        public Stack(int length = 4)
        {
            this.length = length;
            array = new Type[length];
            top = 0;
        }

        private bool IsFull()
        {
            bool output = false;
            if (top == length) 
                output = true;
            return output;
        }

        private void ExtendArray()
        {
            int newLength = length * 2;
            Type[] newArray = new Type[newLength];
            for(int i = 0; i < length; i++)
            {
                newArray[i] = array[i];
            }

            array = newArray;
            length = newLength;
        }

        public void Push(Type input)
        {
            if (IsFull())
            {
                ExtendArray();
            }
            array[top] = input;
            top++;
        }

        public Type Peek()
        {
            return array[top - 1];
        }

        public Type Pop()
        {
            top--;
            return array[top];
        }

        public bool IsEmpty()
        {
            bool output = false;
            if (top == 0) output = true;
            return output;
        }

        public Type this[int index]
        {
            get
            {
                return array[index];
            }
        }
    }
}
