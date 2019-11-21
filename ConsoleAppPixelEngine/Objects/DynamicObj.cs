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
            pX = 0.0f;
            pY = 0.0f;
            vX = 0.0f;
            vY = 0.0f;
            SolidVsDynamic = true;
            SolidVsMap = true;
            Friendly = true;
        }

        /// <summary>
        /// Position X
        /// </summary>
        public float pX { get; set; }
        /// <summary>
        /// Position X
        /// </summary>
        public float pY { get; set; }
        /// <summary>
        /// Velocity  X
        /// </summary>
        public float vX{ get; set; }
        /// <summary>
        /// Velocity Y
        /// </summary>
        public float vY { get; set; }

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
        public virtual void Update(float elapsedTime) {}


    }
}
