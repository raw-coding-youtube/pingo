namespace Pingo.Models
{
    public class DrawEvent
    {
        public int XStartPosition { get; set; }
        public int YStartPosition { get; set; }
        public int ToX { get; set; }
        public int ToY { get; set; }
        public string Color { get; set; }
        public int BrushSize { get; set; }
    }
}