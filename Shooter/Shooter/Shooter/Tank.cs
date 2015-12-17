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
    class Tank
    {
        public Vector2 position;
        public int direction, health, coins, points;
        public String name;
        public Boolean hasShot;
        public int status;           //alive=0; dying=1 dead=2
        public Color color;

        public Tank(int aPlayerName, Color aColor, Vector2 aPosition, int aDirection)
        {
            status=0;
            this.hasShot = false;
            this.coins = this.points = 0;
            this.direction = aDirection;
            this.health = 100;
            this.name = "P" + (aPlayerName).ToString();
            this.color = aColor;
            this.position = aPosition;
        }
        public Tank(int aPlayerName, int aDirection)
        {
            this.hasShot = false;
            this.coins = this.points = 0;
            this.direction = aDirection;
            this.health = 100;
            this.name = "P" + (aPlayerName).ToString();
        }
    }

}
