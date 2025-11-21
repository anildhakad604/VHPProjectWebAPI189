using VHPProjectBAL.OTPService;
using VHPProjectBAL.Services.Designation;
using VHPProjectBAL.Services.Members;
using VHPProjectBAL.Services.OTP;
using VHPProjectBAL.Services.SatsangService;
using VHPProjectBAL.Services.TalukaMaster;
using VHPProjectBAL.Services.ViilageMaster;
using VHPProjectBAL.Services.VillageMaster;
using VHPProjectBAL.Services.Wall;
using VHPProjectCommonUtility.Configuration;
using VHPProjectCommonUtility.Encryption;
using VHPProjectCommonUtility.Logger;
using VHPProjectDAL.DesignationRepo;
using VHPProjectDAL.MemberRepo;
using VHPProjectDAL.OTPRepo;
using VHPProjectDAL.SatsangRepo;
using VHPProjectDAL.TalukaRepo;
using VHPProjectDAL.VillageRepo;
using VHPProjectDAL.WallRepo;

namespace VHPProjectDAL.Helper
{
    public class ServiceRegistry
    {
        public void ConfigureDependencies(IServiceCollection services, AppsettingsConfig appSettings)
        {
            #region Buiseness Access Layer
            //services.AddScoped<IUserService, UserService>();
            //services.AddScoped<IMemberExcelService, MemberExcelService>();
            services.AddScoped<IDesignationService, DesignationService>();
            services.AddScoped<ISatsangService, SatsangService>();
            services.AddScoped<ITalukaService, TalukaService>();
            services.AddScoped<IVillageService, VillageService>();
            services.AddScoped<IOTPService, OTPService>();
            services.AddScoped<IMemberService, MemberService>();
            services.AddScoped<IExcelService, ExcelService>();
            services.AddScoped<IWallService, WallService>();
            #endregion

            #region Data Access Layer
            //services.AddScoped<IEmailRepository, EmailRepository>();
            services.AddScoped<IDesignationRepository, DesignationRepository>();
            services.AddScoped<ISatsangRepository, SatsangRepository>();
            services.AddScoped<IVillageRepository, VillageRepository>();
            services.AddScoped<IOTPRepository, OTPRepository>();
            services.AddScoped<ITalukaRepository, TalukaRepository>();
            services.AddScoped<IMemberRepository, MemberRepository>();
            services.AddScoped<IWallRepository, WallRepository>();
            #endregion

            #region Common Layer
            services.AddSingleton<IEncryptionHelper, EncryptionHelper>();
            services.AddSingleton<ILoggerManager, LoggerManager>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //services.AddSingleton<IPagingParameter, PagingParameter>();
            #endregion
        }
        public void ConfigureDataContext(IServiceCollection services, AppsettingsConfig appSettings)
        {
            var connString = appSettings.MasterProjData.ConnectToDb.ConnectionString;
            //var loggerFactory = LoggerFactory.Create(builder =>
            //{
            //    builder
            //        .AddConsole()
            //        .AddFilter(DbLoggerCategory.Database.Command.Name, LogLevel.Information);
            //});
           //services.AddDbContext<MasterProjContext>(options =>
           //     options.UseMySql(
           //         connString,
           //         new MySqlServerVersion(new Version(8, 0, 21)),
           //         mysqlOptions => {
           //             //// Automatic retry for transient errors
           //             //mysqlOptions.EnableRetryOnFailure(
           //             //    maxRetryCount: 5,
           //             //    maxRetryDelay: TimeSpan.FromSeconds(30),
           //             //    errorNumbersToAdd: null);

           //             // Log generated SQL (development only)
           //             mysqlOptions.CommandTimeout(60);
           //         }
           //     )
           // );
        }
    }
}
