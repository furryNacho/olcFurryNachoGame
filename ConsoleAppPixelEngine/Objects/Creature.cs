using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleAppPixelEngine.Items;
using PixelEngine;

namespace ConsoleAppPixelEngine.Objects
{
    public class Creature : DynamicObj
    {
        public Creature(string name, Sprite sprite)
            : base(name)// Calling the base constructor

        {
            this.Sprite = sprite;
            Health = 5;
            MaxHealth = 10;
            FacingDirection = Direction.EAST;
            sprGraphicsState = GraphicsState.Falling;
            Timer = 0.0f;
            GraphicCounter = 0;
            IsAttackable = true;
        }

        //States
        private Direction FacingDirection { get; set; }
        private GraphicsState sprGraphicsState { get; set; }
        private int GraphicCounter { get; set; }
        private float Timer { get; set; }

        protected Sprite Sprite { get; set; } // m_pSprite
        public int Health { get; set; }
        public int MaxHealth { get; set; }


        public override void Update(float elapsedTime, DynamicObj player)
        {
            if (m_fKnockBackTimer > 0.0f)
            {
                vx = m_fKnockBackDX * 10.0f;
                vy = m_fKnockBackDY * 10.0f;
                IsAttackable = false;
                m_fKnockBackTimer -= elapsedTime;
                if (m_fKnockBackTimer <= 0.0f)
                {
                    m_fStateTick = 0.0f;
                    Controllable = true;
                    IsAttackable = true;
                }
            }
            else
            {

                // En bubbla av tid som förflyttar sig oberoende av resten av spelets state. Skapar ett slags pendel, gissar jag på.
                Timer += elapsedTime;
                if (Timer >= 0.2f)
                {
                    Timer -= 0.2f;

                    // tänkt att "ocilera" (vad fan det nu är på svenska). ticka mellan 0 och 1
                    GraphicCounter++;
                    GraphicCounter %= 2;
                }

                if (Math.Abs(vx) > 0 || Math.Abs(vy) > 0) //fabs?
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


                if (vx < -0.1f)
                    FacingDirection = Direction.WEST;
                if (vx > 0.1f)
                    FacingDirection = Direction.EAST;
                if (vy < -0.1f)
                    FacingDirection = Direction.NORTH;
                if (vy > 0.1f)
                    FacingDirection = Direction.SOUTH;

                Behaviour(elapsedTime, player);
            }
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
                    SheetOffsetY = (int)FacingDirection * 16;
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
            var firstMagicalPlayerParam = new Point((int)((px - ox) * 16.0f), (int)((py - oy) * 16.0f)); // Vart tilen ska ritas.

            // SheetOffsetX och SheetOffsetY ger top left in en sprite
            var secondMagicalPlayerParam = new Point(SheetOffsetX, SheetOffsetY); // Vilken tile i spritesheeten som ska ritas.

            // 16 är för närvarande en full enhet 
            gfx.DrawPartialSprite(firstMagicalPlayerParam, Sprite, secondMagicalPlayerParam, 16, 16);

        } //gfx. // Vet i fasiken vad det är han menar. ska den ha en instance av OneLoneCoder_NachoGame ?

        public virtual void Behaviour(float fElapsedTime, DynamicObj player = null) { }

        public virtual void PerformAttack() { }

        public virtual void KnockBack(float dx, float dy, float dist)
        {
            m_fKnockBackDX = dx;
            m_fKnockBackDY = dy;
            m_fKnockBackTimer = dist;
            SolidVsDynamic = false;
            Controllable = false;
            IsAttackable = false;
        }

 

        public cWeapon pEquipedWeapon = null;

        public int GetFacingDirection()
        {
            return (int)FacingDirection;
        }

        public float m_fStateTick { get; set; }

    }

    public class DynamicCreatureSkelly : Creature
    {

        public DynamicCreatureSkelly() : base("skally", Assets.Instance.GetSprite("skally"))
        {
            Friendly = false;
            Health = 2;
            MaxHealth = 2;
            SolidVsDynamic = true;
            SolidVsMap = true;
              pEquipedWeapon = (cWeapon)Assets.Instance.GetItem("sword"); // ge vapen till fiende
        }

        public override void Behaviour(float fElapsedTime, DynamicObj player = null)
        {
            if (Health <= 0)
            {
                vx = 0;
                vy = 0;
                SolidVsDynamic = false;
                IsAttackable = false;
                return;
            }

            if (player != null)
            {
                // För att jaga player. 

                // no default behaviour
                // Check if player is nearby
                float fTargetX = player.px - px;
                float fTargetY = player.py - py;
                float fDistance = (float)Math.Sqrt(fTargetX * fTargetX + fTargetY * fTargetY);

                m_fStateTick -= fElapsedTime;

                if (m_fStateTick <= 0.0f) // för att inte göra beslut så ofta. 
                {
                    if (fDistance < 6.0f)
                    {
                        vx = (fTargetX / fDistance) * 2.0f;
                        vy = (fTargetY / fDistance) * 2.0f;

                        if (fDistance < 1.5f) // för att attakera med projektil
                            PerformAttack();
                    }
                    else
                    {
                        vx = 0;
                        vy = 0;
                    }

                    m_fStateTick += 1.0f;
                }
            }

        }


        public override void PerformAttack()
        {
            if (pEquipedWeapon == null)
                return;

            pEquipedWeapon.OnUse(this);
        }
    }

    public class DynamicCreatureHero : Creature
    {
        public DynamicCreatureHero() : base("player", Assets.Instance.GetSprite("player"))
        {
            Friendly = true;
            Health = 9;
            MaxHealth = 10;
            m_fStateTick = 2.0f;
            pEquipedWeapon = (cWeapon)Assets.Instance.GetItem("sword");
        }

        public override void PerformAttack()
        {
            if (pEquipedWeapon == null)
                return;

            pEquipedWeapon.OnUse(this);
        }
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
