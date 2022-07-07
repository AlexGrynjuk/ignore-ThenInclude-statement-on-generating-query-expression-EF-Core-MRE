    public class K2DbContext : DbContext
    {

        public K2DbContext(DbContextOptions<K2DbContext> options)
            : base(options)
        {
        }
        
        #region Main

        public virtual DbSet<SystemCodes> SystemCodes { get; set; }        

        #region RoleModel

        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<SystemRole> SystemRole { get; set; }
        public virtual DbSet<Region> Region { get; set; }
        public virtual DbSet<RegionRole> RegionRole { get; set; }
        public virtual DbSet<GroupSettings> GroupSettings { get; set; }

        #endregion


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
        }

        
    }