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
    public class IL_Generator
    {
        ModuleBuilder module_Builder;
        MethodBuilder method_Builder;
        TypeBuilder type_Builder;
        AssemblyName assembly_Name;
        AssemblyBuilder assembly_Builder;
       
        public ILGenerator il_Generator;

        private string es;

        #region Constructor
        public IL_Generator(string path)
        {
            string name = Path.GetFileNameWithoutExtension(path);
            if (name == null)
                throw new Exception("Error");
            assembly_Name = new AssemblyName(name);
            AppDomain app_Domain = System.Threading.Thread.GetDomain();
            string directory_name = Path.GetDirectoryName(path);
            assembly_Builder = app_Domain.DefineDynamicAssembly(assembly_Name, AssemblyBuilderAccess.RunAndSave, directory_name);
            module_Builder = assembly_Builder.DefineDynamicModule(name, name + ".exe");
            type_Builder = module_Builder.DefineType("Program");
            method_Builder = type_Builder.DefineMethod("Main", MethodAttributes.Public | MethodAttributes.Static,typeof(void), null);
            il_Generator = method_Builder.GetILGenerator();
            es = name + ".exe";
        }
        #endregion

        #region Define
        public FieldBuilder Define_Variable(Type type, string name, Scope scope)
        {
            FieldBuilder field=  type_Builder.DefineField(name, type, FieldAttributes.Public | FieldAttributes.Static);
            scope.Add_Var(name, field);
            return field;
        }
        public LocalBuilder Variable_Temporal(Type type)
        {
            return il_Generator.DeclareLocal(type);
        }
        public TypeBuilder Define_Type(string name)
        {
            return module_Builder.DefineType(name, TypeAttributes.Public);
        }
        public MethodBuilder Define_Method(string name, Type returnType, Type[] parameters_Type, Scope scope)
        {
            MethodBuilder result = type_Builder.DefineMethod(name, MethodAttributes.Public | MethodAttributes.Static, returnType, parameters_Type);
            scope.Add_Method(name, result);
            return result;
        }
        public Label Define_Label()
        {
            return il_Generator.DefineLabel();
        }
        public void Mark_Label(Label loc)
        {
            il_Generator.MarkLabel(loc);
        }
        #endregion 

        #region Emit
        public void Tiger_Emit(OpCode op_Code)
        {
            il_Generator.Emit(op_Code);
        }

        public void Tiger_Emit(OpCode op_Code, ConstructorInfo con)
        {
            il_Generator.Emit(op_Code, con);
        }

        public void Tiger_Emit(OpCode op_Code, int arg)
        {
            il_Generator.Emit(op_Code, arg);
        }

        public void Tiger_Emit(OpCode op_Code, Label label)
        {
            il_Generator.Emit(op_Code, label);
        }

        public void Tiger_Emit(OpCode op_Code, LocalBuilder local)
        {
            il_Generator.Emit(op_Code, local);
        }

        public void Tiger_Emit(OpCode op_Codes, MethodInfo method_Info)
        {
            il_Generator.Emit(op_Codes, method_Info);
        }

        public void Tiger_Emit(OpCode op_Code, string str)
        {
            il_Generator.Emit(op_Code, str);
        }

        public void Tiger_Emit(OpCode op_Code, Type cls)
        {
            il_Generator.Emit(op_Code, cls);
        }

        public void Tiger_Emit(OpCode op_Code, FieldBuilder field)
        {
            il_Generator.Emit(op_Code, field);
        }

        public void Tiger_Emit_Call(OpCode op_Code, MethodInfo method_Info, Type[] operational_Type_Params)
        {
            il_Generator.EmitCall(op_Code, method_Info, operational_Type_Params);
        }
        #endregion 

        #region Standard Library
        public void Print()
        {
            MethodInfo writeLine = typeof(Console).GetMethod("Write", new Type[] { typeof(string) });
            il_Generator.Emit(OpCodes.Call, writeLine);
        }
        public void Printi()
        {
            MethodInfo writeLine = typeof(Console).GetMethod("Write", new Type[] { typeof(int) });
            il_Generator.Emit(OpCodes.Call, writeLine);
        }
        public void Ord(Explist_Node params_value, Scope scope)
        {
            Label empty = il_Generator.DefineLabel();
            Label end = il_Generator.DefineLabel();
            MethodInfo getLength = typeof(String).GetMethod("get_Length", new Type[] { });
            il_Generator.Emit(OpCodes.Callvirt, getLength);
            il_Generator.Emit(OpCodes.Brfalse, empty);
            params_value.Expressions[0].Generate_Code(this);
            il_Generator.Emit(OpCodes.Ldc_I4_0);
            MethodInfo gc = typeof(String).GetMethod("get_Chars", new Type[] { typeof(int) });
            il_Generator.Emit(OpCodes.Callvirt, gc);
            il_Generator.Emit(OpCodes.Br, end);
            il_Generator.MarkLabel(empty);
            il_Generator.Emit(OpCodes.Ldc_I4_M1);
            il_Generator.MarkLabel(end);
            
        }
        public void Chr()
        {
            LocalBuilder _char = il_Generator.DeclareLocal(typeof(char));
            il_Generator.Emit(OpCodes.Conv_U2);
            il_Generator.Emit(OpCodes.Stloc, _char);
            il_Generator.Emit(OpCodes.Ldloca, _char);
            MethodInfo to_string = typeof(Char).GetMethod("ToString", new Type[] { });
            il_Generator.Emit(OpCodes.Callvirt, to_string);
        }
        public void Size()
        {
            MethodInfo size = typeof(String).GetProperty("Length").GetGetMethod();
            il_Generator.Emit(OpCodes.Call, typeof(string).GetProperty("Length").GetGetMethod()); ;
        }
        public void Substring()
        {
            MethodInfo substring = typeof(String).GetMethod("Substring", new Type[] { typeof(int), typeof(int) });
            il_Generator.Emit(OpCodes.Callvirt, substring);
        }
        public void Concat()
        {
            MethodInfo concat = typeof(String).GetMethod("Concat", new Type[] { typeof(string), typeof(string) });
            il_Generator.EmitCall(OpCodes.Call, concat,null);
        }
        public void Not()
        {
            il_Generator.Emit(OpCodes.Ldc_I4_0);
            il_Generator.Emit(OpCodes.Ceq);
        }
        public void Exit()
        {
            MethodInfo exit = typeof(Environment).GetMethod("Exit", new Type[] { typeof(int) });
            il_Generator.Emit(OpCodes.Call, exit);
        }
        public void GetLine()
        {
            MethodInfo readLine = typeof(Console).GetMethod("ReadLine", new Type[] { });
            il_Generator.Emit(OpCodes.Call, readLine);
        }
        public void PrintLine()
        {
            MethodInfo writeLine = typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string) });
            il_Generator.Emit(OpCodes.Call, writeLine);
        }
        public void PrintiLine()
        {
            MethodInfo writeLine = typeof(Console).GetMethod("WriteLine", new Type[] { typeof(int) });
            il_Generator.Emit(OpCodes.Call, writeLine);
        }
        #endregion

        public void Save()
        {

            #region Para el tester
            PropertyInfo property = typeof(Environment).GetProperty("ExitCode");
            il_Generator.Emit(OpCodes.Call, property.GetSetMethod());
            #endregion

            #region Para local test
            //_ilGenerator.Emit(OpCodes.Pop);
            //_ilGenerator.Emit(OpCodes.Call, typeof(Console).GetMethod("ReadLine", new Type[] { }));
            //_ilGenerator.Emit(OpCodes.Pop);
            #endregion


            il_Generator.Emit(OpCodes.Ret);
            type_Builder.CreateType();
            assembly_Builder.SetEntryPoint(method_Builder);
            assembly_Builder.Save(es);
        }


    }
}
