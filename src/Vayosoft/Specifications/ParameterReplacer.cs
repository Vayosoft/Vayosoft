using System.Linq.Expressions;

namespace Vayosoft.Specifications
{
    internal class ParameterReplacer : ExpressionVisitor
    {
        private readonly ParameterExpression _parameter;
        private readonly ParameterExpression _replacement;

        public ParameterReplacer(ParameterExpression parameter, ParameterExpression replacement)
        {
            _parameter = parameter;
            _replacement = replacement;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return base.VisitParameter(_parameter == node ? _replacement : node);
        }
    }
}
