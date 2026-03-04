using System;
using System.Collections.Generic;
using System.Text;

namespace SimHammer.Core.Persistance.Tools
{
    public static class PersistanceTools
    {
        public static string NormalizeFactionPartitionKey(string factionName)
        {
            return $"faction-{factionName.ToLower().Replace(' ', '-')}";
        }
    }
}
