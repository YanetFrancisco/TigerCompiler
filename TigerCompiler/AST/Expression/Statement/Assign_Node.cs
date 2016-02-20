using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime;
using System.Reflection;
using System.Reflection.Emit;

namespace TigerCompiler
{
    public class Assign_Node : Statement_Node
    {
        public NonStatement_Node Expression { get { return GetChild(0) as NonStatement_Node; } }

        public Id_Node Id { get; set; }

        public Variable_Info Var_Info { get; set; }

        public Scope scp { get; set; }


        #region Constructor
        public Assign_Node()
        {
            Id = new Id_Node();
        }
        #endregion

        #region Methods
        public override void Check_Semantics(Scope scope, Report report)
        {
            Is_Valid = true;

            if (Var_Info.Is_Locked)
            {
                report.AddError(Id.Line, Id.CharPositionInLine, "The variable " + Id.Text + " cannot be assigned to.");
                Is_Valid = false;
                return;
            }

            if (Expression == null)
            {
                report.AddError(Expression.Line, Expression.CharPositionInLine, "The expression must return a value.");
                Is_Valid = false;
                return;
            }
            Expression.Check_Semantics(scope, report);

            if (!Expression.Is_Valid)
                Is_Valid = false;
            scp = scope;
        }

        public override void Generate_Code(IL_Generator g)
        {
            Expression.Generate_Code(g);
        }
        #endregion
    }
}
