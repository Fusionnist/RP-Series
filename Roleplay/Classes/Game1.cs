using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Xml.Linq;
using System.Collections.Generic;
using System;

namespace Roleplay
{
    enum GameInputMode { ActorMenu, SkillSelect, CastSkill, MoveActor }
    enum GameMode { TilesetEditor, Game, Menus }
    enum TSEMode { Edit, Select}
    enum DrawPhase {Trans, NonTrans }
    public enum ISODIR { UL,UR,DL,DR }

    public class Game1 : Game
    {
        DrawPhase dPhase;
        GameMode gm;

        //keys
        List<KeyLogger> keys;        

        //cleanup
        Creature guy, enemy;
        Vector2 translation;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D tex;
        RenderTarget2D rt, nonTransRt;
        MagicTexture cursor;
        Point virtDim;
        Point winDim;
        Vector2 scale;
        Vector2 rtPos, mousePos;
        bool isReleased, isRightReleased;
        float translationSpeed;
        Tileset ts;
        KeyboardState kbs;

        //temp values
        MagicTexture overlay;
        Point[] currentRange;

        //values
        public float zoom;
        //data
        SpriteSheet sheet;
        public int sheetIndex;

        //editor
        public Tile[] editorTiles;
        int tileIndex;
        TSEMode tseMode;

        //game
        List<Creature> actors;
        int actorKey;
        GameInputMode gim;
        int skillKey;
        //draw tools
        FontDrawer fDrawer;

        //general
        List<Button> buttons;
        
        //setup
        public Game1()
        {
            gm = GameMode.Menus;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            zoom = 0.5f;
        } 
        public void SetupKeys()
        {
            keys = new List<KeyLogger>();
            keys.Add(new KeyLogger(Keys.O, "unzoom"));
            keys.Add(new KeyLogger(Keys.S, "save"));
            keys.Add(new KeyLogger(Keys.P, "zoom"));
            keys.Add(new KeyLogger(Keys.E, "select"));
            keys.Add(new KeyLogger(Keys.M, "menu"));
            keys.Add(new KeyLogger(Keys.I, "toggleSheet"));
        }
        protected override void Initialize()
        {
            translationSpeed = 5f;
            virtDim = new Point(1920, 1080);
            winDim = new Point(960, 540);
            rt = new RenderTarget2D(GraphicsDevice, virtDim.X, virtDim.Y);
            nonTransRt = new RenderTarget2D(GraphicsDevice, virtDim.X, virtDim.Y);
            base.Initialize();

            sheet = new SpriteSheet(XDocument.Load("Content/Xml/Campaign.xml"), Content);

            SetupKeys();
            ResizeWindow();
            CalcScale();
        }
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            cursor = new MagicTexture(Content.Load<Texture2D>("cursor"), new Rectangle(0, 0, 100, 100), Facing.L, 0);
            tex = Content.Load<Texture2D>("grad");

            buttons = new List<Button>();
            
            string str = "abcdefghijklmnopqrstuvwxyz0123456789.!?,':;() ";
            fDrawer = new FontDrawer(Content.Load<Texture2D>("font"), 80, 100, str);

            overlay = new MagicTexture(Content.Load<Texture2D>("overlay"), new Rectangle(0, 0, 200, 100), Facing.N, 0);

            SetupMenu();
        }
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            rt.Dispose();
            nonTransRt.Dispose();
        }
        void SetupSheet()
        {
            tileIndex = 0;
            List<Tile> editorTileList = new List<Tile>();
            for (int i = 0; i < sheet.tileSheet.tileNames.Count; i++)
            {
                editorTileList.Add(sheet.getTile(sheet.tileSheet.tileIDs[i]));
            }
            editorTiles = editorTileList.ToArray();
            SetQuickAccessTiles();
        }
        void SetupTSE()
        {
            tseMode = TSEMode.Edit;
            SetupSheet();         
            ts = GetTileset();
            SetQuickAccessTiles();
            //buttons
            buttons.Clear();
        }
        void SetupMenu()
        {
            MagicTexture test2 = new MagicTexture(tex, new Rectangle(0, 0, tex.Width, tex.Height), Facing.N, 0);

            buttons.Clear();
            buttons.Add(new Button(test2, new Vector2(300, 100), "TilesetEditor"));
            buttons.Add(new Button(test2, new Vector2(300, 300), "GameStart"));
        }
        void SetupGame()
        {
            //load saved input mode ect ect ect ect
            gim = GameInputMode.ActorMenu;

            ts = GetTileset();

            guy = sheet.getCreature(0);
            guy.BecomePlayer();
            guy.LearnSkill(new Skill(SkillTrajectory.Linear, 4, 10, "skill1", 1));
            guy.LearnSkill(new Skill(SkillTrajectory.Linear, 5, 1, "skill2 lol", 1));
            guy.tsPos = new Point(2, 2);
            PositionToTile(guy);
            enemy = sheet.getCreature(1);
            enemy.LearnSkill(new Skill(SkillTrajectory.Linear, 5, 1, "skill2 lol", 1));

            actors = new List<Creature>();
            actors.Add(guy);
            actors.Add(enemy);

            //buttons
            buttons.Clear();
            CheckActorMenuButtons();
        }

        //get
        public bool IsMouseOnTile()
        {
            Point p = mouseTsPos();
            if(p.X >= 0 && p.X < ts.width)
            {
                if (p.Y >= 0 && p.Y < ts.height)
                {
                    return true;
                }
            }
            return false;
        }
        public bool GetPressed(string name_)
        {
            bool b = false;
            foreach (KeyLogger kl in keys) { if (kl.name == name_) { b = kl.isPressed(); } }
            return b;
        } //get news about the keys
        Creature GetActorAtPos(Point pos)
        {
            foreach (Creature c in actors)
            {
                if (c.tsPos == pos) { return c; }
            }
            return null;
        } //get actor at a specific location
        public Creature CurrentActor()
        {
            return (actors[actorKey]);
        } //get the current actor in the game
        public Tileset GetTileset()
        {
            XDocument doc = XDocument.Load("Content/Xml/TestTileset.xml");
            int tx = int.Parse(doc.Element("Tileset").Attribute("x").Value);
            int ty = int.Parse(doc.Element("Tileset").Attribute("y").Value);

            Tile[,] tiles2 = new Tile[tx, ty];
            foreach (XElement tile in doc.Element("Tileset").Elements("Tile"))
            {
                int x = int.Parse(tile.Attribute("x").Value);
                int y = int.Parse(tile.Attribute("y").Value);
                tiles2[x, y] = sheet.getTile(int.Parse(tile.Value));
            }
            
            return new Tileset(tiles2, tx, ty, 200,100);
        }
        public Tileset Expand(Tileset ts_, ISODIR dir_, int mod_)
        {
            if(!((ts_.tiles.GetLength(0) <= 1 || ts_.tiles.GetLength(1) <= 1) && mod_ < 0))
            {
                Tile[,] newTiles = null;
                Point modder = Point.Zero;
                int xDiff = 0; int yDiff = 0;
                switch (dir_)
                {
                    case (ISODIR.DL):
                        modder = new Point(0, 1 * mod_);
                        yDiff = 0;
                        break;
                    case (ISODIR.DR):
                        modder = new Point(1 * mod_, 0);
                        xDiff = 0;
                        break;
                    case (ISODIR.UR):
                        modder = new Point(0, 1 * mod_);
                        yDiff = 1 * mod_;
                        break;
                    case (ISODIR.UL):
                        modder = new Point(1 * mod_, 0);
                        xDiff = 1 * mod_;
                        break;
                }
                int oW, oH;
                oW = ts_.tiles.GetLength(0);
                oH = ts.tiles.GetLength(1);
                newTiles = new Tile[oW + modder.X, oH + modder.Y];
                for (int x = 0; x < oW; x++)
                {
                    for (int y = 0; y < oH; y++)
                    {
                        if (x + xDiff >= 0 && y + yDiff >= 0 && x + xDiff < oW + modder.X && y + yDiff < oH + modder.Y)
                        {
                            newTiles[x + xDiff, y + yDiff] = ts_.tiles[x, y];
                        }
                    }
                }
                for (int x = 0; x < oW + modder.X; x++)
                {
                    for (int y = 0; y < oH + modder.Y; y++)
                    {
                        if (newTiles[x, y] == null) { newTiles[x, y] = sheet.getTile(sheet.tileSheet.tileIDs[tileIndex]); }
                    }
                }
                return new Tileset(newTiles, oW + modder.X, oH + modder.Y, 200, 100);
            }
            return ts_;
        }
        public Vector2 getMousePos()
        {
            return 1/zoom * (mousePos + translation * -1);
        }
        public Tile nextTile()
        {
            if (tileIndex + 1 >= sheet.tileSheet.tileTIDs.Count)
                return editorTiles[0];
            else
                return editorTiles[tileIndex + 1];
        }
        public Tile lastTile()
        {
            if (tileIndex - 1 < 0)
                return editorTiles[sheet.tileSheet.tileTIDs.Count - 1];
            else
                return editorTiles[tileIndex - 1];
        }
        public Tile currentTile()
        {
            return editorTiles[tileIndex];
        }
        public Point[] pointsInRange(Point p_, int i)
        {
            List<Point> points = new List<Point>();
            int min = -i;
            int max = i;
            for(int x = min; x <= max; x++)
            {
                for (int y = min; y <= max; y++)
                {
                    if(Math.Abs(x)+ Math.Abs(y) <= i) { points.Add(new Point(x+p_.X, y+p_.Y)); }
                    if(x==-2 && y == -2)
                    {

                    }
                }
            }
            return points.ToArray();
        }
        public Creature[] inRangeActors(Point[] p_)
        {
            List<Creature> actorsInRange = new List<Creature>();
            foreach(Creature c in actors)
            {
                foreach(Point p in p_)
                {
                    if(p == c.tsPos) { actorsInRange.Add(c); }
                }
            }
            return actorsInRange.ToArray();
        }

        //save
        public void SaveTileset()
        {
            XElement tilesetEl = new XElement("Tileset");
            tilesetEl.Add(new XAttribute("x", ts.width));
            tilesetEl.Add(new XAttribute("y", ts.height));

            for (int x = 0; x< ts.width; x++)
            {
                for (int y = 0; y < ts.height; y++)
                {
                    XElement tileEl = new XElement("Tile");

                    int id = ts.tiles[x, y].ID;
                    tileEl.SetValue(id);

                    tileEl.Add(new XAttribute("x", x));
                    tileEl.Add(new XAttribute("y", y));

                    tilesetEl.Add(tileEl);
                }
            }

            XDocument doc = XDocument.Load("Content/Xml/TestTileset.xml");
            doc.Element("Tileset").ReplaceWith(tilesetEl);
            doc.Save("Content/Xml/TestTileset.xml");
        }

        //!!! UPDATE !!!
        protected override void Update(GameTime gameTime)
        {
            kbs = Keyboard.GetState();
            UpdateKeys();
            UpdateMouse();
            switch (gm)
            {
                case (GameMode.TilesetEditor):
                    UpdateEditor(gameTime);
                    break;
                case (GameMode.Game):
                    UpdateGame(gameTime);
                    break;
                case (GameMode.Menus):
                    UpdateMenus(gameTime);
                    break;
            }
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            base.Update(gameTime);
        }

        //game
        void UpdateGame(GameTime gt_)
        {
            UpdateTranslation();
            if (GetPressed("unzoom")) { zoom -= 0.1f; }
            if (GetPressed("zoom")) { zoom += 0.1f; }
            if (GetPressed("menu")) { gm = GameMode.Menus; SetupMenu(); }

            PositionToTile(guy);
            PositionToTile(enemy);

            foreach(Creature c in actors)
            {
                c.Update(gt_);
            }

            CheckButtons();

            if (CurrentActor().isPlayer)
            {
                switch (gim)
                {
                    case (GameInputMode.ActorMenu):
                        break;
                    case (GameInputMode.CastSkill):
                        UpdateSkillCasting();
                        break;
                    case (GameInputMode.MoveActor):
                        Move();
                        break;
                }
            }
            else { ApplyTurn(getBestOption()); }

            if(CurrentActor().maxAP == 0)
            {               
                ToggleNextActor();
            }
        }
        void PositionToTile(Entity ent)
        {
            Rectangle R = ts.tiles[ent.tsPos.X, ent.tsPos.Y].GetBaseRect();
            ent.pos.Y = R.Y + 100 - (ent.getFrame(true).Height * 200f / ent.getFrame(true).Width);
            ent.pos.X = R.X;

        }
        //actor turn stuff
        void Move()
        {
            if(GetActorAtPos(mouseTsPos()) == null)
            if (IsleftClicking())
            {
                if (IsMouseOnTile())
                {
                    Point p = mouseTsPos();
                    Point x = p - CurrentActor().tsPos;
                    int ap = Math.Abs(x.X) + Math.Abs(x.Y);
                    if(ap <= CurrentActor().AP)
                    {
                        CurrentActor().AP -= ap;
                        CurrentActor().tsPos = mouseTsPos();
                        ToggleToActorMenu();
                    }
                }
            }
        }
        void SelectSkill(int key)
        {
            skillKey = key;
            currentRange = pointsInRange(CurrentActor().tsPos, CurrentActor().skills[skillKey].range);
        }       
        void CheckActorMenuButtons()
        {
            //add or remove button
            for (int i = buttons.Count - 1; i >= 0; i--)
            {
                if (buttons[i].action == "EndTurn") { buttons.RemoveAt(i); if (buttons.Count <= 0) { break; } }

                if (buttons[i].action == "SelectSkill") { buttons.RemoveAt(i); if (buttons.Count <= 0) { break; } }

                if (buttons[i].action == "MoveActor") { buttons.RemoveAt(i); if (buttons.Count <= 0) { break; } }
            }
            if (actors[actorKey].isPlayer)
            {
                MagicTexture tex = new MagicTexture(Content.Load<Texture2D>("grad"), new Rectangle(0, 0, 1000, 100), Facing.N, 0);
                buttons.Add(new Button(tex, new Vector2(920, 980), "EndTurn"));
                tex = new MagicTexture(Content.Load<Texture2D>("grad"), new Rectangle(0, 0, 1000, 100), Facing.N, 0);
                buttons.Add(new Button(tex, new Vector2(920, 880), "SelectSkill"));
                tex = new MagicTexture(Content.Load<Texture2D>("grad"), new Rectangle(0, 0, 1000, 100), Facing.N, 0);
                buttons.Add(new Button(tex, new Vector2(920, 700), "MoveActor"));
            }
        } //adds or removes buttons based on the current actor
        void CastSkill(int key, Point location)
        {
            if(CurrentActor().AP >= CurrentActor().skills[key].AP)
            {
                CurrentActor().AP -= CurrentActor().skills[key].AP;

                GetActorAtPos(location).hp -= CurrentActor().skills[key].damage;
            }           
        }
        void UpdateActorMenu()
        {

        }
        void UpdateSkillCasting()
        {
            for (int x = 0; x < actors.Count; x++)
            {
                if (x == actorKey) { x++; if (x >= actors.Count) { break; } }
                if (isInRange(actors[x], pointsInRange(actors[actorKey].tsPos, actors[actorKey].skills[skillKey].range)))
                {
                    if (mouseTsPos() == actors[x].tsPos)
                    {
                        if (IsleftClicking())
                        {
                            CastSkill(skillKey, actors[x].tsPos);
                            ToggleToActorMenu();
                        }
                    }
                }
            }
        }
        //toggles
        void ToggleNextActor()
        {
            CurrentActor().ResetAP();
            actorKey++;
            if (actorKey >= actors.Count) { actorKey = 0; }
            while (actors[actorKey].isActive == false) { actorKey++; if (actorKey >= actors.Count) { actorKey = 0; } }
            ToggleToActorMenu();
        }
        void ToggleToActorMenu()
        {
            gim = GameInputMode.ActorMenu;
            //buttons
            buttons.Clear();
            CheckActorMenuButtons();
        }
        void ToggleMoveActor()
        {
            gim = GameInputMode.MoveActor;
            currentRange = pointsInRange(CurrentActor().tsPos, CurrentActor().AP);
        }
        void ToggleSkillSelectButtons()
        {
            if (gim == GameInputMode.SkillSelect)
            {
                for (int x = 0; x < actors[actorKey].skills.Count; x++)
                {
                    MagicTexture tex = new MagicTexture(Content.Load<Texture2D>("grad"), new Rectangle(0, 0, 1000, 100), Facing.N, 0);
                    buttons.Add(new Button(tex, new Vector2(920, 0 + (x * 100)), "SelectSkillKey", actors[actorKey].skills[x].name, x));
                }
            }
            else
            {
                for (int i = buttons.Count - 1; i >= 0; i--)
                {
                    if (buttons[i].action == "SelectSkillKey") { buttons.RemoveAt(i); }
                }
            }
        }
        //AI
        public Turn getBestOption()
        {
            Turn t = new Turn(new TurnOption[] { new TurnOption(OptionType.Move, 10, actors[actorKey].tsPos) });
            int bestFitness = 0;
            for (int k = 0; k < CurrentActor().skills.Count; k++)
            {
                foreach (Creature c in inRangeActors(pointsInRange(CurrentActor().tsPos, CurrentActor().skills[k].range)))
                {
                    int fitness = 0;
                    fitness = CurrentActor().skills[k].damage;

                    if (fitness > bestFitness)
                    {
                        bestFitness = fitness;

                        TurnOption option = new TurnOption(OptionType.Skill, k, c.tsPos);
                        t = new Turn(new TurnOption[] { option });
                    }
                }
            }
            return t;
        }
        void ApplyTurn(Turn turn_)
        {
            foreach(TurnOption opt in turn_.options)
            {
                switch (opt.type)
                {
                    case (OptionType.Skill):
                        CastSkill(opt.key, opt.location);
                        break;
                }
            }
            ToggleNextActor();
        }     

        //menus
        void UpdateMenus(GameTime gt_)
        {
            CheckButtons();
        }
        //editor
        void UpdateEditor(GameTime gt_)
        {
            switch (tseMode)
            {
                case (TSEMode.Edit):
                    UpdateTSE();
                    break;

                case (TSEMode.Select):
                    UpdateTSESelect();
                    break;
            }
            if (GetPressed("save")) { SaveTileset(); }
            if (GetPressed("menu")) { gm = GameMode.Menus; SetupMenu(); }
        }
        public void SetQuickAccessTiles()
        {
            currentTile().pos = new Vector2(200, 50);
            lastTile().pos = new Vector2(100, 50);
            nextTile().pos = new Vector2(300, 50);
        }
        public void SetSelectTilePos()
        {
            int count = 1920 / 200;
            int y = 0; int ii = 0;
            for (int i = 0; i < editorTiles.Length; i++)
            {
                editorTiles[i].pos = new Vector2(ii * 200, y * 100);
                ii++;
                if (ii > count)
                { y++; ii = 0; }
            }
        }
        public void ToggleSelectedTile()
        {
            if (tileIndex >= sheet.tileSheet.tileTIDs.Count)
            { tileIndex = 0; }
            if (tileIndex < 0)
            { tileIndex = sheet.tileSheet.tileTIDs.Count - 1; }
            SetQuickAccessTiles();
        }
        public void ToggleEditorMode()
        {
            switch (tseMode)
            {
                case (TSEMode.Select):
                    SetQuickAccessTiles();
                    tseMode = TSEMode.Edit;
                    break;

                case (TSEMode.Edit):
                    SetSelectTilePos();
                    tseMode = TSEMode.Select;
                    break;
            }

        }
        public void UpdateTSESelect()
        {
            if (GetPressed("select"))
            {
                ToggleEditorMode();
            }
            if (IsleftClicking())
            {
                for (int i = 0; i < editorTiles.Length; i++)
                {
                    if (editorTiles[i].getFrame(true).Contains(mousePos))
                    {
                        tileIndex = i;
                        ToggleEditorMode();
                        break;
                    }
                }
            }
        }
        void SelectTileByID(int ID_)
        {
            for(int x = 0; x < editorTiles.Length; x++)
            {
                if(editorTiles[x].ID == ID_)
                {
                    tileIndex = x;
                    SetQuickAccessTiles();
                }
            }
        }
        public void UpdateTSE()
        {
            ts.PlaceTiles();
            UpdateTranslation();
            bool switched = false;
            if (lastTile().getFrame(true).Contains(mousePos))
            {
                if (IsleftClicking())
                {
                    tileIndex--;
                    ToggleSelectedTile();
                    switched = true;
                }
            }
            if (nextTile().getFrame(true).Contains(mousePos))
            {
                if (IsleftClicking())
                {
                    tileIndex++;
                    ToggleSelectedTile();
                    switched = true;
                }
            }
            if (!switched)
            {
                Point closest = mouseTsPos();
                if (ts.tiles[closest.X, closest.Y].getFrame(true).Contains(getMousePos()))
                {
                    if (IsleftClicking())
                    {                      
                        ts.tiles[closest.X, closest.Y] = sheet.getTile(sheet.tileSheet.tileIDs[tileIndex]);
                        ts.PlaceTiles();
                    }
                    else if (isRightClicking())
                    {
                        SelectTileByID(ts.tiles[closest.X, closest.Y].ID);
                    }
                }
            }
            bool l = IsleftClicking();
            bool r = isRightClicking();
            if (l||r)
            {
                int m = 1;
                if (r) { m = -1; }
                Vector2 mp = getMousePos();

                Vector2 URcorner = new Vector2(100, ts.width * 50 + 50);
                Vector2 DLcorner = new Vector2(ts.height * -100 + ts.width * 100 + 100, ts.height * 50 + 50);
                Vector2 DRcorner = new Vector2(ts.height * -100 + ts.width * 100 + 100, ts.width * 50 + 50);
                Vector2 ULcorner = new Vector2(100, ts.height * 50 + 50);
                 
                if (mp.X > URcorner.X 
                    && mp.Y < URcorner.Y)
                {
                    float xl = DLcorner.X - URcorner.X;
                    float mx = mp.X - URcorner.X;
                    float yl = DLcorner.Y - URcorner.Y;
                    float my = mp.Y - URcorner.Y;

                    yl++;
                    if(mx > xl - (xl / -yl * -my) || xl == 0)
                    { ts = Expand(ts, ISODIR.UR, m); }
                }


                if (mp.X > DRcorner.X
                    && mp.Y > DRcorner.Y)
                {
                    float xl = ULcorner.X - DRcorner.X;
                    float mx = mp.X - DRcorner.X;
                    float yl = ULcorner.Y - DRcorner.Y;
                    float my = mp.Y - DRcorner.Y;

                    if (mx > xl - (xl / -yl * -my) || xl == 0)
                    { ts = Expand(ts, ISODIR.DR, m); }
                }

                if (mp.X < DLcorner.X
                    && mp.Y > DLcorner.Y)
                {
                    float xl = URcorner.X - DLcorner.X;
                    float mx = mp.X - DLcorner.X;
                    float yl = URcorner.Y - DLcorner.Y;
                    float my = mp.Y - DLcorner.Y;

                    if (mx < xl - (xl / -yl * -my) || xl == 0)
                    { ts = Expand(ts, ISODIR.DL, m); }
                }

                if (mp.X < ULcorner.X
                    && mp.Y < ULcorner.Y)
                {
                    float xl = DRcorner.X - ULcorner.X;
                    float mx = mp.X - ULcorner.X;
                    float yl = DRcorner.Y - ULcorner.Y;
                    float my = mp.Y - ULcorner.Y;

                    if (mx < xl - (xl / -yl * -my) || xl == 0)
                    { ts = Expand(ts, ISODIR.UL, m); }
                }
            }
            if (GetPressed("select"))
            {
                ToggleEditorMode();
            }
            if (GetPressed("unzoom")) { zoom -= 0.1f; }
            if (GetPressed("zoom")) { zoom += 0.1f; }
        }
        //general
        public Point mouseTsPos()
        {
            Point closest = new Point(0, 0);
            float closestdist = 1000;
            for (int x = 0; x < ts.width; x++)
            {
                for (int y = 0; y < ts.height; y++)
                {
                    float dist = Vector2.Distance(getMousePos(), ts.tiles[x, y].getMiddle(true));
                    if (dist < closestdist) { closestdist = dist; closest = new Point(x, y); }
                }
            }
            return closest;
        }
        public bool isInRange(Entity ent, Point[] points)
        {
            foreach(Point p in points) { if (p == ent.tsPos) { return true; } }
            return false;
        }
        void CheckButtons()
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                if(buttons[i].getFrame(false).Contains(mousePos) && IsleftClicking())
                {
                    switch (buttons[i].action)
                    {
                        case ("TilesetEditor"):                           
                            gm = GameMode.TilesetEditor; SetupTSE();
                            break;
                        case ("GameStart"):
                            gm = GameMode.Game; SetupGame();
                            break;
                        case ("EndTurn"):
                            ToggleNextActor();
                            ToggleToActorMenu();
                            break;
                        case ("SelectSkill"):
                            gim = GameInputMode.SkillSelect;
                            ToggleSkillSelectButtons();
                            break;
                        case ("MoveActor"):                           
                            ToggleMoveActor();
                            break;
                        case ("SelectSkillKey"):
                            gim = GameInputMode.CastSkill;
                            SelectSkill(buttons[i].key);
                            ToggleSkillSelectButtons();
                            break;
                    }
                }
            }
        }
        void UpdateKeys()
        {
            foreach(KeyLogger kl in keys) { kl.Update(kbs); }
        }
        public void UpdateMouse()
        {
            Vector2 originalPos = Mouse.GetState().Position.ToVector2();
            originalPos.X *= 1 / scale.X;
            originalPos.Y *= 1 / scale.Y;
            mousePos = originalPos;            
            if (Mouse.GetState().LeftButton == ButtonState.Released) { isReleased = true; }
            if (Mouse.GetState().RightButton == ButtonState.Released) { isRightReleased = true; }
        }
        bool IsleftClicking()
        {
            if (Mouse.GetState().LeftButton == ButtonState.Pressed && isReleased)
            {
                isReleased = false;
                return true;
            }
            return false;
        }
        bool isRightClicking()
        {
            if (Mouse.GetState().RightButton == ButtonState.Pressed && isRightReleased)
            {
                isRightReleased = false;
                return true;
            }
            return false;
        }
        public void CalcScale()
        {
            float scaleX = (float)winDim.X / virtDim.X;
            float scaleY = (float)winDim.Y / virtDim.Y;
            if (scaleX > scaleY)
            {
                scale = new Vector2(scaleY);
                rtPos = new Vector2((winDim.X - virtDim.X * scaleY) / 2, 0);
            }
            else {
                scale = new Vector2(scaleX);
                rtPos = new Vector2(0, (winDim.Y - virtDim.Y * scaleX) / 2);
            }
        }
        public void ResizeWindow()
        {
            graphics.PreferredBackBufferHeight = winDim.Y;
            graphics.PreferredBackBufferWidth = winDim.X;
            graphics.ApplyChanges();
        }
        public void UpdateTranslation()
        {
            if (kbs.IsKeyDown(Keys.Up)) { translation.Y += translationSpeed; }
            if (kbs.IsKeyDown(Keys.Down)) { translation.Y -= translationSpeed; }
            if (kbs.IsKeyDown(Keys.Left)) { translation.X += translationSpeed; }
            if (kbs.IsKeyDown(Keys.Right)) { translation.X -= translationSpeed; }
        }  

        //draw      
        void DrawOverlay()
        {
            foreach(Tile t in ts.tiles)
            {
                foreach (Point p in currentRange)
                {
                    if (t.tsPos == p)
                    {
                        overlay.Draw(spriteBatch, t.pos, zoom, true);
                    }
                }
            }
        } //temp?
        void DrawButtons()
        {
            foreach(Button b in buttons)
            {
                b.Draw(spriteBatch, 1f, fDrawer);
            }
        }
        void DrawTSESelect()
        {
            if (dPhase == DrawPhase.Trans) { }              
            else {
                foreach (Tile t in editorTiles) { t.Draw(spriteBatch,1f); }
            }
        }
        void DrawTSE()
        {
            if (dPhase == DrawPhase.Trans)
                ts.Draw(spriteBatch,zoom);
            else {
                lastTile().Draw(spriteBatch,1f);
                nextTile().Draw(spriteBatch,1f);
                currentTile().Draw(spriteBatch,1f);

                if (ts.tiles[mouseTsPos().X, mouseTsPos().Y] != null)
                    fDrawer.DrawText(Vector2.Zero, ""+mouseTsPos(), 1000, 1000, spriteBatch, 1f);
            }
        }
        void DrawEditor()
        {
            switch (tseMode)
            {
                case (TSEMode.Edit):
                    DrawTSE();
                    break;

                case (TSEMode.Select):
                    DrawTSESelect();
                    break;
            }           
        }
        void DrawActorStats()
        {
            fDrawer.DrawText(Vector2.Zero, "hp: " + actors[actorKey].hp, 1920, 1080, spriteBatch, 0.5f);
            fDrawer.DrawText(new Vector2(0, 100), "name: " + actors[actorKey].name, 1920, 1080, spriteBatch, 0.5f);
            fDrawer.DrawText(new Vector2(0, 200), "ap: " + actors[actorKey].AP, 1920, 1080, spriteBatch, 0.5f);
        }
        void DrawSkillSelect()
        {

        }
        void DrawGame() {
            if (dPhase == DrawPhase.Trans)
            {
                ts.Draw(spriteBatch, zoom);
                guy.Draw(spriteBatch, zoom);
                enemy.Draw(spriteBatch, zoom);

                //range overlay
                if (gim == GameInputMode.MoveActor || gim == GameInputMode.CastSkill)
                {
                    DrawOverlay();
                }
            }
            else {             
                DrawActorStats();
                DrawButtons();
                if (gim == GameInputMode.SkillSelect)
                {
                    DrawSkillSelect();
                }
                if (gim == GameInputMode.CastSkill)
                {
                    DrawSkillCasting();
                }
                
                DrawGameDebugInfo();
            }
        }
        void DrawGameDebugInfo()
        {
            if(gim == GameInputMode.ActorMenu)
            {
                fDrawer.DrawText(new Vector2(1400, 0), "actor menu", 1920, 1080, spriteBatch, 0.5f);
            }
            if (gim == GameInputMode.SkillSelect)
            {
                fDrawer.DrawText(new Vector2(1400, 0), "skill select", 1920, 1080, spriteBatch, 0.5f);
            }
            if (gim == GameInputMode.CastSkill)
            {
                fDrawer.DrawText(new Vector2(1400, 0), "casting time", 1920, 1080, spriteBatch, 0.5f);
            }
            fDrawer.DrawText(new Vector2(1400, 0), "" + getBestOption().options[0].key, 1920, 1080, spriteBatch, 0.5f);
        }
        void DrawMenus()
        {
            if (dPhase == DrawPhase.Trans) { }
                
            else {
                fDrawer.DrawText(Vector2.Zero, "test text!", 400, 400, spriteBatch, 0.5f);
                DrawButtons();
            }

        }
        void DrawSkillCasting()
        {
            fDrawer.DrawText(new Vector2(1400,0), actors[actorKey].skills[skillKey].name, 1080, 1920, spriteBatch, 0.5f);
        }
        protected override void Draw(GameTime gameTime)
        {
            dPhase = DrawPhase.Trans;
            //draw translated stuff on the target
            Matrix translator = Matrix.CreateTranslation(translation.X, translation.Y, 0);
            GraphicsDevice.SetRenderTarget(rt);
            spriteBatch.Begin(transformMatrix: translator, samplerState: SamplerState.PointWrap);
            switch (gm)
            {
                case (GameMode.TilesetEditor):
                    DrawEditor();
                    break;
                case (GameMode.Game):
                    DrawGame();
                    break;
                case (GameMode.Menus):
                    DrawMenus();
                    break;
            }           
            spriteBatch.End();
            dPhase = DrawPhase.NonTrans;
            GraphicsDevice.SetRenderTarget(nonTransRt);
            GraphicsDevice.Clear(Color.TransparentBlack);
            spriteBatch.Begin(samplerState: SamplerState.PointWrap);
            switch (gm)
            {
                case (GameMode.TilesetEditor):
                    DrawEditor();
                    break;
                case (GameMode.Game):
                    DrawGame();
                    break;
                case (GameMode.Menus):
                    DrawMenus();
                    break;
            }
            cursor.Draw(spriteBatch, mousePos,1f,false);
            spriteBatch.End();
            //draw overlay
            GraphicsDevice.SetRenderTarget(null);
            //draw the non translated target
            Matrix scaler = Matrix.CreateScale(scale.X, scale.Y, 0);
            spriteBatch.Begin(transformMatrix:scaler);
            spriteBatch.Draw(rt, rtPos, null);
            spriteBatch.Draw(nonTransRt, rtPos, null);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
