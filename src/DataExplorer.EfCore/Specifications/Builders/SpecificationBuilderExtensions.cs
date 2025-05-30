﻿using System.Linq.Expressions;
using DataExplorer.Abstractions.Specifications.Builders;
using DataExplorer.Specifications;
using DataExplorer.Specifications.Exceptions;
using DataExplorer.Specifications.Expressions;
using EFCoreSecondLevelCacheInterceptor;

namespace DataExplorer.EfCore.Specifications.Builders;

/// <summary>
/// Builder extensions.
/// </summary>
[PublicAPI]
public static class SpecificationBuilderExtensions
{
    /// <summary>
    /// Specify a transform function to apply to the <typeparamref name="T"/> element 
    /// to produce a flattened sequence of <typeparamref name="TResult"/> elements.
    /// </summary>
    public static ISpecificationBuilder<T, TResult> SelectMany<T, TResult>(
        this ISpecificationBuilder<T, TResult> specificationBuilder,
        Expression<Func<T, IEnumerable<TResult>>> selector) where T : class
    {
        specificationBuilder.Specification.SelectorMany = selector;

        return specificationBuilder;
    }

#if NET7_0_OR_GREATER 
    /// <summary>
    ///         Specify property and value to be set in ExecuteUpdate method with chaining multiple calls for updating
    ///         multiple columns.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="specificationBuilder">The builder.</param>
    /// <param name="setPropertyCalls">The updates to execute.</param>
    public static IUpdateSpecificationBuilder<T> Modify<T>(this IUpdateSpecificationBuilder<T> specificationBuilder,
        Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> setPropertyCalls) where T : class =>
        Modify(specificationBuilder, setPropertyCalls, true);

    /// <summary>
    ///         Specify property and value to be set in ExecuteUpdate method with chaining multiple calls for updating
    ///         multiple columns.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="specificationBuilder">The builder.</param>
    /// <param name="setPropertyCalls">The updates to execute.</param>
    /// <param name="condition">If false, the criteria won't be added.</param>
    public static IUpdateSpecificationBuilder<T> Modify<T>(this IUpdateSpecificationBuilder<T> specificationBuilder,
        Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> setPropertyCalls, bool condition) where T : class
    {
        if (condition)
        {
            specificationBuilder.Specification.UpdateExpressions ??= new List<Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>>>();
            ((List<Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>>>)specificationBuilder.Specification.UpdateExpressions).Add(setPropertyCalls);
        }

        return specificationBuilder;
    }
#endif
    
    /// <summary>
    /// Specify a predicate that will be applied to the query
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="specificationBuilder"></param>
    /// <param name="criteria"></param>
    public static IBasicSpecificationBuilder<T> Where<T>(this IBasicSpecificationBuilder<T> specificationBuilder,
        Expression<Func<T, bool>> criteria) where T : class =>
        Where(specificationBuilder, criteria, true);

    /// <summary>
    /// Specify a predicate that will be applied to the query
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="specificationBuilder"></param>
    /// <param name="criteria"></param>
    /// <param name="condition">If false, the criteria won't be added.</param>
    public static IBasicSpecificationBuilder<T> Where<T>(this IBasicSpecificationBuilder<T> specificationBuilder,
        Expression<Func<T, bool>> criteria, bool condition) where T : class
    {
        if (condition)
        {
            specificationBuilder.Specification.WhereExpressions ??= new List<WhereExpressionInfo<T>>();
            ((List<WhereExpressionInfo<T>>)specificationBuilder.Specification.WhereExpressions).Add(
                new WhereExpressionInfo<T>(criteria));
        }

        return specificationBuilder;
    }
    
    /// <summary>
    /// Specify a predicate that will be applied to the query
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="specificationBuilder"></param>
    /// <param name="criteria"></param>
    public static ISpecificationBuilder<T> Where<T>(this ISpecificationBuilder<T> specificationBuilder,
        Expression<Func<T, bool>> criteria) where T : class =>
        Where(specificationBuilder, criteria, true);

    /// <summary>
    /// Specify a predicate that will be applied to the query
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="specificationBuilder"></param>
    /// <param name="criteria"></param>
    /// <param name="condition">If false, the criteria won't be added.</param>
    public static ISpecificationBuilder<T> Where<T>(this ISpecificationBuilder<T> specificationBuilder,
        Expression<Func<T, bool>> criteria, bool condition) where T : class
    {
        if (condition)
        {
            specificationBuilder.Specification.WhereExpressions ??= new List<WhereExpressionInfo<T>>();
            ((List<WhereExpressionInfo<T>>)specificationBuilder.Specification.WhereExpressions).Add(
                new WhereExpressionInfo<T>(criteria));
        }

        return specificationBuilder;
    }

    /// <summary>
    /// Specify the query result will be ordered by <paramref name="orderExpression"/> in an ascending order
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="specificationBuilder"></param>
    /// <param name="orderExpression"></param>
    public static IOrderedSpecificationBuilder<T> OrderBy<T>(this ISpecificationBuilder<T> specificationBuilder,
        Expression<Func<T, object?>> orderExpression) where T : class =>
        OrderBy(specificationBuilder, orderExpression, true);

    /// <summary>
    /// Specify the query result will be ordered by <paramref name="orderExpression"/> in an ascending order
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="specificationBuilder"></param>
    /// <param name="orderExpression"></param>
    /// <param name="condition">If false, the expression won't be added. The whole Order chain will be discarded.</param>
    public static IOrderedSpecificationBuilder<T> OrderBy<T>(this ISpecificationBuilder<T> specificationBuilder,
        Expression<Func<T, object?>> orderExpression, bool condition) where T : class
    {
        if (condition)
        {
            specificationBuilder.Specification.OrderExpressions ??= new List<OrderExpressionInfo<T>>();
            ((List<OrderExpressionInfo<T>>)specificationBuilder.Specification.OrderExpressions).Add(
                new OrderExpressionInfo<T>(orderExpression, OrderType.OrderBy));
        }

        var orderedSpecificationBuilder =
            new OrderedSpecificationBuilder<T>(specificationBuilder.Specification, !condition);

        return orderedSpecificationBuilder;
    }

    /// <summary>
    /// Specify the query result will be ordered by <paramref name="orderExpression"/> in a descending order
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="specificationBuilder"></param>
    /// <param name="orderExpression"></param>
    public static IOrderedSpecificationBuilder<T> OrderByDescending<T>(
        this ISpecificationBuilder<T> specificationBuilder, Expression<Func<T, object?>> orderExpression) where T : class =>
        OrderByDescending(specificationBuilder, orderExpression, true);

    /// <summary>
    /// Specify the query result will be ordered by <paramref name="orderExpression"/> in a descending order
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="specificationBuilder"></param>
    /// <param name="orderExpression"></param>
    /// <param name="condition">If false, the expression won't be added. The whole Order chain will be discarded.</param>
    public static IOrderedSpecificationBuilder<T> OrderByDescending<T>(
        this ISpecificationBuilder<T> specificationBuilder, Expression<Func<T, object?>> orderExpression,
        bool condition) where T : class
    {
        if (condition)
        {
            specificationBuilder.Specification.OrderExpressions ??= new List<OrderExpressionInfo<T>>();
            ((List<OrderExpressionInfo<T>>)specificationBuilder.Specification.OrderExpressions).Add(
                new OrderExpressionInfo<T>(orderExpression, OrderType.OrderByDescending));
        }

        var orderedSpecificationBuilder =
            new OrderedSpecificationBuilder<T>(specificationBuilder.Specification, !condition);

        return orderedSpecificationBuilder;
    }

    /// <summary>
    /// Specify the query result will be grouped by <paramref name="groupByExpression"/> in a descending order
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="specificationBuilder"></param>
    /// <param name="groupByExpression"></param>
    public static IGroupedSpecificationBuilder<T> GroupBy<T>(
        this ISpecificationBuilder<T> specificationBuilder,
        Expression<Func<T, object>> groupByExpression) where T : class
    {
        specificationBuilder.Specification.GroupByExpression = groupByExpression;
        
        var includeBuilder = new GroupedSpecificationBuilder<T>(specificationBuilder.Specification);

        return includeBuilder;
    }
    
    /// <summary>
    /// Specify an include expression.
    /// This information is utilized to build Include function in the query, which ORM tools like Entity Framework use
    /// to include related entities (via navigation properties) in the query result.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TProperty"></typeparam>
    /// <param name="specificationBuilder"></param>
    /// <param name="includeExpression"></param>
    public static IIncludableSpecificationBuilder<T, TProperty?> Include<T, TProperty>(
        this ISpecificationBuilder<T> specificationBuilder,
        Expression<Func<T, TProperty?>> includeExpression) where T : class
        => Include(specificationBuilder, includeExpression, true);

    /// <summary>
    /// Specify an include expression.
    /// This information is utilized to build Include function in the query, which ORM tools like Entity Framework use
    /// to include related entities (via navigation properties) in the query result.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TProperty"></typeparam>
    /// <param name="specificationBuilder"></param>
    /// <param name="includeExpression"></param>
    /// <param name="condition">If false, the expression won't be added. The whole Include chain will be discarded.</param>
    public static IIncludableSpecificationBuilder<T, TProperty?> Include<T, TProperty>(
        this ISpecificationBuilder<T> specificationBuilder,
        Expression<Func<T, TProperty?>> includeExpression,
        bool condition) where T : class
    {
        if (condition)
        {
            var info = new IncludeExpressionInfo(includeExpression, typeof(T), typeof(TProperty));
            specificationBuilder.Specification.IncludeExpressions ??= new List<IncludeExpressionInfo>();
            ((List<IncludeExpressionInfo>)specificationBuilder.Specification.IncludeExpressions).Add(info);
        }

        var includeBuilder = new IncludableSpecificationBuilder<T, TProperty?>(specificationBuilder.Specification, !condition);

        return includeBuilder;
    }

    /// <summary>
    /// Specify a collection of navigation properties, as strings, to include in the query.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="specificationBuilder"></param>
    /// <param name="includeString"></param>
    public static ISpecificationBuilder<T> Include<T>(
        this ISpecificationBuilder<T> specificationBuilder,
        string includeString) where T : class
        => Include(specificationBuilder, includeString, true);

    /// <summary>
    /// Specify a collection of navigation properties, as strings, to include in the query.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="specificationBuilder"></param>
    /// <param name="includeString"></param>
    /// <param name="condition">If false, the include expression won't be added.</param>
    public static ISpecificationBuilder<T> Include<T>(
        this ISpecificationBuilder<T> specificationBuilder,
        string includeString,
        bool condition) where T : class
    {
        if (condition)
        {
            specificationBuilder.Specification.IncludeStrings ??= new List<string>();
            ((List<string>)specificationBuilder.Specification.IncludeStrings).Add(includeString);
        }

        return specificationBuilder;
    }
    
    /// <summary>
    /// Specify the query result will be ordered by <paramref name="orderExpression"/> in an ascending order
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="specificationBuilder"></param>
    /// <param name="orderExpression"></param>
    public static IOrderedBasicSpecificationBuilder<T> OrderBy<T>(this IBasicSpecificationBuilder<T> specificationBuilder,
        Expression<Func<T, object?>> orderExpression) where T : class =>
        OrderBy(specificationBuilder, orderExpression, true);

    /// <summary>
    /// Specify the query result will be ordered by <paramref name="orderExpression"/> in an ascending order
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="specificationBuilder"></param>
    /// <param name="orderExpression"></param>
    /// <param name="condition">If false, the expression won't be added. The whole Order chain will be discarded.</param>
    public static IOrderedBasicSpecificationBuilder<T> OrderBy<T>(this IBasicSpecificationBuilder<T> specificationBuilder,
        Expression<Func<T, object?>> orderExpression, bool condition) where T : class
    {
        if (condition)
        {
            specificationBuilder.Specification.OrderExpressions ??= new List<OrderExpressionInfo<T>>();
            ((List<OrderExpressionInfo<T>>)specificationBuilder.Specification.OrderExpressions).Add(
                new OrderExpressionInfo<T>(orderExpression, OrderType.OrderBy));
        }

        var orderedSpecificationBuilder =
            new OrderedBasicSpecificationBuilder<T>(specificationBuilder.Specification, !condition);

        return orderedSpecificationBuilder;
    }

    /// <summary>
    /// Specify the query result will be ordered by <paramref name="orderExpression"/> in a descending order
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="specificationBuilder"></param>
    /// <param name="orderExpression"></param>
    public static IOrderedBasicSpecificationBuilder<T> OrderByDescending<T>(
        this IBasicSpecificationBuilder<T> specificationBuilder, Expression<Func<T, object?>> orderExpression) where T : class =>
        OrderByDescending(specificationBuilder, orderExpression, true);

    /// <summary>
    /// Specify the query result will be ordered by <paramref name="orderExpression"/> in a descending order
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="specificationBuilder"></param>
    /// <param name="orderExpression"></param>
    /// <param name="condition">If false, the expression won't be added. The whole Order chain will be discarded.</param>
    public static IOrderedBasicSpecificationBuilder<T> OrderByDescending<T>(
        this IBasicSpecificationBuilder<T> specificationBuilder, Expression<Func<T, object?>> orderExpression,
        bool condition) where T : class
    {
        if (condition)
        {
            specificationBuilder.Specification.OrderExpressions ??= new List<OrderExpressionInfo<T>>();
            ((List<OrderExpressionInfo<T>>)specificationBuilder.Specification.OrderExpressions).Add(
                new OrderExpressionInfo<T>(orderExpression, OrderType.OrderByDescending));
        }

        var orderedSpecificationBuilder =
            new OrderedBasicSpecificationBuilder<T>(specificationBuilder.Specification, !condition);

        return orderedSpecificationBuilder;
    }

    /// <summary>
    /// Specify the query result will be grouped by <paramref name="groupByExpression"/> in a descending order
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="specificationBuilder"></param>
    /// <param name="groupByExpression"></param>
    public static IGroupedBasicSpecificationBuilder<T> GroupBy<T>(
        this IBasicSpecificationBuilder<T> specificationBuilder,
        Expression<Func<T, object>> groupByExpression) where T : class
    {
        specificationBuilder.Specification.GroupByExpression = groupByExpression;
        
        var groupedBuilder = new GroupedBasicSpecificationBuilder<T>(specificationBuilder.Specification);

        return groupedBuilder;
    }

    /// <summary>
    /// Specify a 'SQL LIKE' operations for search purposes
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="specificationBuilder"></param>
    /// <param name="selector">the property to apply the SQL LIKE against</param>
    /// <param name="searchTerm">the value to use for the SQL LIKE</param>
    /// <param name="searchGroup">the index used to group sets of Selectors and SearchTerms together</param>
    public static IBasicSpecificationBuilder<T> Search<T>(
        this IBasicSpecificationBuilder<T> specificationBuilder,
        Expression<Func<T, string>> selector,
        string searchTerm,
        int searchGroup = 1) where T : class
        => Search(specificationBuilder, selector, searchTerm, true, searchGroup);

    /// <summary>
    /// Specify a 'SQL LIKE' operations for search purposes
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="specificationBuilder"></param>
    /// <param name="selector">the property to apply the SQL LIKE against</param>
    /// <param name="searchTerm">the value to use for the SQL LIKE</param>
    /// <param name="condition">If false, the expression won't be added.</param>
    /// <param name="searchGroup">the index used to group sets of Selectors and SearchTerms together</param>
    public static IBasicSpecificationBuilder<T> Search<T>(
        this IBasicSpecificationBuilder<T> specificationBuilder,
        Expression<Func<T, string>> selector,
        string searchTerm,
        bool condition,
        int searchGroup = 1) where T : class
    {
        if (condition)
        {
            specificationBuilder.Specification.SearchCriterias ??= new List<SearchExpressionInfo<T>>();
            ((List<SearchExpressionInfo<T>>)specificationBuilder.Specification.SearchCriterias).Add(new SearchExpressionInfo<T>(selector, searchTerm, searchGroup));
        }

        return specificationBuilder;
    }
    
    /// <summary>
    /// Specify a 'SQL LIKE' operations for search purposes
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="specificationBuilder"></param>
    /// <param name="selector">the property to apply the SQL LIKE against</param>
    /// <param name="searchTerm">the value to use for the SQL LIKE</param>
    /// <param name="searchGroup">the index used to group sets of Selectors and SearchTerms together</param>
    public static ISpecificationBuilder<T> Search<T>(
        this ISpecificationBuilder<T> specificationBuilder,
        Expression<Func<T, string>> selector,
        string searchTerm,
        int searchGroup = 1) where T : class
        => Search(specificationBuilder, selector, searchTerm, true, searchGroup);

    /// <summary>
    /// Specify a 'SQL LIKE' operations for search purposes
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="specificationBuilder"></param>
    /// <param name="selector">the property to apply the SQL LIKE against</param>
    /// <param name="searchTerm">the value to use for the SQL LIKE</param>
    /// <param name="condition">If false, the expression won't be added.</param>
    /// <param name="searchGroup">the index used to group sets of Selectors and SearchTerms together</param>
    public static ISpecificationBuilder<T> Search<T>(
        this ISpecificationBuilder<T> specificationBuilder,
        Expression<Func<T, string>> selector,
        string searchTerm,
        bool condition,
        int searchGroup = 1) where T : class
    {
        if (condition)
        {
            specificationBuilder.Specification.SearchCriterias ??= new List<SearchExpressionInfo<T>>();
            ((List<SearchExpressionInfo<T>>)specificationBuilder.Specification.SearchCriterias).Add(new SearchExpressionInfo<T>(selector, searchTerm, searchGroup));
        }

        return specificationBuilder;
    }
    
    /// <summary>
    /// Expands given member.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="specificationBuilder">This builder instance</param>
    /// <param name="expression">Member to expand</param>
    /// <returns>Current builder instance</returns>
    public static ISpecificationBuilder<T, TResult> Expand<T, TResult>(
        this ISpecificationBuilder<T, TResult> specificationBuilder, Expression<Func<TResult, object>> expression) where T : class

    {
        specificationBuilder.Specification.MembersToExpand ??= new List<Expression<Func<TResult, object>>>();
        ((List<Expression<Func<TResult, object>>>)specificationBuilder.Specification.MembersToExpand).Add(expression);

        return specificationBuilder;
    }

    public static ISpecificationBuilder<T, TResult> Expand<T, TResult>(
        this ISpecificationBuilder<T, TResult> specificationBuilder, string member) where T : class
    {
        specificationBuilder.Specification.StringMembersToExpand ??= new List<string>();
        ((List<string>)specificationBuilder.Specification.StringMembersToExpand).Add(member);

        return specificationBuilder;
    }

    /// <summary>
    /// Specify the number of elements to return.
    /// </summary>
    /// <param name="specificationBuilder"></param>
    /// <param name="take">number of elements to take</param>
    public static ISpecificationBuilder<T> Take<T>(
        this ISpecificationBuilder<T> specificationBuilder,
        int take) where T : class => Take(specificationBuilder, take, true);

    /// <summary>
    /// Specify the number of elements to return.
    /// </summary>
    /// <param name="specificationBuilder"></param>
    /// <param name="take">number of elements to take</param>
    /// <param name="condition">If false, the value will be discarded.</param>
    public static ISpecificationBuilder<T> Take<T>(
        this ISpecificationBuilder<T> specificationBuilder,
        int take,
        bool condition) where T : class
    {
        if (condition)
        {
            if (specificationBuilder.Specification.Take != null) throw new DuplicateTakeException();

            specificationBuilder.Specification.Take = take;
            specificationBuilder.Specification.IsPagingEnabled = true;
        }

        return specificationBuilder;
    }

    /// <summary>
    /// Specify the number of elements to skip before returning the remaining elements.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="specificationBuilder"></param>
    /// <param name="skip">number of elements to skip</param>
    public static ISpecificationBuilder<T> Skip<T>(
        this ISpecificationBuilder<T> specificationBuilder,
        int skip) where T : class => Skip(specificationBuilder, skip, true);

    /// <summary>
    /// Specify the number of elements to skip before returning the remaining elements.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="specificationBuilder"></param>
    /// <param name="skip">number of elements to skip</param>
    /// <param name="condition">If false, the value will be discarded.</param>
    public static ISpecificationBuilder<T> Skip<T>(
        this ISpecificationBuilder<T> specificationBuilder,
        int skip,
        bool condition) where T : class
    {
        if (condition)
        {
            if (specificationBuilder.Specification.Skip != null) throw new DuplicateSkipException();

            specificationBuilder.Specification.Skip = skip;
            specificationBuilder.Specification.IsPagingEnabled = true;
        }

        return specificationBuilder;
    }

    /// <summary>
    /// Specify a <see cref="PaginationFilter"/> to use
    /// </summary>
    /// <param name="specificationBuilder"></param>
    /// <param name="paginationFilter"><see cref="PaginationFilter"/> to use</param>
    /// <returns>Current <see cref="ISpecificationBuilder{T,TResult}"/> instance</returns>
    public static ISpecificationBuilder<T> WithPaginationFilter<T>(
        this ISpecificationBuilder<T> specificationBuilder,
        PaginationFilter paginationFilter) where T : class
    {
        if (specificationBuilder.Specification.Skip is not null) throw new DuplicateSkipException();
        if (specificationBuilder.Specification.Take is not null) throw new DuplicateTakeException();
        if (specificationBuilder.Specification.PaginationFilter is not null) throw new DuplicatePaginationException();
        if (paginationFilter is null) throw new ArgumentNullException(nameof(paginationFilter));

        specificationBuilder.Specification.PaginationFilter = paginationFilter;

        return specificationBuilder;
    }
    
    /// <summary>
    /// Specify the number of elements to return.
    /// </summary>
    /// <param name="specificationBuilder"></param>
    /// <param name="take">number of elements to take</param>
    public static IBasicSpecificationBuilder<T> Take<T>(
        this IBasicSpecificationBuilder<T> specificationBuilder,
        int take) where T : class => Take(specificationBuilder, take, true);

    /// <summary>
    /// Specify the number of elements to return.
    /// </summary>
    /// <param name="specificationBuilder"></param>
    /// <param name="take">number of elements to take</param>
    /// <param name="condition">If false, the value will be discarded.</param>
    public static IBasicSpecificationBuilder<T> Take<T>(
        this IBasicSpecificationBuilder<T> specificationBuilder,
        int take,
        bool condition) where T : class
    {
        if (condition)
        {
            if (specificationBuilder.Specification.Take != null) throw new DuplicateTakeException();

            specificationBuilder.Specification.Take = take;
            specificationBuilder.Specification.IsPagingEnabled = true;
        }

        return specificationBuilder;
    }

    /// <summary>
    /// Specify the number of elements to skip before returning the remaining elements.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="specificationBuilder"></param>
    /// <param name="skip">number of elements to skip</param>
    public static IBasicSpecificationBuilder<T> Skip<T>(
        this IBasicSpecificationBuilder<T> specificationBuilder,
        int skip) where T : class => Skip(specificationBuilder, skip, true);

    /// <summary>
    /// Specify the number of elements to skip before returning the remaining elements.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="specificationBuilder"></param>
    /// <param name="skip">number of elements to skip</param>
    /// <param name="condition">If false, the value will be discarded.</param>
    public static IBasicSpecificationBuilder<T> Skip<T>(
        this IBasicSpecificationBuilder<T> specificationBuilder,
        int skip,
        bool condition) where T : class
    {
        if (condition)
        {
            if (specificationBuilder.Specification.Skip != null) throw new DuplicateSkipException();

            specificationBuilder.Specification.Skip = skip;
            specificationBuilder.Specification.IsPagingEnabled = true;
        }

        return specificationBuilder;
    }

    /// <summary>
    /// Specify a <see cref="PaginationFilter"/> to use
    /// </summary>
    /// <param name="specificationBuilder"></param>
    /// <param name="paginationFilter"><see cref="PaginationFilter"/> to use</param>
    /// <returns>Current <see cref="ISpecificationBuilder{T,TResult}"/> instance</returns>
    public static IBasicSpecificationBuilder<T> WithPaginationFilter<T>(
        this IBasicSpecificationBuilder<T> specificationBuilder,
        PaginationFilter paginationFilter) where T : class
    {
        if (specificationBuilder.Specification.Skip is not null) throw new DuplicateSkipException();
        if (specificationBuilder.Specification.Take is not null) throw new DuplicateTakeException();
        if (specificationBuilder.Specification.PaginationFilter is not null) throw new DuplicatePaginationException();
        if (paginationFilter is null) throw new ArgumentNullException(nameof(paginationFilter));

        specificationBuilder.Specification.PaginationFilter = paginationFilter;

        return specificationBuilder;
    }

    /// <summary>
    /// Specify a transform function to apply to the result of the query 
    /// and returns the same <typeparamref name="T"/> type
    /// </summary>
    public static ISpecificationBuilder<T> WithPostProcessingAction<T>(
        this ISpecificationBuilder<T> specificationBuilder,
        Func<IEnumerable<T>, IEnumerable<T>> predicate) where T : class
    {
        specificationBuilder.Specification.PostProcessingAction = predicate;

        return specificationBuilder;
    }

    /// <summary>
    /// Specify a transform function to apply to the result of the query.
    /// and returns another <typeparamref name="TResult"/> type
    /// </summary>
    public static ISpecificationBuilder<T, TResult> WithPostProcessingAction<T, TResult>(
        this ISpecificationBuilder<T, TResult> specificationBuilder,
        Func<IEnumerable<TResult>, IEnumerable<TResult>> predicate) where T : class
    {
        specificationBuilder.Specification.PostProcessingAction = predicate;

        return specificationBuilder;
    }


    /// <summary>
    /// Specify whether results should be cached using <see cref="SecondLevelCacheInterceptor"/>.
    /// </summary>
    /// <param name="specificationBuilder"></param>
    /// <param name="withCaching">Whether to cache results</param>
    /// <returns><see cref="ICacheSpecificationBuilder{T}"/> instance</returns>
    public static ICacheSpecificationBuilder<T> WithCaching<T>(
        this ISpecificationBuilder<T> specificationBuilder, bool withCaching = true) where T : class
    {
        specificationBuilder.Specification.IsCacheEnabled = withCaching;

        return new CacheSpecificationBuilder<T>(specificationBuilder.Specification);
    }

    /// <summary>
    /// If the entity instances are modified, this will be detected
    /// by the change tracker.
    /// </summary>
    /// <param name="specificationBuilder"></param>
    public static ISpecificationBuilder<T> AsTracking<T>(
        this ISpecificationBuilder<T> specificationBuilder) where T : class
        => AsTracking(specificationBuilder, true);

    /// <summary>
    /// If the entity instances are modified, this will be detected
    /// by the change tracker.
    /// </summary>
    /// <param name="specificationBuilder"></param>
    /// <param name="condition">If false, the setting will be discarded.</param>
    public static ISpecificationBuilder<T> AsTracking<T>(
        this ISpecificationBuilder<T> specificationBuilder,
        bool condition) where T : class
    {
        if (condition)
        {
            specificationBuilder.Specification.IsAsNoTracking = false;
            specificationBuilder.Specification.IsAsNoTrackingWithIdentityResolution = false;
            specificationBuilder.Specification.IsAsTracking = true;
        }

        return specificationBuilder;
    }
    
    /// <summary>
    /// If the entity instances are modified, this will not be detected
    /// by the change tracker.
    /// </summary>
    /// <param name="specificationBuilder"></param>
    public static ISpecificationBuilder<T> AsNoTracking<T>(
        this ISpecificationBuilder<T> specificationBuilder) where T : class
        => AsNoTracking(specificationBuilder, true);

    /// <summary>
    /// If the entity instances are modified, this will not be detected
    /// by the change tracker.
    /// </summary>
    /// <param name="specificationBuilder"></param>
    /// <param name="condition">If false, the setting will be discarded.</param>
    public static ISpecificationBuilder<T> AsNoTracking<T>(
        this ISpecificationBuilder<T> specificationBuilder,
        bool condition) where T : class
    {
        if (condition)
        {
            specificationBuilder.Specification.IsAsTracking = false;
            specificationBuilder.Specification.IsAsNoTrackingWithIdentityResolution = false;
            specificationBuilder.Specification.IsAsNoTracking = true;
        }

        return specificationBuilder;
    }
    
    /// <summary>
    /// The query will then keep track of returned instances 
    /// (without tracking them in the normal way) 
    /// and ensure no duplicates are created in the query results
    /// </summary>
    /// <remarks>
    /// for more info: https://docs.microsoft.com/en-us/ef/core/change-tracking/identity-resolution#identity-resolution-and-queries
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    /// <param name="specificationBuilder"></param>
    public static ISpecificationBuilder<T> AsNoTrackingWithIdentityResolution<T>(
        this ISpecificationBuilder<T> specificationBuilder) where T : class
        => AsNoTrackingWithIdentityResolution(specificationBuilder, true);

    /// <summary>
    /// The query will then keep track of returned instances 
    /// (without tracking them in the normal way) 
    /// and ensure no duplicates are created in the query results
    /// </summary>
    /// <remarks>
    /// for more info: https://docs.microsoft.com/en-us/ef/core/change-tracking/identity-resolution#identity-resolution-and-queries
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    /// <param name="specificationBuilder"></param>
    /// <param name="condition">If false, the setting will be discarded.</param>
    public static ISpecificationBuilder<T> AsNoTrackingWithIdentityResolution<T>(
        this ISpecificationBuilder<T> specificationBuilder,
        bool condition) where T : class
    {
        if (condition)
        {
            specificationBuilder.Specification.IsAsTracking = false;
            specificationBuilder.Specification.IsAsNoTracking = false;
            specificationBuilder.Specification.IsAsNoTrackingWithIdentityResolution = true;
        }

        return specificationBuilder;
    }

    /// <summary>
    /// Specify whether to use tracking use split query
    /// </summary>
    /// <returns><see cref="ISpecificationBuilder{T}"/> instance</returns>
    public static ISpecificationBuilder<T> AsSplitQuery<T>(
        this ISpecificationBuilder<T> specificationBuilder) where T : class
    {
        specificationBuilder.Specification.IsAsSplitQuery = true;

        return specificationBuilder;
    }

    /// <summary>
    /// Specify a transform function to apply to the <typeparamref name="T"/> element 
    /// to produce another <typeparamref name="TResult"/> element.
    /// </summary>
    public static ISpecificationBuilder<T, TResult> Select<T, TResult>(
        this ISpecificationBuilder<T, TResult> specificationBuilder,
        Expression<Func<T, TResult>> selector) where T : class
    {
        specificationBuilder.Specification.Selector = selector;

        return specificationBuilder;
    }

    /// <summary>
    /// The query will ignore the defined global query filters
    /// </summary>
    /// <remarks>
    /// for more info: https://docs.microsoft.com/en-us/ef/core/querying/filters
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    /// <param name="specificationBuilder"></param>
    public static ISpecificationBuilder<T> IgnoreQueryFilters<T>(
        this ISpecificationBuilder<T> specificationBuilder) where T : class
        => IgnoreQueryFilters(specificationBuilder, true);

    /// <summary>
    /// The query will ignore the defined global query filters
    /// </summary>
    /// <remarks>
    /// for more info: https://docs.microsoft.com/en-us/ef/core/querying/filters
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    /// <param name="specificationBuilder"></param>
    /// <param name="condition">If false, the setting will be discarded.</param>
    public static ISpecificationBuilder<T> IgnoreQueryFilters<T>(
        this ISpecificationBuilder<T> specificationBuilder,
        bool condition) where T : class
    {
        if (condition)
        {
            specificationBuilder.Specification.IgnoreQueryFilters = true;
        }

        return specificationBuilder;
    }
}
