using MMogri.Input;
using MMogri.Renderer;

namespace MMogri
{
    class MapScreen : ContentFrame
    {
        ClientGameState gameState;

        public MapScreen(GameWindow w, InputHandler i) : base(w, i)
        { }

        public void Init(ClientGameState g)
        {
            gameState = g;

            DrawFrame(1, 2, window.sizeX - 20, window.sizeY - 4);

            window.SetLine(gameState.mapName, posX + width - 5 - gameState.mapName.Length, posY);

            UpdateMap();
        }

        public void UpdateMap()
        {
            for (int y = 0; y < height - 2; y++)
            {
                window.SetPosition(posX + 1, height - y);
                for (int x = 0; x < width - 2; x++)
                {
                    int x0 = gameState.posX - (int)(width * .5f) + x;
                    int y0 = gameState.posY - (int)(height * .5f) + y;

                    if (x0 >= 0 && x0 < gameState.mapSizeX && y0 >= 0 && y0 < gameState.mapSizeY)
                    {
                        window.SetColor((Color)gameState.GetMapTile(x0, y0).color);
                        window.SetNext(gameState.GetMapTile(x0, y0).tag);
                    }
                    else
                    {
                        window.SetNext(' ');
                    }
                }
            }
        }
    }
}