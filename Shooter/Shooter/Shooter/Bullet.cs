using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Shooter
{
    class Bullet
    {
        public int direction;
        public Vector2 position;
        public Boolean hit;

        public Bullet(int aDirection, Vector2 aPosition)
        {
            hit = false;
            direction = aDirection;
            position = aPosition;
        }
        
    }
}
