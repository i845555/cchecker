grammar CCalc;

// Parser rules
prog: expr EOF;

expr
    : expr op=('*'|'/') expr     # MulDiv
    | expr op=('+'|'-') expr     # AddSub
    | INT                        # Int
    | '(' expr ')'               # Parens
    ;

// Lexer rules
INT: [0-9]+;
WS: [ \t\r\n]+ -> skip;
