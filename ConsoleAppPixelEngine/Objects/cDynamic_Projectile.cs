using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PixelEngine;

namespace ConsoleAppPixelEngine.Objects
{
    public class cDynamic_Projectile : DynamicObj
    {
        public cDynamic_Projectile(float ox, float oy, bool bFriend, float velx, float vely, float duration, Sprite sprite, float tx, float ty)
        :base("projectile")
        {
            pSprite = sprite;
            fSpriteX = tx;
            fSpriteY = ty;
            fDuration = duration;
            px = ox;
            py = oy;
            vx = velx;
            vy = vely;
            SolidVsDynamic = false;
            SolidVsMap = true;
            IsProjectile = true;
            IsAttackable = false;
            Friendly = bFriend;
        }


        public Sprite pSprite { get; set; }
        public float fSpriteX { get; set; }
        public float fSpriteY { get; set; }
        public float fDuration { get; set; }
        public bool bOneHit { get; set; } = true;
        public int nDamage { get; set; }

        public override void DrawSelf(OneLoneCoder_NachoGame gfx, float ox, float oy)
        {
            var p1 = new Point((int)((px - ox) * 16), (int)((py - oy) * 16));
            var p2 = new Point((int)(fSpriteX * 16), (int)(fSpriteY * 16));
            gfx.DrawPartialSprite(p1, pSprite, p2, 16, 16);
        }

        public override void Update(float fElapsedTime, DynamicObj player = null)
        {
            fDuration -= fElapsedTime;
            if (fDuration <= 0.0f)
                Redundant = true;
        }


    }
}
