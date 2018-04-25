using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using BudgetPeeker.Models;

namespace BudgetPeeker.Persistence
{
    public class BudgetPeekerDbContext : DbContext
    {
        public virtual DbSet<ApprovedBudget> ApprovedBudget { get; set; }

        public BudgetPeekerDbContext(DbContextOptions options)
            : base(options)
        {
        }

//        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//        {
//            if (!optionsBuilder.IsConfigured)
//            {
//                optionsBuilder.UseSqlServer("server=localhost;database=SacCityBudget;user id=SA;password=Joseph25!");
//            }
//        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApprovedBudget>(entity =>
            {
                entity.HasIndex(e => e.Id)
                    .HasName("ApprovedBudget_ID_uindex")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Account)
                    .HasColumnName("ACCOUNT")
                    .IsUnicode(false);

                entity.Property(e => e.AccountCategory)
                    .HasColumnName("ACCOUNT_CATEGORY")
                    .IsUnicode(false);

                entity.Property(e => e.AccountDescription)
                    .HasColumnName("ACCOUNT_DESCRIPTION")
                    .IsUnicode(false);

                entity.Property(e => e.Activity)
                    .HasColumnName("ACTIVITY")
                    .IsUnicode(false);

                entity.Property(e => e.AllAccounts)
                    .HasColumnName("ALL_ACCOUNTS")
                    .IsUnicode(false);

                entity.Property(e => e.AllDepts)
                    .HasColumnName("ALL_DEPTS")
                    .IsUnicode(false);

                entity.Property(e => e.AllFunds)
                    .HasColumnName("ALL_FUNDS")
                    .IsUnicode(false);

                entity.Property(e => e.AllOperUnits)
                    .HasColumnName("ALL_OPER_UNITS")
                    .IsUnicode(false);

                entity.Property(e => e.BudgetAmount).HasColumnName("BUDGET_AMOUNT");

                entity.Property(e => e.DepartmentDescription)
                    .HasColumnName("DEPARTMENT_DESCRIPTION")
                    .IsUnicode(false);

                entity.Property(e => e.DepartmentDivision)
                    .HasColumnName("DEPARTMENT_DIVISION")
                    .IsUnicode(false);

                entity.Property(e => e.DepartmentOpUnit)
                    .HasColumnName("DEPARTMENT_OP_UNIT")
                    .IsUnicode(false);

                entity.Property(e => e.Deptid)
                    .HasColumnName("DEPTID")
                    .IsUnicode(false);

                entity.Property(e => e.Exprev)
                    .HasColumnName("EXPREV")
                    .IsUnicode(false);

                entity.Property(e => e.Fund)
                    .HasColumnName("FUND")
                    .IsUnicode(false);

                entity.Property(e => e.FundDescription)
                    .HasColumnName("FUND_DESCRIPTION")
                    .IsUnicode(false);

                entity.Property(e => e.FundGroup)
                    .HasColumnName("FUND_GROUP")
                    .IsUnicode(false);

                entity.Property(e => e.FundSummary)
                    .HasColumnName("FUND_SUMMARY")
                    .IsUnicode(false);

                entity.Property(e => e.Net)
                    .HasColumnName("NET")
                    .IsUnicode(false);

                entity.Property(e => e.OjbectClass)
                    .HasColumnName("OJBECT_CLASS")
                    .IsUnicode(false);

                entity.Property(e => e.OperUnitGroup)
                    .HasColumnName("OPER_UNIT_GROUP")
                    .IsUnicode(false);

                entity.Property(e => e.OperatingUnit)
                    .HasColumnName("OPERATING_UNIT")
                    .IsUnicode(false);

                entity.Property(e => e.OperatingUnitDescription)
                    .HasColumnName("OPERATING_UNIT_DESCRIPTION")
                    .IsUnicode(false);

                entity.Property(e => e.Version)
                    .HasColumnName("VERSION")
                    .IsUnicode(false);

                entity.Property(e => e.Year)
                    .HasColumnName("YEAR")
                    .IsUnicode(false);
            });
        }
    }
}
