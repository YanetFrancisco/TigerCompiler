using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime;
using System.Reflection;
using System.Reflection.Emit;


namespace TigerCompiler
{
    public class Arraydec_Node : NonStatement_Node
    {
        public NonStatement_Node Length { get { return GetChild(0) as NonStatement_Node; } }

        public NonStatement_Node Array_Type { get { return GetChild(1) as NonStatement_Node; } }

        public Scope scp { get; set; }

        #region Constructor
        public Arraydec_Node()
        {
        }
        #endregion

        #region Methods
        public override void Check_Semantics(Scope scope, Report report)
        {
            Is_Valid = true;

            if (Length == null)
            {
                report.AddError(Length.Line, Length.CharPositionInLine, "The length expression must return an integer value.");
                Is_Valid = false;
                Type_Info = new Type_Info(Tiger_Type.Error);
                return;
            }
            Length.Check_Semantics(scope, report);
            if (Length.Type_Info.Basic_Type != Tiger_Type.Int)
            {
                if(Length.Is_Valid)
                    report.AddError(Length.Line, Length.CharPositionInLine, "The expression must return an integer value.");
                Is_Valid = false;
                Type_Info = new Type_Info(Tiger_Type.Error);
                return;
            }

            if (Array_Type == null)
            {
                report.AddError(Array_Type.Line, Array_Type.CharPositionInLine, "The type expression must return a value.");
                Is_Valid = false;
                Type_Info = new Type_Info(Tiger_Type.Error);
                return;
            }
            Array_Type.Check_Semantics(scope, report);

            if (!Array_Type.Is_Valid)
                Is_Valid = false;

            Type_Info = Is_Valid ? Array_Type.Type_Info : new Type_Info(Tiger_Type.Error);
            scp = scope;
        }

        public override void Generate_Code(IL_Generator g)
        {
            LocalBuilder dim = g.il_Generator.DeclareLocal(typeof(int));
            LocalBuilder iterador = g.il_Generator.DeclareLocal(typeof(int));
            LocalBuilder vararray = g.il_Generator.DeclareLocal(typeof(Array)); ;

            Label init = g.Define_Label();
            Label end = g.Define_Label();
            Label noerror = g.Define_Label();
            Label count = g.Define_Label();

            Type type = null;
            switch (Array_Type.Type_Info.Basic_Type)
            {
                case Tiger_Type.Int:
                    type = typeof(int);
                    break;
                case Tiger_Type.String:
                    type = typeof(string);
                    break;
                case Tiger_Type.Array:
                    type = typeof(Array);
                    break;
                case Tiger_Type.Record:
                    type = typeof(object);
                    break;
                case Tiger_Type.Nil:
                    type = typeof(object);
                    break;
            }

            Length.Generate_Code(g);
            g.Tiger_Emit(OpCodes.Stloc, dim);
            g.Tiger_Emit(OpCodes.Ldloc, dim);
            g.Tiger_Emit(OpCodes.Ldc_I4_0);
            g.Tiger_Emit(OpCodes.Bge, noerror);
            g.Tiger_Emit(OpCodes.Ldstr, "the dimension of the array must be > 0");
            g.Tiger_Emit(OpCodes.Newobj, typeof(System.Exception).GetConstructor(new Type[] { typeof(string) }));
            g.Tiger_Emit(OpCodes.Throw);
            g.Tiger_Emit(OpCodes.Br, end);
            g.Mark_Label(noerror);
            g.Tiger_Emit(OpCodes.Ldloc, dim);
            if (type != null) g.Tiger_Emit(OpCodes.Newarr, type);
            g.Tiger_Emit(OpCodes.Stloc, vararray);
            g.Tiger_Emit(OpCodes.Ldc_I4_0);
            g.Tiger_Emit(OpCodes.Stloc, iterador);
            g.Tiger_Emit(OpCodes.Br, count);
            g.Mark_Label(init);
            g.Tiger_Emit(OpCodes.Ldloc, vararray);
            g.Tiger_Emit(OpCodes.Ldloc, iterador);
            Array_Type.Generate_Code(g);
            g.Tiger_Emit(OpCodes.Stelem, type);
            g.Tiger_Emit(OpCodes.Ldloc, iterador);
            g.Tiger_Emit(OpCodes.Ldc_I4_1);
            g.Tiger_Emit(OpCodes.Add);
            g.Tiger_Emit(OpCodes.Stloc, iterador);
            g.Mark_Label(count);
            g.Tiger_Emit(OpCodes.Ldloc, iterador);
            g.Tiger_Emit(OpCodes.Ldloc, dim);
            g.Tiger_Emit(OpCodes.Blt, init);
            g.Tiger_Emit(OpCodes.Ldloc, vararray);
            g.Mark_Label(end);



            //LocalBuilder dim = g.Variable_Temporal(typeof(int));
            //LocalBuilder iterador = g.Variable_Temporal(typeof(int));
            //LocalBuilder vararray = g.Variable_Temporal(typeof(Array)); ;

            //Label init = g.Define_Label();
            //Label end = g.Define_Label();
            //Label noerror = g.Define_Label();
            //Label count = g.Define_Label();

            //Type type = null;
            //switch (Array_Type.Type_Info.Basic_Type)
            //{
            //    case Tiger_Type.Int:
            //        type = typeof(int);
            //        break;
            //    case Tiger_Type.String:
            //        type = typeof(string);
            //        break;
            //    case Tiger_Type.Array:
            //        type = typeof(Array);
            //        break;
            //    case Tiger_Type.Record | Tiger_Type.Nil:
            //        type = typeof(object);
            //        break;
            //}



            //Length.Generate_Code(g); 
            //g.Tiger_Emit(OpCodes.Stloc, dim.LocalIndex);
            //g.Tiger_Emit(OpCodes.Ldloc, dim.LocalIndex);
            //g.Tiger_Emit(OpCodes.Ldc_I4_0);
            //g.Tiger_Emit(OpCodes.Bge, noerror);
            //g.Tiger_Emit(OpCodes.Ldstr, "the dimension of the array must be > 0");
            //g.Tiger_Emit(OpCodes.Newobj, typeof(System.Exception).GetConstructor(new Type[] { typeof(string) }));
            //g.Tiger_Emit(OpCodes.Throw);
            //g.Tiger_Emit(OpCodes.Br, end);
            //g.Mark_Label(noerror);
            //g.Tiger_Emit(OpCodes.Ldloc, dim.LocalIndex);
            //if (type != null) g.Tiger_Emit(OpCodes.Newarr, type); 
            //g.Tiger_Emit(OpCodes.Stloc, vararray.LocalIndex); 
            //g.Tiger_Emit(OpCodes.Ldc_I4_0);                   
            //g.Tiger_Emit(OpCodes.Stloc, iterador.LocalIndex); 
            //g.Tiger_Emit(OpCodes.Br, count);
            //g.Mark_Label(init);                       
            //g.Tiger_Emit(OpCodes.Ldloc, vararray.LocalIndex); 
            //g.Tiger_Emit(OpCodes.Ldloc, iterador.LocalIndex);
            //Array_Type.Generate_Code(g);                
            //g.Tiger_Emit(OpCodes.Stelem, type);          
            //g.Tiger_Emit(OpCodes.Ldloc, iterador.LocalIndex); 
            //g.Tiger_Emit(OpCodes.Ldc_I4_1);
            //g.Tiger_Emit(OpCodes.Add);
            //g.Tiger_Emit(OpCodes.Stloc, iterador.LocalIndex);
            //g.Mark_Label(count);                  
            //g.Tiger_Emit(OpCodes.Ldloc, iterador.LocalIndex);
            //g.Tiger_Emit(OpCodes.Ldloc, dim.LocalIndex);
            //g.Tiger_Emit(OpCodes.Blt, init);
            //g.Tiger_Emit(OpCodes.Ldloc, vararray.LocalIndex);
            //g.Mark_Label(end);
        }
        #endregion
    }
}
