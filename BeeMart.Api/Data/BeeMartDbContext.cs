using BeeMart.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace BeeMart.Api.Data;

public class BeeMartDbContext : DbContext
{
    public BeeMartDbContext(DbContextOptions<BeeMartDbContext> options) : base(options) { }

    public DbSet<MsUser> Users => Set<MsUser>();
    public DbSet<MsProduct> Products => Set<MsProduct>();
    public DbSet<LtPaymentMethod> PaymentMethods => Set<LtPaymentMethod>();
    public DbSet<TrTransaction> Transactions => Set<TrTransaction>();
    public DbSet<TrTransactionDetail> TransactionDetails => Set<TrTransactionDetail>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        // Shared audit columns on every entity.
        foreach (var et in b.Model.GetEntityTypes())
        {
            b.Entity(et.ClrType).Property<DateTime>(nameof(AuditableEntity.Datein)).HasColumnType("datetime2");
            b.Entity(et.ClrType).Property<DateTime?>(nameof(AuditableEntity.Dateup)).HasColumnType("datetime2");
            b.Entity(et.ClrType).Property<string>(nameof(AuditableEntity.Userin)).HasMaxLength(36);
            b.Entity(et.ClrType).Property<string>(nameof(AuditableEntity.Userup)).HasMaxLength(36);
        }

        b.Entity<MsUser>(e =>
        {
            e.ToTable("MsUser");
            e.HasKey(x => x.IdUser);
            e.Property(x => x.IdUser).HasMaxLength(5).IsFixedLength(false);
            e.Property(x => x.Username).HasMaxLength(50).IsRequired();
            e.Property(x => x.FullName).HasMaxLength(300).IsRequired();
            e.Property(x => x.PasswordHash).HasMaxLength(255).IsRequired();
            e.Property(x => x.Role).HasMaxLength(20).IsRequired();
            e.HasIndex(x => x.Username).IsUnique();
        });

        b.Entity<MsProduct>(e =>
        {
            e.ToTable("MsProduct");
            e.HasKey(x => x.IdProduct);
            e.Property(x => x.IdProduct).HasMaxLength(36);
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
            e.Property(x => x.Weight).HasColumnType("decimal(10,2)");
            e.Property(x => x.Price).HasColumnType("decimal(18,2)");
            e.Property(x => x.Discount).HasColumnType("decimal(5,2)");
        });

        b.Entity<LtPaymentMethod>(e =>
        {
            e.ToTable("LtPaymentMethod");
            e.HasKey(x => x.IdPaymentMethod);
            // Matches SQL: INT IDENTITY(1,1) — database-generated.
            e.Property(x => x.IdPaymentMethod).ValueGeneratedOnAdd();
            e.Property(x => x.MethodName).HasMaxLength(50).IsRequired();
        });

        b.Entity<TrTransaction>(e =>
        {
            e.ToTable("TrTransaction");
            e.HasKey(x => x.IdTransaction);
            e.Property(x => x.IdTransaction).HasMaxLength(36);
            e.Property(x => x.InvoiceNo).HasMaxLength(20).IsRequired();
            e.Property(x => x.TransactionDate).HasColumnType("datetime2");
            e.Property(x => x.IdUser).HasMaxLength(5).IsRequired();
            e.Property(x => x.TotalPrice).HasColumnType("decimal(18,2)");
            e.HasIndex(x => x.InvoiceNo).IsUnique();

            e.HasOne(x => x.User)
                .WithMany(u => u.Transactions)
                .HasForeignKey(x => x.IdUser)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.PaymentMethod)
                .WithMany(p => p.Transactions)
                .HasForeignKey(x => x.IdPaymentMethod)
                .OnDelete(DeleteBehavior.Restrict);
        });

        b.Entity<TrTransactionDetail>(e =>
        {
            e.ToTable("TrTransactionDetail");
            e.HasKey(x => x.IdTransactionDetail);
            e.Property(x => x.IdTransactionDetail).HasMaxLength(36);
            e.Property(x => x.IdTransaction).HasMaxLength(36).IsRequired();
            e.Property(x => x.IdProduct).HasMaxLength(36).IsRequired();
            e.Property(x => x.Price).HasColumnType("decimal(18,2)");
            e.Property(x => x.Discount).HasColumnType("decimal(5,2)");
            e.Property(x => x.SubTotal).HasColumnType("decimal(18,2)");

            e.HasOne(x => x.Transaction)
                .WithMany(t => t.Details)
                .HasForeignKey(x => x.IdTransaction)
                .OnDelete(DeleteBehavior.Cascade); // details belong to their header

            e.HasOne(x => x.Product)
                .WithMany(p => p.TransactionDetails)
                .HasForeignKey(x => x.IdProduct)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
