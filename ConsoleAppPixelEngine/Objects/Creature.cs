using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PixelEngine;

namespace ConsoleAppPixelEngine.Objects
{
    public class Creature : DynamicObj
    {
        public Creature(string name, Sprite sprite)
            :base(name)// Calling the base constructor

        {
            this.Sprite = sprite;
            Health = 10;
            MaxHealth = 10;
            FacingDirection = Direction.EAST;
            sprGraphicsState = GraphicsState.Falling;
            Timer = 0.0f;
            GraphicCounter = 0;
        }

        //States
        private Direction FacingDirection { get; set; }
        private GraphicsState sprGraphicsState { get; set; }
        private int GraphicCounter { get; set; }
        private float Timer { get; set; }
    
        protected Sprite Sprite { get; set; } // m_pSprite
        public int Health { get; set; }
        public int MaxHealth { get; set; }


        public override void Update(float elapsedTime)
        {
            // En bubbla av tid som förflyttar sig oberoende av resten av spelets state. Skapar ett slags pendel, gissar jag på.
            Timer += elapsedTime;
            if (Timer >=0.2f)
            {
                Timer -= 0.2f;

                // tänkt att "ocilera" (vad fan det nu är på svenska). ticka mellan 0 och 1
                GraphicCounter++; 
                GraphicCounter %= 2;
            }

            if (Math.Abs(vX) > 0 || Math.Abs(vY) > 0) //fabs?
            {
                sprGraphicsState = GraphicsState.Walking;
            }
            else
            {
                sprGraphicsState = GraphicsState.Standing;
            }

            if (Health <= 0)
            {
                sprGraphicsState = GraphicsState.Dead;
            }


            if (vX < -0.1f)
                FacingDirection = Direction.WEST;
            if (vX > 0.1f)
                FacingDirection = Direction.EAST;
            if (vY < -0.1f)
                FacingDirection = Direction.NORTH;
            if (vY > 0.1f)
                FacingDirection = Direction.SOUTH;

        }

        public override void DrawSelf(OneLoneCoder_NachoGame gfx, float ox, float oy)  // gfx  = graphics //  olcConscoleGameEngineOOP
        {
            // Måste draw rätt sprite som passar state som creature är i, in this point in time

            // Mosvarar vart på spriten som ska ritas. 
            int SheetOffsetX = 0; //Uppe till vänster är sheet offset 0. (noll index)
            int SheetOffsetY = 0;// Om y är 1 så är det en rad ner (noll index)

            switch (sprGraphicsState)
            {
                case GraphicsState.Standing:
                    SheetOffsetX = (int)FacingDirection * 16;
                    break;

                case GraphicsState.Walking:
                    SheetOffsetX = (int)FacingDirection * 16; // så typ den övre raden är åt vilket håll, sen switcha mellan övre raden och undre raden i hans sprite.
                    SheetOffsetY = (int) FacingDirection * 16;
                    break;

                case GraphicsState.Celebrating:
                    SheetOffsetX = 4 * 16; // för det finns bara 1 sprite för det..
                    break;

                case GraphicsState.Dead:
                    SheetOffsetX = 4 * 16; // fyra till höger,
                    SheetOffsetY = 1 * 16; // en ner
                    break;
            }


            //Sen är det dags att rita ut spriten

            // dynamiska objektet finns i world space, men måste rita den i scrren space. 1 - 1 translation eftersom alla enheter är en / en enheter.
            //Vi måste bara ta reda på vart kameran titar i world space.
            var firstMagicalPlayerParam = new Point((int)((pX - ox)*16.0f), (int)((pY - oy)*16.0f)); // Vart tilen ska ritas.

            // SheetOffsetX och SheetOffsetY ger top left in en sprite
            var secondMagicalPlayerParam = new Point(SheetOffsetX, SheetOffsetY); // Vilken tile i spritesheeten som ska ritas.

            // 16 är för närvarande en full enhet 
            gfx.DrawPartialSprite(firstMagicalPlayerParam, Sprite, secondMagicalPlayerParam, 16, 16);

        } //gfx. // Vet i fasiken vad det är han menar. ska den ha en instance av OneLoneCoder_NachoGame ?

      

     

    }

    /// <summary>
    ///  Riktning
    /// </summary>
    enum Direction
    {
        SOUTH = 0,
        WEST = 1,
        NORTH = 2,
        EAST = 3
    }

    enum GraphicsState
    {
        Standing,
        Walking,
        Celebrating,
        Dead,
        Falling
    }

}
