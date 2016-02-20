using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr.Runtime;
using System.Reflection.Emit;

namespace TigerCompiler
{
    public abstract class Atomic_Node : NonStatement_Node
    {
        #region Constructor
        public Atomic_Node() { }
        #endregion
    }
}
