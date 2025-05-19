using Microsoft.EntityFrameworkCore;
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
            List<FilterParams> AllFilterParams = new List<FilterParams>();
            foreach (var filterParam in query.FilterParams)
            {
                if (filterParam != null)
                {
                    if (filterParam.Option == FilterParams.Options.orEqual.ToString())
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
                        if (DateTime.TryParse(filterParam.Value.ToString(), out var dtmValue) && filterParam.ValueType == string.Empty)
                        {
                            if (filterParam.Option.ToLower() == FilterParams.Options.dateIs.ToString().ToLower()) queries = queries.Where(e => EF.Property<DateTime>(e, filterParam.Key).Equals(dtmValue));
                            if (filterParam.Option.ToLower() == FilterParams.Options.dateIsNot.ToString().ToLower()) queries = queries.Where(e => EF.Property<DateTime>(e, filterParam.Key) != dtmValue);
                            if (filterParam.Option.ToLower() == FilterParams.Options.dateIsBefore.ToString().ToLower()) queries = queries.Where(e => EF.Property<DateTime>(e, filterParam.Key) > dtmValue);
                            if (filterParam.Option.ToLower() == FilterParams.Options.dateIsAfter.ToString().ToLower()) queries = queries.Where(e => EF.Property<DateTime>(e, filterParam.Key) < dtmValue);

                            if (filterParam.Option.ToLower() == FilterParams.Options.equals.ToString().ToLower()) queries = queries.Where(e => EF.Property<DateTime>(e, filterParam.Key).Equals(dtmValue));
                            if (filterParam.Option.ToLower() == FilterParams.Options.min.ToString().ToLower()) queries = queries = queries.Where(e => EF.Property<DateTime>(e, filterParam.Key) > dtmValue);
                            if (filterParam.Option.ToLower() == FilterParams.Options.minEqual.ToString().ToLower()) queries = queries = queries.Where(e => EF.Property<DateTime>(e, filterParam.Key) >= dtmValue);
                            if (filterParam.Option.ToLower() == FilterParams.Options.max.ToString().ToLower()) queries = queries = queries.Where(e => EF.Property<DateTime>(e, filterParam.Key) < dtmValue);
                            if (filterParam.Option.ToLower() == FilterParams.Options.maxEqual.ToString().ToLower()) queries = queries = queries.Where(e => EF.Property<DateTime>(e, filterParam.Key) <= dtmValue);
                        }
                        else if (double.TryParse(filterParam.Value.ToString(), out var doubleValue) && filterParam.ValueType == string.Empty)
                        {
                            if (filterParam.Option.ToLower() == FilterParams.Options.equals.ToString().ToLower()) queries = queries.Where(e => EF.Property<double>(e, filterParam.Key).Equals(doubleValue));
                            if (filterParam.Option.ToLower() == FilterParams.Options.notEquals.ToString().ToLower()) queries = queries.Where(e => EF.Property<double>(e, filterParam.Key) != doubleValue);
                            if (filterParam.Option.ToLower() == FilterParams.Options.lt.ToString().ToLower()) queries = queries.Where(e => EF.Property<double>(e, filterParam.Key) < doubleValue);
                            if (filterParam.Option.ToLower() == FilterParams.Options.lte.ToString().ToLower()) queries = queries.Where(e => EF.Property<double>(e, filterParam.Key) <= doubleValue);
                            if (filterParam.Option.ToLower() == FilterParams.Options.gt.ToString().ToLower()) queries = queries.Where(e => EF.Property<double>(e, filterParam.Key) > doubleValue);
                            if (filterParam.Option.ToLower() == FilterParams.Options.gte.ToString().ToLower()) queries = queries.Where(e => EF.Property<double>(e, filterParam.Key) >= doubleValue);


                            if (filterParam.Option.ToLower() == FilterParams.Options.min.ToString().ToLower()) queries = queries.Where(e => EF.Property<double>(e, filterParam.Key) > doubleValue);
                            if (filterParam.Option.ToLower() == FilterParams.Options.minEqual.ToString().ToLower()) queries = queries.Where(e => EF.Property<double>(e, filterParam.Key) >= doubleValue);
                            if (filterParam.Option.ToLower() == FilterParams.Options.max.ToString().ToLower()) queries = queries.Where(e => EF.Property<double>(e, filterParam.Key) < doubleValue);
                            if (filterParam.Option.ToLower() == FilterParams.Options.maxEqual.ToString().ToLower()) queries = queries.Where(e => EF.Property<double>(e, filterParam.Key) <= doubleValue);
                        }
                        else if (int.TryParse(filterParam.Value.ToString(), out var intValue) && filterParam.ValueType == string.Empty)
                        {
                            if (filterParam.Option.ToLower() == FilterParams.Options.equals.ToString().ToLower()) queries = queries.Where(e => EF.Property<double>(e, filterParam.Key).Equals(intValue));
                            if (filterParam.Option.ToLower() == FilterParams.Options.notEquals.ToString().ToLower()) queries = queries.Where(e => EF.Property<double>(e, filterParam.Key) != intValue);
                            if (filterParam.Option.ToLower() == FilterParams.Options.lt.ToString().ToLower()) queries = queries.Where(e => EF.Property<double>(e, filterParam.Key) < intValue);
                            if (filterParam.Option.ToLower() == FilterParams.Options.lte.ToString().ToLower()) queries = queries.Where(e => EF.Property<double>(e, filterParam.Key) <= intValue);
                            if (filterParam.Option.ToLower() == FilterParams.Options.gt.ToString().ToLower()) queries = queries.Where(e => EF.Property<double>(e, filterParam.Key) > intValue);
                            if (filterParam.Option.ToLower() == FilterParams.Options.gte.ToString().ToLower()) queries = queries.Where(e => EF.Property<double>(e, filterParam.Key) >= intValue);


                            if (filterParam.Option.ToLower() == FilterParams.Options.equals.ToString().ToLower()) queries = queries.Where(e => EF.Property<int>(e, filterParam.Key).Equals(intValue));
                            if (filterParam.Option.ToLower() == FilterParams.Options.min.ToString().ToLower()) queries = queries.Where(e => EF.Property<int>(e, filterParam.Key) > intValue);
                            if (filterParam.Option.ToLower() == FilterParams.Options.minEqual.ToString().ToLower()) queries = queries.Where(e => EF.Property<int>(e, filterParam.Key) >= intValue);
                            if (filterParam.Option.ToLower() == FilterParams.Options.max.ToString().ToLower()) queries = queries.Where(e => EF.Property<int>(e, filterParam.Key) < intValue);
                            if (filterParam.Option.ToLower() == FilterParams.Options.maxEqual.ToString().ToLower()) queries = queries.Where(e => EF.Property<int>(e, filterParam.Key) <= intValue);
                        }
                        else if (bool.TryParse(filterParam.Value.ToString(), out var bolValue) && filterParam.ValueType == string.Empty)
                        {
                            if (filterParam.Option.ToLower() == FilterParams.Options.equals.ToString().ToLower()) queries = queries.Where(e => EF.Property<bool>(e, filterParam.Key).Equals(bolValue));
                            if (filterParam.Option.ToLower() == FilterParams.Options.notEquals.ToString().ToLower()) queries = queries.Where(e => EF.Property<bool>(e, filterParam.Key) != bolValue);
                        }
                        else
                        {
                            if (filterParam.Option.ToLower() == FilterParams.Options.equals.ToString().ToLower()) queries = queries.Where(e => EF.Property<string>(e, filterParam.Key).Equals((string)filterParam.Value));
                            if (filterParam.Option.ToLower() == FilterParams.Options.notEquals.ToString().ToLower()) queries = queries.Where(e => EF.Property<string>(e, filterParam.Key) != (string)filterParam.Value);
                            if (filterParam.Option.ToLower() == FilterParams.Options.contains.ToString().ToLower()) queries = queries.Where(e => EF.Property<string>(e, filterParam.Key).Contains((string)filterParam.Value));
                            if (filterParam.Option.ToLower() == FilterParams.Options.notContains.ToString().ToLower()) queries = queries.Where(e => !EF.Property<string>(e, filterParam.Key).Contains((string)filterParam.Value));


                            if (filterParam.Option.ToLower() == FilterParams.Options.startsWith.ToString().ToLower()) queries = queries.Where(e => EF.Property<string>(e, filterParam.Key).StartsWith((string)filterParam.Value));
                            if (filterParam.Option.ToLower() == FilterParams.Options.endsWith.ToString().ToLower()) queries = queries.Where(e => EF.Property<string>(e, filterParam.Key).EndsWith((string)filterParam.Value));
                        }
                    }
                }
            }

            // Apply Sorting
            string allSortBy = string.Join(",", query.SortParams.Select(sort =>
            {
                string sortDirection = sort?.Option == SortParams.Options.ASC.ToString() ? "ascending" : "descending";
                return $"{sort?.Column} {sortDirection}";
            }));
            if (allSortBy != "")
            {
                queries = queries.OrderBy(allSortBy);
            }

            return queries;
        }

    }
}
