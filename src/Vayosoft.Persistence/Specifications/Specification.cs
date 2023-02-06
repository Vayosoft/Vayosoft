using System.Linq.Expressions;
using Vayosoft.Commons.Models;
using Vayosoft.Commons.Models.Pagination;
using Vayosoft.Persistence.Criterias;

namespace Vayosoft.Persistence.Specifications
{
    public class Specification<T> : PagingModelBase, ISpecification<T> where T : class
    {
        public ICriteria<T> Criteria { get; private set; }
        public Sorting<T> Sorting { get; private set; }
        public Expression<Func<T, object>> GroupBy { get; private set; }

        public Specification(ICriteria<T> criteria = null, Sorting<T> sorting = null)
        {
            Criteria = criteria ?? new Criteria<T>();
            if (sorting != null)
            {
                Sorting = sorting;
            }
        }

        protected void Where(Expression<Func<T, bool>> predicate)
        {
            Criteria = new Criteria<T>(predicate);
        }

        protected Specification<T> Include(Expression<Func<T, object>> includeExpression)
        {
            ((Criteria<T>)Criteria)._includes.Add(includeExpression);
            return this;
        }

        protected void OrderBy(Expression<Func<T, object>> orderByExpression)
        {
            Sorting = new Sorting<T>(orderByExpression, SortOrder.Asc);
        }

        protected void OrderByDescending(Expression<Func<T, object>> orderByDescExpression)
        {
            Sorting = new Sorting<T>(orderByDescExpression, SortOrder.Desc);
        }

        protected void ApplyGroupBy(Expression<Func<T, object>> groupByExpression)
        {
            GroupBy = groupByExpression;
        }
    }

    public class Specification<TResult, TSource> : Specification<TSource>, ISpecification<TResult, TSource>
        where TResult : class
        where TSource : class
    {
        public Specification(ICriteria<TSource> criteria = null, Sorting<TSource> sorting = null)
            : base(criteria, sorting) { }
    }
}
