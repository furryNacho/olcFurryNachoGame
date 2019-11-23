using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppPixelEngine.Objects
{
    public class DynamicObj
    {
        public DynamicObj() { }
        public DynamicObj(string name)
        {
            this.Name = name;
            px = 0.0f;
            py = 0.0f;
            vx = 0.0f;
            vy = 0.0f;
            SolidVsDynamic = true;
            SolidVsMap = true;
            Friendly = true;
            Redundant = false;
            IsAttackable = false;
            IsProjectile = false;
        }

        /// <summary>
        /// Position X
        /// </summary>
        public float px { get; set; }
        /// <summary>
        /// Position X
        /// </summary>
        public float py { get; set; }
        /// <summary>
        /// Velocity  X
        /// </summary>
        public float vx{ get; set; }
        /// <summary>
        /// Velocity Y
        /// </summary>
        public float vy { get; set; }

        /// <summary>
        /// Är solid mot kartan
        /// </summary>
        public bool SolidVsMap { get; set; }
        /// <summary>
        /// Är solid mot andra objekt
        /// </summary>
        public bool SolidVsDynamic { get; set; }
        /// <summary>
        /// Är vänlig
        /// </summary>
        public bool Friendly { get; set; }
        /// <summary>
        /// Namnet på dynamiskt objekt
        /// </summary>
        public string Name { get; set; }

        public bool Redundant { get; set; }

        public bool IsAttackable { get; set; }
        public bool IsProjectile { get; set; }

        public bool Controllable { get; set; } = true;
        protected float m_fKnockBackTimer = 0.0f;
        protected float m_fKnockBackDX = 0.0f;
        protected float m_fKnockBackDY = 0.0f;

        public static OneLoneCoder_NachoGame g_engine { get; set; }

        /// <summary>
        /// Ansvarar själv för hur den ser ut på skärmen.  Tar en instans av olcGameEnigne
        /// ox oy offset som typ betyder kamera
        /// </summary>
        public virtual void DrawSelf(OneLoneCoder_NachoGame graphics, float ox, float oy) { } //gfx. // Vet i fasiken vad det är han menar. ska den ha en instance av OneLoneCoder_NachoGame ?
        //public virtual void DrawSelf(olcConscoleGameEngineOOP graphics, float ox, float oy){} //gfx. // Vet i fasiken vad det är han menar. ska den ha en instance av OneLoneCoder_NachoGame ?

        /// <summary>
        /// Elapsed time
        /// </summary>
        /// <param name="el"></param>
        public virtual void Update(float elapsedTime, DynamicObj player = null) { }

        public virtual void OnInteract(DynamicObj player = null) { }

    }
}
