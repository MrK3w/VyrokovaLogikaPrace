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




prog: expr+ ;

expr
    : '¬' expr               # Negation
    | '¬¬' (VAR | '(' expr ')')    # DoubleNegationRule
    | expr '∧' expr          # Conjunction
    | expr '∨' expr          # Disjunction
    | expr '≡' expr          # Equivalence 
    | expr '⇒' expr          # Implication
    | VAR                    # Variable
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

WS
    : (' ' | '\r' | '\n') -> channel(HIDDEN)
    ;

/*
 * Parser Rules
 */
 UNSUPPORTED_OPERATOR: ~[a-zA-Z⇒∧∨≡¬] ;// Match any character not explicitly defined as another token