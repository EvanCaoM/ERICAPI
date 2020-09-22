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
    public class AppDbContext : DbContext
    {
        /// <summary>
        /// 一个数据库的构造函数
        /// </summary>
        /// <param name="options"></param>
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {

        }


        /// <summary>
        /// 设备po归并表
        /// </summary>
        public DbSet<tdsAupo> tdsAupos { get; set; }
        /// <summary>
        /// po归并查询视图
        /// </summary>
        public DbSet<v_sIFREupoMain> v_sIFREupoMains { get; set; }
        /// <summary>
        /// 底账序号查询视图
        /// </summary>
        public DbSet<v_sIFRDeclitem> v_sIFRDeclitem { get; set; }

        public DbSet<DropdownList> dropdownLists { get; set; }

        /// <summary>
        /// 注册实体
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<tdsAupo>()
                .HasKey(k => new { k.MANDT, k.BUKRS, k.EBELN, k.EBELP });
            modelBuilder.Entity<v_sIFREupoMain>()
                .HasKey(k => new { k.BUKRS, k.EBELN, k.EBELP });
            modelBuilder.Entity<v_sIFRDeclitem>()
                .HasKey(k => new { k.BUKRS, k.ACCNO, k.DECLITEM });
            modelBuilder.Entity<DropdownList>()
                .HasKey(k => k.drpValue);
        }
    }
}
