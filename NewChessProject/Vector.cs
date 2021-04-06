using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewChessProject
{
    enum SpecialMove
    {
        Nothing,
        CastleLeft,
        CastleRight,
        DoubleFoward,
        EnPassante
    }

    struct Vector
    {
        private int x;
        private int y;
        SpecialMove specialMove;  //Special move is a flag for vectors representing a move which affects more than one piece of the player

        public static readonly Vector NullVector;

        static Vector()
        {
            NullVector = new Vector(int.MinValue, int.MinValue);
        }

        public SpecialMove SpecialMove
        {
            get
            {
                return specialMove;
            }
        }

        public int X
        {
            get
            {
                return x;
            }
        }

        public int Y
        {
            get
            {
                return y;
            }
        }

        public Vector(int in_x, int in_y, SpecialMove sp = SpecialMove.Nothing)
        {
            specialMove = sp;
            x = in_x;
            y = in_y;
        }

        public override bool Equals(object obj)
        {
            return ((Vector)obj).X == X && ((Vector)obj).Y == Y;
        }

        public override int GetHashCode()
        {
            return x + y * Board.boardWidth;
        }

        public static Vector operator +(Vector v1, Vector v2)
        {
            return new Vector(v1.X + v2.X, v1.Y + v2.Y);
        }

        public static Vector operator -(Vector v1, Vector v2)
        {
            return new Vector(v1.X - v2.X, v1.Y - v2.Y);
        }

        public static bool operator ==(Vector v1, Vector v2)
        {
            return v1.X == v2.X && v1.Y == v2.Y;
        }

        public static bool operator !=(Vector v1, Vector v2)
        {
            return v1.X != v2.X || v1.Y != v2.Y;
        }

    }
}
