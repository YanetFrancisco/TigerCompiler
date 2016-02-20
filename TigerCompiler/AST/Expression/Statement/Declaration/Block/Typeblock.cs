using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime;
using System.Reflection;
using System.Reflection.Emit;

namespace TigerCompiler
{
    public class Typeblock_Node : Block_Node
    {
        private List<Typedec_Node> type_decs;

        private bool init;

        public List<Typedec_Node> Type_Decs
        {
            get
            {
                if (!init)
                {
                    for (int i = 0; i < ChildCount; i++)
                    {
                        type_decs.Add(GetChild(i) as Typedec_Node);
                    }
                    init = true;
                }
                return type_decs;
            }
        }

        #region Constructor
        public Typeblock_Node()
        {
            type_decs = new List<Typedec_Node>();
            init = false;
        }
        #endregion
    }
}
