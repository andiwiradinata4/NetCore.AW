﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using AW.Core.DTOs;

namespace AW.Core.Extensions
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> IncludeProperties<T>(this IQueryable<T> query, params string[] properties) where T : class
        {
            foreach (var property in properties)
            {
                string[] strings = property.Split(',');
                foreach (string str in strings)
                {
                    query = query.Include(str);
                }
            }
            return query;
        }

        public static IQueryable<T> SetQuery<T>(this IQueryable<T> queries, QueryObject query) where T : class
        {
            // Apply Filtering
            //foreach (var filterParam in query.FilterParams)
            //List<FilterParams> AllFilterParams = JsonConvert.DeserializeObject<List<FilterParams>>(query.FilterParams) ?? new List<FilterParams>();

            List<FilterParams> AllFilterParams = new List<FilterParams>();
            string[] listFilterParams = query.FilterParams.Replace("[", "").Replace("]", "").Split("}");
            //foreach (string str in listFilterParams)
            //{
            //    FilterParams data = JsonConvert.DeserializeObject<FilterParams>(str) ?? new FilterParams();
            //    if (string.IsNullOrEmpty(str)) continue;
            //    if (string.IsNullOrEmpty(data.Key)) continue;
            //    AllFilterParams.Add(data);
            //}
            foreach (var filterParam in AllFilterParams)
            {
                if (filterParam != null)
                {
                    if (filterParam.Option == FilterParams.Options.orEqual)
                    {
                        var values = ((string)filterParam.Value).ToString().Split(",").Select(c => c.Trim()).ToList() ?? new List<string>();
                        var parameter = Expression.Parameter(typeof(T), "e");
                        var property = Expression.Property(parameter, filterParam.Key);
                        Expression? orExpression = null;

                        foreach (var value in values)
                        {
                            ConstantExpression constant;
                            if (DateTime.TryParse(value.ToString(), out var dtValue))
                            {
                                constant = Expression.Constant(dtValue);
                            }
                            else if (int.TryParse(value, out var iValue))
                            {
                                constant = Expression.Constant(iValue);
                            }
                            else if (double.TryParse(value, out var doValue))
                            {
                                constant = Expression.Constant(doValue);
                            }
                            else
                            {
                                constant = Expression.Constant(value);
                            }
                            var equality = Expression.Equal(property, constant);

                            orExpression = orExpression == null ? equality : Expression.OrElse(orExpression, equality);
                        }

                        if (orExpression != null)
                        {
                            var lamda = Expression.Lambda<Func<T, bool>>(orExpression, parameter);
                            queries = queries.Where(lamda);
                        }
                    }
                    else
                    {
                        if (DateTime.TryParse(filterParam.Value.ToString(), out var dtmValue))
                        {
                            if (filterParam.Option == FilterParams.Options.eq) queries = queries.Where(e => EF.Property<DateTime>(e, filterParam.Key).Equals(dtmValue));
                            if (filterParam.Option == FilterParams.Options.min) queries = queries = queries.Where(e => EF.Property<DateTime>(e, filterParam.Key) > dtmValue);
                            if (filterParam.Option == FilterParams.Options.minEqual) queries = queries = queries.Where(e => EF.Property<DateTime>(e, filterParam.Key) >= dtmValue);
                            if (filterParam.Option == FilterParams.Options.max) queries = queries = queries.Where(e => EF.Property<DateTime>(e, filterParam.Key) < dtmValue);
                            if (filterParam.Option == FilterParams.Options.maxEqual) queries = queries = queries.Where(e => EF.Property<DateTime>(e, filterParam.Key) <= dtmValue);
                        }
                        else if (double.TryParse(filterParam.Value.ToString(), out var doubleValue))
                        {
                            if (filterParam.Option == FilterParams.Options.eq) queries = queries.Where(e => EF.Property<double>(e, filterParam.Key).Equals(doubleValue));
                            if (filterParam.Option == FilterParams.Options.min) queries = queries.Where(e => EF.Property<double>(e, filterParam.Key) > doubleValue);
                            if (filterParam.Option == FilterParams.Options.minEqual) queries = queries.Where(e => EF.Property<double>(e, filterParam.Key) >= doubleValue);
                            if (filterParam.Option == FilterParams.Options.max) queries = queries.Where(e => EF.Property<double>(e, filterParam.Key) < doubleValue);
                            if (filterParam.Option == FilterParams.Options.maxEqual) queries = queries.Where(e => EF.Property<double>(e, filterParam.Key) <= doubleValue);
                        }
                        else if (int.TryParse(filterParam.Value.ToString(), out var intValue))
                        {
                            if (filterParam.Option == FilterParams.Options.eq) queries = queries.Where(e => EF.Property<int>(e, filterParam.Key).Equals(intValue));
                            if (filterParam.Option == FilterParams.Options.min) queries = queries.Where(e => EF.Property<int>(e, filterParam.Key) > intValue);
                            if (filterParam.Option == FilterParams.Options.minEqual) queries = queries.Where(e => EF.Property<int>(e, filterParam.Key) >= intValue);
                            if (filterParam.Option == FilterParams.Options.max) queries = queries.Where(e => EF.Property<int>(e, filterParam.Key) < intValue);
                            if (filterParam.Option == FilterParams.Options.maxEqual) queries = queries.Where(e => EF.Property<int>(e, filterParam.Key) <= intValue);
                        }
                        else if (bool.TryParse(filterParam.Value.ToString(), out var bolValue))
                        {
                            if (filterParam.Option == FilterParams.Options.eq) queries = queries.Where(e => EF.Property<bool>(e, filterParam.Key).Equals(bolValue));
                        }
                        else
                        {
                            if (filterParam.Option == FilterParams.Options.eq) queries = queries.Where(e => EF.Property<string>(e, filterParam.Key).Equals((string)filterParam.Value));
                            if (filterParam.Option == FilterParams.Options.contains) queries = queries.Where(e => EF.Property<string>(e, filterParam.Key).Contains((string)filterParam.Value));
                            if (filterParam.Option == FilterParams.Options.startWith) queries = queries.Where(e => EF.Property<string>(e, filterParam.Key).StartsWith((string)filterParam.Value));
                            if (filterParam.Option == FilterParams.Options.endtWith) queries = queries.Where(e => EF.Property<string>(e, filterParam.Key).EndsWith((string)filterParam.Value));
                        }
                    }
                }
            }

            // Apply Sorting
            //List<SortParams> AllSortParams = JsonConvert.DeserializeObject<List<SortParams>>(query.SortParams) ?? new List<SortParams>();
            //string allSortBy = string.Join(",", query.SortParams.Select(sort =>

            List<SortParams> AllSortParams = new List<SortParams>();
            string[] listSortParams = query.SortParams.Replace("[", "").Replace("]", "").Split("}");
            //foreach (string str in listFilterParams)
            //{
            //    SortParams data = JsonConvert.DeserializeObject<SortParams>(str) ?? new SortParams();
            //    if (string.IsNullOrEmpty(str)) continue;
            //    if (string.IsNullOrEmpty(data.Column)) continue;
            //    AllSortParams.Add(data);
            //}

            string allSortBy = string.Join(",", AllSortParams.Select(sort =>
            {
                string sortDirection = sort?.Option == SortParams.Options.ASC ? "ascending" : "descending";
                return $"{sort?.Column} {sortDirection}";
            }));
            if (allSortBy != "")
            {
                queries = queries.OrderBy(allSortBy);
            }

            // Apply Pagination
            if (query.Page > 0) queries = queries.Skip((query.Page - 1) * query.PageSize).Take(query.PageSize);

            // Apply Includes
            var allIncludes = query.Includes.Split(",", StringSplitOptions.RemoveEmptyEntries).Select(c => c.Trim()).ToList();
            allIncludes.ForEach(e =>
            {
                queries = queries.Include(e);
            });

            //var cols = query.Columns?.Split(",", StringSplitOptions.RemoveEmptyEntries);
            //var selector = Expression.Lambda<Func<T>>

            //if (query.Columns != null)
            //{
            //    string selectedColumns = "new(" + query.Columns + ")";
            //    IQueryable queryable = queries.Select(selectedColumns);

            //    IQueryable<T> values = (IQueryable<T>)queryable;
            //}

            return queries;
        }

    }
}