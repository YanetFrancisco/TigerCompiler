using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime;
using System.Reflection.Emit;

namespace TigerCompiler
{
    public class Funcdec_Node : Funcblock_Node
    {
        ILGenerator aux_il_generator;

        public Id_Node Id { get { return GetChild(0) as Id_Node; } }

        public Typefields_Node Params { get { return GetChild(1) is Typefields_Node ? GetChild(1) as Typefields_Node : null; } }

        public Statement_Node Function { get { return GetChild(1) is Typefields_Node ? GetChild(2) as Statement_Node : GetChild(1) as Statement_Node; } }

        public Scope scp { get; set; }

        #region Constructor
        public Funcdec_Node()
        {
        }
        #endregion

        #region Methods
        public override void Check_Semantics(Scope scope, Report report)
        {
            Is_Valid = true;

            Scope f_scope = scope.Create_Child_Scope();
            Procedure_Info info = scope.Find_Procedure_Info(Id.Text);
            foreach (var p_info in info.Params_info())
                f_scope.Add_Info(p_info.ID, p_info);

            Function.Check_Semantics(f_scope, report);
            //parche
            //Proc_Node parche = Function as Proc_Node;
            //if (parche != null)
            //{
            //    if (info.Return_Type == null && !parche.Return)
            //    { Function.Is_Valid = false; }
            //}
            //parche
            if (!Function.Is_Valid)
                Is_Valid = false;
            scp = f_scope;
        }

        public void Preliminar_Semantic_Check(Scope scope, Report report)
        {
            Is_Valid = true;

            if (scope.Contain_Info(Id.Text, false))//si contiene una funcion o variable con el mismo nombre declarada en el mismo scope
            {
                report.AddError(Id.Line, Id.CharPositionInLine, "The ID " + Id.Text + " already exists in the current context.");
                Is_Valid = false;
                return;
            }
            Type_Info return_info = null;

            if (Function is Func_Node)
            {
                Statement_Node return_type = (Function as Func_Node).Return_Type;
                return_info = scope.Find_Type_Info(return_type.Text, true);
                
                if (return_info == null)
                {
                    report.AddError(return_type.Line, return_type.CharPositionInLine, "The function return type " + return_type.Text + " is not defined in the current context");
                    Is_Valid = false;
                    return;
                }
                (Function as Func_Node).Return_Info = return_info;
            }

            Dictionary<string, Variable_Info> f_info = new Dictionary<string, Variable_Info>();
            if (Params != null)
            {
                Params.Check_Semantics(scope, report);
                if (Params.Is_Valid)
                {
                    List<Typefield_Node> f_params = Params.Fields;
                    
                    foreach (Typefield_Node param in f_params)
                    {
                        Type_Info info = scope.Find_Type_Info(param.Type_Field.Text, true);
                        Variable_Info v_info = new Variable_Info(info, false);
                        v_info.ID = param.Name_Field.Text;
                        f_info.Add(v_info.ID, v_info);
                    }
                    
                }
                else
                    Is_Valid = false;
            }
            Procedure_Info p_info = new Procedure_Info(f_info, return_info);
            p_info.ID = Id.Text;
            scope.Add_Info(p_info.ID, p_info); 
        }

        public override void Generate_Code(IL_Generator g)
        {
            ILGenerator previous = g.il_Generator;
            g.il_Generator = aux_il_generator;
            List<KeyValuePair<string, LocalBuilder>> locals = new List<KeyValuePair<string, LocalBuilder>>();

            Type type;

            if (Params != null)
            {
                for (int i = 0; i < Params.Fields.Count; i++)
                {
                    if (Params.Fields[i].Type_Field.Text == "int")
                        type = typeof(int);

                    else if (Params.Fields[i].Type_Field.Text == "string")
                        type = typeof(string);

                    else
                        type = scp.Find_Type(Params.Fields[i].Type_Field.Text);

                    FieldBuilder variable = g.Define_Variable(type, Params.Fields[i].Name_Field.Text, scp);
                    g.Tiger_Emit(OpCodes.Ldarg, i);
                    g.Tiger_Emit(OpCodes.Stsfld, variable);
                }
            }

            Function.Generate_Code(g);
            
            g.Tiger_Emit(OpCodes.Ret);
            g.il_Generator = previous;
        }

        public void Declare(IL_Generator g, Scope scope)
        {
            Type returntype = typeof(void);
            if (Function is Func_Node)
            {
                string typename = (Function as Func_Node).Return_Type.Text;
                if (typename == "int")
                    returntype = typeof(int);
                else if (typename == "string")
                    returntype = typeof(string);
                else
                    returntype = scope.Find_Type(typename);
            }
            List<Type> params_types = new List<Type>();
            if (Params != null)
            {
                for (int i = 0; i < Params.Fields.Count; i++)
                {
                    string param_type = Params.Fields[i].Type_Field.Text;

                    if (param_type == "int")
                        params_types.Add(typeof(int));
                    else if (param_type == "string")
                        params_types.Add(typeof(string));
                    else if (scope.Find_Type(param_type)!=null)
                        params_types.Add(scope.Find_Type(param_type));
                }
            }
            if (returntype == null) returntype = typeof(void);
            MethodBuilder method = g.Define_Method(Id.Text, returntype, params_types.ToArray(), scope);
            aux_il_generator = method.GetILGenerator();

        }
        #endregion
    }
}
            
        
