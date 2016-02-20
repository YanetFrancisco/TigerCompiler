using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime;
using System.Reflection;
using System.Reflection.Emit;

namespace TigerCompiler
{
    public class Func_Node : Statement_Node
    {
        //en este statement_node se guarda uno de 2: id_node o type_node
        public Statement_Node Return_Type { get { return GetChild(0) as Statement_Node; } }

        public Type_Info Return_Info { get; set; }

        public NonStatement_Node Body { get { return GetChild(1) as NonStatement_Node; } }

        public Scope scp { get; set; }

        #region Constructor
        public Func_Node()
        {
        }
        #endregion

        #region Methods
        public override void Check_Semantics(Scope scope, Report report)
        {
            Is_Valid = true;


            if (Body != null)
            {
                Body.Check_Semantics(scope, report);

                if (Body.Is_Valid)
                {
                    if (!Body.Type_Info.Equals(Return_Info))
                    {
                        report.AddError(Body.Line, Body.CharPositionInLine, "The function return type does not match the expression return type.");
                        Is_Valid = false;
                        return;
                    }
                    if (Body.Type_Info.Basic_Type == Tiger_Type.Void)
                    {
                        report.AddError(Body.Line, Body.CharPositionInLine, "The expression of the function must return a value.");
                        Is_Valid = false;
                        return;
                    }
                }
                else
                    Is_Valid = false;
            }
            else
            {
                report.AddError(Body.Line, Body.CharPositionInLine, "The expression of the function must return a value.");
                Is_Valid = false;
                return;
            }
            scp = scope;

            //if (Body == null || Body.Type_Info.Basic_Type == Tiger_Type.Void)
            //{
            //    report.AddError(Body.Line, Body.CharPositionInLine, "The expression of the function must return a value.");
            //    Is_Valid = false;
            //    return;
            //}
            //Body.Check_Semantics(scope, report);

            //if (Body.Is_Valid)
            //{
            //    if (!Body.Type_Info.Equals(Return_Info))
            //    {
            //        report.AddError(Body.Line, Body.CharPositionInLine, "The function return type does not match the expression return type.");
            //        Is_Valid = false;
            //        return;
            //    }
            //}
            //else
            //    Is_Valid = false;
            //scp = scope;
            
        }

        public override void Generate_Code(IL_Generator g)
        {
            Body.Generate_Code(g);
        }
        #endregion
    }
}
