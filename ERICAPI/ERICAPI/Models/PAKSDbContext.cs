using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace ERICAPI.Models
{
    /// <summary>
    /// DBContext
    /// </summary>
    public class PAKSDbContext : DbContext
    {
        public PAKSDbContext(DbContextOptions<PAKSDbContext> options) : base(options)
        {

        }


        public DbSet<View_ResalePOMappingInfo> View_ResalePOMappingInfo { get; set; }

        /// <summary>
        /// 注册实体
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<View_ResalePOMappingInfo>()
                .HasKey(k => new { k.CompanyCode });
        }
    }
}
