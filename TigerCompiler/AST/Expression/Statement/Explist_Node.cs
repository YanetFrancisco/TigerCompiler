using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime;

namespace TigerCompiler
{
    public class Explist_Node : Statement_Node
    {
        List<NonStatement_Node> expressions;

        public Scope scp { get; set; }

        bool init;

        public List<NonStatement_Node> Expressions
        {
            get 
            {
                if (!init)
                {
                    for (int i = 0; i < ChildCount; i++)
                    {
                        expressions.Add(GetChild(i) as NonStatement_Node);
                    }
                    init = true;
                }
                return expressions;
            }
        }

        #region Constructor
        public Explist_Node()
        {
            expressions = new List<NonStatement_Node>();
            init = false;
        }
        #endregion

        #region Methods
        public override void Check_Semantics(Scope scope, Report report)
        {
            Is_Valid = true;

            foreach (var exp in Expressions)
            {
                if (exp == null)
                {
                    report.AddError(exp.Line, exp.CharPositionInLine, "The parameter expressions must return a value.");
                    Is_Valid = false;
                    return;
                }
                exp.Check_Semantics(scope, report);
                if (!exp.Is_Valid)
                {
                    Is_Valid = false;
                    return;
                } 
                if (exp.Type_Info.Depth > scope.Depth)
                {
                    report.AddError(exp.Line, exp.CharPositionInLine, "The value returned by the parameter expression is out of scope.");
                    Is_Valid = false;
                    return;
                }
            }
            scp = scope;
        }

        public override void Generate_Code(IL_Generator g)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
