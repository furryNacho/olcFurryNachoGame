using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleAppPixelEngine.Items;
using PixelEngine;

namespace ConsoleAppPixelEngine.Objects
{
    public class cDynamic_Item : DynamicObj
    {


        public cDynamic_Item(float x, float y, Item item)
        :base("pickup")
        {
            px = x;
            py = y;
            SolidVsDynamic = false;
            SolidVsMap = false;
            Friendly = true;
            bCollected = false;
            this.item = item;
        }

        public Item item { get; set; }
        public bool bCollected { get; set; }

        public override void DrawSelf(OneLoneCoder_NachoGame graphics, float ox, float oy)
        {
            if(bCollected)
                return;


            var firstMagicalPlayerParamNew = new Point((int)((px - ox)*16.0f), (int)((py-oy)*16.0f));
            var secondMagicalPlayerParamNew = new Point((int)(0), (int)(0));
            graphics.DrawPartialSprite(firstMagicalPlayerParamNew, item.pSprite, secondMagicalPlayerParamNew, 16, 16);
            
        }

        public override void OnInteract(DynamicObj player = null)
        {
            if (bCollected)
                return;



            if (item.OnInteract(player))
            {
                // Add item to inventory
                g_engine.GiveItem(item);
            }

            bCollected = true;
        }
    }
}
