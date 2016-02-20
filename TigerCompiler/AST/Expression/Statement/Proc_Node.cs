using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime;
using System.Reflection;
using System.Reflection.Emit;

namespace TigerCompiler
{
    public class Proc_Node : Statement_Node
    {
        public Scope scp { get; set; }

        public Expression_Node Body { get { return GetChild(0) as Expression_Node; } }

        public bool Return { get { return false; } }

        #region Constructor
        public Proc_Node()
        {
        }
        #endregion

        #region Methods
        public override void Check_Semantics(Scope scope, Report report)
        {
            Is_Valid = true;

            NonStatement_Node proc_body = Body as NonStatement_Node;
            // proc_body.scp != null &&
            //if (proc_body != null)
            //{
            //    Body.Check_Semantics(scope, report);
            //    if (!Body.Is_Valid)
            //    {
            //        Is_Valid = false;
            //        return; 
            //    }
            //    if (proc_body.Type_Info.Basic_Type != Tiger_Type.Void)
            //    {
            //        report.AddError(Body.Line, Body.CharPositionInLine, "The expression of the procedure must not return a value.");
            //        Is_Valid = false;
            //        return;
            //    }
           
            //}
            //scp = scope;






            if (proc_body != null && proc_body.scp != null && proc_body.Type_Info.Basic_Type != Tiger_Type.Void)
            {
                report.AddError(Body.Line, Body.CharPositionInLine, "The expression of the procedure must not return a value.");
                Is_Valid = false;
                return;
            }
            Body.Check_Semantics(scope, report);

            if (proc_body != null && proc_body.Type_Info.Basic_Type != Tiger_Type.Void)
            {
                report.AddError(Body.Line, Body.CharPositionInLine, "The expression of the procedure must not return a value.");
                Is_Valid = false;
                return;
            }

            if (!Body.Is_Valid)
                Is_Valid = false;
            scp = scope;


          

        }

        public override void Generate_Code(IL_Generator g)
        {
            Body.Generate_Code(g);
        }
        #endregion
    }
}
