using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime;
using System.Reflection;
using System.Reflection.Emit;

namespace TigerCompiler
{
    public class Block_Node : Declaration_Node
    {
        public Block_Node() { }

        public override void Generate_Code(IL_Generator g)
        {
            throw new NotImplementedException();
        }
    }
}
