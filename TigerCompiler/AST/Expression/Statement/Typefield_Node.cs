using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime;

namespace TigerCompiler
{
    public class Typefield_Node : Statement_Node
    {
        public Id_Node Name_Field { get { return GetChild(0) as Id_Node; } }

        public Scope scp { get; set; }

		//en este statement_node se guarda uno de 2: id_node o type_node
        public Statement_Node Type_Field { get { return GetChild(1) as Statement_Node; } }

        #region Constructor
        public Typefield_Node()
        {
        }
        #endregion

        #region Methods
        public override void Check_Semantics(Scope scope, Report report)
        {
            Is_Valid = true;

            if (!scope.Contain_Type_Info(Type_Field.Text))
            {
                report.AddError(Type_Field.Line, Type_Field.CharPositionInLine, "The type " +  Type_Field.Text + " does not exist in the current context.");
                Is_Valid = false;
                return;
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

