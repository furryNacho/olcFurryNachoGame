using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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


        public void LoadSprites()
        {


            Load("forest", RootPath(@"Properties\testtilesheet.bmp"));
            Load("font", RootPath(@"Properties\Retro.png"));
            Load("player", RootPath(@"Properties\SpriteScarlet.bmp"));
            Load("skally", RootPath(@"Properties\skally.png"));

            //Load("forest", @"C:\Users\kim_k\source\repos\ConsoleAppPixelEngine\ConsoleAppPixelEngine\Properties\testtilesheet.bmp");
            //Load("font", @"C:\Users\kim_k\source\repos\ConsoleAppPixelEngine\ConsoleAppPixelEngine\Properties\Retro.png");
            //Load("player", @"C:\Users\kim_k\source\repos\ConsoleAppPixelEngine\ConsoleAppPixelEngine\Properties\SpriteScarlet.bmp");
            //Load("skally", @"C:\Users\kim_k\source\repos\ConsoleAppPixelEngine\ConsoleAppPixelEngine\Properties\skally.png");
        }

        private void Load(string sName, string sFileName)
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
