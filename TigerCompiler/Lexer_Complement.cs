using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr.Runtime;

namespace TigerCompiler.Grammar 
{
    public partial class tigerLexer
    {

        public Report Errors { get; set; }      
        

        public override string GetErrorMessage(RecognitionException e, string[] tokenNames)
        {
            string message = base.GetErrorMessage(e, tokenNames);
            Errors.AddError(e.Line, e.CharPositionInLine,message);   
            return message;
        }
    }

    public partial class tigerParser
    {

        public Report Errors { get; set; }

        public override string GetErrorMessage(RecognitionException e, string[] tokenNames)
        {

            string message = base.GetErrorMessage(e, tokenNames);
            Errors.AddError(e.Line, e.CharPositionInLine, message);
            return message;
        }
    }
}
