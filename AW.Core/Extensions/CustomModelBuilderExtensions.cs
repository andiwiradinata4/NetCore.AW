using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AW.Core.Extensions
{
    public static class CustomModelBuilderExtensions
    {
        public static void InitContext(this ModelBuilder modelBuilder, string defaultSchema = "dbo")
        {
            /// Set Default Schema
            modelBuilder.HasDefaultSchema(defaultSchema);

            /// Set IsConcurrencyToken 
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var rowVersionProperty = entityType.GetProperties().FirstOrDefault(e => e.Name == "RowVersion");
                if (rowVersionProperty != null && rowVersionProperty.ClrType == typeof(byte[])) modelBuilder.Entity(entityType.ClrType).Property("RowVersion").IsRowVersion().IsConcurrencyToken();
            }
        }

        public static void ExcludeMigration<T>(this ModelBuilder modelBuilder) where T : class
        {
            modelBuilder.Entity<T>().ToTable(nameof(T), e => e.ExcludeFromMigrations());
        }

        public static void SetToView<T>(this ModelBuilder modelBuilder, string viewName) where T : class
        {
            modelBuilder.Entity<T>(e =>
            {
                e.HasNoKey();
                e.ToView(viewName);
            });

            modelBuilder.ExcludeMigration<T>();
        }

        public static void SetRelationship(this ModelBuilder modelBuilder, DeleteBehavior deleteBehavior)
        {
            modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()).ToList().ForEach(e => { e.DeleteBehavior = deleteBehavior; });
        }
    }
}
