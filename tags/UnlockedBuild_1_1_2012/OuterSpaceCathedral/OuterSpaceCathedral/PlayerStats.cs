using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OuterSpaceCathedral
{
    public class PlayerStatsManager
    {
        List<PlayerStats> players = new List<PlayerStats>() { new PlayerStats(), new PlayerStats(), new PlayerStats(), new PlayerStats() };

        public List<PlayerStats> Players
        {
            get
            {
                return players;
            }
        }
    }

    public class PlayerStats
    {
        public int Joins { set; get; }
        public int Kills { set; get; }
        public int Deaths { set; get; }
    }
}
