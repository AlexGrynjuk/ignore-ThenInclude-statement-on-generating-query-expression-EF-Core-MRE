    public class Startup
    {
        private readonly Logger _logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

        public const string CorsPolicyName = "CorsPolicy";
        private readonly string _host;
        private readonly string _ver;
        private readonly string _swaggerDescr;
        private readonly string _swaggerTitle;

        public IConfiguration _configuration { get; set; }
        public IWebHostEnvironment _environment { get; set; }

        public Startup(IWebHostEnvironment environment)
        {
            _environment = environment;
            _configuration = new ConfigurationBuilder()
                .SetBasePath(environment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{_environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddEFConfiguration(o => o.UseSqlServer(
                    new ConfigurationBuilder()
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                        .AddJsonFile($"appsettings.{_environment.EnvironmentName}.json", optional: true, reloadOnChange: false)
                        .AddEnvironmentVariables().Build().GetConnectionString("MsSqlConnection"),                    
                    dbContextOptions => dbContextOptions.CommandTimeout(60)), _environment)
                .Build();

            _host = System.Net.Dns.GetHostName();
            string assemblyVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            _ver = assemblyVersion;
            _swaggerDescr = $"* Environment: {environment.EnvironmentName}\n* Assembly version: {assemblyVersion}\n* Host: {_host}";
            _swaggerTitle = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;

        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(Microsoft.AspNetCore.Server.IIS.IISServerDefaults.AuthenticationScheme);

            services.Configure<IISServerOptions>(options =>
            {
                options.AutomaticAuthentication = true;
            });

            var origins = GetAllowedOrigins();
            services.AddCors(options =>
            {
                options.AddPolicy(name: CorsPolicyName,
                    builder => builder.WithOrigins(origins)
                                        .AllowAnyHeader()
                                        .AllowAnyMethod()
                                        .AllowCredentials());
            });

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new Mappers.MappingProfile());
            });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddMvc(o =>
                {
                    var policy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build();
                    o.Filters.Add(new AuthorizeFilter(policy));
                })
               .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
               .AddNewtonsoftJson(o =>
                   {
                       o.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                       o.SerializerSettings.ContractResolver = new DefaultContractResolver();
                   }
               );

            services.AddSwaggerDocument(o => { o.Version = "2"; o.Title = "K2Interaction API"; o.Description = _swaggerDescr; });

            services.AddControllers();

            services.AddOData();

            services.AddMvcCore(o =>
            {
                foreach (var outputFormatter in o.OutputFormatters.OfType<OutputFormatter>().Where(x => x.SupportedMediaTypes.Count == 0))
                {
                    outputFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/prs.odatatestxx-odata"));
                }

                foreach (var inputFormatter in o.InputFormatters.OfType<InputFormatter>().Where(x => x.SupportedMediaTypes.Count == 0))
                {
                    inputFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/prs.odatatestxx-odata"));
                }
            });

            services.AddSignalR();

            services.AddScoped<IRoleModelRepository, RoleModelRepository>();
            services.AddScoped<IRoleModelService, RoleModelService>();

            services.AddHealthChecks();

            var context = services.BuildServiceProvider()
                       .GetService<K2DbContext>();

            var systemRoles = (from sr in context.SystemRole
                               join ro in context.Role on sr.RoleId equals ro.Id
                               where (sr.IsActive && ro.IsActive)
                               select sr).Distinct();

            services.AddAuthorization(options =>
            {                
                foreach (var systemRole in systemRoles)
                {
                    var systemRoleIdentity = RoleIdentityHelper.UniqueClaimName((SystemCodesEnum)systemRole.SystemId, (RoleEnum)systemRole.RoleId);
                    options.AddPolicy(systemRoleIdentity, policy => policy.RequireAssertion(context => context.User.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == systemRoleIdentity)));
                }
            });

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseFileServer();

            app.UseRouting();

            app.UseCors(CorsPolicyName);  

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.EnableDependencyInjection();
                endpoints.Select().Expand().Count().Filter().OrderBy().MaxTop(100).SkipToken();
                endpoints.MapODataRoute("odata", "odata", GetEdmModel());
                endpoints.MapHealthChecks("/health");
            });

            app.UseOpenApi();
            app.UseSwaggerUi3();
        }

        private string[] GetAllowedOrigins()
        {
            var loadedOrigins = _configuration.GetSection("CorsOrigins").Get<List<string>>().ToArray();
            _logger.Info($"Loaded Origins: '{ string.Join(", ", loadedOrigins) }'");
            if (loadedOrigins.Length < 1)
            {
                return new string[] { "*" };
            }
            return loadedOrigins;
        }

        private static IEdmModel GetEdmModel()
        {
            var builder = new ODataConventionModelBuilder();

            builder.EntitySet<SystemCodes>(nameof(SystemCodes));
            builder.EntitySet<Role>(nameof(Role));
            builder.EntitySet<SystemRole>(nameof(SystemRole));
            builder.EntitySet<Region>(nameof(Region));
            builder.EntitySet<RegionRole>(nameof(RegionRole));
            builder.EntitySet<GroupSettings>(nameof(GroupSettings));

            return builder.GetEdmModel();
        }
    }