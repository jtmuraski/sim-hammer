using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimHammer.Core.Services.Interfaces
{
    public interface IDiceRoller
    {
        int RollD6();
        int RollD6WithModifier(int modifier);
    }
}
