using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Xml.Linq;
using System.Collections.Generic;
using System;

namespace Roleplay
{
    enum GameMode { TilesetEditor, Game, Menus }
    enum TSEMode { Edit, Select}
    enum DrawPhase {Trans, NonTrans }
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        DrawPhase dPhase;
        GameMode gm;

        Keys k_TSESelect;
        bool p_TSESelect, pb_TSESelect;
        Keys k_TSESave;
        bool p_TSESave, pb_TSESave;

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
        bool isReleased;
        float translationSpeed;
        Tileset ts;
        KeyboardState kbs;
        Button b;
        //data
        TileSheet sheet;

        //editor
        public Tile[] editorTiles;
        string[] tileNames;
        int tileIndex;
        TSEMode tseMode;

        //setup
        public Game1()
        {
            gm = GameMode.Menus;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

        }
        public void SetupKeys()
        {
            k_TSESave = Keys.S;
            k_TSESelect = Keys.E;
        }
        protected override void Initialize()
        {
            translationSpeed = 5f;
            virtDim = new Point(1920, 1080);
            winDim = new Point(960, 540);
            rt = new RenderTarget2D(GraphicsDevice, virtDim.X, virtDim.Y);
            nonTransRt = new RenderTarget2D(GraphicsDevice, virtDim.X, virtDim.Y);
            base.Initialize();
            sheet = new TileSheet("Content/Xml/Tiles.xml", Content);

            SetupKeys();
            ResizeWindow();
            CalcScale();
        }
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            cursor = new MagicTexture(Content.Load<Texture2D>("cursor"), new Rectangle(0, 0, 100, 100), Facing.L);
            tex = Content.Load<Texture2D>("grad");
            MagicTexture test2 = new MagicTexture(tex, new Rectangle(0, 0, tex.Width, tex.Height), Facing.N);
            b = new Button(test2, new Vector2(300, 100), "TilesetEditor");
        }
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            rt.Dispose();
            nonTransRt.Dispose();
        }
        void SetupTSE()
        {
            tseMode = TSEMode.Edit;
            tex = Content.Load<Texture2D>("tile");
            tileIndex = 0;
            List<Tile> editorTileList = new List<Tile>();
            for(int i = 0; i<sheet.tiles.Length; i++)
            {
                editorTileList.Add(GetTile(sheet.tiles[i]));
            }
            editorTiles = editorTileList.ToArray();
            SetQuickAccessTiles();
            ts = GetTileset();
        }
        //get
        public int getTileIndex(string tilename_)
        {
            int index = 0;
            for (int i = 0; i < sheet.tiles.Length; i++)
            {
                if (sheet.tiles[i] == tilename_)
                {
                    index = i;
                    break;
                }
            }
            return index;
        }
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
                tiles2[x, y] = GetTile(tile.Value);
            }
            
            return new Tileset(tiles2, tx, ty, 200,100);
        }
        public Tile GetTile(string tilename_)
        {
             int index = getTileIndex(tilename_);
             MagicTexture ttt = new MagicTexture(sheet.src, new Rectangle(sheet.tpos[index].X, sheet.tpos[index].Y, sheet.w, sheet.h) , Facing.N);
             return new Tile(ttt, Vector2.Zero, tilename_);        
        }
        public Vector2 getMousePos()
        {
            return mousePos + translation * -1;
        }
        public Tile nextTile()
        {
            if (tileIndex + 1 >= sheet.tiles.Length)
                return editorTiles[0];
            else
                return editorTiles[tileIndex + 1];
        }
        public Tile lastTile()
        {
            if (tileIndex - 1 < 0)
                return editorTiles[sheet.tiles.Length - 1];
            else
                return editorTiles[tileIndex - 1];
        }
        public Tile currentTile()
        {
            return editorTiles[tileIndex];
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

                    string name = ts.tiles[x, y].name;
                    tileEl.SetValue(name);

                    int index = getTileIndex(name);
                    tileEl.Add(new XAttribute("x", x));
                    tileEl.Add(new XAttribute("y", y));

                    tilesetEl.Add(tileEl);
                }
            }

            XDocument doc = XDocument.Load("Content/Xml/TestTileset.xml");
            doc.Element("Tileset").ReplaceWith(tilesetEl);
            doc.Save("Content/Xml/TestTileset.xml");
        }
        //update
        public void SetQuickAccessTiles()
        {           
            currentTile().pos = new Vector2(200, 50);
            lastTile().pos = new Vector2(100, 50);
            nextTile().pos = new Vector2(300, 50);
        }
        public void SetSelectTilePos()
        {
            for(int i = 0; i<editorTiles.Length; i++)
            {
                editorTiles[i].pos = new Vector2(i * 200, 50);
            }
        }
        public void ToggleSelectedTile()
        {
            if(tileIndex >= sheet.tiles.Length)
            { tileIndex = 0; }
            if (tileIndex < 0)
            { tileIndex = sheet.tiles.Length - 1; }
            SetQuickAccessTiles();

            SaveTileset();
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
        public void UpdateKeys()
        {
            //toggle TSESelect
            if (kbs.IsKeyUp(k_TSESelect)) { pb_TSESelect = false; }
            if (kbs.IsKeyDown(k_TSESelect) && !pb_TSESelect) { pb_TSESelect = true; p_TSESelect = true; }
            else if (kbs.IsKeyDown(k_TSESelect)) { p_TSESelect = false; }
            //save
            if (kbs.IsKeyUp(k_TSESave)) { pb_TSESave = false; }
            if (kbs.IsKeyDown(k_TSESave) && !pb_TSESave) { pb_TSESave = true; p_TSESave = true; }
            else if (kbs.IsKeyDown(k_TSESave)) { p_TSESave = false; }
        }
        public void UpdateMouse()
        {
            Vector2 originalPos = Mouse.GetState().Position.ToVector2();
            originalPos.X *= 1 / scale.X;
            originalPos.Y *= 1 / scale.Y;
            mousePos = originalPos;            
            if (Mouse.GetState().LeftButton == ButtonState.Released) { isReleased = true; }
        }
        bool IsClicking()
        {
            if (Mouse.GetState().LeftButton == ButtonState.Pressed && isReleased)
            {
                isReleased = false;
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
        public void UpdateTSESelect()
        {
            if (p_TSESelect)
            {
                ToggleEditorMode();
            }
            if (IsClicking())
            {
                for (int i = 0; i < editorTiles.Length; i++)
                {
                    if (editorTiles[i].getFrame().Contains(mousePos))
                    {
                        tileIndex = i;
                        ToggleEditorMode();
                        break;
                    }
                }
            }
        }
        public void UpdateTSE()
        {
            UpdateTranslation();
            if (IsClicking())
            {
                bool switched = false;
                if (lastTile().getFrame().Contains(mousePos))
                {
                    tileIndex--;
                    ToggleSelectedTile();
                    switched = true;
                }
                if (nextTile().getFrame().Contains(mousePos))
                {
                    tileIndex++;
                    ToggleSelectedTile();
                    switched = true;
                }
                if (!switched)
                {
                    Point closest = new Point(0, 0);
                    float closestdist = 1000;
                    for (int x = 0; x < ts.width; x++)
                    {
                        for (int y = 0; y < ts.height; y++)
                        {
                            float dist = Vector2.Distance(getMousePos(), ts.tiles[x, y].getMiddle());
                            if (dist < closestdist) { closestdist = dist; closest = new Point(x, y); }
                        }
                    }
                    if (ts.tiles[closest.X, closest.Y].getFrame().Contains(getMousePos()))
                    {
                        ts.tiles[closest.X, closest.Y] = GetTile(sheet.tiles[tileIndex]);
                        ts.PlaceTiles();
                    }
                }
            }
            if (p_TSESelect)
            {
                ToggleEditorMode();
            }

        }
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
            if (p_TSESave) { SaveTileset(); }
            
        }
        void UpdateMenus(GameTime gt_)
        {
            if (b.getFrame().Contains(getMousePos()) && IsClicking()) { gm = GameMode.TilesetEditor; SetupTSE(); }
        }
        void UpdateGame(GameTime gt_)
        {

        }
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

        //draw
        void DrawTSESelect()
        {
            if (dPhase == DrawPhase.Trans) { }              
            else {
                foreach (Tile t in editorTiles) { t.Draw(spriteBatch); }
            }
        }
        void DrawTSE()
        {
            if (dPhase == DrawPhase.Trans)
                ts.Draw(spriteBatch);
            else {
                lastTile().Draw(spriteBatch);
                nextTile().Draw(spriteBatch);
                currentTile().Draw(spriteBatch);
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
        void DrawGame() {
            if (dPhase == DrawPhase.Trans)
            {

            }
            else {

            }
        }
        void DrawMenus()
        {
            if (dPhase == DrawPhase.Trans) { }
                
            else {
                b.Draw(spriteBatch);
            }

        }
        protected override void Draw(GameTime gameTime)
        {
            dPhase = DrawPhase.Trans;
            //draw translated stuff on the target
            Matrix translator = Matrix.CreateTranslation(translation.X, translation.Y, 0);
            GraphicsDevice.SetRenderTarget(rt);
            spriteBatch.Begin(transformMatrix: translator);
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
            spriteBatch.Begin();
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
            cursor.Draw(spriteBatch, mousePos);
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
