using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UncannyValleyMod
{
    public sealed class ModSaveData
    {
        public bool weaponObtained { get; set; } = false;
        public bool isBasementOpen { get; set; } = false;
        public int ExampleNumber { get; set; } = 0;

        Dictionary<int, bool> questObtained { get; set; } = new Dictionary<int, bool>();
    }
}
