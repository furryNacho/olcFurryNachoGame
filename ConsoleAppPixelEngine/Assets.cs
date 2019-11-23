using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleAppPixelEngine.Items;
using PixelEngine;

namespace ConsoleAppPixelEngine
{
    public sealed class Assets
    {
        Assets()
        {
        }
        private static readonly object padlock = new object();
        private static Assets instance = null;
        public static Assets Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new Assets();
                    }
                    return instance;
                }
            }
        }

        private Dictionary<string, Sprite> m_mapSprites { get; set; } = new Dictionary<string, Sprite>();
        private Dictionary<string, cMap> m_mapMaps { get; set; } = new Dictionary<string, cMap>();

        private Dictionary<string, Item> m_mapItems { get; set; } = new Dictionary<string, Item>();

        public Item GetItem(string name)
        {
            Item item;
            if (m_mapItems.TryGetValue(name, out item))
            {
                return item;
            }
            else
            {
                return null;
            }
        }

        public cMap GetMap(string name)
        {
            cMap cMap;
            if (m_mapMaps.TryGetValue(name, out cMap))
            {
                return cMap;
            }
            else
            {
                return null;
            }
        }

        public Sprite GetSprite(string name)
        {
            Sprite sprite;
            if (m_mapSprites.TryGetValue(name, out sprite))
            {
                return sprite;
            }
            else
            {
                return null;
            }
        }

        public void LoadItems()
        {
            var hart = new cItem_Health();
            m_mapItems.Add("health", hart);
            var boost = new cItem_HealthBoost();
            m_mapItems.Add("healthboost", boost);

            var sword = new cWeapon_Sword();
            m_mapItems.Add("sword", sword);
        }

        public void LoadMaps()
        {
            var lvl1 = new cMap_Village1();
            m_mapMaps.Add("lvl1", lvl1);
            var lvl2 = new cMap_Home1();
            m_mapMaps.Add("lvl2", lvl2);
        }

        public void LoadSprites()
        {


            LoadS("forest", RootPath(@"Properties\testtilesheet.bmp"));
            LoadS("font", RootPath(@"Properties\Retro.png"));
            LoadS("player", RootPath(@"Properties\SpriteScarlet.bmp"));
            LoadS("skally", RootPath(@"Properties\skally.png"));

            LoadS("sword", RootPath(@"Properties\sword.png"));
            LoadS("health", RootPath(@"Properties\health.png"));
            LoadS("healthboost", RootPath(@"Properties\healthboost.png"));
            //Load("forest", @"C:\Users\kim_k\source\repos\ConsoleAppPixelEngine\ConsoleAppPixelEngine\Properties\testtilesheet.bmp");
            //Load("font", @"C:\Users\kim_k\source\repos\ConsoleAppPixelEngine\ConsoleAppPixelEngine\Properties\Retro.png");
            //Load("player", @"C:\Users\kim_k\source\repos\ConsoleAppPixelEngine\ConsoleAppPixelEngine\Properties\SpriteScarlet.bmp");
            //Load("skally", @"C:\Users\kim_k\source\repos\ConsoleAppPixelEngine\ConsoleAppPixelEngine\Properties\skally.png");
        }
        private void LoadS(string sName, string sFileName)
        {
            var sprite = Sprite.Load(sFileName);
            m_mapSprites.Add(sName, sprite);
        }

      


        public string RootPath(string Path = "")
        {
            try
            {


                string fileLocation = System.IO.Path.Combine(Environment.CurrentDirectory, Path); // ex  @"Content\Settings\favIconNacho.png"
                if (!File.Exists(fileLocation))
                {
                    //throw new Exception("Can't find directory folder " + Path);
                    return @"C:\Users\kim_k\source\repos\ConsoleAppPixelEngine\ConsoleAppPixelEngine\" + Path;
                }


                //TODO: hantera Path och se till så det finns mappar och skit i produktion/debug
                //    //var asdf = Environment.CurrentDirectory;
                //    //asdf="C:\\Users\\kim_k\\source\\repos\\ConsoleAppPixelEngine\\ConsoleAppPixelEngine\\bin\\Debug"
                //    //Börja kolla att path finns
                //    if (!string.IsNullOrEmpty(Path))
                //{
                //    // param ex: @"Content\Load\Tiles"
                //    string checkDir = System.IO.Path.Combine(Environment.CurrentDirectory, Path);
                //    if (System.IO.Directory.Exists(checkDir))
                //    {
                //        throw new Exception("Can't find directory folder "+ Path);
                //    }
                //}

                //string fileLocation = System.IO.Path.Combine(Environment.CurrentDirectory, @"Content\Settings\favIconNacho.png");
                //if (!File.Exists(fileLocation))
                //{
                //    try
                //    {
                //        //@"C:\Users\kim_k\source\repos\FurryNachoLevelEditor\FurryNachoLevelEditor\Content\Settings\favIconNacho.png"
                //    }
                //    catch (Exception e)
                //    {
                //    }
                //}

                return fileLocation;
            }
            catch (Exception e)
            {
                throw;
            }



        }

    }

}
