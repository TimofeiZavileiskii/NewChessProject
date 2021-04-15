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
        HilightType type;

        public BoardIndicator(Vector vector, HilightType type)
        {
            this.vector = vector;
            this.type = type;
        }

        public Vector Position
        {
            get
            {
                return vector;
            }
        }

        public HilightType Type
        {
            get
            {
                return type;
            }
        }


    }
}
