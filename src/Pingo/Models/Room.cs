using System.Collections.Generic;
using System.Linq;

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
        public bool Started { get; set; }
        public string Admin => Users.FirstOrDefault();

        public string DrawingUser { get; set; }
    }
}