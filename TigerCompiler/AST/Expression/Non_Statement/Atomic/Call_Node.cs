using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime;
using System.Reflection;
using System.Reflection.Emit;

namespace TigerCompiler
{
    public class Call_Node : Atomic_Node
    {
        public Explist_Node Params
        {
            get { return ChildCount > 0 ? GetChild(0) as Explist_Node : null; }
        }

        public Scope scp { get; set; }

        public Id_Node Id { get; set; }

        #region Constructor
        public Call_Node()
        {
            Id = new Id_Node();
        }
        #endregion

        #region Methods
        public override void Check_Semantics(Scope scope, Report report)
        {
            Is_Valid = true;
            if (Params != null)
            {
                Params.Check_Semantics(scope, report);

                if (!Params.Is_Valid)
                {
                    Is_Valid = false;
                    Type_Info = new Type_Info(Tiger_Type.Error);
                }
            }
            scp = scope;
		}
        
        public override void Generate_Code(IL_Generator g)
        {
            List<string> var_names = new List<string>();
            List<KeyValuePair<string, LocalBuilder>> saved_values;
            List<Type> parametersType = new List<System.Type>();
            Save_Values(g, var_names, out saved_values, scp);
            if (Params != null)
            {
                foreach (var node in Params.Expressions)
                    node.Generate_Code(g);
            }
           
                switch (Id.Text)
                {
                    case "print":
                        g.Print();
                        break;
                    case "printi":
                        g.Printi();
                        break;
                    case "ord":
                        g.Ord(Params, scp);
                        break;
                    case "chr":
                        g.Chr();
                        break;
                    case "size":
                        g.Size();
                        break;
                    case "substring":
                        g.Substring();
                        break;
                    case "concat":
                        g.Concat();
                        break;
                    case "not":
                        g.Not();
                        break;
                    case "exit":
                        g.Exit();
                        break;
                    case "getline":
                        g.GetLine();
                        break;
                    case "printline":
                        g.PrintLine();
                        break;
                    case "printiline":
                        g.PrintiLine();
                        break;
                    default:
                        
                        int i = 0;
                        if(Params != null)
                        {
                        foreach (var parameterExpr in Params.Expressions)
                        {
                            switch ((parameterExpr as NonStatement_Node).Type_Info.Basic_Type)
                            {
                                case Tiger_Type.Int:
                                    parametersType.Add(typeof(int));
                                    break;
                                case Tiger_Type.String:
                                    parametersType.Add(typeof(string));
                                    break;
                                case Tiger_Type.Array:
                                    parametersType.Add(typeof(Array));
                                    break;
                                case Tiger_Type.Record:
                                    parametersType.Add(typeof(object));
                                    break;
                            }
                            i++;
                        }
                        }
                        MethodInfo func = scp.Find_Method_Info(Id.Text);
                        g.Tiger_Emit_Call(OpCodes.Call, func, parametersType.ToArray());
                        break;
                }
            
            Restore_Values(saved_values, scp, g);
        }

        public void Save_Values(IL_Generator g, List<string> var_names, out List<KeyValuePair<string, LocalBuilder>> saved_values, Scope scope)
        {
            saved_values = new List<KeyValuePair<string, LocalBuilder>>();
            if (Is_Recursive(out var_names))
            {
                foreach (var var in var_names)
                {
                    FieldBuilder field = scope.Find_Var(var);
                    LocalBuilder temp = g.Variable_Temporal(field.FieldType);
                    g.Tiger_Emit(OpCodes.Ldsfld, field);
                    g.Tiger_Emit(OpCodes.Stloc, temp.LocalIndex);
                    saved_values.Add(new KeyValuePair<string, LocalBuilder>(var, temp));
                }
            }
        }

        public void Restore_Values(List<KeyValuePair<string, LocalBuilder>> saved_values, Scope scope, IL_Generator g)
        {
            foreach (var var in saved_values)
            {
                FieldBuilder field = scope.Find_Var(var.Key);
                g.Tiger_Emit(OpCodes.Ldloc, var.Value.LocalIndex);
                g.Tiger_Emit(OpCodes.Stsfld, field);
            }
        }

        public bool Is_Recursive(out List<string> vars)
        {
            vars = new List<string>();
            Expression_Node recursive_func =
                this.Get_Nodes_To_Root().FirstOrDefault(x => x is Funcdec_Node && (x as Funcdec_Node).Id.Text == Id.Text);

            if (recursive_func == null)
                return false;

            List<Expression_Node> decs =
                this.Get_Nodes_To_Root().TakeWhile(x => x != recursive_func).Where(x => x is For_Node || x is Declist_Node || x is Var_Node).ToList();

            List<Expression_Node> parent_var_decs = new List<Expression_Node>(decs.Where(x => x is Var_Node));

            foreach (Expression_Node n in decs)
                //si es la variable de un for la agrego
                if (n is For_Node)
                    vars.Add((n as For_Node).Id.Text);
                else if (n is Declist_Node)
                {
                    var let = n as Declist_Node;
                    foreach (Var_Node v in let.Children.Where(x => x is Var_Node))
                        //si la variable no esta definida fuera del declist la agrego
                        if (!parent_var_decs.Contains(v))
                            vars.Add(v.Id.Text);
                }
            foreach (var v in (recursive_func as Funcdec_Node).Params.Fields)
                //agrego las variables q sean parametro de mi funcion
                vars.Add(v.Name_Field.Text);

            return true;
        }
        #endregion
    }
}
