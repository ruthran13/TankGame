using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Shooter
{
    public class LifePack
    {
        public int lifeTime;
        public Vector2 position;
        public Boolean isAlive;
        public int spentTime;

        public LifePack(int aLifeTime, Vector2 aPosition)
        {
            lifeTime = aLifeTime;
            position = aPosition;
            isAlive = true;
            spentTime = 0;
        }
    }
}
