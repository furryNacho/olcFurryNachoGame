using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleAppPixelEngine.Commands;
using ConsoleAppPixelEngine.Objects;

namespace ConsoleAppPixelEngine.Quest
{
    public class Quest
    {
        public Quest()
        {

        }



        public string sName { get; set; }
        public bool bCompleted { get; set; } = false;
        public static ScriptProcessor g_script { get; set; }
        public static OneLoneCoder_NachoGame g_engine { get; set; }



        public virtual bool OnInteraction(List<DynamicObj> vecDynobs, DynamicObj target, NATURE nature) { return true; }

        public virtual bool PopulateDynamics(List<DynamicObj> vecDyns, string sMap) { return true; }

    }



    class cQuest_MainQuest : Quest
    {

        public cQuest_MainQuest(OneLoneCoder_NachoGame engine)
        {
            g_engine = engine;
        }

        public override bool PopulateDynamics(List<DynamicObj> vecDyns, string sMap)
        {
            if (sMap == "lvl1")
            {
                Creature c1 = new Creature("sarah", Assets.Instance.GetSprite("skally"));
                c1.px = 6.0f;
                c1.py = 4.0f;
                c1.Friendly = true;
                vecDyns.Add(c1);
            }

            if (sMap == "lvl2")
            {
                Creature c1 = new Creature("bob", Assets.Instance.GetSprite("skally"));
                c1.px = 12.0f;
                c1.py = 4.0f;
                vecDyns.Add(c1);
            }


            return true;
        }

        public override bool OnInteraction(List<DynamicObj> vecDynobs, DynamicObj target, NATURE nature)
        {
            if (target.Name == "sarah")
            {
                //g_script.AddCommand(new CommandShowDialog(new List<string>()
                //{
                //    "Sarah"
                //}));


                //X(ShowDialog({ "[Sarah]", "You have no additional", "quests!" }));

                //if (g_engine.HasItem(Assets.Instance.GetItem("Health Boost"))) 
                if (g_engine.HasItem(Assets.Instance.GetItem("healthboost"))) // healthboost
                {
                    //X(ShowDialog({ "[Sarah]", "Woooooow! You have a health", "boost!" }));
                    g_script.AddCommand(new CommandShowDialog(new List<string>()
                    {
                        "Woooooow! You have a health boost!"
                    }));

                }
                else
                {
                    //X(ShowDialog({ "[Sarah]", "Boooooo! You dont have a health", "boost!" }));
                    g_script.AddCommand(new CommandShowDialog(new List<string>()
                    {
                        "Boooooo! You dont have a health boost!"
                    }));

                }

            }


            if (target.Name == "bob")
            {
                //X(ShowDialog({ "[Bob]", "I need you to do", "something for me!" }));
                //X(ShowDialog({ "[Bob]", "Predictably, there are", "rats in my basement!"}));
                g_script.AddCommand(new CommandShowDialog(new List<string>()
                {
                    "I need you to do", "something for me!"
                }));



                //X(AddQuest(new cQuest_BobsQuest()));
                //g_script.AddQuest(new cQuest_BobsQuest());
                g_engine.AddQuest(new cQuest_BobsQuest());
            }

            return false;
        }


        private int m_nPhase = 0;
    };


    class cQuest_BobsQuest : Quest
    {

        public override bool PopulateDynamics(List<DynamicObj> vecDyns, string sMap)
        {
            return true;
        }

        public override bool OnInteraction(List<DynamicObj> vecDynobs, DynamicObj target, NATURE nature)
        {
            if (target.Name == "sarah")
            {
                //X(ShowDialog({ "[Sarah]", "You are doing Bob's", "quest!" }));

                g_script.AddCommand(new CommandShowDialog(new List<string>()
                {
                    "bobs quest "
                }));

                return true;
            }

            return false;
        }


        private int m_nPhase = 0;
    };

}
