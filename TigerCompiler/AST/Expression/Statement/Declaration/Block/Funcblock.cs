using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime;
using System.Reflection;
using System.Reflection.Emit;

namespace TigerCompiler
{
    public class Funcblock_Node : Block_Node
    {
        List<Funcdec_Node> func_decs;

        private bool init;

        public List<Funcdec_Node> Func_Decs
        {
            get
            {
                if (!init)
                {
                    for (int i = 0; i < ChildCount; i++)
                    {
                        func_decs.Add(GetChild(i) as Funcdec_Node);
                    }
                    init = true;
                }
               return func_decs;
            }
        }

        #region Constructor
        public Funcblock_Node()
        {
            func_decs = new List<Funcdec_Node>();
            init = false;
        }
        #endregion
    }
}
