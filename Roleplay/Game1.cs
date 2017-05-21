using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Xml.Linq;
using System.Collections.Generic;
using System;

namespace Roleplay
{
    enum GameMode { TilesetEditor, Game, Menus }
    enum DrawPhase {Trans, NonTrans }
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        DrawPhase dPhase;
        GameMode gm;
        //KEYS
        Keys k_SaveTileset;
        bool p_SaveTileset, pb_SaveTileset;

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

        //editor
        Tile currentTile;
        string[] tileNames;
        int tileIndex;

        //setup
        public Game1()
        {
            gm = GameMode.Menus;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

        }
        public void SetupKeys()
        {
            k_SaveTileset = Keys.S;
        }
        protected override void Initialize()
        {
            translationSpeed = 5f;
            virtDim = new Point(1920, 1080);
            winDim = new Point(960, 540);
            rt = new RenderTarget2D(GraphicsDevice, virtDim.X, virtDim.Y);
            nonTransRt = new RenderTarget2D(GraphicsDevice, virtDim.X, virtDim.Y);
            base.Initialize();

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
            tex = Content.Load<Texture2D>("tile");
            tileIndex = 0;
            tileNames = new string[] { "tile", "tile2" };
            currentTile = GetTile(tileNames[tileIndex]);
            ts = GetTileset();
        }
        //get
        public Tileset GetTileset()
        {
            XDocument doc = XDocument.Load("Content/Xml/TestTileset.xml");
            int tx = int.Parse(doc.Element("Tileset").Attribute("x").Value);
            int ty = int.Parse(doc.Element("Tileset").Attribute("y").Value);

            Tile[,] tiles2 = new Tile[tx, ty];
            foreach (XElement tile in doc.Element("Tileset").Elements("Tile"))
            {
                MagicTexture ttt = new MagicTexture(Content.Load<Texture2D>(tile.Value), new Rectangle(0, 0, 200, 100), Facing.N);
                int x = int.Parse(tile.Attribute("x").Value);
                int y = int.Parse(tile.Attribute("y").Value);
                tiles2[x,y] = new Tile(ttt, Vector2.Zero);
            }
            
            return new Tileset(tiles2, tx, ty, 200,100);
        }
        public Tile GetTile(string tilename_)
        {
             MagicTexture ttt = new MagicTexture(Content.Load<Texture2D>(tilename_), new Rectangle(0, 0, 200, 100), Facing.N);
             return new Tile(ttt, Vector2.Zero);        
        }
        public Vector2 getMousePos()
        {
            return mousePos + translation * -1;
        }
        //update
        public void ToggleSelectedTile()
        {
            tileIndex++;
            if(tileIndex >= tileNames.Length) { tileIndex = 0; }
            currentTile = GetTile(tileNames[tileIndex]);
        }
        public void UpdateKeys()
        {
            if (kbs.IsKeyUp(k_SaveTileset)) { pb_SaveTileset = false; }
            if (kbs.IsKeyDown(k_SaveTileset) && !pb_SaveTileset) { pb_SaveTileset = true; p_SaveTileset = true; }
            else if (kbs.IsKeyDown(k_SaveTileset)) { p_SaveTileset = false; }
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
        void UpdateEditor(GameTime gt_)
        {
            UpdateTranslation();
            if(p_SaveTileset)
            {
                //save
            }
            if (IsClicking())
            {
                bool switched = false;
                if (currentTile.getFrame().Contains(mousePos))
                {
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
                        ts.tiles[closest.X, closest.Y] = GetTile(tileNames[tileIndex]);
                        ts.PlaceTiles();
                    }
                }                
            }
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
        void DrawEditor()
        {
            if(dPhase == DrawPhase.Trans)
                ts.Draw(spriteBatch);
            else {
                currentTile.Draw(spriteBatch);
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
