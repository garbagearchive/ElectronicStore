using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace FinalAPIDoAn.Models;

public partial class OnlineStore2Context : DbContext
{
    public OnlineStore2Context()
    {
    }

    public OnlineStore2Context(DbContextOptions<OnlineStore2Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Discount> Discounts { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductDiscount> ProductDiscounts { get; set; }

    public virtual DbSet<ProductRepair> ProductRepairs { get; set; }

    public virtual DbSet<ProductReview> ProductReviews { get; set; }

    public virtual DbSet<Shipping> Shippings { get; set; }

    public virtual DbSet<ShoppingCart> ShoppingCarts { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=IDEAPAD;Initial Catalog=OnlineStore2;Persist Security Info=True;User ID=sa;Password=1234567;Encrypt=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Categori__19093A2B4E83C6A8");
        });

        modelBuilder.Entity<Discount>(entity =>
        {
            entity.HasKey(e => e.DiscountId).HasName("PK__Discount__E43F6DF625B5DA7C");

            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Orders__C3905BAF469C7359");

            entity.Property(e => e.OrderDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.OrderStatus).HasDefaultValue("Pending");
            entity.Property(e => e.PaymentStatus).HasDefaultValue("Unpaid");
            entity.Property(e => e.ShippingStatus).HasDefaultValue("Not Shipped");

            entity.HasOne(d => d.User).WithMany(p => p.Orders).HasConstraintName("FK__Orders__UserID__46E78A0C");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.OrderDetailId).HasName("PK__OrderDet__D3B9D30C11306B72");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails).HasConstraintName("FK__OrderDeta__Order__49C3F6B7");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderDetails).HasConstraintName("FK__OrderDeta__Produ__4AB81AF0");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payments__9B556A5857AC10D4");

            entity.Property(e => e.PaymentDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.PaymentStatus).HasDefaultValue("Pending");

            entity.HasOne(d => d.Order).WithMany(p => p.Payments).HasConstraintName("FK__Payments__OrderI__5441852A");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Products__B40CC6ED67F9A4C4");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Category).WithMany(p => p.Products).HasConstraintName("FK__Products__Catego__403A8C7D");
        });

        modelBuilder.Entity<ProductDiscount>(entity =>
        {
            entity.HasKey(e => e.ProductDiscountId).HasName("PK__ProductD__B8D3D9C1221E6AD6");

            entity.HasOne(d => d.Discount).WithMany(p => p.ProductDiscounts).HasConstraintName("FK__ProductDi__Disco__6C190EBB");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductDiscounts).HasConstraintName("FK__ProductDi__Produ__6B24EA82");
        });

        modelBuilder.Entity<ProductRepair>(entity =>
        {
            entity.HasKey(e => e.RepairId).HasName("PK__ProductR__07D0BDCDD4DD8033");

            entity.Property(e => e.RepairRequestDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.RepairStatus).HasDefaultValue("Pending");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductRepairs).HasConstraintName("FK__ProductRe__Produ__628FA481");

            entity.HasOne(d => d.User).WithMany(p => p.ProductRepairs).HasConstraintName("FK__ProductRe__UserI__6383C8BA");
        });

        modelBuilder.Entity<ProductReview>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("PK__ProductR__74BC79AEFFD82F90");

            entity.Property(e => e.ReviewDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductReviews).HasConstraintName("FK__ProductRe__Produ__5CD6CB2B");

            entity.HasOne(d => d.User).WithMany(p => p.ProductReviews).HasConstraintName("FK__ProductRe__UserI__5DCAEF64");
        });

        modelBuilder.Entity<Shipping>(entity =>
        {
            entity.HasKey(e => e.ShippingId).HasName("PK__Shipping__5FACD460DB1E4B24");

            entity.Property(e => e.ShippingStatus).HasDefaultValue("Not Shipped");

            entity.HasOne(d => d.Order).WithMany(p => p.Shippings).HasConstraintName("FK__Shipping__OrderI__5812160E");
        });

        modelBuilder.Entity<ShoppingCart>(entity =>
        {
            entity.HasKey(e => e.CartId).HasName("PK__Shopping__51BCD797B92FF3D4");

            entity.Property(e => e.AddedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Product).WithMany(p => p.ShoppingCarts).HasConstraintName("FK__ShoppingC__Produ__4F7CD00D");

            entity.HasOne(d => d.User).WithMany(p => p.ShoppingCarts).HasConstraintName("FK__ShoppingC__UserI__4E88ABD4");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC147B37E1");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Role).HasDefaultValue("Customer");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
