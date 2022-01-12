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
using OrganizationEntityContracts.ViewModels.OrgEntityViewModel.RoomViewModels;

namespace OrganizationEntity.WebHost.Areas.Admin.RabbitMQ.RoomConsumer
{
    public class GetUpdateRoomConsumer : IConsumer<GetUpdateRoomRequest>
    {
        private readonly IRoomService _roomService;
        private readonly IMapper _mapper;

        public GetUpdateRoomConsumer(IRoomService roomService, IMapper mapper)
        {
            _roomService = roomService ?? throw new ArgumentNullException(nameof(roomService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task Consume(ConsumeContext<GetUpdateRoomRequest> context)
        {
            var result = await _roomService.GetUpdateRoomAsync(context.Message.RoomId, ClaimsPrincipal.Current);
            var response = new GetUpdateRoomResponse();

            if (!result.Success)
                response.Notification = new NotificationViewModel(result.Errors, e: result.AspNetException);
            else
            {
                response.Notification = new NotificationViewModel();
                response.Model = _mapper.Map<UpdateRoomViewModel>(result.Data);
            }

            await context.RespondAsync<GetUpdateRoomResponse>(response);
        }
    }

    public class GetUpdateRoomConsumerDefinition : ConsumerDefinition<GetUpdateRoomConsumer>
    {
        public GetUpdateRoomConsumerDefinition()
        {
            EndpointName = ApiShowConstants.GetUpdateRoom;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<GetUpdateRoomConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseRetry(x => x.Intervals(100, 500, 1000));
        }
    }
}