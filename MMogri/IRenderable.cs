namespace MMogri.Renderer
{
    interface IRenderable
    {
        char GetTag(Tile t);
        Color GetColor(Tile t);
    }
}