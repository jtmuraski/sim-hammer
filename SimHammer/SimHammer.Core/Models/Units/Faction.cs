using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SimHammer.Core.Models.Units
{
    public class Faction
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public Faction()
        {
            
        }
        public Faction(string name, string description)
        {
            Name = name;
            Description = description; 
            Id = Name.ToLower().Replace(' ', '-');
        }
    }
}
