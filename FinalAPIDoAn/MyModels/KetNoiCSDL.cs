using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace FinalAPIDoAn.MyModels;

public partial class KetNoiCSDL : DbContext
{
    public KetNoiCSDL()
    {
    }

    public KetNoiCSDL(DbContextOptions<KetNoiCSDL> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductRepair> ProductRepairs { get; set; }

    public virtual DbSet<ProductReview> ProductReviews { get; set; }

    public virtual DbSet<Shipping> Shippings { get; set; }

    public virtual DbSet<ShoppingCart> ShoppingCarts { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Categori__19093A2B09AE5E7A");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Orders__C3905BAF7C82EAD0");

            entity.Property(e => e.OrderDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.OrderStatus).HasDefaultValue("Pending");
            entity.Property(e => e.PaymentStatus).HasDefaultValue("Unpaid");
            entity.Property(e => e.ShippingStatus).HasDefaultValue("Not Shipped");

            entity.HasOne(d => d.User).WithMany(p => p.Orders).HasConstraintName("FK__Orders__UserID__46E78A0C");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.OrderDetailId).HasName("PK__OrderDet__D3B9D30C09685113");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails).HasConstraintName("FK__OrderDeta__Order__49C3F6B7");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderDetails).HasConstraintName("FK__OrderDeta__Produ__4AB81AF0");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payments__9B556A589A085611");

            entity.Property(e => e.PaymentDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.PaymentStatus).HasDefaultValue("Pending");

            entity.HasOne(d => d.Order).WithMany(p => p.Payments).HasConstraintName("FK__Payments__OrderI__5441852A");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Products__B40CC6EDE42380E4");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Category).WithMany(p => p.Products).HasConstraintName("FK__Products__Catego__403A8C7D");
        });

        modelBuilder.Entity<ProductRepair>(entity =>
        {
            entity.HasKey(e => e.RepairId).HasName("PK__ProductR__07D0BDCD7FF15A20");

            entity.Property(e => e.RepairRequestDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.RepairStatus).HasDefaultValue("Pending");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductRepairs).HasConstraintName("FK__ProductRe__Produ__778AC167");

            entity.HasOne(d => d.User).WithMany(p => p.ProductRepairs).HasConstraintName("FK__ProductRe__UserI__787EE5A0");
        });

        modelBuilder.Entity<ProductReview>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("PK__ProductR__74BC79AE532DE49A");

            entity.Property(e => e.ReviewDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductReviews).HasConstraintName("FK__ProductRe__Produ__71D1E811");

            entity.HasOne(d => d.User).WithMany(p => p.ProductReviews).HasConstraintName("FK__ProductRe__UserI__72C60C4A");
        });

        modelBuilder.Entity<Shipping>(entity =>
        {
            entity.HasKey(e => e.ShippingId).HasName("PK__Shipping__5FACD460C8ABD38D");

            entity.Property(e => e.ShippingStatus).HasDefaultValue("Not Shipped");

            entity.HasOne(d => d.Order).WithMany(p => p.Shippings).HasConstraintName("FK__Shipping__OrderI__5812160E");
        });

        modelBuilder.Entity<ShoppingCart>(entity =>
        {
            entity.HasKey(e => e.CartId).HasName("PK__Shopping__51BCD7971E8F0FDF");

            entity.Property(e => e.AddedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Product).WithMany(p => p.ShoppingCarts).HasConstraintName("FK__ShoppingC__Produ__4F7CD00D");

            entity.HasOne(d => d.User).WithMany(p => p.ShoppingCarts).HasConstraintName("FK__ShoppingC__UserI__4E88ABD4");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCACB975315C");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Role).HasDefaultValue("Customer");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
