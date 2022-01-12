using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Contracts.Constants;
using Contracts.Enums;
using Contracts.ResponseModels;
using GreenPipes;
using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.Definition;
using OrganizationEntity.Core.Abstractions.Service;
using OrganizationEntityContracts.ViewModels.OrgEntityViewModel.BuildingViewModels;

namespace OrganizationEntity.WebHost.Areas.Admin.RabbitMQ.BuildingConsumer
{
    public class AllBuildingConsumer : IConsumer<BuildingViewModel>
    {
        private readonly IBuildingService _buildingService;
        private readonly IMapper _mapper;

        public AllBuildingConsumer(IBuildingService buildingService, IMapper mapper)
        {
            _buildingService = buildingService ?? throw new ArgumentNullException(nameof(buildingService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task Consume(ConsumeContext<BuildingViewModel> context)
        {
            var result = await _buildingService.GetAllAsync(ClaimsPrincipal.Current);

            var response = new AllBuildingReponse();
            if (!result.Success)
                response.Notification = new NotificationViewModel(result.Errors, e: result.AspNetException);
            else
            {
                response.Notification = new NotificationViewModel();
                response.Model = _mapper.Map<IEnumerable<BuildingViewModel>>(result.Data);
            }

            await context.RespondAsync<AllBuildingReponse>(response);
        }
    }

    public class AllBuildingConsumerDefinition : ConsumerDefinition<AllBuildingConsumer>
    {
        public AllBuildingConsumerDefinition()
        {
            EndpointName = ApiShowConstants.GetAllBuilding;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<AllBuildingConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseRetry(x => x.Intervals(100, 500, 1000));
        }

    }
}