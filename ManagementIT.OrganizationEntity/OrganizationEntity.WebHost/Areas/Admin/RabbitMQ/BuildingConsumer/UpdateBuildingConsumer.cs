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
    public class UpdateBuildingConsumer : IConsumer<UpdateBuildingViewModel>
    {
        private readonly IBuildingService _buildingService;

        public UpdateBuildingConsumer(IBuildingService buildingService)
        {
            _buildingService = buildingService ?? throw new ArgumentNullException(nameof(buildingService));
        }

        public async Task Consume(ConsumeContext<UpdateBuildingViewModel> context)
        {
            var existName = _buildingService.ExistEntityByName(context.Message.Name, context.Message.Id);
            if (!existName)
            {
                var model = new BuildingDTO(context.Message.Name, context.Message.Address, context.Message.Floor,
                    context.Message.Id);
                var result = await _buildingService.UpdateAsync(model, ClaimsPrincipal.Current);

                var response = result.Success
                    ? new NotificationViewModel()
                    : new NotificationViewModel(result.Errors, e: result.AspNetException);

                await context.RespondAsync<NotificationViewModel>(response);
            }
            else
                await context.RespondAsync<NotificationViewModel>(
                    new NotificationViewModel(new[] {TypeOfErrors.ExistNameEntity}));
        }
    }

    public class UpdateBuildingConsumerDefinition : ConsumerDefinition<UpdateBuildingConsumer>
    {
        public UpdateBuildingConsumerDefinition()
        {
            EndpointName = ApiShowConstants.UpdateBuilding;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<UpdateBuildingConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseRetry(x => x.Intervals(100, 500, 1000));
        }
    }
}