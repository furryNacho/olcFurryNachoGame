using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleAppPixelEngine.Objects;

namespace ConsoleAppPixelEngine.Commands
{
    public class Command
    {
        public Command()
        {
            Completed = false;
            Started = false;
        }

        public bool Completed { get; set; }
        public bool Started { get; set; }

        public virtual void Start() { }
        public virtual void Update(float elapsedTime) { }


        public static OneLoneCoder_NachoGame g_engine { get; set; }

    }

    public class ScriptProcessor
    {
        public ScriptProcessor()
        {
            UserControlEnabled = true;
        }

        private List<Command> listCommands { get; set; } = new List<Command>();

        public bool UserControlEnabled { get; set; }

        public virtual void AddCommand(Command cmd)
        {
            listCommands.Add(cmd);
        }

        public void ProcessCommands(float elapsedTime)
        {
            UserControlEnabled = !listCommands.Any(); // finns inget i listan ge kontroll till spelaren

            if (!UserControlEnabled) // Om det finns nåt i listan, gör nåt
            {
                if (!listCommands.FirstOrDefault().Completed) // Om inte den första processen i listan är klar
                {
                    if (!listCommands.FirstOrDefault().Started)// Om första objektet inte är startad, starta den
                    {
                        listCommands.FirstOrDefault().Start();
                        listCommands.FirstOrDefault().Started = true;
                    }
                    else
                    {
                        // currently in process
                        listCommands.FirstOrDefault().Update(elapsedTime);
                    }

                }
                else
                {
                    //Command has been completed
                    listCommands.RemoveAt(0);
                }
            }

        }

        public void CompletedCommand()
        {
            if (listCommands.Any())
            {
                listCommands.FirstOrDefault().Completed = true;
            }
        }
    }


    public class CommandMoveTo : Command
    {
        // target x and y position in world space, duration hur fort det ska gå
        public CommandMoveTo(DynamicObj myObject, float x, float y, float duration = 0.0f)
        {
            TargetPosX = x;
            TargetPosY = y;
            TimeSoFar = 0.0f;
            Duration = Math.Max(duration, 0.001f);
            this.myObject = myObject;
        }

        public override void Start()
        {
            StartPosX = myObject.pX;
            StartPosY = myObject.pY;
        }
        public override void Update(float elapsedTime)
        {
            // linger interpolation
            TimeSoFar += elapsedTime;
            float t = TimeSoFar / Duration;
            if (t > 1.0f)
            {
                t = 1.0f;
            }

            // speed = distance over time
            myObject.pX = (TargetPosX - StartPosX) * t + StartPosX;
            myObject.pY = (TargetPosY - StartPosY) * t + StartPosY;
            myObject.vX = (TargetPosX - StartPosX) / Duration;
            myObject.vY = (TargetPosY - StartPosY) / Duration;

            if (TimeSoFar >= Duration)
            {
                //Object has reached destination, sp stop
                myObject.pX = TargetPosX;
                myObject.pY = TargetPosY;
                myObject.vX = 0.0f;
                myObject.vY = 0.0f;
                Completed = true;
            }

        }


        // Klassen tar kontroll över objektet.
        private DynamicObj myObject { get; set; }
        // startposition
        public float StartPosX { get; set; }
        public float StartPosY { get; set; }
        //Slutposition
        public float TargetPosX { get; set; }
        public float TargetPosY { get; set; }
        // Store value for the duration
        public float Duration { get; set; }
        //time passed so far
        public float TimeSoFar { get; set; }

    }




    public class CommandShowDialog : Command
    {
        public CommandShowDialog(List<string> line)
        {
            listLines = line;
        }

        public List<string> listLines { get; set; } = new List<string>();

        public override void Start()
        {
            g_engine.ShowDialog(listLines);
        }


    }


}
