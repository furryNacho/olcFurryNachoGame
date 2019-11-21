using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using ConsoleAppPixelEngine.Commands;
using ConsoleAppPixelEngine.Objects;
using PixelEngine;
using Gamepad.Library;


namespace ConsoleAppPixelEngine
{
    public class OneLoneCoder_NachoGame : Game
    {

        public OneLoneCoder_NachoGame()
        {
            this.AppName = "olcNacho";
        }

        static void Main(string[] args)
        {
            OneLoneCoder_NachoGame spr = new OneLoneCoder_NachoGame();
            spr.Construct(256, 240, 4, 4);
            spr.Start();
        }

        private void SetUpStuff()
        {
            SlimDx = new SlimDXGamepad();

            SlimDx.SetUp();

            IIP = SlimDx.IIP;
        }
        public SlimDXGamepad SlimDx { get; set; }
        public IsItPressed IIP { get; set; }




        private cMap m_pCurrentMap = null;


        private DynamicObj m_pPlayer = null;


        private List<DynamicObj> listDynamics = new List<DynamicObj>();




        bool bPlayerOnGround = false;
        private float playerTimer = 0.0f;
        private int GraphicsCounter = 0;

        private PlayerOrientation PlayerOrientation = PlayerOrientation.Right;
        bool HasMovedUp = false;

        // Camera properties
        float fCameraPosX = 0.0f;
        float fCameraPosY = 0.0f;

        // Sprite Resources

        private Sprite m_spriteFont;


        private ScriptProcessor m_script;




        /************
         * OnCreate *
         ************/
        public override void OnCreate()
        {
            Command.g_engine = this; // hyffsat med spagetti. För att command ska kunna prata med main här och skriva dialog. 

            Assets.Instance.LoadSprites();

            m_pCurrentMap = new cMap_Village1();

            m_spriteFont = Assets.Instance.GetSprite("font");

            m_script = new ScriptProcessor();


            // Skapa spelarens dynamiska objekt
            m_pPlayer = new Creature("player", Assets.Instance.GetSprite("player"));
            m_pPlayer.pX = 5.0f; // vat player ska ritas ut första gången.
            m_pPlayer.pY = 5.0f;


            DynamicObj cDynamic1 = new Creature("skelly1", Assets.Instance.GetSprite("skally"));
            cDynamic1.pX = 15.0f;
            cDynamic1.pY = 5.0f;

            DynamicObj cDynamic2 = new Creature("skelly2", Assets.Instance.GetSprite("skally"));
            cDynamic2.pX = 8.0f;
            cDynamic2.pY = 5.0f;


            listDynamics.Add(m_pPlayer);
            listDynamics.Add(cDynamic1);
            listDynamics.Add(cDynamic2);




            if (SlimDx == null)
            {
                SetUpStuff();
                SlimDx.timer_Tick();
            }
            else
            {

            }
        }

        /***********
        * OnUpdate *
        ************/
        public override void OnUpdate(float fElapsedTime)
        {
            m_script.ProcessCommands(fElapsedTime); // måste ha högst prioritet
            if (m_script.UserControlEnabled)
            {
                // tillåt check för input, egentligen // borde innefatta  "// Handle Input" fast det går ju sönder då, vet i fasiken hur han gjorde det.
            }
            else
            {
                if (m_bShowDialog)
                {
                    if (GetKey(Key.Space).Released || GetKey(Key.T).Released)
                    {
                        m_bShowDialog = false;
                        m_script.CompletedCommand();
                    }
                }
            }

            SlimDx.timer_Tick();
            IIP = SlimDx.IIP;


            //  DynamicObj myObject = m_pPlayer;

            bool firstInList = true;
            foreach (var myObject in listDynamics)
            {


                // Handle Input
                if (Focus && firstInList && m_script.UserControlEnabled)
                {
                    playerTimer += fElapsedTime;
                    if (playerTimer <= 0.1f)
                    {
                        GraphicsCounter = 0;

                    }
                    else if (playerTimer <= 0.2f)
                    {
                        GraphicsCounter = 1;

                    }
                    else if (playerTimer <= 0.3f)
                    {
                        GraphicsCounter = 2;

                    }
                    else if (playerTimer <= 0.4f)
                    {
                        GraphicsCounter = 3;

                    }
                    else if (playerTimer <= 0.5f)
                    {
                        GraphicsCounter = 4;
                        //playerTimer -= 1.0f;
                        playerTimer = 0.0f;

                    }
                    else
                    {
                        GraphicsCounter = 0;
                        playerTimer = 0.0f;

                    }






                    //Test Gamepad
                    if (!IIP.idle)
                    {

                        if (IIP.right)
                        {

                        }
                        if (IIP.left)
                        {

                        }
                        if (IIP.jump)
                        {

                        }
                        if (IIP.otherButton)
                        {

                        }
                        if (IIP.start)
                        {

                        }
                        if (IIP.select)
                        {

                        }
                    }
                    //End test Gamepad


                    //if (GetKey(VK_UP).bHeld)
                    if (GetKey(Key.Up).Down)
                    {
                        //fPlayerVelY = -6.0f;
                        m_pPlayer.vY = -6.0f;

                    }

                    //if (GetKey(VK_DOWN).bHeld)
                    if (GetKey(Key.Down).Down)
                    {
                        //fPlayerVelY = 6.0f;
                        m_pPlayer.vY = 6.0f;
                    }

                    //if (GetKey(VK_RIGHT).bHeld)
                    if (GetKey(Key.Right).Down || IIP.right)
                    {
                        PlayerOrientation = PlayerOrientation.Right;
                        // fPlayerVelX += (bPlayerOnGround ? 25.0f : 15.0f) * fElapsedTime;
                        m_pPlayer.vX += (bPlayerOnGround ? 25.0f : 15.0f) * fElapsedTime; // Okså hastigheten

                        //if (bPlayerOnGround)
                        //{
                        //    nDirModY = 0;
                        //    nDirModX = GraphicsCounter;
                        //}
                        //else
                        //{
                        //    nDirModY = PlayerOrientation == PlayerOrientation.Right ? 2 : 3;
                        //    nDirModX = 0;
                        //}

                    }

                    //if (GetKey(VK_LEFT).bHeld)
                    if (GetKey(Key.Left).Down || IIP.left)
                    {
                        PlayerOrientation = PlayerOrientation.Left;
                        //fPlayerVelX += (bPlayerOnGround ? -25.0f : -15.0f) * fElapsedTime;
                        m_pPlayer.vX += (bPlayerOnGround ? -25.0f : -15.0f) * fElapsedTime; // Okså hastigheten

                        //if (bPlayerOnGround)
                        //{
                        //    nDirModY = 1;
                        //    nDirModX = GraphicsCounter;
                        //}
                        //else
                        //{
                        //    nDirModY = PlayerOrientation == PlayerOrientation.Right ? 2 : 3;
                        //    nDirModX = 0;
                        //}

                    }



                    //if (GetKey(VK_SPACE).bPressed) // Har ingen hoppa i rpg 
                    if (GetKey(Key.Space).Pressed || IIP.jump)
                    {
                        if (myObject.vY == 0)
                        {
                            myObject.vY = -12.0f;
                            //// nDirModX = 1;
                            //nDirModX = 0;
                            //// nDirModY = 2; // 2 Eller 3 beroended på vilket håll gubbe vänd mot
                            //nDirModY = PlayerOrientation == PlayerOrientation.Right ? 2 : 3;
                        }
                    }



                    // Testa script : scriptProcessor.
                    if (GetKey(Key.Z).Released)
                    {
                        // spelaren går i en tiangel
                        //m_script.AddCommand(new CommandMoveTo(m_pPlayer, 10, 10, 3.0f));
                        //m_script.AddCommand(new CommandMoveTo(m_pPlayer, 15, 10, 3.0f));
                        //m_script.AddCommand(new CommandMoveTo(m_pPlayer, 15, 15, 3.0f));
                        //m_script.AddCommand(new CommandMoveTo(m_pPlayer, 10, 10, 3.0f));

                        //m_script.AddCommand(new CommandMoveTo(listDynamics[1], 15, 5, 2.0f));
                        //m_script.AddCommand(new CommandMoveTo(listDynamics[1], 8, 5, 2.0f));

                        m_script.AddCommand(new CommandMoveTo(listDynamics[2], 8, 5, 2.0f));
                        m_script.AddCommand(new CommandMoveTo(listDynamics[2], 15, 5, 2.0f));

                        m_script.AddCommand(new CommandShowDialog(new List<string>()
                        {
                            "Grr"
                        }));
                        m_script.AddCommand(new CommandShowDialog(new List<string>()
                            {
                            "Jag ar ett monster"
                            }));

                    }


                    firstInList = false;

                }



                // Gravity
                myObject.vY += 20.0f * fElapsedTime;




                // Drag
                if (bPlayerOnGround)
                {
                    myObject.vX += -3.0f * myObject.vX * fElapsedTime;
                    if (Math.Abs(myObject.vX) < 0.01f)
                        myObject.vX = 0.0f;
                }

                // Clamp velocities
                if (myObject.vX > 10.0f)
                    myObject.vX = 10.0f;

                if (myObject.vX < -10.0f)
                    myObject.vX = -10.0f;

                if (myObject.vY > 100.0f)
                    myObject.vY = 100.0f;

                if (myObject.vY < -100.0f)
                    myObject.vY = -100.0f;




                // DynamicObj myObject = m_pPlayer;

                float fNewObjectPosX = myObject.pX + myObject.vX * fElapsedTime;
                float fNewObjectrPosY = myObject.pY + myObject.vY * fElapsedTime;



                // Check for Collision
                //if (fPlayerVelX <= 0) // Moving Left
                if (myObject.vX <= 0) // Moving Left
                {

                    if (m_pCurrentMap.GetSolid((int)(fNewObjectPosX + 0.0f), (int)(myObject.pY + 0.0f)) || m_pCurrentMap.GetSolid((int)(fNewObjectPosX + 0.0f), (int)(myObject.pY + 0.9f)))
                    {
                        fNewObjectPosX = (int)fNewObjectPosX + 1;
                        //fPlayerVelX = 0;
                        myObject.vX = 0;
                    }

                }
                else // Moving Right
                {

                    if (m_pCurrentMap.GetSolid((int)(fNewObjectPosX + 1.0f), (int)(myObject.pY + 0.0f)) || m_pCurrentMap.GetSolid((int)(fNewObjectPosX + 1.0f), (int)(myObject.pY + 0.9f)))
                    {
                        fNewObjectPosX = (int)fNewObjectPosX;
                        //fPlayerVelX = 0;
                        myObject.vX = 0;
                    }

                }

                bPlayerOnGround = false;

                //if (fPlayerVelY <= 0) // Moving Up
                if (myObject.vY <= 0)
                {

                    if (m_pCurrentMap.GetSolid((int)(fNewObjectPosX + 0.0f), (int)fNewObjectrPosY) || m_pCurrentMap.GetSolid((int)(fNewObjectPosX + 0.9f), (int)fNewObjectrPosY))
                    {
                        fNewObjectrPosY = (int)fNewObjectrPosY + 1;
                        //fPlayerVelY = 0;
                        myObject.vY = 0;
                    }


                    HasMovedUp = true;
                }
                else // Moving Down
                {

                    if (m_pCurrentMap.GetSolid((int)(fNewObjectPosX + 0.0f), (int)(fNewObjectrPosY + 1.0f)) || m_pCurrentMap.GetSolid((int)(fNewObjectPosX + 0.9f), (int)(fNewObjectrPosY + 1.0f)))
                    {
                        fNewObjectrPosY = (int)fNewObjectrPosY;
                        //fPlayerVelY = 0;
                        myObject.vY = 0;
                        bPlayerOnGround = true;
                    }


                }

                var didJustHitGround = HasMovedUp && bPlayerOnGround;
                if (didJustHitGround)
                {
                    HasMovedUp = false;
                    //nDirModX = 0;
                    //nDirModY = PlayerOrientation == PlayerOrientation.Right ? 0 : 1;
                }

                // Apply new position
                //fPlayerPosX = fNewObjectPosX;
                myObject.pX = fNewObjectPosX;
                //fPlayerPosY = fNewObjectrPosY;
                myObject.pY = fNewObjectrPosY;



                //Uppdatera Objektet!
                myObject.Update(fElapsedTime);




            }

            //// Link camera to player position
            fCameraPosX = m_pPlayer.pX; // Ganska bra om det finns direkt tillgång till spelar obj, även om det kommer finnas massa olika obj, för kameran vill alltid följa spelaren.
            fCameraPosY = m_pPlayer.pY;

            // Draw Level
            int nTileWidth = 16;
            int nTileHeight = 16;
            //int nVisibleTilesX = ScreenWidth() / nTileWidth;
            int nVisibleTilesX = ScreenWidth / nTileWidth;
            //int nVisibleTilesY = ScreenHeight() / nTileHeight;
            int nVisibleTilesY = ScreenHeight / nTileHeight;

            // Calculate Top-Leftmost visible tile
            float fOffsetX = fCameraPosX - (float)nVisibleTilesX / 2.0f;
            float fOffsetY = fCameraPosY - (float)nVisibleTilesY / 2.0f;

            // Clamp camera to game boundaries
            if (fOffsetX < 0) fOffsetX = 0;
            if (fOffsetY < 0) fOffsetY = 0;
            // v1
            //if (fOffsetX > nLevelWidth - nVisibleTilesX) fOffsetX = nLevelWidth - nVisibleTilesX;
            //if (fOffsetY > nLevelHeight - nVisibleTilesY) fOffsetY = nLevelHeight - nVisibleTilesY;

            if (fOffsetX > m_pCurrentMap.nWidth - nVisibleTilesX) fOffsetX = m_pCurrentMap.nWidth - nVisibleTilesX;
            if (fOffsetY > m_pCurrentMap.nHeight - nVisibleTilesY) fOffsetY = m_pCurrentMap.nHeight - nVisibleTilesY;



            // Get offsets for smooth movement
            float fTileOffsetX = (fOffsetX - (int)fOffsetX) * nTileWidth;
            float fTileOffsetY = (fOffsetY - (int)fOffsetY) * nTileHeight;

            // Draw visible tile map
            for (int x = -1; x < nVisibleTilesX + 1; x++)
            {
                for (int y = -1; y < nVisibleTilesY + 1; y++)
                {

                    PixelEngine.Point firstMagicalParam = new Point();
                    PixelEngine.Point secondMagicalParam = new Point();

                    // m_pCurrentMap[0]
                    int idx = m_pCurrentMap.GetIndex((int)(x + fOffsetX), (int)(y + fOffsetY));

                    //int sx = idx % 10; //column that the sprite is on  // TODO: hårt nummer && tror det 'r antal tiles som finns i spriten
                    //int sy = idx / 10; // row that the tile is on  // TODO: hårt nummer 

                    int sx = idx % 5; //column that the sprite is on  // TODO: hårt nummer && tror det 'r antal tiles som finns i spriten
                    int sy = idx / 5; // row that the tile is on  // TODO: hårt nummer 


                    var firstMagicalPlayerParamNew = new Point((int)(x * nTileWidth - fTileOffsetX), (int)(y * nTileHeight - fTileOffsetY));
                    var secondMagicalPlayerParamNew = new Point((int)(sx * nTileWidth), (int)(sy * nTileWidth));
                    DrawPartialSprite(firstMagicalPlayerParamNew, m_pCurrentMap.pSprite, secondMagicalPlayerParamNew, nTileWidth, nTileHeight);





                }
            }


            // Draw all objekts
            bool firstInDraw = true;
            foreach (var myObject in listDynamics)
            {
                if (firstInDraw)
                {
                    firstInDraw = false;
                }
                else
                {
                    myObject.DrawSelf(this, fOffsetX, fOffsetY);
                }
            }


            // hac tills vidare för att rita spelare längst fram. det är en lista och det sista som ritas är det som syns högst upp
            m_pPlayer.DrawSelf(this, fOffsetX, fOffsetY); // hyffsat obra, fixa loopen ist.



            if (m_bShowDialog)
            {
                DisplayDialog(m_vecDialogToShow, 20, 20);
            }


            //DrawBigText("Hello everybody "+idxBig,30,30 );
            //idxBig++;

        }

        private int idxBig = 0;

        void DrawBigText(string sText, int x, int y)
        {
            int i = 0;
            foreach (var c in sText)
            {
                int sx = ((c - 32) % 16) * 8;
                int sy = ((c - 32) / 16) * 8;

                var firstMagicalPlayerParam = new Point((x + i * 8), y);
                var secondMagicalPlayerParam = new Point(sx, sy);

                DrawPartialSprite(firstMagicalPlayerParam, m_spriteFont, secondMagicalPlayerParam, 8, 8);

                i++;
            }
        }


        private List<string> m_vecDialogToShow;
        private bool m_bShowDialog = false;
        private float m_fDialogX = 0.0f;
        private float m_fDialogy = 0.0f;
        public void ShowDialog(List<string> listLines)
        {
            m_vecDialogToShow = listLines;
            m_bShowDialog = true;
        }

        public void DisplayDialog(List<string> listText, int x, int y)
        {
            int nMaxLineLength = 0;
            int nLines = listText.Count;

            foreach (var l in listText)
            {
                if (l.Length > nMaxLineLength)
                {
                    nMaxLineLength = l.Length; // ta ut den längsta raden för att sätta BREDD PÅ RUTAN
                }
            }

            var point = new Point((x - 1), (y - 1));

            FillRect(point, (nMaxLineLength * 8 + 1), (y + nLines * 8 + 1), Pixel.Random());

            var point1 = new Point((x - 2), (y - 2));
            var point2 = new Point((x - 2), (y + nLines * 8 + 1));
            DrawLine(point1, point2, Pixel.Random());

            var point3 = new Point((x + nMaxLineLength * 8 + 1), (y - 2));
            var point4 = new Point((x + nMaxLineLength * 8 + 1), (y + nLines * 8 + 1));
            DrawLine(point3, point4, Pixel.Random());

            var point5 = new Point((x - 2), (y - 2));
            var point6 = new Point((x + nMaxLineLength * 8 + 1), (y - 2));
            DrawLine(point5, point6, Pixel.Random());

            var point7 = new Point((x - 2), (y + nLines * 8 + 1));
            var point8 = new Point((x + nMaxLineLength * 8 + 1), (y + nLines * 8 + 1));
            DrawLine(point7, point8, Pixel.Random());

            for (int i = 0; i < listText.Count; i++)
            {
                DrawBigText(listText[i], x, y + 1 * 8);
            }

        }

    };



    enum PlayerOrientation
    {
        Right = 0,
        Left = 1
    }


}