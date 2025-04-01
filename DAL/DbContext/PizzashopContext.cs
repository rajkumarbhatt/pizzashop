using System;
using System.Collections.Generic;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.DBContext;

public partial class PizzaShopContext : DbContext
{
    public PizzaShopContext()
    {
    }

    public PizzaShopContext(DbContextOptions<PizzaShopContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<CustomerFavourite> CustomerFavourites { get; set; }

    public virtual DbSet<CustomerReview> CustomerReviews { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<Item> Items { get; set; }

    public virtual DbSet<ItemModifiergroup> ItemModifiergroups { get; set; }

    public virtual DbSet<Modifier> Modifiers { get; set; }

    public virtual DbSet<ModifierGroup> ModifierGroups { get; set; }

    public virtual DbSet<ModifierModifiergroupMapping> ModifierModifiergroupMappings { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<OrderModifier> OrderModifiers { get; set; }

    public virtual DbSet<OrderTableMapping> OrderTableMappings { get; set; }

    public virtual DbSet<OrderTaxis> OrderTaxes { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<ResetPasswordLink> ResetPasswordLinks { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RolePermission> RolePermissions { get; set; }

    public virtual DbSet<Section> Sections { get; set; }

    public virtual DbSet<State> States { get; set; }

    public virtual DbSet<Table> Tables { get; set; }

    public virtual DbSet<TaxesFee> TaxesFees { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<WaitingList> WaitingLists { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost:5433;Database=pizzashop;Username=postgres;Password=Tatva@123");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("pgcrypto");

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("categories_pkey");

            entity.ToTable("categories");

            entity.HasIndex(e => e.Name, "categories_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Description)
                .HasMaxLength(200)
                .HasColumnName("description");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("is_deleted");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.CategoryCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("categories_created_by_fkey");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.CategoryUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("categories_updated_by_fkey");
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("cities_pkey");

            entity.ToTable("cities");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.StateId).HasColumnName("state_id");

            entity.HasOne(d => d.State).WithMany(p => p.Cities)
                .HasForeignKey(d => d.StateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("cities_state_id_fkey");
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("countries_pkey");

            entity.ToTable("countries");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("customers_pkey");

            entity.ToTable("customers");

            entity.HasIndex(e => e.Email, "customers_email_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.CustomerCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("customers_created_by_fkey");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.CustomerUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("customers_updated_by_fkey");
        });

        modelBuilder.Entity<CustomerFavourite>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("customer_favourites");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.ItemId).HasColumnName("item_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

            entity.HasOne(d => d.CreatedByNavigation).WithMany()
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("customer_favourites_created_by_fkey");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany()
                .HasForeignKey(d => d.UpdatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("customer_favourites_updated_by_fkey");
        });

        modelBuilder.Entity<CustomerReview>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("customer_reviews_pkey");

            entity.ToTable("customer_reviews");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Ambience).HasColumnName("ambience");
            entity.Property(e => e.AverageRating).HasColumnName("average_rating");
            entity.Property(e => e.Comment)
                .HasMaxLength(200)
                .HasColumnName("comment");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.Food).HasColumnName("food");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.Service).HasColumnName("service");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.CustomerReviewCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("customer_reviews_created_by_fkey");

            entity.HasOne(d => d.Customer).WithMany(p => p.CustomerReviews)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("customer_reviews_customer_id_fkey");

            entity.HasOne(d => d.Order).WithMany(p => p.CustomerReviews)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("customer_reviews_order_id_fkey");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.CustomerReviewUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("customer_reviews_updated_by_fkey");
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("invoice_pkey");

            entity.ToTable("invoice");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.OrderId).HasColumnName("order_id");

            entity.HasOne(d => d.Order).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("invoice_order_id_fkey");
        });

        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("items_pkey");

            entity.ToTable("items");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.DefaultTax)
                .HasDefaultValue(false)
                .HasColumnName("default_tax");
            entity.Property(e => e.Description)
                .HasMaxLength(200)
                .HasColumnName("description");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(100)
                .HasColumnName("image_url");
            entity.Property(e => e.IsAvailable)
                .HasDefaultValue(true)
                .HasColumnName("is_available");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("is_deleted");
            entity.Property(e => e.ItemType)
                .HasMaxLength(50)
                .HasColumnName("item_type");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Price)
                .HasPrecision(10, 2)
                .HasColumnName("price");
            entity.Property(e => e.Quantity)
                .HasDefaultValue(0)
                .HasColumnName("quantity");
            entity.Property(e => e.ShortCode)
                .HasMaxLength(10)
                .HasColumnName("short_code");
            entity.Property(e => e.TaxPercentage)
                .HasPrecision(5, 2)
                .HasColumnName("tax_percentage");
            entity.Property(e => e.Unit)
                .HasMaxLength(50)
                .HasColumnName("unit");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

            entity.HasOne(d => d.Category).WithMany(p => p.Items)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("items_category_id_fkey");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ItemCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("items_created_by_fkey");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.ItemUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("items_updated_by_fkey");
        });

        modelBuilder.Entity<ItemModifiergroup>(entity =>
        {
            entity.HasKey(e => new { e.ItemId, e.ModifiergroupId }).HasName("item_modifiergroups_pkey");

            entity.ToTable("item_modifiergroups");

            entity.Property(e => e.ItemId).HasColumnName("item_id");
            entity.Property(e => e.ModifiergroupId).HasColumnName("modifiergroup_id");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.MaxValue).HasColumnName("maxValue");
            entity.Property(e => e.MinValue).HasColumnName("minValue");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ItemModifiergroupCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("item_modifiergroups_created_by_fkey");

            entity.HasOne(d => d.Item).WithMany(p => p.ItemModifiergroups)
                .HasForeignKey(d => d.ItemId)
                .HasConstraintName("item_modifiergroups_item_id_fkey");

            entity.HasOne(d => d.Modifiergroup).WithMany(p => p.ItemModifiergroups)
                .HasForeignKey(d => d.ModifiergroupId)
                .HasConstraintName("item_modifiergroups_modifiergroup_id_fkey");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.ItemModifiergroupUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("item_modifiergroups_updated_by_fkey");
        });

        modelBuilder.Entity<Modifier>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("modifiers_pkey");

            entity.ToTable("modifiers");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Description)
                .HasMaxLength(200)
                .HasColumnName("description");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("is_deleted");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Price)
                .HasPrecision(10, 2)
                .HasColumnName("price");
            entity.Property(e => e.Quantity)
                .HasDefaultValue(0)
                .HasColumnName("quantity");
            entity.Property(e => e.Unit)
                .HasMaxLength(50)
                .HasColumnName("unit");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ModifierCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("modifiers_created_by_fkey");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.ModifierUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("modifiers_updated_by_fkey");

            entity.HasMany(d => d.Modifiergroups).WithMany(p => p.Modifiers)
                .UsingEntity<Dictionary<string, object>>(
                    "ModifierMapping",
                    r => r.HasOne<ModifierGroup>().WithMany()
                        .HasForeignKey("ModifiergroupId")
                        .HasConstraintName("modifier_modifiergroups_modifiergroup_id_fkey"),
                    l => l.HasOne<Modifier>().WithMany()
                        .HasForeignKey("ModifierId")
                        .HasConstraintName("modifier_modifiergroups_modifier_id_fkey"),
                    j =>
                    {
                        j.HasKey("ModifierId", "ModifiergroupId").HasName("modifier_modifiergroups_pkey");
                        j.ToTable("modifier_mapping");
                        j.IndexerProperty<int>("ModifierId").HasColumnName("modifier_id");
                        j.IndexerProperty<int>("ModifiergroupId").HasColumnName("modifiergroup_id");
                    });
        });

        modelBuilder.Entity<ModifierGroup>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("modifier_groups_pkey");

            entity.ToTable("modifier_groups");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Description)
                .HasMaxLength(200)
                .HasColumnName("description");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("is_deleted");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ModifierGroupCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("modifier_groups_created_by_fkey");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.ModifierGroupUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("modifier_groups_updated_by_fkey");
        });

        modelBuilder.Entity<ModifierModifiergroupMapping>(entity =>
        {
            entity.HasKey(e => new { e.ModifierId, e.ModifiergroupId }).HasName("modifier_modifiergroup_mapping_pkey");

            entity.ToTable("modifier_modifiergroup_mapping");

            entity.Property(e => e.ModifierId).HasColumnName("modifier_id");
            entity.Property(e => e.ModifiergroupId).HasColumnName("modifiergroup_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Modifier).WithMany(p => p.ModifierModifiergroupMappings)
                .HasForeignKey(d => d.ModifierId)
                .HasConstraintName("modifier_modifiergroup_mapping_modifier_id_fkey");

            entity.HasOne(d => d.Modifiergroup).WithMany(p => p.ModifierModifiergroupMappings)
                .HasForeignKey(d => d.ModifiergroupId)
                .HasConstraintName("modifier_modifiergroup_mapping_modifiergroup_id_fkey");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("orders_pkey");

            entity.ToTable("orders");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.PaymentMode)
                .HasMaxLength(50)
                .HasColumnName("payment_mode");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.TotalAmount)
                .HasPrecision(10, 2)
                .HasColumnName("total_amount");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.OrderCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("orders_created_by_fkey");

            entity.HasOne(d => d.Customer).WithMany(p => p.Orders)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("orders_customer_id_fkey");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.OrderUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("orders_updated_by_fkey");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("order_items_pkey");

            entity.ToTable("order_items");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ItemId).HasColumnName("item_id");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.Item).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.ItemId)
                .HasConstraintName("order_items_item_id_fkey");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("order_items_order_id_fkey");
        });

        modelBuilder.Entity<OrderModifier>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("order_modifiers_pkey");

            entity.ToTable("order_modifiers");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.ModifierId).HasColumnName("modifier_id");
            entity.Property(e => e.OrderItemId).HasColumnName("order_item_id");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.OrderModifierCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("order_modifiers_created_by_fkey");

            entity.HasOne(d => d.Modifier).WithMany(p => p.OrderModifiers)
                .HasForeignKey(d => d.ModifierId)
                .HasConstraintName("order_modifiers_modifier_id_fkey");

            entity.HasOne(d => d.OrderItem).WithMany(p => p.OrderModifiers)
                .HasForeignKey(d => d.OrderItemId)
                .HasConstraintName("order_modifiers_order_item_id_fkey");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.OrderModifierUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("order_modifiers_updated_by_fkey");
        });

        modelBuilder.Entity<OrderTableMapping>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("order_table_mapping_pkey");

            entity.ToTable("order_table_mapping");

            entity.HasIndex(e => e.TableId, "fki_a");

            entity.HasIndex(e => e.OrderId, "fki_order_id");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("is_deleted");
            entity.Property(e => e.NoOfPersons).HasColumnName("no_of_persons");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.TableId).HasColumnName("table_id");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.OrderTableMappingCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("created_by");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderTableMappings)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("order_id");

            entity.HasOne(d => d.Table).WithMany(p => p.OrderTableMappings)
                .HasForeignKey(d => d.TableId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("table_id");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.OrderTableMappingUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("updated_by");
        });

        modelBuilder.Entity<OrderTaxis>(entity =>
        {
            entity.HasKey(e => new { e.OrderId, e.TaxId }).HasName("order_taxes_pkey");

            entity.ToTable("order_taxes");

            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.TaxId).HasColumnName("tax_id");
            entity.Property(e => e.TaxAmount)
                .HasPrecision(10, 2)
                .HasColumnName("tax_amount");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderTaxes)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("order_taxes_order_id_fkey");

            entity.HasOne(d => d.Tax).WithMany(p => p.OrderTaxes)
                .HasForeignKey(d => d.TaxId)
                .HasConstraintName("order_taxes_tax_id_fkey");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("permissions_pkey");

            entity.ToTable("permissions");

            entity.HasIndex(e => e.Name, "permissions_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Name)
                .HasMaxLength(60)
                .HasColumnName("name");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.PermissionCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("permissions_created_by_fkey");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.PermissionUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("permissions_updated_by_fkey");
        });

        modelBuilder.Entity<ResetPasswordLink>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("reset-password-links_pkey");

            entity.ToTable("reset-password-links");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Link)
                .HasMaxLength(200)
                .HasColumnName("link");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("roles_pkey");

            entity.ToTable("roles");

            entity.HasIndex(e => e.Name, "roles_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .HasColumnName("name");
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasKey(e => new { e.RoleId, e.PermissionId }).HasName("role_permissions_pkey");

            entity.ToTable("role_permissions");

            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.PermissionId).HasColumnName("permission_id");
            entity.Property(e => e.CanDelete).HasColumnName("can_delete");
            entity.Property(e => e.CanEdit).HasColumnName("can_edit");
            entity.Property(e => e.CanView).HasColumnName("can_view");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.RolePermissionCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("role_permissions_created_by_fkey");

            entity.HasOne(d => d.Permission).WithMany(p => p.RolePermissions)
                .HasForeignKey(d => d.PermissionId)
                .HasConstraintName("role_permissions_permission_id_fkey");

            entity.HasOne(d => d.Role).WithMany(p => p.RolePermissions)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("role_permissions_role_id_fkey");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.RolePermissionUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("role_permissions_updated_by_fkey");
        });

        modelBuilder.Entity<Section>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("sections_pkey");

            entity.ToTable("sections");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Description)
                .HasMaxLength(200)
                .HasColumnName("description");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("is_deleted");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.SectionCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("sections_created_by_fkey");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.SectionUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("sections_updated_by_fkey");
        });

        modelBuilder.Entity<State>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("states_pkey");

            entity.ToTable("states");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CountryId).HasColumnName("country_id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");

            entity.HasOne(d => d.Country).WithMany(p => p.States)
                .HasForeignKey(d => d.CountryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("states_country_id_fkey");
        });

        modelBuilder.Entity<Table>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tables_pkey");

            entity.ToTable("tables");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Capacity).HasColumnName("capacity");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("is_deleted");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.SectionId).HasColumnName("section_id");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.TableCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tables_created_by_fkey");

            entity.HasOne(d => d.Section).WithMany(p => p.Tables)
                .HasForeignKey(d => d.SectionId)
                .HasConstraintName("tables_section_id_fkey");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.TableUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tables_updated_by_fkey");
        });

        modelBuilder.Entity<TaxesFee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("taxes_fees_pkey");

            entity.ToTable("taxes_fees");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Amount)
                .HasPrecision(10, 2)
                .HasColumnName("amount");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.IsDefault)
                .HasDefaultValue(false)
                .HasColumnName("is_default");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("is_deleted");
            entity.Property(e => e.IsEnabled)
                .HasDefaultValue(true)
                .HasColumnName("is_enabled");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.TaxType)
                .HasMaxLength(50)
                .HasColumnName("tax_type");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => e.CreatedBy, "fki_created_by");

            entity.HasIndex(e => e.UpdatedBy, "fki_updated_by");

            entity.HasIndex(e => e.Email, "users_email_key").IsUnique();

            entity.HasIndex(e => e.Username, "users_username_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Address)
                .HasMaxLength(200)
                .HasColumnName("address");
            entity.Property(e => e.CityId).HasColumnName("city_id");
            entity.Property(e => e.CountryId).HasColumnName("country_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .HasColumnName("first_name");
            entity.Property(e => e.HasLoggedInBefore)
                .HasDefaultValue(false)
                .HasColumnName("hasLoggedInBefore");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("isDeleted");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .HasColumnName("last_name");
            entity.Property(e => e.Password).HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.ProfileImage)
                .HasMaxLength(500)
                .HasColumnName("profile_image");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.StateId).HasColumnName("state_id");
            entity.Property(e => e.Status)
                .HasDefaultValue(true)
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("username");
            entity.Property(e => e.ZipCode)
                .HasMaxLength(10)
                .HasColumnName("zip_code");

            entity.HasOne(d => d.City).WithMany(p => p.Users)
                .HasForeignKey(d => d.CityId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("users_city_id_fkey");

            entity.HasOne(d => d.Country).WithMany(p => p.Users)
                .HasForeignKey(d => d.CountryId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("users_country_id_fkey");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.InverseCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("users_created_by_fkey");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("users_role_id_fkey");

            entity.HasOne(d => d.State).WithMany(p => p.Users)
                .HasForeignKey(d => d.StateId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("users_state_id_fkey");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.InverseUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("updated_by");
        });

        modelBuilder.Entity<WaitingList>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("waiting_list_pkey");

            entity.ToTable("waiting_list");

            entity.HasIndex(e => e.SectionId, "fki_section_id_constraint");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("is_deleted");
            entity.Property(e => e.NoOfPersons).HasColumnName("no_of_persons");
            entity.Property(e => e.SectionId).HasColumnName("section_id");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.WaitingListCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("waiting_list_created_by_fkey");

            entity.HasOne(d => d.Customer).WithMany(p => p.WaitingLists)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("waiting_list_customer_id_fkey");

            entity.HasOne(d => d.Section).WithMany(p => p.WaitingLists)
                .HasForeignKey(d => d.SectionId)
                .HasConstraintName("section_id_constraint");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.WaitingListUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("waiting_list_updated_by_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
