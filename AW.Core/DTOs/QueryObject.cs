﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AW.Core.DTOs
{
    public class QueryObject
    {
        public int Page { get; set; } = -1;
        public int PageSize { get; set; } = int.MaxValue;
        public List<FilterParams> FilterParams { get; set; } = new List<FilterParams>();
        public List<SortParams> SortParams { get; set; } = new List<SortParams>();
        public string? Columns { get; set; }
        public string? Filter { get; set; }
        public string Includes { get; set; } = string.Empty;
    }

    public class FilterParams
    {
        public enum Options
        {
            contains,
            notContains,
            equals,
            notEquals,
            startsWith,
            endsWith,
            min,
            max,
            minEqual,
            maxEqual,
            orEqual,
            dateIs,
            dateIsNot,
            dateIsBefore,
            dateIsAfter,
            lt,
            lte,
            gt,
            gte
        };

        public string ValueType { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public string Option { get; set; } = string.Empty;
        public object Value { get; set; } = string.Empty;
    }

    public class SortParams
    {
        public enum Options
        {
            ASC,
            DESC
        }
        public string Column { get; set; } = string.Empty;
        public string Option { get; set; } = "ASC";
    }
}
