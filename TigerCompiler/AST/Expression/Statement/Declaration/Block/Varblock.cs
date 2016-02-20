using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime;
using System.Reflection;
using System.Reflection.Emit;

namespace TigerCompiler
{
    public class Varblock_Node : Block_Node
    {
        private List<Var_Node> var_decs;

        private bool init;

        public List<Var_Node> Var_Decs
        {
            get
            {
                if (!init)
                {
                    for (int i = 0; i < ChildCount; i++)
                    {
                        var_decs.Add(GetChild(i) as Var_Node);
                    }
                    init = true;
                }
                return var_decs;
            }
        }

        #region Constructor
        public Varblock_Node()
        {
            var_decs = new List<Var_Node>();
            init = false;
        }
        #endregion
    }
}
