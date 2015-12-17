using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Shooter
{
    class Obstacle
    {
        public Vector2 position;
        public String type;
        public int damageLevel;

        public Obstacle(String aType, Vector2 aPosition)
        {
            this.type = aType;
            this.damageLevel = 0;
            this.position = aPosition;
        }
    }
}
