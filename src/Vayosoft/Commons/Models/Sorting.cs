﻿using System.Linq.Expressions;

namespace Vayosoft.Commons.Models
{
    public enum SortOrder : byte
    {
        Asc = 1,
        Desc = 2
    }

    public sealed class Sorting<TEntity> : Sorting<TEntity, object> where TEntity : class
    {
        public Sorting(Expression<Func<TEntity, object>> expression, SortOrder sortOrder = SortOrder.Asc) 
            : base(expression, sortOrder) { }

        public static Sorting<TEntity> Asc(Expression<Func<TEntity, object>> expression) =>
            new(expression);
        public static Sorting<TEntity> Desc(Expression<Func<TEntity, object>> expression) =>
            new(expression, SortOrder.Desc);
    }

    public class Sorting<TEntity, TKey>
        where TEntity : class
    {
        public Expression<Func<TEntity, TKey>> Expression { get; }

        public SortOrder SortOrder { get; }

        public Sorting(
            Expression<Func<TEntity, TKey>> expression,
            SortOrder sortOrder = SortOrder.Asc)
        {
            Expression = expression;
            SortOrder = sortOrder;
        }
    }
}