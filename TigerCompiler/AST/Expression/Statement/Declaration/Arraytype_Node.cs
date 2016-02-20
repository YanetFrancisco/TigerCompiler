using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime;
using System.Reflection;
using System.Reflection.Emit;

namespace TigerCompiler
{
    public class Arraytype_Node : Declaration_Node
    {
        //en este statement_node se guarda uno de 2: id_node o type_node
        public Statement_Node Array_Type { get { return GetChild(0) as Statement_Node; } }
        
        public Id_Node Id { get; set; }

        public Scope scp { get; set; }

        #region Constructor
        public Arraytype_Node()
        {
            Id = new Id_Node();
        }
        #endregion

        #region Methods
        public override void Check_Semantics(Scope scope, Report report)
        {
            Is_Valid = true;

            if (!scope.Contain_Type_Info(Array_Type.Text))
            {
                report.AddError(Array_Type.Line, Array_Type.CharPositionInLine, "The type " + Array_Type.Text + " does not exist in the current context.");
                Is_Valid = false;
                return;
            }
            scp = scope;
        }

        public override void Generate_Code(IL_Generator g)
        {
            Type type = typeof(Array);
            string name = Id.Text;
            scp.Add_Type(name, type);

        }
        #endregion
    }
}
