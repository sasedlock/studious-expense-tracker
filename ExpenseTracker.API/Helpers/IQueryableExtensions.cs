using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;

namespace ExpenseTracker.API.Helpers
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> ApplySort<T>(this IQueryable<T> source, string sort)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (sort == null)
            {
                return source;
            }

            var sortList = sort.Split(',');

            string completeSortExpression = "";
            foreach (var sortOption in sortList)
            {
                if (sortOption.StartsWith("-"))
                {
                    completeSortExpression += completeSortExpression + sortOption.Remove(0, 1) + " descending,";
                }
                else
                {
                    completeSortExpression += completeSortExpression + sortOption + ",";
                }
            }

            if (!string.IsNullOrWhiteSpace(completeSortExpression))
            {
                source = source.OrderBy(completeSortExpression.Remove(completeSortExpression.Count() - 1));
            }

            return source;
        }
    }
}