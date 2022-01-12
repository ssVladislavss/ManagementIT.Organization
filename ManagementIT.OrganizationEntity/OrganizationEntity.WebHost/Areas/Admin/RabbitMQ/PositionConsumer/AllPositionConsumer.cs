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
    public class AllPositionConsumer : IConsumer<PositionViewModel>
    {
        private readonly IPositionService _positionService;
        private readonly IMapper _mapper;

        public AllPositionConsumer(IPositionService positionService, IMapper mapper)
        {
            _positionService = positionService ?? throw new ArgumentNullException(nameof(positionService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task Consume(ConsumeContext<PositionViewModel> context)
        {
            var result = await _positionService.GetAllAsync(ClaimsPrincipal.Current);
            var response = new AllPositionResponse();

            if (!result.Success)
                response.Notification = new NotificationViewModel(result.Errors, e: result.AspNetException);
            else
            {
                response.Notification = new NotificationViewModel();
                response.Model = _mapper.Map<IEnumerable<PositionViewModel>>(result.Data);
            }

            await context.RespondAsync<AllPositionResponse>(response);
        }
    }

    public class AllPositionConsumerDefinition : ConsumerDefinition<AllPositionConsumer>
    {
        public AllPositionConsumerDefinition()
        {
            EndpointName = ApiShowConstants.GetAllPosition;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<AllPositionConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseRetry(x => x.Intervals(100, 500, 1000));
        }
    }
}