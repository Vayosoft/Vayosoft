using System.Linq.Expressions;
using Vayosoft.Commons.Models;
using Vayosoft.Persistence.Criterias;

namespace Vayosoft.Persistence.Specifications
{
    public class Specification<T> : ISpecification<T> where T : class
    {
        public ICriteria<T> Criteria { get; private set; }
        public Sorting<T> Sorting { get; private set; }
        public Expression<Func<T, object>> GroupBy { get; private set; }

        public Specification()
        {
            Criteria = new Criteria<T>();
        }

        public Specification(ICriteria<T> criteria)
        {
            Criteria = criteria;
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
}
