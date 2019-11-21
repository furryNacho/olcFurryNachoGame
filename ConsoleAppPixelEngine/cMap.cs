using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PixelEngine;
using System.IO;
using Newtonsoft;
using Newtonsoft.Json;


namespace ConsoleAppPixelEngine
{
    public class cMap
    {
        public int nWidth;
        public int nHeight;
        public string sName;
        //olcSprite* pSprite;
        public Sprite pSprite;

        private int[] m_indices = null; // plural för index 
        private bool[] m_solids = null;

        // public static cScriptProcessor[] g_script;

        public cMap()
        {
            nWidth = 0;
            nHeight = 0;



            sName = "";
            pSprite = null;
        }

        public int GetIndex(int x, int y) // typ som gettile. Hämta rätt sprite?
        {
            if (x >= 0 && x < nWidth && y >= 0 && y < nHeight)
                return m_indices[y * nWidth + x];
            else
                return 0;
        }


        public bool GetSolid(int x, int y) //  samma som get findex fast för en annan array
        {
            if (x >= 0 && x < nWidth && y >= 0 && y < nHeight)
            {

                var solidvärde = m_solids[y * nWidth + x];

                // Har en liten plan på att kunna sätta fler värden än bara solide eller inte. Typ solid om över men inte under.
                // skulle kunna göra om m_solids till att förvara nummer eller dylikt sen kolla på värdet hur tilen ska bete sig. 
                // Lagom standsatt tills jag får tag på en map editor

                return solidvärde;
            }
            else
                return true;
        }


        //public abstract bool Create(string fileData, olcSprite* sprite, string name);
        public bool Create(string fileData, Sprite sprite, string name)
        {

            sName = name; // namnet sparat lokalt
            pSprite = sprite; // pekaren till spriten

            // Ladda filen som innehåller lvl data:
            bool experimentWithJson = true;
            if (experimentWithJson)
            {

                string json = File.ReadAllText(fileData);
                LevelJsonObj account = JsonConvert.DeserializeObject<LevelJsonObj>(json); ;
                nHeight = account.Height;
                nWidth = account.Width;

                m_indices = account.TileIndex;
             
                //m_solids = account.AttributeIndex;

                // Only use solid not solid for now.
                m_solids = new bool[nWidth * nHeight];
                var length = account.AttributeIndex.Length;
                for (int i = 0; i < length; i++)
                {
                    if (account.AttributeIndex[i].Equals(0))
                    {
                        m_solids[i] = false;
                    }
                    else
                    {
                        m_solids[i] = true;
                    }
                        

                }

            }
            else
            {


                //File.ReadAllText("Resources\\Settings.txt")
                var fileDataString = File.ReadAllText(fileData);
                if (!string.IsNullOrEmpty(fileDataString))
                {
                    // Nollindexerat Första är höjden andra är bredden
                    // sen är det par. Först vilken tile att använda, sen om den är solid eller inte. 
                    //t ex: 64 32 9 0 9 0


                    // crossX har gjort en sprite viewer. //TODO hitta den
                    // ittey ? har gjort en mapp editor // TODO: hitta den






                    //endast de två första värdena 
                    var firstTwo = String.Join(" ", fileDataString.Split(' ').Take(2));

                    string[] firstTwoArray = firstTwo.Split(' ');
                    nHeight = Int32.Parse(firstTwoArray[1]); // testar byta plats på dessa och det händer grejer..
                    nWidth = Int32.Parse(firstTwoArray[0]); // testar byta plats på dessa och det händer grejer.. // Kan också vara kartan (village1.lvl.txt) inte säker på om den är som den ska liksom

                    // Skapa våra två arrayer, solid och indexar. 
                    m_solids = new bool[nWidth * nHeight];
                    m_indices = new int[nWidth * nHeight];



                    //utan de två första i strängen
                    var resultNoWH = String.Join(" ", fileDataString.Split(' ').Skip(2));
                    string[] resultNoWHArray = resultNoWH.Split(' ');










                    var röv1 = new List<string>();
                    var röv2 = new List<string>();



                    for (int i = 0; i < resultNoWHArray.Length; i++)
                    {
                        var framröv = resultNoWHArray[i];

                        if (i % 2 == 0)
                        {
                            röv1.Add(framröv);

                        }
                        else
                        {
                            röv2.Add(framröv);

                        }
                    }


                    for (int i = 0; i < röv1.Count; i++)
                    {
                        m_indices[i] = int.Parse(röv1[i]);
                    }

                    for (int i = 0; i < röv2.Count; i++)
                    {
                        m_solids[i] = röv2[i].Equals("1");

                    }


                    return true;

                }

            }

            return false;
        }



        //public virtual bool PopulateDynamics(vector<cDynamic*> &vecDyns)
        //{
        //    return false;
        //}

        //public virtual bool OnInteraction(vector<cDynamic*> &vecDynobs, cDynamic* target, NATURE nature)
        //{
        //    return false;
        //}


        public class LevelJsonObj
        {
            public int[] TileIndex { get; set; }
            public int[] AttributeIndex { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
        }

    };



    class cMap_Village1 : cMap
    {

        public CreateObj CreateObj { get; set; }

        public cMap_Village1()
        {





            this.CreateObj = new CreateObj()
            {
                // TODO: path
                //fileData = @"C:\Users\kim_k\source\repos\ConsoleAppPixelEngine\ConsoleAppPixelEngine\Properties\village1.lvl.txt",
                //fileData = @"C:\Users\kim_k\source\repos\ConsoleAppPixelEngine\ConsoleAppPixelEngine\Properties\mapoutput.txt",
                fileData = Assets.Instance.RootPath(@"Properties\mapoutput.txt"),


               // sprite = Assets.Instance.GetSprite("village"),
                sprite = Assets.Instance.GetSprite("forest"),
                name = "coder town why not indeed",
            };

            CrateFromChild();
        }

        public bool CrateFromChild()
        {
            return this.Create(CreateObj.fileData, CreateObj.sprite, CreateObj.name);
        }



        //bool PopulateDynamics(vector<cDynamic*> &vecDyns) override;
        //bool OnInteraction(vector<cDynamic*> &vecDynobs, cDynamic* target, NATURE nature) override;


    }

    class cMap_Home1 : cMap
    {

        public CreateObj CreateObj { get; set; }

        public cMap_Home1()
        {
            this.CreateObj = new CreateObj()
            {
                // TODO: path
                fileData = @"C:\Users\kim_k\source\repos\ConsoleAppPixelEngine\ConsoleAppPixelEngine\Properties\village1.lvl.txt",
                sprite = Sprite.Load(@"C:\Users\kim_k\source\repos\ConsoleAppPixelEngine\ConsoleAppPixelEngine\Properties\toml_spritesheetdark.spr"),
                name = "coder town why not indeed",
            };

            CrateFromChild();
        }

        public bool CrateFromChild()
        {
            return this.Create(CreateObj.fileData, CreateObj.sprite, CreateObj.name);
        }

        //bool PopulateDynamics(vector<cDynamic*> &vecDyns) override;
        //bool OnInteraction(vector<cDynamic*> &vecDynobs, cDynamic* target, NATURE nature) override;
    }


    class CreateObj
    {
        public string fileData { get; set; }
        public Sprite sprite { get; set; }
        public string name { get; set; }
    }

}
