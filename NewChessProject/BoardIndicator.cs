using System;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewChessProject
{
    struct BoardIndicator
    {
        Vector vector;
        Color colour;

        public BoardIndicator(Vector vector, Color colour)
        {
            this.vector = vector;
            this.colour = colour;
        }

        public Vector Position
        {
            get
            {
                return vector;
            }
        }

        public Color Colour
        {
            get
            {
                return colour;
            }
        }


    }
}
