using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewChessProject
{
    class HistoryNode
    {
        private List<PieceRepresentation> boardPosition;
        private HistoryNode previousNode;

        public HistoryNode(List<PieceRepresentation> boardPosition, HistoryNode previousNode)
        {
            this.boardPosition = boardPosition;
            this.previousNode = previousNode;
        }

        public List<PieceRepresentation> BoardPosition
        {
            get
            {
                return boardPosition;
            }
            set
            {
                boardPosition = value;
            }
        }

        public HistoryNode PreviousNode
        {
            get
            {
                return previousNode;
            }
            set
            {
                previousNode = value;
            }
        }

        public static bool operator ==(HistoryNode hn1, HistoryNode hn2)
        {
            bool output = true;
            if (((object)hn1) == null || ((object)hn2) == null)
            {
                Console.WriteLine("null");
                if(((object)hn1) == null && ((object)hn2) == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }


            foreach (PieceRepresentation pr in hn1.BoardPosition)
            {
                if (!hn2.BoardPosition.Contains(pr))
                {
                    output = false;
                    break;
                }
            }
            
            return (hn1.BoardPosition.Count == hn2.BoardPosition.Count) && output;
        }

        public static bool operator !=(HistoryNode hn1, HistoryNode hn2)
        {
            return !(hn1 == hn2);
        }

    }
}
