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
        Texture2D blankTexture;
        Texture2D checkerTileTexture;
        bool contentLoaded;
        GraphicsDevice device;

        SpriteFont font;

        public DrawDriver()
        {

            contentLoaded = false;
        }

        public void LoadContent(GraphicsDevice device, ContentManager contentManager, ParticleManager particleManager)
        {
            this.device = device;
            blankTexture = new Texture2D(device, 1, 1);
            blankTexture.SetData<Color>(new Color[] { Color.White });
            contentLoaded = true;
            font = contentManager.Load<SpriteFont>("SpriteFont1");
            checkerTileTexture = contentManager.Load<Texture2D>(@"Sprites/CheckerPiece");
        }
        public void DrawPossibleMoves(SpriteBatch spriteBatch, Camera cam, List<CheckerTile> allTile, int TILE_SCALE, CheckerPiece SelectedPiece)
        {
            spriteBatch.Begin(SpriteSortMode.BackToFront,
                          BlendState.AlphaBlend,
                          null,
                          null,
                          null,
                          null,
                          cam.get_transformation(device));
            for (int i = 0; i < allTile.Count(); i++)
            {
                spriteBatch.Draw(checkerTileTexture, new Vector2((allTile[i].Col * TILE_SCALE + TILE_SCALE * .5f), allTile[i].Row * TILE_SCALE + TILE_SCALE * .5f), new Rectangle(SelectedPiece.Color * TILE_SCALE, 0, TILE_SCALE, TILE_SCALE), new Color(255, 255, 255, 127), 0, new Vector2(TILE_SCALE / 2, TILE_SCALE / 2), 1, SpriteEffects.None, 0);
            }
            spriteBatch.End();
        }
        public void DrawPieces(SpriteBatch spriteBatch, Camera cam, List<CheckerPiece> AllPieces, int TILE_SCALE)
        {
            spriteBatch.Begin(SpriteSortMode.BackToFront,
                          BlendState.AlphaBlend,
                          null,
                          null,
                          null,
                          null,
                          cam.get_transformation(device));
            for (int i = 0; i < AllPieces.Count(); i++)
            {
                CheckerPiece currentPiece = AllPieces[i];
                if (currentPiece != null)
                {
                    spriteBatch.Draw(checkerTileTexture, new Vector2((currentPiece.Col * TILE_SCALE + TILE_SCALE * .5f), currentPiece.Row * TILE_SCALE + TILE_SCALE * .5f), new Rectangle(currentPiece.Color * TILE_SCALE, 0, TILE_SCALE, TILE_SCALE), Color.White, 0, new Vector2(TILE_SCALE / 2.0f, TILE_SCALE / 2.0f), 1, SpriteEffects.None, 0);
                    /*
                    if (currentPiece.Color == 0)
                        spriteBatch.Draw(blankTexture, new Rectangle((int)(-halfWidth + currentPiece.Col * TILE_SCALE) + 10, (int)(-halfHeight) + currentPiece.Row * TILE_SCALE + 10, TILE_SCALE - 20, TILE_SCALE - 20), null, Color.Red, 0, Vector2.Zero, SpriteEffects.None, 0);
                    else
                        spriteBatch.Draw(blankTexture, new Rectangle((int)(-halfWidth + currentPiece.Col * TILE_SCALE) + 10, (int)(-halfHeight) + currentPiece.Row * TILE_SCALE + 10, TILE_SCALE - 20, TILE_SCALE - 20), null, Color.Gray, 0, Vector2.Zero, SpriteEffects.None, 0);
                     */
                }

            }
            spriteBatch.End();
        }
        public void DrawCheckerBoard(CheckerTile[,] tileBoard, List<CheckerPiece> allPieces, CheckerPiece SelectedPiece, List<CheckerPiece>[] MovablePieces, Dictionary<CheckerPiece, List<CheckerTile>> MoveDict, int TILE_SCALE, SpriteBatch spriteBatch, Camera cam)
        {
            if (!contentLoaded)
                return;
            spriteBatch.Begin(SpriteSortMode.BackToFront,
                          BlendState.AlphaBlend,
                          null,
                          null,
                          null,
                          null,
                          cam.get_transformation(device));
            //Checker Tiles           
            for (int row = 0; row < tileBoard.GetLength(0); row++)
            {
                for (int col = 0; col < tileBoard.GetLength(1); col++)
                {
                    if (tileBoard[row, col].Color == 1)
                    {
                        spriteBatch.Draw(blankTexture, new Rectangle((int)(col * TILE_SCALE), (int)(row * TILE_SCALE), (int)TILE_SCALE, (int)TILE_SCALE), null, Color.Gray, 0, Vector2.Zero, SpriteEffects.None, 1);
                    }
                    else
                    {
                        spriteBatch.Draw(blankTexture, new Rectangle((int)(col * TILE_SCALE), (int)(row * TILE_SCALE), (int)TILE_SCALE, (int)TILE_SCALE), null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 1);
                    }
                }
            }
            //Lines
            for (int row = 0; row <= tileBoard.GetLength(0); row++)
            {
                spriteBatch.Draw(blankTexture, new Rectangle(0, (int)(row * TILE_SCALE), (int)(tileBoard.GetLength(1) * TILE_SCALE), 1), null, Color.Black, 0, Vector2.Zero, SpriteEffects.None, 0);
            }
            for (int col = 0; col <= tileBoard.GetLength(1); col++)
            {
                spriteBatch.Draw(blankTexture, new Rectangle((int)(col * TILE_SCALE), 0, 1, (int)(tileBoard.GetLength(0) * TILE_SCALE)), null, Color.Black, 0, Vector2.Zero, SpriteEffects.None, 0);
            }
            spriteBatch.End();

            //Draw Pieces
            DrawPieces(spriteBatch, cam, allPieces, TILE_SCALE);

            //Draw the possible moves
            if (SelectedPiece != null && MovablePieces != null && MovablePieces[SelectedPiece.Color].Contains(SelectedPiece) && MoveDict.ContainsKey(SelectedPiece))
            {
                DrawPossibleMoves(spriteBatch, cam, MoveDict[SelectedPiece], TILE_SCALE, SelectedPiece);
            }
        }
    }
}
