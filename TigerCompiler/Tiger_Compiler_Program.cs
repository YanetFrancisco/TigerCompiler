using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;
using Antlr.Runtime;
using System.IO;

namespace TigerCompiler
{
    public class Tiger_Compiler_Program
    {
        protected Expression_Node Root { get; set; }

        public Tiger_Compiler_Program(Expression_Node root)
        {
            Root = root as Expression_Node;
        }

        public Report CheckSemantics(Scope scope)
        {
            //Dictionary<string, Variable_Info> info;
            //Variable_Info var;
            Report report = new Report();
            scope.Add_Type_Info(new Int_Info { ID = "int" });
            scope.Add_Type_Info(new String_Info() { ID = "string" });

            scope.Add_Standard_Func();//////newwwwwwwwww

            //info = new Dictionary<string, Variable_Info>();
            //var =   new Variable_Info(new String_Info(), false);
            //var.ID = "s";
            //info.Add("s", var);
            //scope.Add_Info("print", new Procedure_Info(info, null));

            //info = new Dictionary<string, Variable_Info>();
            //var = new Variable_Info(new Int_Info(), false);
            //var.ID = "i";
            //info.Add("i", var);
            //scope.Add_Info("printi", new Procedure_Info(info, null));

            //info = new Dictionary<string, Variable_Info>();
            //scope.Add_Info("getline", new Procedure_Info(info, new String_Info()));

            //info = new Dictionary<string, Variable_Info>();
            //var = new Variable_Info(new String_Info(), false);
            //var.ID = "s";
            //info.Add("s", var);
            //scope.Add_Info("printline", new Procedure_Info(info, null));

            //info = new Dictionary<string, Variable_Info>();
            //var = new Variable_Info(new Int_Info(), false);
            //var.ID = "s";
            //info.Add("s", var);
            //scope.Add_Info("printiline", new Procedure_Info(info, null));

            //info = new Dictionary<string, Variable_Info>();
            //var = new Variable_Info(new String_Info(), false);
            //var.ID = "s";
            //info.Add("s", var);
            //scope.Add_Info("ord", new Procedure_Info(info, new Int_Info()));

            //info = new Dictionary<string, Variable_Info>();
            //var = new Variable_Info(new Int_Info(), false);
            //var.ID = "i";
            //info.Add("i", var);
            //scope.Add_Info("chr", new Procedure_Info(info, new String_Info()));

            //info = new Dictionary<string, Variable_Info>();
            //var = new Variable_Info(new String_Info(), false);
            //var.ID = "s";
            //info.Add("s", var);
            //scope.Add_Info("size", new Procedure_Info(info, new Int_Info()));

            //info = new Dictionary<string, Variable_Info>();
            //var = new Variable_Info(new String_Info(), false);
            //var.ID = "s";
            //info.Add("s", var);
            //var = new Variable_Info(new Int_Info(), false);
            //var.ID = "f";
            //info.Add("f", var);
            //var = new Variable_Info(new Int_Info(), false);
            //var.ID = "n";
            //info.Add("n", var);
            //scope.Add_Info("substring", new Procedure_Info(info, new String_Info()));

            //info = new Dictionary<string, Variable_Info>();
            //var = new Variable_Info(new String_Info(), false);
            //var.ID = "s1";
            //info.Add("s1", var);
            //var = new Variable_Info(new String_Info(), false);
            //var.ID = "s2";
            //info.Add("s2", var);
            //scope.Add_Info("concat", new Procedure_Info(info, new String_Info()));

            //info = new Dictionary<string, Variable_Info>();
            //var = new Variable_Info(new Int_Info(), false);
            //var.ID = "i";
            //info.Add("i", var);
            //scope.Add_Info("not", new Procedure_Info(info, new Int_Info()));

            //info = new Dictionary<string, Variable_Info>();
            //var = new Variable_Info(new Int_Info(), false);
            //var.ID = "i";
            //info.Add("i", var);
            //scope.Add_Info("exit", new Procedure_Info(info, null));



            Root.Check_Semantics(scope, report);
            return report;
        }

        public bool GenerateCode(string executableLocation)
        {
            IL_Generator g = new IL_Generator(executableLocation);
            Type excp = typeof(Exception);
            ConstructorInfo exctor = excp.GetConstructor(new Type[] { });
            MethodInfo getError = typeof(Console).GetMethod("get_Error", new Type[] { });
            MethodInfo writeLineMI = typeof(TextWriter).GetMethod("WriteLine", new Type[] { typeof(string) });
            MethodInfo exToStrMI = excp.GetMethod("ToString");
            LocalBuilder temp = g.Variable_Temporal(excp);
            LocalBuilder temp2 = g.Variable_Temporal(typeof(int));
            g.Tiger_Emit(OpCodes.Ldc_I4_0);
            g.Tiger_Emit(OpCodes.Stloc, temp2.LocalIndex);
            Label exBlock = g.il_Generator.BeginExceptionBlock();

            Root.Generate_Code(g);
            var nodeaux = (Root as NonStatement_Node);
            if (nodeaux != null && nodeaux.Type_Info.Basic_Type != Tiger_Type.Void)
                g.Tiger_Emit(OpCodes.Pop);

            g.il_Generator.BeginCatchBlock(excp);
            g.Tiger_Emit(OpCodes.Stloc_S, temp);
            g.Tiger_Emit(OpCodes.Ldstr, "Caught {0}");
            g.Tiger_Emit_Call(OpCodes.Call, getError, null);
            g.Tiger_Emit(OpCodes.Ldloc_S, temp);
            g.Tiger_Emit_Call(OpCodes.Callvirt, exToStrMI, null);
            g.Tiger_Emit_Call(OpCodes.Callvirt, writeLineMI, null);
            g.Tiger_Emit(OpCodes.Ldc_I4_1);
            g.Tiger_Emit(OpCodes.Stloc_S, temp2);
            g.il_Generator.EndExceptionBlock();

            g.Tiger_Emit(OpCodes.Ldloc, temp2.LocalIndex);
            g.Save();
            return true; 
        }
    }
}
