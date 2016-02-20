using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;

namespace TigerCompiler
{
    public class Scope
    {
        protected Scope Parent { get; set; }
        protected List<Scope> Children { get; set; }
        public int Depth { get; set; }
        protected Dictionary<string, Tiger_Info> info;
        protected Dictionary<string, Type_Info> type_info;
		public List<string> unresolved;
        public Dictionary<string, KeyValuePair<string, bool>> unresolved_p;


        private Dictionary<string, Type> types;
		private Dictionary<string, FieldBuilder> variables;
        private Dictionary<string, KeyValuePair<ConstructorBuilder, List<FieldBuilder>>> record_fields;
        private Dictionary<string, MethodInfo> method_infos;

        //public static bool IsStandardFunction(string name)
        //{
        //    switch (name)
        //    {
        //        case "print":
        //        case "printi":
        //        case "ord":
        //        case "chr":
        //        case "size":
        //        case "substring":
        //        case "concat":
        //        case "not":
        //        case "exit":
        //        case "printline":
        //        case "printiline":
        //        case "getline":
        //            return true;
        //        default:
        //          return  false;
        //    }
        //}


        #region Constructor
        public Scope()
        {
            Children = new List<Scope>();
            info = new Dictionary<string, Tiger_Info>();
            type_info = new Dictionary<string, Type_Info>();
            unresolved = new List<string>();
            unresolved_p = new Dictionary<string, KeyValuePair<string, bool>>();
            Depth = 0;

            types = new Dictionary<string, Type>();
            variables = new Dictionary<string, FieldBuilder>();
            record_fields = new Dictionary<string, KeyValuePair<ConstructorBuilder, List<FieldBuilder>>>();
            method_infos = new Dictionary<string, MethodInfo>();

        }
        #endregion

        #region Methods
        public Scope Create_Child_Scope()
        {
            Scope child = new Scope();
            child.Parent = this;
            child.Depth = this.Depth + 1;
            child.Add_Standard_Func();///////////agrego siempre las func standar a todos los scope
            Children.Add(child);
            return child;
        }

        public void Add_Standard_Func()/////////////////////newwwwwwwwww
        {
            Dictionary<string, Variable_Info> _info;
            Variable_Info _var;

            _info = new Dictionary<string, Variable_Info>();
            _var = new Variable_Info(new String_Info(), false);
            _var.ID = "s";
            _info.Add("s", _var);
            this.Add_Info("print", new Procedure_Info(_info, null));

            _info = new Dictionary<string, Variable_Info>();
            _var = new Variable_Info(new Int_Info(), false);
            _var.ID = "i";
            _info.Add("i", _var);
            this.Add_Info("printi", new Procedure_Info(_info, null));

            _info = new Dictionary<string, Variable_Info>();
            this.Add_Info("getline", new Procedure_Info(_info, new String_Info()));

            _info = new Dictionary<string, Variable_Info>();
            _var = new Variable_Info(new String_Info(), false);
            _var.ID = "s";
            _info.Add("s", _var);
            this.Add_Info("printline", new Procedure_Info(_info, null));

            _info = new Dictionary<string, Variable_Info>();
            _var = new Variable_Info(new Int_Info(), false);
            _var.ID = "s";
            _info.Add("s", _var);
            this.Add_Info("printiline", new Procedure_Info(_info, null));

            _info = new Dictionary<string, Variable_Info>();
            _var = new Variable_Info(new String_Info(), false);
            _var.ID = "s";
            _info.Add("s", _var);
            this.Add_Info("ord", new Procedure_Info(_info, new Int_Info()));

            _info = new Dictionary<string, Variable_Info>();
            _var = new Variable_Info(new Int_Info(), false);
            _var.ID = "i";
            _info.Add("i", _var);
            this.Add_Info("chr", new Procedure_Info(_info, new String_Info()));

            _info = new Dictionary<string, Variable_Info>();
            _var = new Variable_Info(new String_Info(), false);
            _var.ID = "s";
            _info.Add("s", _var);
            this.Add_Info("size", new Procedure_Info(_info, new Int_Info()));

            _info = new Dictionary<string, Variable_Info>();
            _var = new Variable_Info(new String_Info(), false);
            _var.ID = "s";
            _info.Add("s", _var);
            _var = new Variable_Info(new Int_Info(), false);
            _var.ID = "f";
            _info.Add("f", _var);
            _var = new Variable_Info(new Int_Info(), false);
            _var.ID = "n";
            _info.Add("n", _var);
            this.Add_Info("substring", new Procedure_Info(_info, new String_Info()));

            _info = new Dictionary<string, Variable_Info>();
            _var = new Variable_Info(new String_Info(), false);
            _var.ID = "s1";
            _info.Add("s1", _var);
            _var = new Variable_Info(new String_Info(), false);
            _var.ID = "s2";
            _info.Add("s2", _var);
            this.Add_Info("concat", new Procedure_Info(_info, new String_Info()));

            _info = new Dictionary<string, Variable_Info>();
            _var = new Variable_Info(new Int_Info(), false);
            _var.ID = "i";
            _info.Add("i", _var);
            this.Add_Info("not", new Procedure_Info(_info, new Int_Info()));

            _info = new Dictionary<string, Variable_Info>();
            _var = new Variable_Info(new Int_Info(), false);
            _var.ID = "i";
            _info.Add("i", _var);
            this.Add_Info("exit", new Procedure_Info(_info, null));
        }

        public Tiger_Info Find_Info(string name)
        {
            if (info.ContainsKey(name)) return info[name];
            if (Parent != null) return Parent.Find_Info(name);
            return null;
        }

        public Type_Info Find_Type_Info(string name, bool rec)
        {
            if (type_info.ContainsKey(name)) return type_info[name];
            if (Parent != null && rec) return Parent.Find_Type_Info(name,rec);
            return null;
        }

        public void Add_Info(string name, Tiger_Info inf)
        {
            info.Add(name, inf);
        }

        public void Add_Type_Info( Type_Info inf)
        {
            if (unresolved.Contains(inf.ID))
            {
                type_info[inf.ID] = inf;
                unresolved.Remove(inf.ID);
            }
            else type_info.Add(inf.ID, inf);
        }

        public void Add_Unresolved(Type_Info inf)
        {
            unresolved.Add(inf.ID);
            type_info.Add(inf.ID, inf);
           // unresolved_p.Add(me, new KeyValuePair<string, bool>(inf.ID, is_field));
        }

        public bool Contain_Info(string name, bool all) ////cambio para buscar solo en el scope actual o en todos los scope
        {
            if (info.ContainsKey(name)) return true;
            if (Parent != null && all) return Parent.Contain_Info(name, all);
            return false;
        }

        public bool Contain_Type_Info(string name)
        {
            if (type_info.ContainsKey(name)) return true;
            if (Parent != null) return Parent.Contain_Type_Info(name);
            return false;
        }

        public Procedure_Info Find_Procedure_Info(string name)
        {
            if (info.Keys.Contains(name) && info[name] is Procedure_Info)
                return (Procedure_Info)info[name];
            if (Parent != null)
                return Parent.Find_Procedure_Info(name);
            return null;
        }

        public Variable_Info Find_Variable_Info(string name, bool all) ////cambio para buscar solo en el scope actual o en todos los scope
        {
            if (info.Keys.Contains(name) && info[name] is Variable_Info)
                return (Variable_Info)info[name];
            if (Parent != null && all)
                return Parent.Find_Variable_Info(name, all);
            return null;
        }

        public void Add_Type(string name, Type type)
        {
            if (!types.ContainsKey(name))
                types.Add(name, type);
        }
       
        public Type Find_Type(string name)
        {
            if (types.ContainsKey(name))
                return types[name];
            if (Parent != null)
                return Parent.Find_Type(name);
            return null;
        }

        public void Add_Field(string name, ConstructorBuilder ctor, List<FieldBuilder> fields)
        {
            if (!record_fields.ContainsKey(name))
                record_fields.Add(name, new KeyValuePair<ConstructorBuilder, List<FieldBuilder>>(ctor, fields));
        }

        public KeyValuePair<ConstructorBuilder,List<FieldBuilder>> Return_Field(string key)
        {
            if (record_fields.ContainsKey(key))
                return record_fields[key];
            if (Parent != null)
                return Parent.Return_Field(key);
            return new KeyValuePair<ConstructorBuilder,List<FieldBuilder>>();
        }

        public void Update_Field(string name, ConstructorBuilder ctor, List<FieldBuilder> fields)
        {
            record_fields[name] = new KeyValuePair<ConstructorBuilder, List<FieldBuilder>>(ctor, fields);
        }

        public FieldBuilder Find_Var(string name)
        {
            if (variables.ContainsKey(name))
                return variables[name];
            if (Parent != null)
                return Parent.Find_Var(name);
            return null;
        }

        public MethodInfo Find_Method_Info(string name)
        {
            if (method_infos.ContainsKey(name))
                return method_infos[name];
            else if (Parent != null)
                return Parent.Find_Method_Info(name);
            return null;

        }

        public void Add_Var(string name, FieldBuilder field)
        {
            if (!variables.ContainsKey(name))
                variables.Add(name, field);
        }

        public void Add_Method(string name, MethodInfo method)
        {
            if(!method_infos.ContainsKey(name))
                 method_infos.Add(name, method);
        }

        #endregion

    }
}
