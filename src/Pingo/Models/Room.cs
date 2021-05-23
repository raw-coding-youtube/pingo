using System.Collections.Generic;

namespace Pingo.Models
{
    public class Room
    {
        public Room()
        {
            Users = new List<string>();
            DrawEvents = new List<DrawEvent>();
        }
        
        public int Id { get; set; }
        public List<string> Users { get; set; }
        public string Word { get; set; } = "forest";

        public List<DrawEvent> DrawEvents { get; set; }

    }
}