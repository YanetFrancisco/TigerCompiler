using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime;

namespace TigerCompiler
{
    public class Declist_Node : NonStatement_Node
    {
        private List<Block_Node> blocks;

        public Scope scp { get; set; }

        private bool init;

        public List<Block_Node> Blocks 
        {
            get
            {
                if (!init)
                {
                    for (int i = 0; i < ChildCount - 1; i++)
                    {
                        blocks.Add(GetChild(i) as Block_Node);
                    }
                    if (!(GetChild(ChildCount - 1) is Expseq_Node))
                        blocks.Add(GetChild(ChildCount - 1) as Block_Node);
                    init = true;
                }
                return blocks;
            }
        }

        public Expseq_Node Exp_Seq { get { return ((GetChild(ChildCount - 1) is Expseq_Node)) ? (GetChild(ChildCount - 1) as Expseq_Node) : null; } }

        Dictionary<string, string> Types;

        public Declist_Node()
        {
            Types = new Dictionary<string, string>();

            blocks = new List<Block_Node>();
            init = false;
        }

        public override void Check_Semantics(Scope scope, Report report)
        {
            Is_Valid = true;

            Scope let_scope = scope.Create_Child_Scope();

            foreach (var block in Blocks)
            {
                if (block is Varblock_Node)
                {
                    Varblock_Node var_block = block as Varblock_Node;

                    foreach (Var_Node var_dec in var_block.Var_Decs)
                    {
                        var_dec.Check_Semantics(let_scope, report);
                        if (!var_dec.Is_Valid)
                        {
                            Is_Valid = false;
                            Type_Info = new Type_Info(Tiger_Type.Error);
                            return;
                        }
                    }
                }

                else if (block is Funcblock_Node)
                {
                    Funcblock_Node func_block = block as Funcblock_Node;

                    foreach (Funcdec_Node func_dec in func_block.Func_Decs)
                    {
                        func_dec.Preliminar_Semantic_Check(let_scope, report);
                        if (!func_dec.Is_Valid)
                        {
                            Is_Valid = false;
                            Type_Info = new Type_Info(Tiger_Type.Error);
                            return;
                        }
                    }

                    foreach (Funcdec_Node func_dec in func_block.Func_Decs)
                    {
                        func_dec.Check_Semantics(let_scope, report);
                        if (!func_dec.Is_Valid)
                        {
                            Is_Valid = false;
                            Type_Info = new Type_Info(Tiger_Type.Error);
                            return;
                        }
                    }
                }

                else //Typeblock_Node
                {
                    Typeblock_Node type_block = block as Typeblock_Node;

                    Check_Type_Declist(type_block.Type_Decs, let_scope, report);
                    if (report.List_Errors.Count > 0)
                    {
                        Is_Valid = false;
                        Type_Info = new Type_Info(Tiger_Type.Error);
                        return;
                    }
                }
            }

            if (Exp_Seq != null)
            {
                Exp_Seq.Check_Semantics(let_scope, report);
                if (Exp_Seq.Is_Valid)
                    Type_Info = Exp_Seq.Type_Info;
                else
                {
                    Is_Valid = false;
                    Type_Info = new Type_Info(Tiger_Type.Error);
                    return;
                }
            }
            else
                Type_Info = new Type_Info(Tiger_Type.Void);

            if (Type_Info.Depth > scope.Depth)
            {
                report.AddError(Line, CharPositionInLine, "The expression type returned by the let is not visible in the current scope");
                Is_Valid = false;
                Type_Info = new Type_Info(Tiger_Type.Error);
            }
            //scp = scope;
            scp = let_scope;
        }

        private bool Contains_Cyclic_Typedec(List<Typedec_Node> type_decs)
        {
            string alias_type;
            string nested_type;

            foreach (Typedec_Node type_dec in type_decs)
            {
                alias_type = type_dec.Id.Text;
                nested_type = "";
                Statement_Node type = type_dec._Type;

                if (type is Arraytype_Node)
                {
                    Statement_Node array_type = (type as Arraytype_Node).Array_Type;

                    if (array_type is Id_Node)
                        nested_type = array_type.Text;
                }
                else if (type is Id_Node)
                    nested_type = type.Text;

                if (nested_type != "")
                {
                    if (!Types.ContainsKey(nested_type))
                    {
                        Types.Add(nested_type, "");
                    }
                    //actualizar el tipo
                    else
                    {
                        string aux_type = Types[Types[nested_type]];
                        while (aux_type != "")
                        {
                            Types[nested_type] = aux_type;
                            aux_type = Types[aux_type];
                        }
                    }
                    if (!Types.ContainsKey(alias_type))
                    {
                        if (Types[nested_type] == "")
                            Types.Add(alias_type, nested_type);
                        else
                            Types.Add(alias_type, Types[nested_type]);
                    }
                    //si contiene el alias actualizo con el nuevo tipo
                    else
                    {
                        if (Types[nested_type] == "")
                            Types[alias_type] = nested_type;
                        else
                        {
                            Types[alias_type] = Types[nested_type];
                            //if (Types[alias_type] == Types[nested_type])
                            //    return true;
                        }
                        if (Types[alias_type] == Types[nested_type])////////////cambiooooooooo
                            return true;
                    }
                }
            }
            return false;
        }

        private void Check_Type_Declist(List<Typedec_Node> type_decs, Scope let_scope, Report report)
        {
            foreach (Typedec_Node type_dec in type_decs)
            {
                type_dec.Preliminar_Semantic_Check(let_scope, report);
                if (!type_dec.Is_Valid)
                {
                    Is_Valid = false;
                    Type_Info = new Type_Info(Tiger_Type.Error);
                    return;
                }
            }

            if (!Contains_Cyclic_Typedec(type_decs))
            {


                foreach (Typedec_Node type_dec in type_decs)
                {
                    type_dec.Check_Semantics(let_scope, report);
                    if (!type_dec.Is_Valid)
                    {
                        Is_Valid = false;
                        Type_Info = new Type_Info(Tiger_Type.Error);
                        return;
                    }
                }
            }
            else
            {
                report.AddError(Line, CharPositionInLine, "A type declaration inconsistency has been found. Check for cyclic type declarations.");
                Is_Valid = false;
                Type_Info = new Type_Info(Tiger_Type.Error);
            }
        }

        public override void Generate_Code(IL_Generator g)
        {
            foreach (var block in Blocks)
            {
                if (block is Typeblock_Node)
                {
                    foreach (var type in (block as Typeblock_Node).Type_Decs)
                    {
                        (type as Typedec_Node).Generate_Code(g);
                    }
                }
                else if (block is Funcblock_Node)
                {
                    //aqui hice cambios(declaro todo primero y depues genero el codigo)
                    foreach (var type in (block as Funcblock_Node).Func_Decs)
                        (type as Funcdec_Node).Declare(g, scp);

                    foreach (var type in (block as Funcblock_Node).Func_Decs)
                        (type as Funcdec_Node).Generate_Code(g);
                }
                else
                {
                    foreach (var type in (block as Varblock_Node).Var_Decs)
                    {
                        (type as Var_Node).Generate_Code(g);
                    }
                }
            }
            if (Exp_Seq != null)
                Exp_Seq.Generate_Code(g);
        }
    }
}
