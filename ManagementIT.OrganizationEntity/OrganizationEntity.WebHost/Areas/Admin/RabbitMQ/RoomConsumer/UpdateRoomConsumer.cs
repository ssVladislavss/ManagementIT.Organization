using System;
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
using OrganizationEntity.Core.Models.RoomModels;
using OrganizationEntityContracts.ViewModels.OrgEntityViewModel.RoomViewModels;

namespace OrganizationEntity.WebHost.Areas.Admin.RabbitMQ.RoomConsumer
{
    public class UpdateRoomConsumer : IConsumer<UpdateRoomViewModel>
    {
        private readonly IRoomService _roomService;
        private readonly IMapper _mapper;

        public UpdateRoomConsumer(IRoomService roomService, IMapper mapper)
        {
            _roomService = roomService ?? throw new ArgumentNullException(nameof(roomService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task Consume(ConsumeContext<UpdateRoomViewModel> context)
        {
            var existName = _roomService.ExistEntityByName(context.Message.Name, context.Message.Id);
            if (!existName)
            {
                var model = _mapper.Map<RoomDTO>(context.Message);
                var result = await _roomService.UpdateAsync(model, ClaimsPrincipal.Current);

                var response = result.Success ? new NotificationViewModel() : new NotificationViewModel(result.Errors, e: result.AspNetException);

                await context.RespondAsync<NotificationViewModel>(response);
            }
            else await context.RespondAsync<NotificationViewModel>(new NotificationViewModel(new[] { TypeOfErrors.ExistNameEntity }));
                
        }
    }

    public class UpdateRoomConsumerDefinition : ConsumerDefinition<UpdateRoomConsumer>
    {
        public UpdateRoomConsumerDefinition()
        {
            EndpointName = ApiShowConstants.UpdateRoom;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<UpdateRoomConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseRetry(x => x.Intervals(100, 500, 1000));
        }
    }
}