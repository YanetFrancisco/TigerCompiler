grammar tiger;

options 
{
	language = CSharp3;
	output = AST;
	backtrack= true;
	

}
tokens
{
	VALUE;
	ARRAYDEC;
	RECORDDEC;
	TYPEDEC;
	EXPSEQ;
	EXPLIST;
	FIELDLIST;
	FIELD;
	TYPEFIELDS;
	TYPEFIELD;
	DECLIST;
	ARRAYTYPE;
	RECORDTYPE;
	FUNCDEC;
	CALL;
	IDACCESS;
	AACCESS;
	PROC;
	NEG;
	ACCESS;
	STRING;
	TYPEBLOCK;
	VARBLOCK;
	FUNCBLOCK;
}

@lexer::header
{
    using System;
}

@lexer::namespace { TigerCompiler.Grammar }

@lexer::ctorModifier { public }

@parser::header 
{ 
    using System;
}

@parser::namespace { TigerCompiler.Grammar }


@parser::ctorModifier { public }



//Palabras reservadas

ARRAY:	'array';
OF:	'of';
FUNC:	'function';
VAR:	'var';
IF:	'if';
THEN:	'then';
ELSE:	'else';
WHILE:	'while';
DO:	'do';
FOR:	'for';
TO:	'to';
BREAK:	'break';
LET:	'let';
IN:	'in';
END:	'end';
TYPETOK:'type';
TYPE:	'int' | 'string';
NIL:	 'nil';

//Operadores

PLUS:	'+'; 
MINUS:	'-'; 
MULT:	'*';
DIV:	'/';
EQ:	'=';
NOTEQ:	'<>'; 
LTEQ:	'<=';
GTEQ:	'>=';
LT:	'<';
GT:	'>';
AND:	'&';
OR:	'|' ;

OPAR:	'(';
CPAR:	')';
OBRACK:	'[';
CBRACK:	']';
OKEYS:	'{';
CKEYS:	'}';
DOT:	'.';
COMMA:	',';
SMCOL:	';';
COLON:	':';
ASSIGN:	':=';

ID  :	('a'..'z'|'A'..'Z') ('a'..'z'|'A'..'Z'|'0'..'9'|'_')*;
    

INT :	('0'..'9')+ ;


WS  :   ( ' ' | '\t' | '\r' | '\n') {$channel=Hidden;};


//COMMENT:   '/*' (options {greedy=false;} : .)* (COMMENT (options {greedy=false;} : .)*)? '*/' {$channel=Hidden;};
COMMENT:   '/*' (options {greedy=false;} : COMMENT |.)* '*/' {$channel=Hidden;};


fragment DIGIT : '0'..'9';
fragment BINARYDIGIT : '0'..'1';
fragment OCTAL : '0'..'7';

fragment BACK_SLASH   : '\\';
fragment DOUBLE_QUOTE : '\"';
fragment PRINT_CHAR   : ((' '..'!')|('#'.. '[')|(']'..'~')) ;
fragment HALFOFBYTE
	:
	 '0' DIGIT DIGIT
	|'1' (BINARYDIGIT DIGIT | '2' OCTAL  );	

PRINTABLECHAR :	((' ' .. '!') | ('#'..'[') | (']'..'~'));    


	
STRING:   DOUBLE_QUOTE (ESC_SEQ | PRINTABLECHAR)* DOUBLE_QUOTE;

ESC_SEQ	: 
	BACK_SLASH
	('n' 
	|'r' 
	|'t' 
	|DOUBLE_QUOTE
	|BACK_SLASH
	|HALFOFBYTE
	|(WS)+ BACK_SLASH
	);	
	
//Gramatica	

public program:	exp EOF!;

exp:		auxOR  (OR^  auxOR) *  ;

auxOR:		auxAND (AND^ auxAND)* ;

auxAND:		auxPM ((EQ^ auxPM) | (NOTEQ^ auxPM) | (LTEQ^ auxPM) | (GTEQ^ auxPM) | (LT^ auxPM) | (GT^ auxPM))? ;

auxPM:		auxMD ((PLUS^ auxMD) | (MINUS^ auxMD))*;

auxMD:		auxEXP ((MULT^ auxEXP) | (DIV^ auxEXP))*;
		
auxEXP: 	
			STRING -> STRING
		|	MINUS auxEXP -> ^(NEG auxEXP)
		|	INT -> INT
		|	NIL -> NIL
		|	BREAK -> BREAK
		|	ID right?-> ^(VALUE ID right?)
		|	ifthen -> ifthen
		| 	while -> while
		|	for -> for
		|	let -> let
		|	OPAR expseq? CPAR -> ^(EXPSEQ expseq?);

right:
			record -> record 
		|   array -> array			
		|	call -> call
		|   lvalue? assign? -> ^(ACCESS lvalue? assign?);
	
array:		OBRACK exp CBRACK OF exp -> ^(ARRAYDEC exp exp);

record:		OKEYS fieldlist? CKEYS -> ^(RECORDDEC fieldlist?);

let:		LET declist IN expseq? END -> ^(DECLIST declist ^(EXPSEQ expseq)?);

declist	: 	declaration+ -> declaration+;

declaration: type_decl_block | var_decl_block | func_decl_block;
	
type_decl_block: typedec+ -> ^(TYPEBLOCK typedec+);
	
var_decl_block: vardec+  -> ^(VARBLOCK vardec+);
	
func_decl_block: funcdec+ -> ^(FUNCBLOCK funcdec+); 


typedec	:	TYPETOK ID EQ type -> ^(TYPEDEC ID type);

type	:	
		TYPE -> TYPE 
	| 	ID -> ID
	| 	OKEYS typefields? CKEYS -> ^(RECORDTYPE typefields?)
	| 	ARRAY OF (TYPE -> ^(ARRAYTYPE TYPE) | ID -> ^(ARRAYTYPE ID));

funcdec	:	FUNC ID OPAR typefields? CPAR procOrfunc
		-> ^(FUNCDEC ID typefields? procOrfunc);


procOrfunc
	:	COLON (ID | TYPE) EQ exp -> ^(FUNC ID? TYPE? exp )
	|	EQ exp -> ^(PROC exp);


vardec 	:	VAR ID (COLON (ID | TYPE))? ASSIGN exp
		-> ^(VAR ID ID? TYPE? exp);

for	:	FOR ID ASSIGN exp TO exp DO exp
		-> ^(FOR ID exp exp exp);

while	:	WHILE exp DO exp
		-> ^(WHILE exp exp);

ifthen	:	IF exp THEN exp ( ELSE exp)?
		-> ^(IF exp exp (exp)?);

call	:	OPAR explist? CPAR -> ^(CALL explist?);

	
assign	:	ASSIGN exp -> ^(ASSIGN exp);

lvalue 	:	(arrayAccess | idAccess)+ ;
	
arrayAccess :	OBRACK exp CBRACK -> ^(AACCESS exp);
	
idAccess	:	DOT ID -> ^(IDACCESS ID);

expseq :	exp (SMCOL exp)* -> exp (exp)*;

explist :	exp (COMMA exp)* -> ^(EXPLIST exp (exp)*);

//fieldlist :	ID EQ exp (COMMA ID EQ exp)* -> ^(FIELDLIST ID exp (ID exp)*);

//typefields:	ID COLON (TYPE | ID) (COMMA typefields)* -> ^(TYPEFIELDS ID ID? TYPE? typefields*);



typefields:	typefield (COMMA typefield)* -> ^(TYPEFIELDS typefield (typefield)*);

typefield: ID COLON (TYPE | ID)-> ^(TYPEFIELD ID ID? TYPE?);

fieldlist :	field (COMMA field)* -> ^(FIELDLIST field (field)*);

field :	ID EQ exp -> ^(FIELD ID exp);
