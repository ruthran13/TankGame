using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Shooter
{
    public class Coins
    {
        public int lifeTime, value, spentTime;
        public Vector2 position;
        public Boolean isAlive;

        public Coins(int aLifeTime, int aValue, Vector2 aPosition)
        {
            lifeTime = aLifeTime;
            value = aValue;
            position = aPosition;
            isAlive = true;
            spentTime = 0;
        }
    }
}
