using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TigerCompiler
{
    public enum ErrorCodes
    {   NoError, 
        WrongParameters,
        FileError, 
        SyntaxError, 
        SemanticError, 
        CodeGenerationError,
        UnexpectedError 
    }
}
