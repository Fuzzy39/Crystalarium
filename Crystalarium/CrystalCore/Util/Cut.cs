using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace CrystalCore.Util
{
    struct Cut
    {

        // A cut represents a slice to be taked off of a rectangle, in a particular direction.
        public Rectangle parent { get; private set; }
        public int amount;
        public Direction dir; // which side of the parent are we cutting from?

        public float ratio
        {
            get
            {
                if (dir.IsHorizontal())
                {
                    return (float)amount / parent.Width;
                }

                return (float)amount / parent.Height;
            }

            set
            {
                if (dir.IsHorizontal())
                {
                    amount = (int)(value * parent.Width);
                }

                amount = (int)(value * parent.Height);
            }

        }

        public Rectangle slice
        {
            // returns what remains of the parent after the cut is taken.
            get
            {
                switch (dir)
                {
                    case Direction.up:
                        return new Rectangle(parent.X, parent.Y + amount, parent.Width, parent.Height - amount);
                    case Direction.left:
                        return new Rectangle(parent.X + amount, parent.Y, parent.Width - amount, parent.Height);
                    case Direction.down:
                        return new Rectangle(parent.X, parent.Y, parent.Width, parent.Height - amount);
                    case Direction.right:
                        return new Rectangle(parent.X, parent.Y, parent.Width - amount, parent.Height);

                }
                // this should never happen.
                Debug.Assert(false);
                return new Rectangle(0, 0, 0, 0);



            }
        }

        public Cut(Rectangle rect, Direction d, int amount)
        {
            parent = rect;
            dir = d;
            this.amount = amount;
        }

        public Cut(Rectangle rect, Direction d, float ratio) : this(rect, d, 0)
        {
            this.ratio = ratio;
        }

    }
}
