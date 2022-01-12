using System;
using System.Collections.Generic;
using System.Linq;
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
using OrganizationEntity.Core.Models.PositionModels;
using OrganizationEntityContracts.ViewModels.OrgEntityViewModel.PositionViewModels;

namespace OrganizationEntity.WebHost.Areas.Admin.RabbitMQ.PositionConsumer
{
    public class CreatePositionConsumer : IConsumer<CreatePositionViewModel>
    {
        private readonly IPositionService _positionService;

        public CreatePositionConsumer(IPositionService positionService)
        {
            _positionService = positionService ?? throw new ArgumentNullException(nameof(positionService));
        }

        public async Task Consume(ConsumeContext<CreatePositionViewModel> context)
        {
            var existNameEntity = _positionService.ExistEntityByName(context.Message.Name);
            if (!existNameEntity)
            {
                var model = new PositionDTO(context.Message.Name);
                var result = await _positionService.AddAsync(model, ClaimsPrincipal.Current);

                var response = result.Success ? new NotificationViewModel() : new NotificationViewModel(result.Errors, e: result.AspNetException);

                await context.RespondAsync<NotificationViewModel>(response);
            }
            else 
                await context.RespondAsync<NotificationViewModel>(new NotificationViewModel(new[] { TypeOfErrors.ExistNameEntity }));
        }
    }

    public class CreatePositionConsumerDefinition : ConsumerDefinition<CreatePositionConsumer>
    {
        public CreatePositionConsumerDefinition()
        {
            EndpointName = ApiShowConstants.CreatePosition;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<CreatePositionConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseRetry(x => x.Intervals(100, 500, 1000));
        }
    }
}
