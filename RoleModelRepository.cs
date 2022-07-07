    public interface IRoleModelRepository
    {
        IQueryable<RegionRole> GetRegionRoles(SystemCodesEnum SystemId);
    }
    
    public class RoleModelRepository : IRoleModelRepository
    {
        private readonly K2DbContext _context;
     
        public RoleModelRepository(K2DbContext context, ILogger<RoleModelRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException();
        }

        public IQueryable<RegionRole> GetRegionRoles(SystemCodesEnum systemId)
        {
            return _context.RegionRole
                .Include(x => x.SystemRole)
                .Where(w => w.SystemRole.SystemId.Equals((int)systemId));
        }
    }