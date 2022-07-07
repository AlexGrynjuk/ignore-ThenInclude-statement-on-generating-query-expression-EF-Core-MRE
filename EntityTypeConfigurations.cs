    public class GroupSettingsEntityTypeConfiguration : IEntityTypeConfiguration<GroupSettings>
    {
        public void Configure(EntityTypeBuilder<GroupSettings> builder)
        {
            builder.ToTable(nameof(GroupSettings), DbConst.MainSchemaName);

            builder.HasKey(k => k.Id);

            builder.Property(p => p.Id)
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("NEWSEQUENTIALID()");            

            builder.HasOne(p => p.RegionRole)
                .WithMany(m => m.GroupSettings)
                .HasForeignKey(f => f.RegionRoleId)
                .IsRequired(true);
        }
    }

    public class RegionRoleEntityTypeConfiguration : IEntityTypeConfiguration<RegionRole>
    {
        public void Configure(EntityTypeBuilder<RegionRole> builder)
        {
            builder.ToTable(nameof(RegionRole), DbConst.MainSchemaName);

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("NEWSEQUENTIALID()");

            builder.HasOne(p => p.SystemRole)
                .WithMany(m => m.RegionRoles)
                .HasForeignKey(f => f.SystemRoleId)
                .IsRequired(true);

            builder.HasOne(p => p.Region)
                .WithMany()
                .HasForeignKey(f => f.RegionId)
                .IsRequired(true);

            builder.HasIndex(p => new { p.RegionId, p.SystemRoleId })
                .IsUnique(true);
        }
    }

    public class RegionEntityTypeConfiguration : IEntityTypeConfiguration<Region>
    {
        public void Configure(EntityTypeBuilder<Region> builder)
        {
            builder.ToTable(nameof(Region), DbConst.MainSchemaName);

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .UseIdentityColumn();

            builder.Property(p => p.TypeId)
                .IsRequired(true);

            builder.HasOne(p => p.RegionType)
                .WithMany()
                .HasForeignKey(f => f.TypeId)
                .IsRequired(true);
        }
    }

    class RegionTypeEntityTypeConfiguration : IEntityTypeConfiguration<RegionType>
    {
        public void Configure(EntityTypeBuilder<RegionType> builder)
        {
            builder.ToTable(nameof(RegionType), DbConst.MainSchemaName);

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .UseIdentityColumn();

            builder.Property(p => p.TypeName)
                .HasMaxLength(512)
                .IsRequired(true);
        }
    }

    public class SystemRoleEntityTypeConfiguration : IEntityTypeConfiguration<SystemRole>
    {
        public void Configure(EntityTypeBuilder<SystemRole> builder)
        {
            builder.ToTable(nameof(SystemRole), DbConst.MainSchemaName);

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("NEWSEQUENTIALID()");

            builder.HasOne(p => p.Role)
                .WithMany()
                .HasForeignKey(f => f.RoleId)
                .IsRequired(true);

            builder.HasOne(p => p.SystemCode)
                .WithMany(m => m.SystemRoles)
                .HasForeignKey(f => f.SystemId)
                .IsRequired(true);


            builder.HasIndex(p => new { p.RoleId, p.SystemId })
                .IsUnique(true);

        }
    }

    public class RoleEntityTypeConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable(nameof(Role), DbConst.MainSchemaName);

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .UseIdentityColumn();
        }
    }

    public class SystemCodesEntityTypeConfiguration : IEntityTypeConfiguration<SystemCodes>
    {
        public void Configure(EntityTypeBuilder<SystemCodes> builder)
        {
            builder.ToTable("SystemCodes", DbConst.MainSchemaName);

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .ValueGeneratedNever();
        }
    }