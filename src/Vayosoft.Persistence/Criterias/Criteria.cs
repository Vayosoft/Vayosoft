using System.Linq.Expressions;
using Vayosoft.Persistence.Specifications;

namespace Vayosoft.Persistence.Criterias
{
    public class Criteria<T> : ICriteria<T> where T : class
    {
        private Func<T, bool> _function;

        private Func<T, bool> Function => _function ??= _predicate.Compile();
        private Expression<Func<T, bool>> _predicate;

        public Criteria() { }

        public Criteria(Expression<Func<T, bool>> predicate)
        {
            _predicate = predicate;
        }

        public bool IsSatisfiedBy(T entity)
            => Function.Invoke(entity);

        public Expression<Func<T, bool>> ToExpression()
        {
            return _predicate ?? (arg => true);
        }

        internal readonly List<Expression<Func<T, object>>> _includes = new();
        public IReadOnlyCollection<Expression<Func<T, object>>> Includes => _includes.AsReadOnly();


        public void Where(Expression<Func<T, bool>> predicate)
        {
            _predicate = predicate;
        }

        public void WhereIf(bool cnd, Expression<Func<T, bool>> predicate)
        {
            _predicate = cnd
            ? predicate
            : p => true;
        }

        public Criteria<T> Include(Expression<Func<T, object>> includeExpression)
        {
            _includes.Add(includeExpression);
            return this;
        }

        public static implicit operator Func<T, bool>(Criteria<T> spec)
        {
            return spec.Function;
        }

        public static implicit operator Expression<Func<T, bool>>(Criteria<T> spec)
        {
            return spec._predicate;
        }

        public static bool operator true(Criteria<T> _) => false;
        public static bool operator false(Criteria<T> _) => false;

        public static Criteria<T> operator !(Criteria<T> spec)
        {
            return new Criteria<T>(
                Expression.Lambda<Func<T, bool>>(
            Expression.Not(spec._predicate.Body), spec._predicate.Parameters));
        }

        public static Criteria<T> operator &(Criteria<T> left, Criteria<T> right)
        {
            var leftExpr = left._predicate;
            var rightExpr = right._predicate;
            var leftParam = leftExpr.Parameters[0];
            var rightParam = rightExpr.Parameters[0];

            var result = new Criteria<T>(
                Expression.Lambda<Func<T, bool>>(
                    Expression.AndAlso(
                        leftExpr.Body,
                        new ParameterReplacer(rightParam, leftParam).Visit(rightExpr.Body)),
                    leftParam));

            result._includes.AddRange(left.Includes.Union(right.Includes));

            return result;
        }

        public static Criteria<T> operator |(Criteria<T> left, Criteria<T> right)
        {
            var leftExpr = left._predicate;
            var rightExpr = right._predicate;
            var leftParam = leftExpr.Parameters[0];
            var rightParam = rightExpr.Parameters[0];

            var result = new Criteria<T>(
                Expression.Lambda<Func<T, bool>>(
                    Expression.OrElse(
                        leftExpr.Body,
                        new ParameterReplacer(rightParam, leftParam).Visit(rightExpr.Body)),
                    leftParam));

            result._includes.AddRange(left.Includes.Union(right.Includes));

            return result;
        }
    }
}
