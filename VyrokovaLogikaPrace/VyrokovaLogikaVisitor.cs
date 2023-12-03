using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VyrokovaLogikaPrace
{
    public class VyrokovaLogikaVisitor : VyrokovaLogikaBaseVisitor<Node>
    {
        public override Node VisitVariable(VyrokovaLogikaParser.VariableContext context)
        {
            return new ValueNode(context.VAR().GetText());
        }

        public override Node VisitParens(VyrokovaLogikaParser.ParensContext context)
        {
            return Visit(context.expr());
        }

        public override Node VisitNegation(VyrokovaLogikaParser.NegationContext context)
        {
            Node innerExpression;

            if (context.VAR() != null)
            {
                // Handle the case where the double negation is applied to a variable
                innerExpression = new ValueNode(context.VAR().GetText());
            }
            else if (context.expr() != null)
            {
                // Handle the case where the double negation is applied to an expression in parentheses
                innerExpression = Visit(context.expr());
            }
            else
            {
                // Handle unexpected cases, or throw an exception
                innerExpression = null;
            }

            if (innerExpression != null)
            {
                return new NegationOperatorNode(innerExpression);
            }

            // Handle the case where the inner expression is null (optional)
            return null;
        }

        public override Node VisitDoubleNegationRule(VyrokovaLogikaParser.DoubleNegationRuleContext context)
        {
            Node innerExpression;

            if (context.VAR() != null)
            {
                // Handle the case where the double negation is applied to a variable
                innerExpression = new ValueNode(context.VAR().GetText());
            }
            else if (context.expr() != null)
            {
                // Handle the case where the double negation is applied to an expression in parentheses
                innerExpression = Visit(context.expr());
            }
            else
            {
                // Handle unexpected cases, or throw an exception
                innerExpression = null;
            }

            if (innerExpression != null)
            {
                return new DoubleNegationOperatorNode(innerExpression);
            }

            // Handle the case where the inner expression is null (optional)
            return null;
        }

        public override Node VisitImplication(VyrokovaLogikaParser.ImplicationContext context)
        {
            return new ImplicationOperatorNode(Visit(context.expr(0)), Visit(context.expr(1)));
        }

        public override Node VisitConjunction(VyrokovaLogikaParser.ConjunctionContext context)
        {
            return new ConjunctionOperatorNode(Visit(context.expr(0)), Visit(context.expr(1)));
        }

        public override Node VisitDisjunction(VyrokovaLogikaParser.DisjunctionContext context)
        {
            return new DisjunctionOperatorNode(Visit(context.expr(0)), Visit(context.expr(1)));
        }

        public override Node VisitEquivalence(VyrokovaLogikaParser.EquivalenceContext context)
        {
            return new EqualityOperatorNode(Visit(context.expr(0)), Visit(context.expr(1)));
        }
    }
}


