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
            return new NegationOperatorNode(Visit(context.expr()));
        }

        public override Node VisitDoubleNegationRule(VyrokovaLogikaParser.DoubleNegationRuleContext context)
        {
            return new DoubleNegationOperatorNode(Visit(context.expr()));
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


