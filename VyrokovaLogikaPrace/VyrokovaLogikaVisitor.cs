using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VyrokovaLogikaPrace
{
    public class VyrokovaLogikaVisitor : VyrokovaLogikaBaseVisitor<Node>
    {
        private int globalIdCounter = 1;

        private int GetNextId()
        {
            return globalIdCounter++;
        }

        public override Node VisitVariable(VyrokovaLogikaParser.VariableContext context)
        {
            return new ValueNode(context.VAR().GetText(), GetNextId());
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
                innerExpression = new ValueNode(context.VAR().GetText(), GetNextId());
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
                return new NegationOperatorNode(innerExpression, GetNextId(), context.GetText());
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
                innerExpression = new ValueNode(context.VAR().GetText(), GetNextId());
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
                return new DoubleNegationOperatorNode(innerExpression, GetNextId(), context.GetText());
            }

            // Handle the case where the inner expression is null (optional)
            return null;
        }

        public override Node VisitImplication(VyrokovaLogikaParser.ImplicationContext context)
        {
            return new ImplicationOperatorNode(Visit(context.expr(0)), Visit(context.expr(1)), GetNextId(), context.GetText());
        }

        public override Node VisitConjunction(VyrokovaLogikaParser.ConjunctionContext context)
        {
            return new ConjunctionOperatorNode(Visit(context.expr(0)), Visit(context.expr(1)), GetNextId(), context.GetText());
        }

        public override Node VisitDisjunction(VyrokovaLogikaParser.DisjunctionContext context)
        {
            return new DisjunctionOperatorNode(Visit(context.expr(0)), Visit(context.expr(1)), GetNextId(), context.GetText());
        }

        public override Node VisitEquivalence(VyrokovaLogikaParser.EquivalenceContext context)
        {
            return new EqualityOperatorNode(Visit(context.expr(0)), Visit(context.expr(1)), GetNextId(), context.GetText());
        }
    }
}


