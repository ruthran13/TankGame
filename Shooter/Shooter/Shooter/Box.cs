using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
namespace Shooter
{
    class Box
    {
        public int f,h,c;
        public Boolean computed;
        public Boolean computing;
        //private int p;
        public Box predecessor;
        public Vector2 position;
        
        public Box()
        {
            f = 1000;
            h = 1000;
        }

        public Box(int anH, int aCost, Vector2 aPosition)
        {
            this.h = anH;
            this.c = aCost;
            this.f = anH+aCost;
            this.position = aPosition;
        }
    }
}
