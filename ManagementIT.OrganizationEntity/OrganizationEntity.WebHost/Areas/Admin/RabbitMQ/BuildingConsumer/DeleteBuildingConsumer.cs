using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Contracts.Constants;
using Contracts.Enums;
using Contracts.ResponseModels;
using GreenPipes;
using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.Definition;
using OrganizationEntity.Core.Abstractions.Service;
using OrganizationEntity.Core.Models.BuildingModels;
using OrganizationEntityContracts.ViewModels.OrgEntityViewModel.BuildingViewModels;

namespace OrganizationEntity.WebHost.Areas.Admin.RabbitMQ.BuildingConsumer
{
    public class DeleteBuildingConsumer : IConsumer<DeleteBuildingRequest>
    {
        private readonly IBuildingService _buildingService;

        public DeleteBuildingConsumer(IBuildingService buildingService)
        {
            _buildingService = buildingService ?? throw new ArgumentNullException(nameof(buildingService));
        }

        public async Task Consume(ConsumeContext<DeleteBuildingRequest> context)
        {
            var result = await _buildingService.DeleteAsync(context.Message.BuildingId, ClaimsPrincipal.Current);

            var response = result.Success
                ? new NotificationViewModel()
                : new NotificationViewModel(result.Errors, e: result.AspNetException);

            await context.RespondAsync<NotificationViewModel>(response);
        }
    }

    public class DeleteBuildingConsumerDefinition : ConsumerDefinition<DeleteBuildingConsumer>
    {
        public DeleteBuildingConsumerDefinition()
        {
            EndpointName = ApiShowConstants.DeleteBuilding;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<DeleteBuildingConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseRetry(x => x.Intervals(100, 500, 1000));
        }
    }
}