﻿using System.Linq.Expressions;
using Vayosoft.Commons;
using Vayosoft.Commons.Entities;
using Vayosoft.Persistence;
using Vayosoft.Specifications;

namespace Vayosoft.Utilities
{
    public static class LinqExtensions
    {
        public static IQueryable<T> Where<T>(this ILinqProvider linqProvider, Expression<Func<T, bool>> expr)
            where T : class, IEntity
            => linqProvider.AsQueryable<T>().Where(expr);

        public static IQueryable<T> WhereIf<T>(this ILinqProvider linqProvider, bool cnd, Expression<Func<T, bool>> expr)
            where T : class, IEntity
            => linqProvider.AsQueryable<T>().WhereIf(cnd, expr);

        public static IQueryable<T> WhereIf<T>(this IQueryable<T> queryable, bool cnd, Expression<Func<T, bool>> expr)
            => cnd
                ? queryable.Where(expr)
                : queryable;

        public static IQueryable<T> WhereIf<T>(this IQueryable<T> queryable, ISpecification<T> spec)
            where T : class, IEntity 
            => new SpecificationEvaluator<T>().Evaluate(queryable, spec);

        public static IQueryable<T> Apply<T>(this IQueryable<T> source, ILinqSpecification<T> spec)
            where T : class
            => spec.Apply(source);

        public static IQueryable<T> ApplyIfPossible<T>(this IQueryable<T> source, object spec)
            where T : class
            => spec is ILinqSpecification<T> specification
                ? specification.Apply(source)
                : source;

        public static IQueryable<TDest> Project<TSource, TDest>(this IQueryable<TSource> source, IProjector projector)
            => projector.Project<TSource, TDest>(source);

        public static TEntity ById<TEntity>(this ILinqProvider linqProvider, long id)
            where TEntity : class, IEntity<long>
            => linqProvider.AsQueryable<TEntity>().ById(id);

        public static TEntity ById<TEntity>(this IQueryable<TEntity> queryable, long id)
            where TEntity : class, IEntity<long>
            => queryable.SingleOrDefault(x => x.Id == id);
    }
}
