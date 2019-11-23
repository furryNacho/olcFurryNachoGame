using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleAppPixelEngine.Objects;
using PixelEngine;

namespace ConsoleAppPixelEngine.Items
{
    public class Item
    {
        public Item(string name, Sprite sprite, string desc)
        {
            sName = name;
            pSprite = sprite;
            sDescription = desc;
        }

        public virtual bool OnInteract(DynamicObj myobject) { return false; }
        public virtual bool OnUse(DynamicObj myobject) { return false; }


        public string sName { get; set; }
        public string sDescription { get; set; }
        public Sprite pSprite { get; set; }
        public bool bKeyItem { get; set; } = false;
        public bool bEquipable { get; set; } = false;

        public static OneLoneCoder_NachoGame g_engine { get; set; }
    };



    class cItem_Health : Item // give player 10hp
    {

        public cItem_Health() : base("Small Health", Assets.Instance.GetSprite("health"), "Restores 10 health")
        {

        }
        public override bool OnInteract(DynamicObj myobject)
        {
            OnUse(myobject);
            return false; // Just absorb
        }
        public override bool OnUse(DynamicObj myobject)
        {
            if (myobject != null)
            {
                Creature dyn = (Creature)myobject;
                dyn.Health = Math.Min(dyn.Health + 10, dyn.MaxHealth); // inte mer än max
            }
            return true; // försvinner
        }
    };

    class cItem_HealthBoost : Item // raise max hp 10
    {

        public cItem_HealthBoost()
        : base("Health Boost", Assets.Instance.GetSprite("healthboost"), "Increases Max Health by 10")
        {

        }
        public override bool OnInteract(DynamicObj myobject)
        {
            return true; // Add to inventory
        }

        public override bool OnUse(DynamicObj myobject)
        {
            if (myobject != null)
            {
                Creature dyn = (Creature)myobject;
                dyn.MaxHealth += 10;
                dyn.Health = dyn.MaxHealth;
            }

            return true; // Remove from inventory
        }
    };


    public class cWeapon : Item
    {

        public cWeapon(string name, Sprite sprite, string desc, int dmg)
            : base(name, sprite, desc)
        {
            nDamage = dmg;
        }

        public override bool OnInteract(DynamicObj myobject)
        {
            return false;
        }

        public override bool OnUse(DynamicObj myobject)
        {
            return false;
        }


        public int nDamage = 0;
    };


    class cWeapon_Sword : cWeapon
    {

        public cWeapon_Sword()
        : base("Basic Sword", Assets.Instance.GetSprite("sword"), "A wooden sword, 5 dmg", 5)
        {

        }


        public override bool OnUse(DynamicObj myobject)
        {
            // When weapons are used, they are used on the object that owns the weapon, i.e.
            // the attacker. However this does not imply the attacker attacks themselves

            // Get direction of attacker
            Creature aggressor = (Creature)myobject;

            // Determine attack origin
            float x = 0, y = 0, vx = 0, vy = 0;
            if (aggressor.GetFacingDirection() == 0) // South
            {
                x = aggressor.px;
                y = aggressor.py + 1.0f;
                vx = 0.0f; vy = 1.0f;
            }

            if (aggressor.GetFacingDirection() == 1) // East
            {
                x = aggressor.px - 1.0f;
                y = aggressor.py;
                vx = -1.0f; vy = 0.0f;
            }

            if (aggressor.GetFacingDirection() == 2) // North
            {
                x = aggressor.px;
                y = aggressor.py - 1.0f;
                vx = 0.0f; vy = -1.0f;
            }

            if (aggressor.GetFacingDirection() == 3) // West
            {
                x = aggressor.px + 1.0f;
                y = aggressor.py;
                vx = 1.0f; vy = 0.0f;
            }

            if (aggressor.Health == aggressor.MaxHealth)
            {
                // Beam sword
                cDynamic_Projectile pLaser = new cDynamic_Projectile(x, y, aggressor.Friendly, vx * 15.0f, vy * 15.0f, 1.0f, Assets.Instance.GetSprite("sword"), (aggressor.GetFacingDirection() + 3) % 4 + 1, 1.0f);
                pLaser.SolidVsMap = true;
                pLaser.SolidVsDynamic = false;
                pLaser.nDamage = 5;
                pLaser.bOneHit = false;
                g_engine.AddProjectile(pLaser);
            }

            cDynamic_Projectile p = new cDynamic_Projectile(x, y, aggressor.Friendly, aggressor.vx, aggressor.vy, 0.1f, Assets.Instance.GetSprite("sword"), (aggressor.GetFacingDirection() + 3) % 4 + 1, 0.0f);
            p.SolidVsMap = false;
            p.SolidVsDynamic = false;
            p.nDamage = 5;
            p.bOneHit = true;

            g_engine.AddProjectile(p);

            return false;
        }
    };

}

