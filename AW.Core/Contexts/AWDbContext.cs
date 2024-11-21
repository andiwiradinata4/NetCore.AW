using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AW.Core.Entities;
using AW.Core.Extensions;

namespace AW.Core.Contexts
{
    public class AWDbContext : DbContext
    {
        public static bool ValidateDatabaseModel = false;
        public AWDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {
            if (ValidateDatabaseModel)
            {
                var pendingMigrations = this.Database.GetPendingMigrations().ToList();
                if (pendingMigrations.Any() || !this.Database.GetMigrations().Contains(this.Database.GetAppliedMigrations().ToList().Last()))
                {
                    throw new Exception("The Database Schema Changed.Please Check the migration version");

                }
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("dbo");
            base.OnModelCreating(modelBuilder);

            /// Set Default Model ForeignKey is Restrict
            modelBuilder.SetRelationship(DeleteBehavior.Restrict);

            /// Set IsConcurrencyToken 
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var rowVersionProperty = entityType.GetProperties().FirstOrDefault(e => e.Name == "RowVersion");
                if (rowVersionProperty != null && rowVersionProperty.ClrType == typeof(byte[])) modelBuilder.Entity(entityType.ClrType).Property("RowVersion").IsRowVersion().IsConcurrencyToken();
            }

            //modelBuilder.ExcludeMigration<BloodType>();
            //modelBuilder.SetToView<vwProduct>("E1AW_vwProduct");

        }
    }
}
