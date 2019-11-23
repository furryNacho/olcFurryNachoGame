using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using ConsoleAppPixelEngine.Commands;
using ConsoleAppPixelEngine.Items;
using ConsoleAppPixelEngine.Objects;
using ConsoleAppPixelEngine.Quest;
using PixelEngine;
using Gamepad.Library;


namespace ConsoleAppPixelEngine
{
    enum Mode
    {
        MODE_TITLE,
        MODE_LOCAL_MAP,
        MODE_WORLD_MAP,
        MODE_INVENTORY,
        MODE_SHOP
    };
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


        //private DynamicObj m_pPlayer = null;
        private Creature m_pPlayer = null;


        private List<DynamicObj> listDynamics = new List<DynamicObj>(); // skapas av kartan (  m_pCurrentMap.PopulateDynamics(listDynamics);)




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

        private List<Quest.Quest> m_listQuests { get; set; } = new List<Quest.Quest>();

        private List<Item> m_listItems { get; set; } = new List<Item>();

       
       

        private List<DynamicObj> m_vecProjectiles { get; set; }=new List<DynamicObj>(); // Transient

        int m_nGameMode = (int)Mode.MODE_LOCAL_MAP;
        int m_nInvSelectX = 0;
        int m_nInvSelectY = 0;


        /************
         * OnCreate *
         ************/
        public override void OnCreate()
        {

            Command.g_engine = this; // hyffsat med spagetti. För att command ska kunna prata med main här och skriva dialog. 

            DynamicObj.g_engine = this;

            Item.g_engine = this;  // lite knull för item behöver engine för projektiler

            Assets.Instance.LoadSprites();
            Assets.Instance.LoadMaps();
            Assets.Instance.LoadItems();

            m_listItems.Add(Assets.Instance.GetItem("sword"));// inte helt nöjd med denna projektiler och vapen

            //m_pCurrentMap = new cMap_Village1();
            //   m_pCurrentMap = Assets.Instance.GetMap("lvl1");

            m_spriteFont = Assets.Instance.GetSprite("font");

            m_script = new ScriptProcessor();

            cMap.g_script = m_script;
            Quest.Quest.g_script = m_script;

            m_listQuests.Add(new cQuest_MainQuest(this));
            //m_listQuests.Add(new cQuest_MainQuest());

            // Skapa spelarens dynamiska objekt
            //m_pPlayer = new Creature("player", Assets.Instance.GetSprite("player"));
             m_pPlayer = new DynamicCreatureHero();

            // m_pPlayer.px = 5.0f; // vat player ska ritas ut första gången.
            // m_pPlayer.py = 5.0f;


            //DynamicObj cDynamic1 = new Creature("skelly1", Assets.Instance.GetSprite("skally"));
            //cDynamic1.px = 15.0f;
            //cDynamic1.py = 5.0f;

            //DynamicObj cDynamic2 = new Creature("skelly2", Assets.Instance.GetSprite("skally"));
            //cDynamic2.px = 8.0f;
            //cDynamic2.py = 5.0f;


            //      listDynamics.Add(m_pPlayer);
            //listDynamics.Add(cDynamic1);
            //listDynamics.Add(cDynamic2);
            ChangeMap("lvl1", 5, 5);



            if (SlimDx == null)
            {
                SetUpStuff();
                SlimDx.timer_Tick();
            }
            else
            {

            }
        }


        private void UpdateLocalMap(float fElapsedTime)
        {


            m_script.ProcessCommands(fElapsedTime); // måste ha högst prioritet

            // proj knull
            // Erase and delete redundant projectiles	
            // m_vecProjectiles.erase(remove_if(m_vecProjectiles.begin(), m_vecProjectiles.end(), [](const cDynamic* d) { return ((cDynamic_Projectile*)d)->bRedundant; }), m_vecProjectiles.end());
            if (m_vecProjectiles != null && m_vecProjectiles.Any())
                if (m_vecProjectiles.Exists(x => x.Redundant == true))
                {
                    var tempList = m_vecProjectiles.Where(x => x.Redundant == false).ToList();
                    if (tempList.Any())
                    {
                        m_vecProjectiles = tempList;
                    }
                    else
                    {
                        m_vecProjectiles = new List<DynamicObj>();
                    }
                }
            // end proj knull

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


            // Krabb för projektil
            bool firstInList = true;
            bool bWorkingWithProjectiles = false;
            List<List<DynamicObj>> krabbMedProjektiler = new List<List<DynamicObj>>();
            krabbMedProjektiler.Add(listDynamics);
            if(m_vecProjectiles != null)
              krabbMedProjektiler.Add(m_vecProjectiles);
            foreach (var myObjectList in krabbMedProjektiler) // galet jäkla knulleri
            {

                foreach (var myObject in myObjectList)
                //foreach (var myObject in listDynamics) // TODO: känns som det blir betdligt bättre respnse om man flyttar lopen så inte allt körs i denna, men då tycks andra objekt förlora förmågan till gravitation och dylikt, kolla på det.
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
                            m_pPlayer.vy = -6.0f;

                        }

                        //if (GetKey(VK_DOWN).bHeld)
                        if (GetKey(Key.Down).Down)
                        {
                            //fPlayerVelY = 6.0f;
                            m_pPlayer.vy = 6.0f;
                        }

                        //if (GetKey(VK_RIGHT).bHeld)
                        if (GetKey(Key.Right).Down || IIP.right)
                        {
                            PlayerOrientation = PlayerOrientation.Right;
                            // fPlayerVelX += (bPlayerOnGround ? 25.0f : 15.0f) * fElapsedTime;
                            m_pPlayer.vx += (bPlayerOnGround ? 25.0f : 15.0f) * fElapsedTime; // Okså hastigheten

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
                            m_pPlayer.vx += (bPlayerOnGround ? -25.0f : -15.0f) * fElapsedTime; // Okså hastigheten

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
                            if (myObject.vy == 0)
                            {
                                myObject.vy = -12.0f;
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

                        // Testa interact
                        // // Interaction requested
                        if (GetKey(Key.I).Released)
                        {
                            // Grab a point from the direction the player is facing and check for interactions										
                            float fTestX = 0; float fTestY = 0;

                            if (m_pPlayer.GetFacingDirection() == 0) // South
                            {
                                fTestX = m_pPlayer.px + 0.5f;
                                fTestY = m_pPlayer.py + 1.5f;
                            }

                            if (m_pPlayer.GetFacingDirection() == 1) // West
                            {
                                fTestX = m_pPlayer.px - 0.5f;
                                fTestY = m_pPlayer.py + 0.5f;
                            }

                            if (m_pPlayer.GetFacingDirection() == 2) // North
                            {
                                fTestX = m_pPlayer.px + 0.5f;
                                fTestY = m_pPlayer.py - 0.5f;
                            }

                            if (m_pPlayer.GetFacingDirection() == 3) // East
                            {
                                fTestX = m_pPlayer.px + 1.5f;
                                fTestY = m_pPlayer.py + 0.5f;
                            }

                            // Check if test point has hit a dynamic object
                            bool bHitSomething = false;
                            foreach (var dyns in listDynamics)
                            {

                                // Här vill jag nog har nåt snarlikt för att hoppa på fiende
                                if (fTestX > dyns.px && fTestX < (dyns.px + 1.0f) && fTestY > dyns.py && fTestY < (dyns.py + 1.0f))
                                {
                                    if (dyns.Friendly)
                                    {

                                        bHitSomething = true;

                                        // Iterate through quest stack until something responds, the base quests should capture
                                        // interactions that are not specfied in other quests
                                        foreach (var quest in m_listQuests)
                                            if (quest.OnInteraction(listDynamics, dyns, NATURE.TALK))
                                            {
                                                bHitSomething = true;
                                                break;
                                            }

                                        // Some objects just do stuff when you interact with them
                                        //dyns.OnInteract(m_pPlayer);

                                        // Then check if it is map related
                                        m_pCurrentMap.OnInteraction(listDynamics, dyns, NATURE.TALK);
                                    }
                                    else
                                    {
                                        // Interaction was with something not friendly - only enemies
                                        // are not friendly, so perfrom attack
                                        m_pPlayer.PerformAttack();
                                    }
                                }
                            }

                            if (!bHitSomething) // Default action is attack
                            {
                                m_pPlayer.PerformAttack();
                            }
                        }


                        if (GetKey(Key.M).Released)
                            m_nGameMode = (int)Mode.MODE_INVENTORY;


                        firstInList = false;

                    }



                    // Gravity
                    myObject.vy += 20.0f * fElapsedTime;




                    // Drag
                    if (bPlayerOnGround)
                    {
                        myObject.vx += -3.0f * myObject.vx * fElapsedTime;
                        if (Math.Abs(myObject.vx) < 0.01f)
                            myObject.vx = 0.0f;
                    }

                    // Clamp velocities
                    if (myObject.vx > 10.0f)
                        myObject.vx = 10.0f;

                    if (myObject.vx < -10.0f)
                        myObject.vx = -10.0f;

                    if (myObject.vy > 100.0f)
                        myObject.vy = 100.0f;

                    if (myObject.vy < -100.0f)
                        myObject.vy = -100.0f;



                    // hade väl räckt med att köra loopen från här, kanske?
                    // DynamicObj myObject = m_pPlayer;

                    float fNewObjectPosX = myObject.px + myObject.vx * fElapsedTime;
                    float fNewObjectPosY = myObject.py + myObject.vy * fElapsedTime;

                    // Collision
                    float fBorder = 0.1f;// Hårdkoda hitbox (bevara för rpg!!)
                    //projektil  bool bCollisionWithMap = false;
                    if (myObject.vx <= 0) // Moving Left
                    {
                        //(bevara för rpg!!)
                        if (m_pCurrentMap.GetSolid((int)(fNewObjectPosX + 0.0f), (int)(myObject.py + 0.0f)) || m_pCurrentMap.GetSolid((int)(fNewObjectPosX + 0.0f), (int)(myObject.py + 0.9f)))
                        //if (m_pCurrentMap.GetSolid((int)(fNewObjectPosX + fBorder), (int)(myObject.py + fBorder + 0.0f)) || m_pCurrentMap.GetSolid((int)(fNewObjectPosX + fBorder), (int)(myObject.py + (1.0f - fBorder))))
                        {
                            fNewObjectPosX = (int)fNewObjectPosX + 1;
                            //fPlayerVelX = 0;
                            myObject.vx = 0;
                            //projektil   bCollisionWithMap = true;
                        }

                    }
                    else // Moving Right
                    {

                        //(bevara för rpg!!)
                        if (m_pCurrentMap.GetSolid((int)(fNewObjectPosX + (1.0f - fBorder)), (int)(myObject.py + fBorder + 0.0f)) || m_pCurrentMap.GetSolid((int)(fNewObjectPosX + (1.0f - fBorder)), (int)(myObject.py + (1.0f - fBorder))))
                        //if (m_pCurrentMap.GetSolid((int)(fNewObjectPosX + fBorder + 0.0f), (int)(fNewObjectPosY + fBorder)) || m_pCurrentMap.GetSolid((int)(fNewObjectPosX + (1.0f - fBorder)), (int)(fNewObjectPosY + fBorder)))
                        {
                            fNewObjectPosX = (int)fNewObjectPosX;
                            //fPlayerVelX = 0;
                            myObject.vx = 0;
                            //projektil    bCollisionWithMap = true;
                        }

                    }

                    bPlayerOnGround = false;

                    //if (fPlayerVelY <= 0) // Moving Up
                    if (myObject.vy <= 0)
                    {
                        //(bevara för rpg!!)
                        if (m_pCurrentMap.GetSolid((int)(fNewObjectPosX + 0.0f), (int)fNewObjectPosY) || m_pCurrentMap.GetSolid((int)(fNewObjectPosX + 0.9f), (int)fNewObjectPosY))
                        //if (m_pCurrentMap.GetSolid((int)(fNewObjectPosX + fBorder + 0.0f), (int)(fNewObjectPosY + fBorder)) || m_pCurrentMap.GetSolid((int)(fNewObjectPosX + (1.0f - fBorder)), (int)(fNewObjectPosY + fBorder)))
                        {
                            fNewObjectPosY = (int)fNewObjectPosY + 1;
                            //fPlayerVelY = 0;
                            myObject.vy = 0;
                            //projektil    bCollisionWithMap = true;
                        }


                        HasMovedUp = true;
                    }
                    else // Moving Down
                    {
                        //(bevara för rpg!!)
                        if (m_pCurrentMap.GetSolid((int)(fNewObjectPosX + 0.0f), (int)(fNewObjectPosY + 1.0f)) || m_pCurrentMap.GetSolid((int)(fNewObjectPosX + 0.9f), (int)(fNewObjectPosY + 1.0f)))
                        // if (m_pCurrentMap.GetSolid((int)(fNewObjectPosX + fBorder + 0.0f), (int)(fNewObjectPosY + (1.0f - fBorder))) || m_pCurrentMap.GetSolid((int)(fNewObjectPosX + (1.0f - fBorder)), (int)(fNewObjectPosY + (1.0f - fBorder))))
                        {
                            fNewObjectPosY = (int)fNewObjectPosY;
                            //fPlayerVelY = 0;
                            myObject.vy = 0;
                            bPlayerOnGround = true;
                           //projektil bCollisionWithMap = true;
                        }


                    }

                    // gjorde ta bort projektil väldigt snabb.. tror det har med min loop att göra
                    //if (myObject.IsProjectile && bCollisionWithMap)
                    //{
                    //    myObject.Redundant = true;
                    //}

                    var didJustHitGround = HasMovedUp && bPlayerOnGround;
                    if (didJustHitGround)
                    {
                        HasMovedUp = false;
                        //nDirModX = 0;
                        //nDirModY = PlayerOrientation == PlayerOrientation.Right ? 0 : 1;
                    }


                    //Collision dynamiska objekt
                    float fDynamicObjectPosX = fNewObjectPosX;
                    float fDynamicObjectPosY = fNewObjectPosY;
                    foreach (var dyn in listDynamics)
                    {
                        if (dyn != myObject)
                        {
                            // if the object is solid then the player must not overlapp
                            if (dyn.SolidVsDynamic && myObject.SolidVsDynamic)
                            {
                                // Check if bounding rectangles overlap
                                if (fDynamicObjectPosX < (dyn.px + 1.0f) && (fDynamicObjectPosX + 1.0f) > dyn.px &&
                                    myObject.py < (dyn.py + 1.0f) && (myObject.py + 1.0f) > dyn.py)
                                {
                                    // First Check Horizontally - Check Left
                                    if (myObject.vx <= 0)
                                    {

                                        fDynamicObjectPosX = dyn.px + 1.0f;
                                       
                                    }
                                    else
                                    {

                                        fDynamicObjectPosX = dyn.px - 1.0f;
                                    }
                                }

                                if (fDynamicObjectPosX < (dyn.px + 1.0f) && (fDynamicObjectPosX + 1.0f) > dyn.px &&
                                    fDynamicObjectPosY < (dyn.py + 1.0f) && (fDynamicObjectPosY + 1.0f) > dyn.py)
                                {

                                    // First Check Vertically - Check Left
                                    if (myObject.vy <= 0)
                                        fDynamicObjectPosY = dyn.py + 1.0f;
                                    else
                                        fDynamicObjectPosY = dyn.py - 1.0f;
                                }
                            }
                            else
                            {
                                if (myObject == listDynamics[0])
                                {
                                    // Object is player and can interact with things
                                    if (fDynamicObjectPosX < (dyn.px + 1.0f) && (fDynamicObjectPosX + 1.0f) > dyn.px &&
                                        myObject.py < (dyn.py + 1.0f) && (myObject.py + 1.0f) > dyn.py)
                                    {
                                        // First check if object is part of a quest
                                        foreach (var quest in m_listQuests)
                                        {
                                            if (quest.OnInteraction(listDynamics, dyn, NATURE.WALK))
                                                break;
                                        }

                                        // Then check if it is map related
                                        m_pCurrentMap.OnInteraction(listDynamics, dyn, NATURE.WALK);

                                        // Finally just check the object
                                        dyn.OnInteract(myObject);
                                    }
                                }
                                else
                                {
                                    if (bWorkingWithProjectiles)
                                    {
                                        if (fDynamicObjectPosX < (dyn.px + 1.0f) && (fDynamicObjectPosX + 1.0f) > dyn.px &&
                                            fDynamicObjectPosY < (dyn.py + 1.0f) && (fDynamicObjectPosY + 1.0f) > dyn.py)
                                        {
                                            if (dyn.Friendly != myObject.Friendly)
                                            {
                                                // We know object is a projectile, so dyn is something
                                                // opposite that it has overlapped with											
                                                if (dyn.IsAttackable)
                                                {
                                                    // Dynamic object is a creature
                                                    Damage((cDynamic_Projectile)myObject, (Creature)dyn);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }



                    // Apply new position
                    //fPlayerPosX = fNewObjectPosX;
                    //myObject.px = fNewObjectPosX;
                    //fPlayerPosY = fNewObjectrPosY;
                    //myObject.py = fNewObjectPosY;

                    myObject.px = fDynamicObjectPosX;
                    myObject.py = fDynamicObjectPosY;





                    //Uppdatera Objektet!
                    myObject.Update(fElapsedTime, m_pPlayer);


                    // PART 3 time: 47:21 han har lagt loopen annorlunda här. KAnske kan vara bra för nåt, men jag fortsätter så här just nu 

                }

                bWorkingWithProjectiles = true;
            }

            // Jag gissar lite här på vad det är han försöker åstadkomma
            var switchList = new List<Quest.Quest>();
            foreach (var q in m_listQuests)
            {
                if (!q.bCompleted)
                {
                    switchList.Add(q);
                }
            }
            m_listQuests = switchList;


            //// Link camera to player position
            fCameraPosX = m_pPlayer.px; // Ganska bra om det finns direkt tillgång till spelar obj, även om det kommer finnas massa olika obj, för kameran vill alltid följa spelaren.
            fCameraPosY = m_pPlayer.py;

            // Draw Levels
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
            foreach (var VARIABLE in krabbMedProjektiler)// Knulleriet fortsätter
                foreach (var myObject in VARIABLE)
                //foreach (var myObject in listDynamics)
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


            // Draw health to screen
            string sHealth = "HP: " + m_pPlayer.Health.ToString() + "/" + m_pPlayer.MaxHealth.ToString();
            DisplayDialog(new List<string>() { sHealth }, 160, 10);



            if (m_bShowDialog)
            {
                DisplayDialog(m_vecDialogToShow, 20, 20);
            }


            //DrawBigText("Hello everybody "+idxBig,30,30 );
            //idxBig++;
        }

        private void UpdateInventory(float fElapsedTime)
        {
            //Fill(0, 0, ScreenWidth(), ScreenHeight(), L' ');
            this.Clear((Pixel)Pixel.Presets.Black);
            DrawBigText("INVENTORY", 4, 4);


            int i = 0;
            Item highlighted = null;

            // Draw Consumables
            foreach (var item in m_listItems)
            {
                int x = i % 4;
                int y = i / 4;
                i++;

                var fPoint = new Point(8 + x * 20, 20 + y * 20);
                var sPoint = new Point(0, 0);
                DrawPartialSprite(fPoint, item.pSprite, sPoint, 16, 16);

                if (m_nInvSelectX == x && m_nInvSelectY == y)
                    highlighted = item;
            }

            // Draw selection reticule
            var pointParamOne = new Point(6 + (m_nInvSelectX) * 20, 18 + (m_nInvSelectY) * 20);
            var pointParamTwo = new Point(6 + (m_nInvSelectX + 1) * 20, 18 + (m_nInvSelectY) * 20);
            var color = Pixel.FromRgb((uint)Pixel.Presets.White);
            DrawLine(pointParamOne, pointParamTwo, color);
            pointParamOne = new Point(6 + (m_nInvSelectX) * 20, 18 + (m_nInvSelectY + 1) * 20);
            pointParamTwo = new Point(6 + (m_nInvSelectX + 1) * 20, 18 + (m_nInvSelectY + 1) * 20);
            DrawLine(pointParamOne, pointParamTwo, color);
            pointParamOne = new Point(6 + (m_nInvSelectX) * 20, 18 + (m_nInvSelectY) * 20);
            pointParamTwo = new Point(6 + (m_nInvSelectX) * 20, 18 + (m_nInvSelectY + 1) * 20);
            DrawLine(pointParamOne, pointParamTwo, color);
            pointParamOne = new Point(6 + (m_nInvSelectX + 1) * 20, 18 + (m_nInvSelectY) * 20);
            pointParamTwo = new Point(6 + (m_nInvSelectX + 1) * 20, 18 + (m_nInvSelectY + 1) * 20);
            DrawLine(pointParamOne, pointParamTwo, color);

            if (GetKey(Key.Left).Released) m_nInvSelectX--;
            if (GetKey(Key.Right).Released) m_nInvSelectX++;
            if (GetKey(Key.Up).Released) m_nInvSelectY--;
            if (GetKey(Key.Down).Released) m_nInvSelectY++;
            if (m_nInvSelectX < 0) m_nInvSelectX = 3;
            if (m_nInvSelectX >= 4) m_nInvSelectX = 0;
            if (m_nInvSelectY < 0) m_nInvSelectY = 3;
            if (m_nInvSelectY >= 4) m_nInvSelectY = 0;

            if (GetKey(Key.M).Released)
                m_nGameMode = (int)Mode.MODE_LOCAL_MAP;

            DrawBigText("SELECTED:", 8, 160);

            if (highlighted != null)
            {
                DrawBigText("SELECTED:", 8, 160);
                DrawBigText(highlighted.sName, 8, 170);

                DrawBigText("DESCRIPTION:", 8, 190);
                DrawBigText(highlighted.sDescription, 8, 200);

                if (!highlighted.bKeyItem)
                {
                    DrawBigText("(Press SPACE to use)", 80, 160);
                }

                if (GetKey(Key.Space).Released)
                {
                    // Use selected item 
                    if (!highlighted.bKeyItem)
                    {
                        if (highlighted.OnUse(m_pPlayer))
                        {
                            // Item has signalled it must be consumed, so remove it
                            TakeItem(highlighted);
                        }
                    }
                    else
                    {

                    }
                }
            }

            DrawBigText("LOCATION:", 128, 8);
            DrawBigText(m_pCurrentMap.sName, 128, 16);

            DrawBigText("HEALTH: " + m_pPlayer.Health.ToString(), 128, 32);
            DrawBigText("MAX HEALTH: " + m_pPlayer.MaxHealth.ToString(), 128, 40);

        }

        /***********
        * OnUpdate *
        ************/
        public override void OnUpdate(float fElapsedTime)
        {

            switch (m_nGameMode)
            {
                //case MODE_TITLE:
                //return UpdateTitleScreen(fElapsedTime);
                case (int)Mode.MODE_LOCAL_MAP:
                    UpdateLocalMap(fElapsedTime);
                    return;
                //case MODE_WORLD_MAP:
                //	return UpdateWorldMap(fElapsedTime);
                case (int)Mode.MODE_INVENTORY:
                    UpdateInventory(fElapsedTime);
                    return;
                    //case MODE_SHOP:
                    //return UpdateShop(fElapsedTime);
            }



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


        public void ChangeMap(string sMapName, float x, float y)
        {
            listDynamics.Clear();
            listDynamics.Add(m_pPlayer);
            m_pCurrentMap = Assets.Instance.GetMap(sMapName);

            m_pPlayer.px = x;
            m_pPlayer.py = y;

            m_pCurrentMap.PopulateDynamics(listDynamics);


            foreach (var q in m_listQuests)
            {
                q.PopulateDynamics(listDynamics, m_pCurrentMap.sName);
            }
        }


        public void AddQuest(Quest.Quest quest)
        {
            //m_listQuests.Add(quest);
            // egentligen add to front
            var oldSwitchArou = new List<Quest.Quest>();
            oldSwitchArou.Add(quest);
            foreach (var q in m_listQuests)
            {
                oldSwitchArou.Add(q);
            }

            m_listQuests = oldSwitchArou;
        }

        public bool GiveItem(Item item)
        {
            m_listItems.Add(item);
            return true;
        }
        public bool TakeItem(Item item)
        {
            if (item != null && m_listItems.Contains(item))
            {
                m_listItems.Remove(item);
                return true;
            }
            else
                return false;
        }
        public bool HasItem(Item item)
        {
            return m_listItems.Contains(item);
        }

        public void Attack(DynamicObj aggressor, cWeapon weapon) // TODO: fick denna ur koden men 
        {
            weapon.OnUse(aggressor);
        }


        public void AddProjectile(cDynamic_Projectile proj)
        {
            m_vecProjectiles.Add(proj);
        }

        public void Damage(cDynamic_Projectile projectile, Creature victim)
        {
            if (victim != null)
            {
                // Attack victim with damage
                victim.Health -= projectile.nDamage;

                // Knock victim back
                float tx = victim.px - projectile.px;
                float ty = victim.py - projectile.py;
                float d = (float)Math.Sqrt(tx * tx + ty * ty);
                if (d < 1) d = 1.0f;

                // After a hit, they object experiences knock back, where it is temporarily
                // under system control. This delivers two functions, the first being
                // a visual indicator to the player that something has happened, and the second
                // it stops the ability to spam attacks on a single creature
                                victim.KnockBack(tx / d, ty / d, 0.2f);

                if (victim != m_pPlayer)
                {
                    victim.OnInteract(m_pPlayer);
                }
                else
                {
                    // We must ensure the player is never pushed out of bounds by the physics engine. This
                    // is a bit of a hack, but it allows knockbacks to occur providing there is an exit
                    // point for the player to be knocked back into. If the player is "mobbed" then they
                    // become trapped, and must fight their way out
                    victim.SolidVsDynamic = true;
                }


                if (projectile.bOneHit)
                    projectile.Redundant = true;
            }
        }

    };



    enum PlayerOrientation
    {
        Right = 0,
        Left = 1
    }


}