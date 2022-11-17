using System.Linq.Expressions;
using Vayosoft.Commons.Models;

namespace Vayosoft.Specifications
{
    public class Specification<TEntity> : ISpecification<TEntity> where TEntity : class
    {
        private Func<TEntity, bool> _function;

        private Func<TEntity, bool> Function => _function ??= _predicate.Compile();

        private Expression<Func<TEntity, bool>> _predicate;

        public Specification() { }

        public Specification(Expression<Func<TEntity, bool>> predicate) {
            _predicate = predicate;
        }

        public bool IsSatisfiedBy(TEntity entity)
            => Function.Invoke(entity);

        public Expression<Func<TEntity, bool>> ToExpression()
        {
            return _predicate;
        }

        private readonly List<Expression<Func<TEntity, object>>> _includes = new();
        public IReadOnlyCollection<Expression<Func<TEntity, object>>> Includes => _includes.AsReadOnly();
           

        public Sorting<TEntity> Sorting { get; private set; }

        protected void Where(Expression<Func<TEntity, bool>> predicate) {
            _predicate = predicate;
        }

        protected void WhereIf(bool cnd, Expression<Func<TEntity, bool>> predicate) {
            _predicate = cnd
                ? predicate 
                : p => true;
        }

        protected Specification<TEntity> Include(Expression<Func<TEntity, object>> includeExpression) {
            _includes.Add(includeExpression);
            return this;
        }

        protected void OrderBy(Expression<Func<TEntity, object>> orderByExpression) {
            Sorting = new Sorting<TEntity>(orderByExpression, SortOrder.Asc);
        }

        protected void OrderByDescending(Expression<Func<TEntity, object>> orderByDescExpression) {
            Sorting = new Sorting<TEntity>(orderByDescExpression, SortOrder.Desc);
        }

        public static implicit operator Func<TEntity, bool>(Specification<TEntity> spec)
        {
            return spec.Function;
        }

        public static implicit operator Expression<Func<TEntity, bool>>(Specification<TEntity> spec)
        {
            return spec._predicate;
        }

        public static bool operator true(Specification<TEntity> _) => false;

        public static bool operator false(Specification<TEntity> _) => false;

        public static Specification<TEntity> operator !(Specification<TEntity> spec)
        {
            return new Specification<TEntity>(
                Expression.Lambda<Func<TEntity, bool>>(
                    Expression.Not(spec._predicate.Body), spec._predicate.Parameters));
        }

        public static Specification<TEntity> operator &(Specification<TEntity> left, Specification<TEntity> right)
        {
            var leftExpr = left._predicate;
            var rightExpr = right._predicate;
            var leftParam = leftExpr.Parameters[0];
            var rightParam = rightExpr.Parameters[0];

            var result = new Specification<TEntity>(
                Expression.Lambda<Func<TEntity, bool>>(
                    Expression.AndAlso(
                        leftExpr.Body,
                        new ParameterReplacer(rightParam, leftParam).Visit(rightExpr.Body)),
                    leftParam));

            return result;
        }

        public static Specification<TEntity> operator |(Specification<TEntity> left, Specification<TEntity> right)
        {
            var leftExpr = left._predicate;
            var rightExpr = right._predicate;
            var leftParam = leftExpr.Parameters[0];
            var rightParam = rightExpr.Parameters[0];

            return new Specification<TEntity>(
                Expression.Lambda<Func<TEntity, bool>>(
                    Expression.OrElse(
                        leftExpr.Body,
                        new ParameterReplacer(rightParam, leftParam).Visit(rightExpr.Body)),
                    leftParam));
        }
    }
}
