using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UncannyValleyMod
{
    internal abstract class Token
    {
        public ModSaveData saveModel;

        public abstract bool UpdateContext();
    }
}
