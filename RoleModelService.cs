    public interface IRoleModelService
    {
        Task<List<RegionRole>> GetRegionRoleAsync(SystemCodesEnum systemId, CancellationToken cancellationToken);
    }
    
    public class RoleModelService : IRoleModelService
    {
        private readonly IRoleModelRepository _dbRepository;

        public RoleModelService(IRoleModelRepository roleModelRepository)
        {
            _dbRepository = roleModelRepository ?? throw new ArgumentNullException(nameof(IRoleModelRepository));
        }    
        public async Task<List<RegionRole>> GetRegionRoleAsync(SystemCodesEnum systemId, CancellationToken cancellationToken)
        {
            var settings = await _dbRepository.GetRegionRoles(systemId)
                .Include(x => x.Region)
                .Include(x => x.SystemRole)
                .ThenInclude(x => x.Role)
                .Include(x => x.GroupSettings)
                .Where(w => w.GroupSettings.Any())
                .ToListAsync(cancellationToken);

            return settings;
        }
    }