using Microsoft.EntityFrameworkCore;
using Vayosoft.Commons;
using Vayosoft.Commons.Entities;
using Vayosoft.Commons.Models.Pagination;
using Vayosoft.Persistence.EntityFramework.Extensions;
using Vayosoft.Persistence.Extensions;
using Vayosoft.Persistence.Queries;
using Vayosoft.Queries;

namespace Vayosoft.Persistence.EntityFramework.Queries
{
    public class ProjectionQueryHandler<TSpecification, TSource, TDest>
        : IQueryHandler<SpecificationQuery<TSpecification, IEnumerable<TDest>>, IEnumerable<TDest>>,
            IQueryHandler<SpecificationQuery<TSpecification, int>, int>
        where TSource : class, IEntity
        where TDest : class
    {
        protected readonly ILinqProvider LinqProvider;
        protected readonly IProjector Projector;

        public ProjectionQueryHandler(ILinqProvider linqProvider, IProjector projector)
        {
            LinqProvider = linqProvider;
            Projector = projector;
        }

        protected virtual IQueryable<TDest> GetQueryable(TSpecification spec)
            => LinqProvider
                .AsQueryable<TSource>()
                .ApplyIfPossible(spec)
                .Project<TSource, TDest>(Projector)
                .ApplyIfPossible(spec);

        public virtual async Task<IEnumerable<TDest>> Handle(SpecificationQuery<TSpecification, IEnumerable<TDest>> request, CancellationToken cancellationToken)
        {
            return await GetQueryable(request.Specification).ToArrayAsync(cancellationToken: cancellationToken);
        }

        public Task<int> Handle(SpecificationQuery<TSpecification, int> request, CancellationToken cancellationToken)
        {
            return GetQueryable(request.Specification).CountAsync(cancellationToken: cancellationToken);
        }
    }

    public class PagingQueryHandler<TSpec, TEntity, TDto> : ProjectionQueryHandler<TSpec, TEntity, TDto>,
        IQueryHandler<SpecificationQuery<TSpec, IPagedEnumerable<TDto>>, IPagedEnumerable<TDto>>
        where TEntity : class, IEntity
        where TDto : class, IEntity
        where TSpec : IPagingModel
    {
        public PagingQueryHandler(ILinqProvider linqProvider, IProjector projector)
            : base(linqProvider, projector) { }

        public IQueryHandler<SpecificationQuery<TSpec, IPagedEnumerable<TDto>>, IPagedEnumerable<TDto>> AsPaged() => this;

        public override async Task<IEnumerable<TDto>> Handle(SpecificationQuery<TSpec, IEnumerable<TDto>> request, CancellationToken cancellationToken)
        {
            return await GetQueryable(request.Specification)
                .Paginate(request.Specification)
                .ToArrayAsync(cancellationToken);
        }

        public Task<IPagedEnumerable<TDto>> Handle(SpecificationQuery<TSpec, IPagedEnumerable<TDto>> request, CancellationToken cancellationToken)
        {
            return GetQueryable(request.Specification)
                .ToPagedEnumerableAsync(request.Specification, cancellationToken);
        }
    }
}
