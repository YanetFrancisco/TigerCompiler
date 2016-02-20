using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime;

namespace TigerCompiler
{
    public abstract class Binary_Node : NonStatement_Node
    {
        public NonStatement_Node Left
        {
            get
            {
                return GetChild(0) as NonStatement_Node;
                
            }
        }

        public NonStatement_Node Right
        {
            get
            {
                return GetChild(1) as NonStatement_Node;

            }
        }                           

        #region Constructor
        public Binary_Node() { }
        #endregion 
    }
}
