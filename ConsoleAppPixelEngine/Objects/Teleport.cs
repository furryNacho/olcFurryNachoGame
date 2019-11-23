using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PixelEngine;

namespace ConsoleAppPixelEngine.Objects
{
    public class Teleport : DynamicObj
    {
        public Teleport(float x, float y, string sMapName, float tx, float ty):base("Teleport")
        {
            px = x;
            py = y;
            this.fMapPosX = tx;
            this.fMapPosY = ty;
            this.sMApName = sMapName;
            SolidVsDynamic = false;
            SolidVsMap = false;
        }

        public override void DrawSelf(OneLoneCoder_NachoGame graphics, float ox, float oy)
        {
            // Does nothing
            // för att kunna se
            int f = (int)(((px + 0.5f) - ox) * 16.0f);
            int s = (int)(((py + 0.5f) - oy) * 16.0f);
            var point = new Point(f, s);
            //   graphics.DrawCircle(((px + 0.5f) - ox) * 16.0f, ((py + 0.5f) - oy) * 16.0f, 0.5f * 16.0f); // For debugging
            var radius = (int)(16.0f * 0.5f);
            //var radius = 20;
            var color = Pixel.Random();
            graphics.DrawCircle(point, radius, color);
        }
      
        public override void Update(float elapsedTime, DynamicObj player=null)
        {
            //does nothing
        }

        //public override void Update(float elapsedTime, DynamicObj player = null)
        //{
        //}

        public string sMApName { get; set; }
        public float fMapPosX { get; set; }
        public float fMapPosY { get; set; }
    }
}
