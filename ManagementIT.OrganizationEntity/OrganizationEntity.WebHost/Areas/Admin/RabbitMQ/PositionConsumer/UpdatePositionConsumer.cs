using System;
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
using OrganizationEntity.Core.Models.PositionModels;
using OrganizationEntityContracts.ViewModels.OrgEntityViewModel.PositionViewModels;

namespace OrganizationEntity.WebHost.Areas.Admin.RabbitMQ.PositionConsumer
{
    public class UpdatePositionConsumer : IConsumer<UpdatePositionViewModel>
    {
        private readonly IPositionService _positionService;

        public UpdatePositionConsumer(IPositionService positionService)
        {
            _positionService = positionService ?? throw new ArgumentNullException(nameof(positionService));
        }

        public async Task Consume(ConsumeContext<UpdatePositionViewModel> context)
        {
            var existNameEntity = _positionService.ExistEntityByName(context.Message.Name, context.Message.Id);
            if (!existNameEntity)
            {
                var model = new PositionDTO(context.Message.Name, context.Message.Id);
                var result = await _positionService.UpdateAsync(model, ClaimsPrincipal.Current);

                var response = result.Success ? new NotificationViewModel() : new NotificationViewModel(result.Errors, e: result.AspNetException);

                await context.RespondAsync<NotificationViewModel>(response);
            }
            else
                await context.RespondAsync<NotificationViewModel>(new NotificationViewModel(new[] { TypeOfErrors.ExistNameEntity }));
        }
    }

    public class UpdatePositionConsumerDefinition : ConsumerDefinition<UpdatePositionConsumer>
    {
        public UpdatePositionConsumerDefinition()
        {
            EndpointName = ApiShowConstants.UpdatePosition;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<UpdatePositionConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseRetry(x => x.Intervals(100, 500, 1000));
        }
    }
}