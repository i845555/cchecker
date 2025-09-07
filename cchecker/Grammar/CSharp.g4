// CSharp.g4 - Minimal subset grammar for C# (for demonstration/testing)
// Notes:
// - This is a simplified grammar covering basic constructs: using directives,
//   namespaces, classes, fields, methods, blocks, variable declarations, and
//   simple expressions. It is NOT a full C# grammar.
// - It is designed to be self-contained and compileable under ANTLR4, but
//   not meant to replace the full official grammar.

grammar CSharp;

// Parser rules

prog
    : compilation_unit EOF
    ;

compilation_unit
    : using_directive* namespace_member_declaration*
    ;

using_directive
    : 'using' qualified_name ';'
    ;

namespace_member_declaration
    : namespace_declaration
    | type_declaration
    ;

namespace_declaration
    : 'namespace' qualified_name '{' namespace_member_declaration* '}'
    ;

type_declaration
    : class_declaration
    ;

class_declaration
    : 'public'? 'class' IDENTIFIER class_body
    ;

class_body
    : '{' class_member_declaration* '}'
    ;

class_member_declaration
    : method_declaration
    | field_declaration
    | ';'
    ;

method_declaration
    : 'public'? type IDENTIFIER '(' parameter_list? ')' block
    ;

parameter_list
    : parameter (',' parameter)*
    ;

parameter
    : type IDENTIFIER
    ;

field_declaration
    : 'public'? type variable_declarators ';'
    ;

variable_declarators
    : variable_declarator (',' variable_declarator)*
    ;

variable_declarator
    : IDENTIFIER ('=' expression)?
    ;

type
    : predefined_type
    | qualified_name
    | IDENTIFIER
    ;

predefined_type
    : 'void'
    | 'bool'
    | 'byte'
    | 'char'
    | 'short'
    | 'int'
    | 'long'
    | 'float'
    | 'double'
    | 'decimal'
    | 'string'
    ;

qualified_name
    : IDENTIFIER ('.' IDENTIFIER)*
    ;

block
    : '{' statement* '}'
    ;

statement
    : block
    | variable_declaration
    | expression_statement
    | return_statement
    | ';'
    ;

variable_declaration
    : type variable_declarators ';'
    ;

expression_statement
    : expression ';'
    ;

return_statement
    : 'return' expression? ';'
    ;

expression
    : assignment
    ;

assignment
    : conditional_expr ('=' assignment)?
    ;

conditional_expr
    : logical_or_expr
    ;

logical_or_expr
    : logical_and_expr ('||' logical_and_expr)*
    ;

logical_and_expr
    : equality_expr ('&&' equality_expr)*
    ;

equality_expr
    : relational_expr (('==' | '!=') relational_expr)*
    ;

relational_expr
    : additive_expr (('<' | '>' | '<=' | '>=') additive_expr)*
    ;

additive_expr
    : multiplicative_expr (('+' | '-') multiplicative_expr)*
    ;

multiplicative_expr
    : unary_expr (('*' | '/' | '%') unary_expr)*
    ;

unary_expr
    : primary
    | ('+' | '-' | '!') unary_expr
    ;

primary
    : literal
    | IDENTIFIER
    | qualified_name
    | '(' expression ')'
    ;

literal
    : INT_LITERAL
    | STRING_LITERAL
    | 'true'
    | 'false'
    | 'null'
    ;

// Lexer rules

IDENTIFIER
    : IDENTIFIER_START IDENTIFIER_PART*
    ;

fragment IDENTIFIER_START
    : [a-zA-Z_]
    ;

fragment IDENTIFIER_PART
    : [a-zA-Z0-9_]
    ;

INT_LITERAL
    : [0-9]+
    ;

STRING_LITERAL
    : '"' (~["\\] | '\\' .)* '"'
    ;

WS
    : [ \t\r\n\u000C]+ -> skip
    ;

LINE_COMMENT
    : '//' ~[\r\n]* -> skip
    ;

COMMENT
    : '/*' .*? '*/' -> skip
    ;
