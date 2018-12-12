using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace PetesPizzaPagoda.DataAccess
{
    public partial class _1811proj0Context : DbContext
    {
        public _1811proj0Context()
        {
        }

        public _1811proj0Context(DbContextOptions<_1811proj0Context> options)
            : base(options)
        {
        }

        public virtual DbSet<LocationInventory> LocationInventory { get; set; }
        public virtual DbSet<Locations> Locations { get; set; }
        public virtual DbSet<OrderEntries> OrderEntries { get; set; }
        public virtual DbSet<Orders> Orders { get; set; }
        public virtual DbSet<PizzaToppings> PizzaToppings { get; set; }
        public virtual DbSet<Pizzas> Pizzas { get; set; }
        public virtual DbSet<Toppings> Toppings { get; set; }
        public virtual DbSet<Users> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.0-rtm-35687");

            modelBuilder.Entity<LocationInventory>(entity =>
            {
                entity.HasKey(e => e.LiId)
                    .HasName("PK_LocationInventory_ID");

                entity.ToTable("LocationInventory", "Piz");

                entity.Property(e => e.LiId).HasColumnName("LI_ID");

                entity.HasOne(d => d.LocationNavigation)
                    .WithMany(p => p.LocationInventory)
                    .HasForeignKey(d => d.Location)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LocationInventory_Location_LocationID");

                entity.HasOne(d => d.ToppingNavigation)
                    .WithMany(p => p.LocationInventory)
                    .HasForeignKey(d => d.Topping)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LocationInventory_Topping_ToppingID");
            });

            modelBuilder.Entity<Locations>(entity =>
            {
                entity.HasKey(e => e.LId)
                    .HasName("PK_Location_ID");

                entity.ToTable("Locations", "Piz");

                entity.Property(e => e.LId).HasColumnName("L_ID");

                entity.Property(e => e.City)
                    .IsRequired()
                    .HasMaxLength(25);

                entity.Property(e => e.State)
                    .IsRequired()
                    .HasMaxLength(25);
            });

            modelBuilder.Entity<OrderEntries>(entity =>
            {
                entity.HasKey(e => e.OeId)
                    .HasName("PK_OrderEntry_ID");

                entity.ToTable("OrderEntries", "Piz");

                entity.Property(e => e.OeId).HasColumnName("OE_ID");

                entity.HasOne(d => d.OnOrderNavigation)
                    .WithMany(p => p.OrderEntries)
                    .HasForeignKey(d => d.OnOrder)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderEntry_Order_OrderID");

                entity.HasOne(d => d.PizzaNavigation)
                    .WithMany(p => p.OrderEntries)
                    .HasForeignKey(d => d.Pizza)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderEntry_Pizza_PizzaID");
            });

            modelBuilder.Entity<Orders>(entity =>
            {
                entity.HasKey(e => e.OId)
                    .HasName("PK_Order_ID");

                entity.ToTable("Orders", "Piz");

                entity.Property(e => e.OId).HasColumnName("O_ID");

                entity.Property(e => e.TotalPrice).HasColumnType("money");

                entity.HasOne(d => d.OrderForNavigation)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.OrderFor)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Order_User_UserID");

                entity.HasOne(d => d.OrderedFromNavigation)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.OrderedFrom)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Order_Location_LocationID");
            });

            modelBuilder.Entity<PizzaToppings>(entity =>
            {
                entity.HasKey(e => e.PtId)
                    .HasName("PK_PizzaTopping_ID");

                entity.ToTable("PizzaToppings", "Piz");

                entity.Property(e => e.PtId).HasColumnName("PT_ID");

                entity.HasOne(d => d.PizzaNavigation)
                    .WithMany(p => p.PizzaToppings)
                    .HasForeignKey(d => d.Pizza)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PizzaTopping_Pizza_PizzaID");

                entity.HasOne(d => d.ToppingNavigation)
                    .WithMany(p => p.PizzaToppings)
                    .HasForeignKey(d => d.Topping)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PizzaTopping_Topping_ToppingID");
            });

            modelBuilder.Entity<Pizzas>(entity =>
            {
                entity.HasKey(e => e.PId)
                    .HasName("PK_Pizza_ID");

                entity.ToTable("Pizzas", "Piz");

                entity.Property(e => e.PId).HasColumnName("P_ID");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Price).HasColumnType("money");
            });

            modelBuilder.Entity<Toppings>(entity =>
            {
                entity.HasKey(e => e.TId)
                    .HasName("PK_Topping_ID");

                entity.ToTable("Toppings", "Piz");

                entity.Property(e => e.TId).HasColumnName("T_ID");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Users>(entity =>
            {
                entity.HasKey(e => e.UId)
                    .HasName("PK_User_ID");

                entity.ToTable("Users", "Piz");

                entity.Property(e => e.UId).HasColumnName("U_ID");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.DefaultLocationNavigation)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.DefaultLocation)
                    .HasConstraintName("FK_User_Location_LocationID");
            });
        }
    }
}
