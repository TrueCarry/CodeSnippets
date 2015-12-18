﻿namespace Dixin.Linq.Fundamentals
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Runtime.CompilerServices;

    public static class CompiledLinqToObjects
    {
        [CompilerGenerated]
        private static Func<int, bool> cachedAnonymousMethodDelegate;

        [CompilerGenerated]
        private static bool Positive0(int value)
        {
            return value > 0;
        }

        public static IEnumerable<int> Positive(IEnumerable<int> source)
        {
            return Enumerable.Where(
                source,
                cachedAnonymousMethodDelegate ?? (cachedAnonymousMethodDelegate = Positive0));
        }
    }

    public static class CompiledLinqToSql
    {
        [CompilerGenerated]
        private sealed class Closure
        {
            public string categoryName;
        }

        public static string[] ProductNames(string categoryName)
        {
            Closure closure = new Closure { categoryName = categoryName };
            NorthwindDataContext database = new NorthwindDataContext();

            try
            {
                ParameterExpression product = Expression.Parameter(typeof(Product), "product");

                // Define query
                IQueryable<string> query = database.Products
                    .Where(
                        // product => product.Category.CategoryName == closure.categoryName
                        Expression.Lambda<Func<Product, bool>>(
                            Expression.Equal( // => product.Category.CategoryName == closure.categoryName
                                Expression.Property( // product.Category.CategoryName
                                    Expression.Property(product, "Category"), // product.Category
                                    "CategoryName"), // Category.CategoryName
                                Expression.Field( // Or Expression.Constant(categoryName) works too.
                                    Expression.Constant(closure), "categoryName"), // closure.categoryName
                                false,
                                typeof(string).GetMethod("op_Equals")), // ==
                            product)) // product =>
                    .Select(
                        Expression.Lambda<Func<Product, string>>( // product => product.ProductName
                            Expression.Property(product, "ProductName"), // => product.ProductName
                            product)); // product =>

                // Execute query.
                return query.ToArray();
            }
            finally
            {
                database.Dispose();
            }
        }
    }

    public static partial class LinqToObjects
    {
        public static IEnumerable<Person> Where
            (IEnumerable<Person> source) => source.Where((person, index) => person.Age >= 18 && index % 2 == 0);
    }

    public static partial class Int32Extensions
    {
        public static TResult Select<TResult>(this int value, Func<int, TResult> selector) => selector(value);
    }

    public static partial class Int32Extensions
    {
        public static void QueryExpression()
        {
            int query1 = from zero in default(int) // 0
                         select zero; // 0

            string query2 = from three in 1 + 2 // 3
                            select (three + 4).ToString(CultureInfo.InvariantCulture); // "7"
        }
    }

    public static partial class Int32Extensions
    {
        public static void QueryMethod()
        {
            int query1 = Int32Extensions.Select(default(int), zero => zero);

            string query2 = Int32Extensions.Select(
                1 + 2, three => (three + 4).ToString(CultureInfo.InvariantCulture)); // "7"
        }
    }

    public static partial class ObjectExtensions
    {
        public static TResult Select<TSource, TResult>(this TSource value, Func<TSource, TResult> selector) => selector(value);
    }

    public static partial class ObjectExtensions
    {
        public static void QueryExpression()
        {
            string query = from newGuild in Guid.NewGuid()
                           select newGuild.ToString();
        }

        public static void QueryMethod()
        {
            string query = ObjectExtensions.Select(Guid.NewGuid(), newGuild => newGuild.ToString());
        }
    }
}

#if ERROR
namespace System.Linq
{
    using System.Collections.Generic;

    public static class Enumerable
	{
        public static IEnumerable<TSource> Where<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate);

        public static IEnumerable<TSource> Where<TSource>(this IEnumerable<TSource> source, Func<TSource, int, bool> predicate);
	}
}
#endif