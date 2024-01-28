﻿grammar VyrokovaLogika;

@parser::members
{
    protected const int EOF = Eof;
}

@lexer::members
{
    protected const int EOF = Eof;
    protected const int HIDDEN = Hidden;
}


prog: expr;

expr
    : NEGATION (VAR | LEFTPARENS expr RIGHTPARENS)   # Negation
    | DOUBLE_NEGATION (VAR | LEFTPARENS expr RIGHTPARENS)  # DoubleNegationRule
    | expr CONJUNCTION expr          # Conjunction
    | expr DISJUNCTION expr          # Disjunction
    | expr EQUIVALENCE expr          # Equivalence 
    | expr IMPLICATION expr          # Implication
    | VAR                # Variable
    | LEFTPARENS expr RIGHTPARENS           # parens
    ;

/*
 * Lexer Rules
 */

VAR: [a-zA-Z]+;
IMPLICATION: '⇒';
LEFTPARENS: '(';
RIGHTPARENS: ')';
CONJUNCTION: '∧';
DISJUNCTION: '∨';
EQUIVALENCE: '≡';
NEGATION: '¬';
DOUBLE_NEGATION: '¬¬';
WS: (' ' | '\r' | '\n') -> channel(HIDDEN);