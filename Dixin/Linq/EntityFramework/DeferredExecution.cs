﻿namespace Dixin.Linq.EntityFramework
{
    using System.Data.Entity;
    using System.Diagnostics;
    using System.Linq;

    internal static class DataAccess
    {
        internal static IQueryable<Product> QueryCategoryProducts(string category)
        {
            using (AdventureWorks adventureWorks = new AdventureWorks())
            {
                return adventureWorks.Products.Where(
                    product => product.ProductSubcategory.ProductCategory.Name == category);
            }
        }
    }

    internal static class UI
    {
        internal static void ViewCategoryProducts(string category) => DataAccess
            .QueryCategoryProducts(category)
            .Select(product => product.Name)
            .ForEach(name => Trace.WriteLine(name));
        // InvalidOperationException: The operation cannot be completed because the DbContext has been disposed.
    }

    internal static partial class QueryMethods
    {
        internal static void LazyLoading()
        {
            IQueryable<ProductSubcategory> subcategories = AdventureWorks.ProductSubcategories;
            subcategories.ForEach(subcategory => Trace.WriteLine(
                $@"{subcategory.ProductCategory?.Name}/{subcategory.Name}: {string.Join(
                    ", ", subcategory.Products.Select(product => product.Name))}"));
            // EntityCommandExecutionException: There is already an open DataReader associated with this Command which must be closed first.
        }

        internal static void LazyLoadingWithToArray()
        {
            IQueryable<ProductSubcategory> subcategories = AdventureWorks.ProductSubcategories;
            subcategories.ToArray().ForEach(subcategory => Trace.WriteLine(
                $@"{subcategory.ProductCategory?.Name}/{subcategory.Name}: {string.Join(
                    ", ", subcategory.Products.Select(product => product.Name))}"));
        }

        internal static void EagerLoadingWithSelect()
        {
            var subcategories = AdventureWorks.ProductSubcategories
                .Select(subcategory => new
                {
                    Category = subcategory.ProductCategory.Name,
                    Subcategory = subcategory.Name,
                    Products = subcategory.Products.Select(product => product.Name)
                });
            subcategories.ForEach(subcategory => Trace.WriteLine(
                $"{subcategory.Category}/{subcategory}: {string.Join(", ", subcategory.Products)}"));
        }

        internal static void EagerLoadingWithAssociation()
        {
            using (AdventureWorks adventureWorks = new AdventureWorks())
            {
                IQueryable<ProductSubcategory> subcategories = adventureWorks.ProductSubcategories
                    .Include(subcategory => subcategory.ProductCategory)
                    .Include(subcategory => subcategory.Products);
                subcategories.ForEach(subcategory => Trace.WriteLine(
                    $@"{subcategory.ProductCategory?.Name}/{subcategory.Name}: {string.Join(
                        ", ", subcategory.Products.Select(product => product.Name))}"));
            }
        }

        internal static void ConditionalEagerLoading()
        {
            using (AdventureWorks adventureWorks = new AdventureWorks())
            {
                IQueryable<ProductSubcategory> subcategories = adventureWorks.ProductSubcategories
                    .Include(subcategory => subcategory.Products.Where(product => product.ListPrice > 0));
                subcategories.ForEach(subcategory => Trace.WriteLine(
                    $@"{subcategory.Name}: {string.Join(
                        ", ", subcategory.Products.Select(product => product.Name))}"));
                // ArgumentException: The Include path expression must refer to a navigation property defined on the type. Use dotted paths for reference navigation properties and the Select operator for collection navigation properties.
            }
        }

        internal static void ConditionalEagerLoadingWithSelect()
        {
            using (AdventureWorks adventureWorks = new AdventureWorks())
            {
                var subcategories = adventureWorks.ProductSubcategories
                    .Select(subcategory => new
                        {
                            Subcategory = subcategory,
                            Products = subcategory.Products.Where(product => product.ListPrice > 0)
                        });
                subcategories.ForEach(subcategory => Trace.WriteLine(
                    $@"{subcategory.Subcategory.Name}: {string.Join(
                        ", ", subcategory.Products.Select(product => product.Name))}"));
                // ArgumentException: The Include path expression must refer to a navigation property defined on the type. Use dotted paths for reference navigation properties and the Select operator for collection navigation properties.
            }
        }

        internal static void NoLoading()
        {
            using (AdventureWorks adventureWorks = new AdventureWorks())
            {
                adventureWorks.Configuration.LazyLoadingEnabled = false; // Default: true.
                IQueryable<ProductSubcategory> subcategories = AdventureWorks.ProductSubcategories;
                subcategories.ForEach(subcategory => Trace.WriteLine(
                    $@"{subcategory.ProductCategory?.Name}/{subcategory.Name}: {string.Join(
                        ", ", subcategory.Products.Select(product => product.Name))}"));
            }
        }
    }
}