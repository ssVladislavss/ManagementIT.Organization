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
    
    public class BuildingByIdConsumer : IConsumer<BuildingByIdRequest>
    {
        private readonly IBuildingService _buildingService;
        private readonly IMapper _mapper;

        public BuildingByIdConsumer(IBuildingService buildingService, IMapper mapper)
        {
            _buildingService = buildingService ?? throw new ArgumentNullException(nameof(buildingService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task Consume(ConsumeContext<BuildingByIdRequest> context)
        {
            var result = await _buildingService.GetByIdAsync(context.Message.BuildingId, ClaimsPrincipal.Current);

            var response = new BuildingByIdResponse();
            if (!result.Success)
                response.Notification = new NotificationViewModel(result.Errors, e: result.AspNetException);
            else
            {
                response.Notification = new NotificationViewModel();
                response.Model = _mapper.Map<BuildingViewModel>(result.Data);
            }

            await context.RespondAsync<BuildingByIdResponse>(response);
        }
    }

    public class BuildingByIdConsumerDefinition : ConsumerDefinition<AllBuildingConsumer>
    {
        public BuildingByIdConsumerDefinition()
        {
            EndpointName = ApiShowConstants.GetByIdBuilding;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<AllBuildingConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseRetry(x => x.Intervals(100, 500, 1000));
        }
    }
}