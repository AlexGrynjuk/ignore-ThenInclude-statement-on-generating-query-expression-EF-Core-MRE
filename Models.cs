    public class GroupSettings
    {
        
        public Guid Id { get; set; }

        public Guid RegionRoleId { get; set; }

        #region Dependencies

        public virtual RegionRole RegionRole { get; set; }

        #endregion
    }

    public class RegionRole
    {
        public RegionRole()
        {
            this.GroupSettings = new HashSet<GroupSettings>();
        }

        public Guid Id { get; set; }

        public int RegionId { get; set; }

        public Guid SystemRoleId { get; set; }

        #region Dependencies

        public virtual SystemRole SystemRole { get; set; }

        public virtual Region Region { get; set; }

        public virtual ICollection<GroupSettings> GroupSettings { get; set; }

        #endregion
    }    

    public class Region
    {
        public int Id { get; set; }

        public int TypeId { get; set; }

        public RegionType RegionType { get; set; }
    }

     public class RegionType
    {
        public int Id { get; set; }
        public string TypeName { get; set; }
    }

    public class SystemRole
    {
        public SystemRole()
        {
            this.RegionRoles = new HashSet<RegionRole>();
        }

        public Guid Id { get; set; }

        public int RoleId { get; set; }

        public int SystemId { get; set; }

        #region Dependencies

        public virtual Role Role { get; set; }

        public virtual SystemCodes SystemCode { get; set; }

        public virtual ICollection<RegionRole> RegionRoles { get; set; }

        #endregion
    }

    public class Role
    {
        public int Id { get; set; }
    }

    public class SystemCodes
    {
        public SystemCodes()
        {
            this.SystemRoles = new HashSet<SystemRole>();
        }

        public int Id { get; set; }

        #region Dependecies

        public virtual ICollection<SystemRole> SystemRoles { get; set; }

        #endregion
    }