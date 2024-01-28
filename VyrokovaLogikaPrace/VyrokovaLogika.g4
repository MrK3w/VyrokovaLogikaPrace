grammar VyrokovaLogika;

@parser::members
{
    protected const int EOF = Eof;
}

@lexer::members
{
    protected const int EOF = Eof;
    protected const int HIDDEN = Hidden;
}


prog: expr+;

expr
    : '¬' (VAR | '(' expr ')')   # Negation
    | '¬¬' (VAR | '(' expr ')')  # DoubleNegationRule
    | expr '∧' expr          # Conjunction
    | expr '∨' expr          # Disjunction
    | expr '≡' expr          # Equivalence 
    | expr '⇒' expr          # Implication
    | VAR  ('∧'| '∨' | '≡' | '⇒' | EOF)              # Variable
    | '(' expr ')'           # parens
    ;

/*
 * Lexer Rules
 */

VAR: [a-zA-Z]+;
IMPLICATION: '⇒';
CONJUNCTION: '∧';
DISJUNCTION: '∨';
EQUIVALENCE: '≡';
NEGATION: '¬';
DOUBLE_NEGATION: '¬¬';
WS: (' ' | '\r' | '\n') -> channel(HIDDEN);

