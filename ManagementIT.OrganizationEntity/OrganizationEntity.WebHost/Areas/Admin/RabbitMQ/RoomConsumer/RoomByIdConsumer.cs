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
using OrganizationEntityContracts.ViewModels.OrgEntityViewModel.RoomViewModels;

namespace OrganizationEntity.WebHost.Areas.Admin.RabbitMQ.RoomConsumer
{
    public class RoomByIdConsumer : IConsumer<RoomByIdRequest>
    {
        private readonly IRoomService _roomService;
        private readonly IMapper _mapper;

        public RoomByIdConsumer(IRoomService roomService, IMapper mapper)
        {
            _roomService = roomService ?? throw new ArgumentNullException(nameof(roomService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task Consume(ConsumeContext<RoomByIdRequest> context)
        {
            var result = await _roomService.GetByIdAsync(context.Message.RoomId, ClaimsPrincipal.Current);
            var response = new RoomByIdResponse();

            if (!result.Success)
                response.Notification = new NotificationViewModel(result.Errors, e: result.AspNetException);
            else
            {
                response.Notification = new NotificationViewModel();
                response.Model = _mapper.Map<RoomViewModel>(result.Data);
            }

            await context.RespondAsync<RoomByIdResponse>(response);
        }
    }

    public class RoomByIdConsumerDefinition : ConsumerDefinition<RoomByIdConsumer>
    {
        public RoomByIdConsumerDefinition()
        {
            EndpointName = ApiShowConstants.GetByIdRoom;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<RoomByIdConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseRetry(x => x.Intervals(100, 500, 1000));
        }
    }
}