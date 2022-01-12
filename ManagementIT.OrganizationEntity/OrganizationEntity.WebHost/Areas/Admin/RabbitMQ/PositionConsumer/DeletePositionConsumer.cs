using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Contracts.Constants;
using Contracts.ResponseModels;
using GreenPipes;
using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.Definition;
using OrganizationEntity.Core.Abstractions.Service;
using OrganizationEntityContracts.ViewModels.OrgEntityViewModel.PositionViewModels;

namespace OrganizationEntity.WebHost.Areas.Admin.RabbitMQ.PositionConsumer
{
    public class DeletePositionConsumer : IConsumer<DeletePositionRequest>
    {
        private readonly IPositionService _positionService;

        public DeletePositionConsumer(IPositionService positionService)
        {
            _positionService = positionService ?? throw new ArgumentNullException(nameof(positionService));
        }

        public async Task Consume(ConsumeContext<DeletePositionRequest> context)
        {
            var result = await _positionService.DeleteAsync(context.Message.PositionId, ClaimsPrincipal.Current);

            var response = result.Success ? new NotificationViewModel() : new NotificationViewModel(result.Errors, e: result.AspNetException);

            await context.RespondAsync<NotificationViewModel>(response);
        }
    }

    public class DeletePositionConsumerDefinition : ConsumerDefinition<DeletePositionConsumer>
    {
        public DeletePositionConsumerDefinition()
        {
            EndpointName = ApiShowConstants.DeletePosition;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<DeletePositionConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseRetry(x => x.Intervals(100, 500, 1000));
        }
    }
}