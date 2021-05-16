using System.Collections.Generic;

namespace Pingo.Models
{
    public class Room
    {
        public int Id { get; set; }
        public List<string> Users { get; set; }
        public string Word { get; set; } = "forest";

        public Room()
        {
            Users = new List<string>();
        }
    }
}