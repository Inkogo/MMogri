using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMogri
{
    struct Direction
    {
        int x, y;

        Direction(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static IEnumerable<Direction> Directions()
        {
            yield return Up;
            yield return Left;
            yield return Down;
            yield return Right;
        }

        public static Direction Up
        {
            get
            {
                return new Direction(0, 1);
            }
        }
        public static Direction Down
        {
            get
            {
                return new Direction(0, -1);
            }
        }
        public static Direction Left
        {
            get
            {
                return new Direction(-1, 0);
            }
        }
        public static Direction Right
        {
            get
            {
                return new Direction(1, 0);
            }
        }

        public static implicit operator Point(Direction d)
        {
            return new Point(d.x, d.y);
        }
    }
}