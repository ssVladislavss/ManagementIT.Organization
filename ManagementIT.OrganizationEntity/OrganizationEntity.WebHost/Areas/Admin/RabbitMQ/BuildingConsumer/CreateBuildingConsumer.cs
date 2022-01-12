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
    public class CreateBuildingConsumer : IConsumer<CreateBuildingViewModel>
    {
        private readonly IBuildingService _buildingService;

        public CreateBuildingConsumer(IBuildingService buildingService)
        {
            _buildingService = buildingService ?? throw new ArgumentNullException(nameof(buildingService));
        }

        public async Task Consume(ConsumeContext<CreateBuildingViewModel> context)
        {
            var existName = _buildingService.ExistEntityByName(context.Message.Name);
            if (!existName)
            {
                var model = new BuildingDTO(context.Message.Name, context.Message.Address, context.Message.Floor);
                var result = await _buildingService.AddAsync(model, ClaimsPrincipal.Current);

                var response = result.Success
                    ? new NotificationViewModel()
                    : new NotificationViewModel(result.Errors, e: result.AspNetException);

                await context.RespondAsync<NotificationViewModel>(response);
            }
            else
                await context.RespondAsync<NotificationViewModel>(
                    new NotificationViewModel(new[] { TypeOfErrors.ExistNameEntity }));
        }
    }

    public class CreateBuildingConsumerDefinition : ConsumerDefinition<CreateBuildingConsumer>
    {
        public CreateBuildingConsumerDefinition()
        {
            EndpointName = ApiShowConstants.CreateBuilding;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<CreateBuildingConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseRetry(x => x.Intervals(100, 500, 1000));
        }
    }
}