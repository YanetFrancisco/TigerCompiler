using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr.Runtime;
using System.Reflection.Emit;
using Antlr.Runtime.Tree;

namespace TigerCompiler
{
   
    public abstract class Expression_Node  : CommonTree      
    {
        public bool Is_Valid { get; set; }

        public Scope scp { get; set; }

        #region Constructor 
        public Expression_Node()
        {
        }
        #endregion

        #region Methods
        public IEnumerable<Expression_Node> Get_Nodes_To_Root()
        {
            yield return this;
            if (Parent != null)
            {   Expression_Node pa = this.Parent as Expression_Node;

                foreach(Expression_Node node in pa.Get_Nodes_To_Root())
                    yield return node;
            }
        }

        public IEnumerable<Expression_Node> Contain_Children_That(Func<Expression_Node, bool> predicate)
        {
            if (predicate(this))
                yield return this;
            for (int i = 0; i < ChildCount; i++)
            {
                Expression_Node child = GetChild(i) as Expression_Node;
                foreach (Expression_Node result in child.Contain_Children_That(predicate))
                    yield return result;
            }
        }
		
        public abstract void Check_Semantics(Scope scope, Report report);

        public bool Perform_Semantic_Check(Scope scope, Report report)
        {
            Check_Semantics(scope, report);
            scp = scope;
            return Is_Valid;
        }

        public abstract void Generate_Code(IL_Generator g);
        #endregion

    }
}
