namespace Archimedes.Geometry.Rendering
{
    /// <summary>
    /// An element which can draw itself to a GDI+ G-Context. 
    /// </summary>
    public interface IDrawable
    {
        void Draw(System.Drawing.Graphics g);
    }
}
