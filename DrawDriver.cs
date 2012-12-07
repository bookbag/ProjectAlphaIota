using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectAlphaIota
{
    class DrawDriver
    {
        Texture2D _blankTexture;
        Texture2D _checkerTileTexture;
        bool _contentLoaded;
        GraphicsDevice _device;
        private ContentManager _contentManager;

        SpriteFont _font;

        public DrawDriver()
        {

            _contentLoaded = false;
        }

        public void LoadContent(GraphicsDevice device, ContentManager contentManager, ParticleManager particleManager)
        {
            this._device = device;
            _contentManager = contentManager;
            _blankTexture = new Texture2D(device, 1, 1);
            _blankTexture.SetData<Color>(new Color[] { Color.White });
            _contentLoaded = true;
            _font = contentManager.Load<SpriteFont>("SpriteFont1");
            _checkerTileTexture = contentManager.Load<Texture2D>(@"Sprites/CheckerPiece");
        }
        public void DrawPossibleMoves(SpriteBatch spriteBatch, Camera cam, List<MoveSet> allTile, int TILE_SCALE, CheckerPiece SelectedPiece)
        {
            spriteBatch.Begin(SpriteSortMode.BackToFront,
                          BlendState.AlphaBlend,
                          null,
                          null,
                          null,
                          null,
                          cam.get_transformation(_device));
            for (int i = 0; i < allTile.Count(); i++)
            {
                for (int j = 0; j < allTile[i].MoveList.Count(); j++)
                {
                    Color color = (j == 0) ? new Color(255, 255, 255, 127) : new Color(255, 255, 255, 50);
                    spriteBatch.Draw(_checkerTileTexture,
                                     new Vector2((allTile[i].MoveList[j].Col * TILE_SCALE + TILE_SCALE * .5f),
                                                 allTile[i].MoveList[j].Row * TILE_SCALE + TILE_SCALE * .5f),
                                     new Rectangle(SelectedPiece.Color*TILE_SCALE, 0, TILE_SCALE, TILE_SCALE),
                                     color, 0, new Vector2(TILE_SCALE * .5f, TILE_SCALE * .5f), 1,
                                     SpriteEffects.None, 0);
                }
            }
            spriteBatch.End();
        }
        public void DrawPieces(SpriteBatch spriteBatch, Camera cam, List<CheckerPiece> allPieces, int tileScale)
        {
            spriteBatch.Begin(SpriteSortMode.BackToFront,
                          BlendState.AlphaBlend,
                          null,
                          null,
                          null,
                          null,
                          cam.get_transformation(_device));
            for (int i = 0; i < allPieces.Count(); i++)
            {
                CheckerPiece currentPiece = allPieces[i];
                if (currentPiece != null)
                {
                    spriteBatch.Draw(_checkerTileTexture, new Vector2((currentPiece.Col * tileScale + tileScale * .5f), currentPiece.Row * tileScale + tileScale * .5f), new Rectangle(currentPiece.Color * tileScale, (int)currentPiece.Status * tileScale, tileScale, tileScale), Color.White, 0, new Vector2(tileScale / 2.0f, tileScale / 2.0f), 1, SpriteEffects.None, 0);
                }
            }
            spriteBatch.End();
        }

       
        public void DrawCheckerBoard(CheckerTile[,] tileBoard, List<CheckerPiece> allPieces, CheckerPiece selectedPiece, List<CheckerPiece>[] movablePieces, Dictionary<CheckerPiece, List<MoveSet>> MoveDict, int TILE_SCALE, SpriteBatch spriteBatch, Camera cam)
        {
            if (!_contentLoaded)
                return;
            spriteBatch.Begin(SpriteSortMode.BackToFront,
                          BlendState.AlphaBlend,
                          null,
                          null,
                          null,
                          null,
                          cam.get_transformation(_device));
            //Checker Tiles           
            for (var row = 0; row < tileBoard.GetLength(0); row++)
            {
                for (int col = 0; col < tileBoard.GetLength(1); col++)
                {
                    spriteBatch.Draw(_blankTexture, new Rectangle(col * TILE_SCALE, row * TILE_SCALE, TILE_SCALE, TILE_SCALE), null, (tileBoard[row, col].Color == 1)?Color.Gray:Color.White, 0, Vector2.Zero, SpriteEffects.None, 1); 
                }
            }
            //Lines
            for (var row = 0; row <= tileBoard.GetLength(0); row++)
            {
                spriteBatch.Draw(_blankTexture, new Rectangle(0, row * TILE_SCALE, tileBoard.GetLength(1) * TILE_SCALE, 1), null, Color.Black, 0, Vector2.Zero, SpriteEffects.None, 0);
            }
            for (var col = 0; col <= tileBoard.GetLength(1); col++)
            {
                spriteBatch.Draw(_blankTexture, new Rectangle(col * TILE_SCALE, 0, 1, tileBoard.GetLength(0) * TILE_SCALE), null, Color.Black, 0, Vector2.Zero, SpriteEffects.None, 0);
            }
            spriteBatch.End();

            //Draw Pieces
            DrawPieces(spriteBatch, cam, allPieces, TILE_SCALE);

            //Draw the possible moves
            if (selectedPiece != null && movablePieces != null && movablePieces[selectedPiece.Color].Contains(selectedPiece) && MoveDict.ContainsKey(selectedPiece))
            {
                DrawPossibleMoves(spriteBatch, cam, MoveDict[selectedPiece], TILE_SCALE, selectedPiece);
            }
        }
    }
}
