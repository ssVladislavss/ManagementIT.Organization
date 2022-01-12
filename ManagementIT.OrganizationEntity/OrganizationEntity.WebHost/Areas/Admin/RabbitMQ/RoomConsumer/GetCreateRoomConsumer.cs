using System;
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
using OrganizationEntity.Core.Models.RoomModels;
using OrganizationEntityContracts.ViewModels.OrgEntityViewModel.RoomViewModels;

namespace OrganizationEntity.WebHost.Areas.Admin.RabbitMQ.RoomConsumer
{
    public class GetCreateRoomConsumer : IConsumer<GetCreateRoomRequest>
    {
        private readonly IRoomService _roomService;
        private readonly IMapper _mapper;

        public GetCreateRoomConsumer(IRoomService roomService, IMapper mapper)
        {
            _roomService = roomService ?? throw new ArgumentNullException(nameof(roomService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task Consume(ConsumeContext<GetCreateRoomRequest> context)
        {
            var result = await _roomService.GetCreateRoomAsync(ClaimsPrincipal.Current);
            var response = new GetCreateRoomResponse();

            if (!result.Success)
                response.Notification = new NotificationViewModel(result.Errors, e: result.AspNetException);
            else
            {
                response.Notification = new NotificationViewModel();
                response.Model = _mapper.Map<CreateRoomViewModel>(result.Data);
            }

            await context.RespondAsync<GetCreateRoomResponse>(response);
        }
    }

    public class GetCreateRoomConsumerDefinition : ConsumerDefinition<GetCreateRoomConsumer>
    {
        public GetCreateRoomConsumerDefinition()
        {
            EndpointName = ApiShowConstants.GetCreateRoom;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<GetCreateRoomConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseRetry(x => x.Intervals(100, 500, 1000));
        }
    }
}