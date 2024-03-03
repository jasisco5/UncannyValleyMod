using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UncannyValleyMod
{
    public sealed class ModSaveData
    {
        public bool canSpawnNote { get; set; } = true;
        public bool weaponObtained { get; set; } = false;
        public int ExampleNumber { get; set; } = 0;
    }
}
