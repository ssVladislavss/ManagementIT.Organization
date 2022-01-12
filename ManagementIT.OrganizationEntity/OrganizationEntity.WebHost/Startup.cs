using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using AutoMapper;
using MassTransit;
using MassTransit.Definition;
using Microsoft.EntityFrameworkCore;
using OrganizationEntity.DataAccess.Data;
using OrganizationEntity.Core.Abstractions.TEntityRepository;
using OrganizationEntity.DataAccess.Repositories.TEntityRepository;
using OrganizationEntity.Core.Abstractions.MongoRepository;
using OrganizationEntity.Core.Abstractions.OrganizationEntityRepository;
using OrganizationEntity.DataAccess.Repositories.EmployeeRepository;
using OrganizationEntity.DataAccess.Repositories.DepartmentRepository;
using OrganizationEntity.DataAccess.Repositories.RoomRepository;
using OrganizationEntity.Core.Abstractions.Service;
using OrganizationEntity.DataAccess.Service;
using OrganizationEntity.WebHost.Areas.Admin.RabbitMQ.BuildingConsumer;
using OrganizationEntity.WebHost.Areas.Admin.RabbitMQ.DepartmentConsumer;
using OrganizationEntity.WebHost.Areas.Admin.RabbitMQ.DependencyConsumer;
using OrganizationEntity.WebHost.Areas.Admin.RabbitMQ.EmployeeConsumer;
using OrganizationEntity.WebHost.Areas.Admin.RabbitMQ.PositionConsumer;
using OrganizationEntity.WebHost.Areas.Admin.RabbitMQ.RoomConsumer;
using OrganizationEntity.WebHost.Areas.Admin.RabbitMQ.SubdivisionConsumer;
using OrganizationEntity.WebHost.AutoMapper;

namespace OrganizationEntity.WebHost
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;
        
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            #region Masstransit

            var massTransitSection = Configuration.GetSection("MassTransit");
            var url = massTransitSection.GetValue<string>("Url");
            var host = massTransitSection.GetValue<string>("Host");
            var userName = massTransitSection.GetValue<string>("UserName");
            var password = massTransitSection.GetValue<string>("Password");

            services.AddMassTransit(x =>
            {
                x.AddBus(busFactory =>
                {
                    var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
                    {
                        cfg.Host($"rabbitmq://{url}{host}", configurator =>
                        {
                            configurator.Username(userName);
                            configurator.Password(password);
                        });
                        cfg.ConfigureEndpoints(busFactory, KebabCaseEndpointNameFormatter.Instance);
                        cfg.UseJsonSerializer();
                        cfg.UseHealthCheck(busFactory);
                    });
                    return bus;
                });
                x.AddConsumer<AllBuildingConsumer>(typeof(AllBuildingConsumerDefinition));
                x.AddConsumer<BuildingByIdConsumer>(typeof(BuildingByIdConsumerDefinition));
                x.AddConsumer<CreateBuildingConsumer>(typeof(CreateBuildingConsumerDefinition));
                x.AddConsumer<UpdateBuildingConsumer>(typeof(UpdateBuildingConsumerDefinition));
                x.AddConsumer<DeleteBuildingConsumer>(typeof(DeleteBuildingConsumerDefinition));
                
                x.AddConsumer<AllDepartmentConsumer>(typeof(AllDepartmentConsumerDefinition));
                x.AddConsumer<DepartmentByIdConsumer>(typeof(DepartmentByIdConsumerDefinition));
                x.AddConsumer<CreateDepartmentConsumer>(typeof(CreateDepartmentConsumerDefinition));
                x.AddConsumer<UpdateDepartmentConsumer>(typeof(UpdateDepartmentConsumerDefinition));
                x.AddConsumer<DeleteDepartmentConsumer>(typeof(DeleteDepartmentConsumerDefinition));
                x.AddConsumer<GetCreateDepartmentConsumer>(typeof(GetCreateDepartmentConsumerDefinition));
                x.AddConsumer<GetUpdateDepartmentConsumer>(typeof(GetUpdateDepartmentConsumerDefinition));
                
                x.AddConsumer<AllEmployeeConsumer>(typeof(AllEmployeeConsumerDefinition));
                x.AddConsumer<EmployeebyIdConsumer>(typeof(EmployeebyIdConsumerDefinition));
                x.AddConsumer<CreateEmployeeConsumer>(typeof(CreateEmployeeConsumerDefinition));
                x.AddConsumer<UpdateEmployeeConsumer>(typeof(UpdateEmployeeConsumerDefinition));
                x.AddConsumer<UpdateEmployeePhotoConsumer>(typeof(UpdateEmployeePhotoConsumerDefinition));
                x.AddConsumer<GetCreateEmployeeConsumer>(typeof(GetCreateEmployeeConsumerDefinition));
                x.AddConsumer<GetUpdateEmployeeConsumer>(typeof(GetUpdateEmployeeConsumerDefinition));
                x.AddConsumer<DeleteEmployeePhotoConsumer>(typeof(DeleteEmployeePhotoConsumerDefinition));
                x.AddConsumer<DeleteEmployeeConsumer>(typeof(DeleteEmployeeConsumerDefinition));
                
                x.AddConsumer<AllPositionConsumer>(typeof(AllPositionConsumerDefinition));
                x.AddConsumer<PositionByIdConsumer>(typeof(PositionByIdConsumerDefinition));
                x.AddConsumer<CreatePositionConsumer>(typeof(CreatePositionConsumerDefinition));
                x.AddConsumer<UpdatePositionConsumer>(typeof(UpdatePositionConsumerDefinition));
                x.AddConsumer<DeletePositionConsumer>(typeof(DeletePositionConsumerDefinition));
                
                x.AddConsumer<AllRoomConsumer>(typeof(AllRoomConsumerDefinition));
                x.AddConsumer<RoomByIdConsumer>(typeof(RoomByIdConsumerDefinition));
                x.AddConsumer<CreateRoomConsumer>(typeof(CreateRoomConsumerDefinition));
                x.AddConsumer<UpdateRoomConsumer>(typeof(UpdateRoomConsumerDefinition));
                x.AddConsumer<DeleteRoomConsumer>(typeof(DeleteRoomConsumerDefinition));
                x.AddConsumer<GetCreateRoomConsumer>(typeof(GetCreateRoomConsumerDefinition));
                x.AddConsumer<GetUpdateRoomConsumer>(typeof(GetUpdateRoomConsumerDefinition));
                
                x.AddConsumer<AllSubdivisionConsumer>(typeof(AllSubdivisionConsumerDefinition));
                x.AddConsumer<SubdivisionByIdConsumer>(typeof(SubdivisionByIdConsumerDefinition));
                x.AddConsumer<CreateSubdivisionConsumer>(typeof(CreateSubdivisionConsumerDefinition));
                x.AddConsumer<UpdateSubdivisionConsumer>(typeof(UpdateSubdivisionConsumerDefinition));
                x.AddConsumer<DeleteSubdivisionConsumer>(typeof(DeleteSubdivisionConsumerDefinition));
                
                x.AddConsumer<GetDependencyForApplicationConsumer>(typeof(GetDependencyForApplicationConsumerDefinition));
            });

            services.AddMassTransitHostedService();

            #endregion

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(Configuration.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value);
            });

            var mapperConfig = new MapperConfiguration(x =>
            {
                x.AddProfile(new MappingProfile());
            });
            IMapper mapper = mapperConfig.CreateMapper();

            #region DI

            services.AddSingleton(mapper);
            services.AddTransient(typeof(IGenericRepository<>), typeof(EFGenericRepository<>));
            services.AddTransient<ILogService, LogService>()
                    .AddTransient<IEmployeeRepository, EFEmployeeRepository>()
                    .AddTransient<IDepartmentRepository, EFDepartmentRepository>()
                    .AddTransient<IRoomRepository, EFRoomRepository>()
                    .AddTransient<IEmployeeService, EmployeeService>()
                    .AddTransient<IBuildingService, BuildingService>()
                    .AddTransient<IPositionService, PositionService>()
                    .AddTransient<IDependencyEntityService, DependencyEntityService>()
                    .AddTransient<IDepartmentService, DepartmentService>()
                    .AddTransient<ISubdivisionService, SubdivisionService>()
                    .AddTransient<IRoomService, RoomService>();

            #endregion

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "OrganizationEntity.WebHost", Version = "v1" });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "OrganizationEntity.WebHost v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
