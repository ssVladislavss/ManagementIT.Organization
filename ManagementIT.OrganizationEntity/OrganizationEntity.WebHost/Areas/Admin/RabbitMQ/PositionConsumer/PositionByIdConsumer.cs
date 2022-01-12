using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
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
    public class PositionByIdConsumer : IConsumer<PositionByIdRequest>
    {
        private readonly IPositionService _positionService;
        private readonly IMapper _mapper;

        public PositionByIdConsumer(IPositionService positionService, IMapper mapper)
        {
            _positionService = positionService ?? throw new ArgumentNullException(nameof(positionService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task Consume(ConsumeContext<PositionByIdRequest> context)
        {
            var result = await _positionService.GetByIdAsync(context.Message.PositionId, ClaimsPrincipal.Current);
            var response = new PositionByIdResponse();

            if (!result.Success)
                response.Notification = new NotificationViewModel(result.Errors, e: result.AspNetException);
            else
            {
                response.Notification = new NotificationViewModel();
                response.Model = _mapper.Map<PositionViewModel>(result.Data);
            }

            await context.RespondAsync<PositionByIdResponse>(response);
        }
    }

    public class PositionByIdConsumerDefinition : ConsumerDefinition<PositionByIdConsumer>
    {
        public PositionByIdConsumerDefinition()
        {
            EndpointName = ApiShowConstants.GetByIdPosition;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<PositionByIdConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseRetry(x => x.Intervals(100, 500, 1000));
        }
    }
}