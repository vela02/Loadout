using Market.Core.Entities.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Infrastructure.Database
{
    public partial class DatabaseContext
    {
        private void ModifyTimestamps()
        {
            var entries = ChangeTracker.Entries();

            foreach (var entry in entries)
            {
                var entity = ((BaseEntity)entry.Entity);

                if (entry.State == EntityState.Added)
                {
                    entity.CreatedAt = DateTime.Now;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entity.ModifiedAt = DateTime.Now;
                }
            }
        }

        public override int SaveChanges()
        {
            ModifyTimestamps();

            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            ModifyTimestamps();

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}