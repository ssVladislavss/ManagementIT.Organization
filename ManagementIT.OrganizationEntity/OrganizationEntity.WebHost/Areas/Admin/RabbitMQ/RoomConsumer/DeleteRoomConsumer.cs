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
using OrganizationEntity.Core.Models.RoomModels;
using OrganizationEntityContracts.ViewModels.OrgEntityViewModel.RoomViewModels;

namespace OrganizationEntity.WebHost.Areas.Admin.RabbitMQ.RoomConsumer
{
    public class DeleteRoomConsumer : IConsumer<DeleteRoomRequest>
    {
        private readonly IRoomService _roomService;

        public DeleteRoomConsumer(IRoomService roomService)
        {
            _roomService = roomService ?? throw new ArgumentNullException(nameof(roomService));
        }

        public async Task Consume(ConsumeContext<DeleteRoomRequest> context)
        {
            var result = await _roomService.DeleteAsync(context.Message.RoomId, ClaimsPrincipal.Current);

            var response = result.Success ? new NotificationViewModel() : new NotificationViewModel(result.Errors, e: result.AspNetException);

            await context.RespondAsync<NotificationViewModel>(response);
        }
    }

    public class DeleteRoomConsumerDefinition : ConsumerDefinition<DeleteRoomConsumer>
    {
        public DeleteRoomConsumerDefinition()
        {
            EndpointName = ApiShowConstants.DeleteRoom;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<DeleteRoomConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseRetry(x => x.Intervals(100, 500, 1000));
        }
    }
}