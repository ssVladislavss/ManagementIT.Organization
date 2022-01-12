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
    public class AllRoomConsumer : IConsumer<RoomViewModel>
    {
        private readonly IRoomService _roomService;
        private readonly IMapper _mapper;

        public AllRoomConsumer(IRoomService roomService, IMapper mapper)
        {
            _roomService = roomService ?? throw new ArgumentNullException(nameof(roomService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task Consume(ConsumeContext<RoomViewModel> context)
        {
            var result = await _roomService.GetAllAsync(ClaimsPrincipal.Current);
            var response = new AllRoomResponse();

            if (!result.Success)
                response.Notification = new NotificationViewModel(result.Errors, e: result.AspNetException);
            else
            {
                response.Notification = new NotificationViewModel();
                response.Model = _mapper.Map<IEnumerable<RoomViewModel>>(result.Data);
            }

            await context.RespondAsync<AllRoomResponse>(response);
        }
    }

    public class AllRoomConsumerDefinition : ConsumerDefinition<AllRoomConsumer>
    {
        public AllRoomConsumerDefinition()
        {
            EndpointName = ApiShowConstants.GetAllRoom;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<AllRoomConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseRetry(x => x.Intervals(100, 500, 1000));
        }
    }
}