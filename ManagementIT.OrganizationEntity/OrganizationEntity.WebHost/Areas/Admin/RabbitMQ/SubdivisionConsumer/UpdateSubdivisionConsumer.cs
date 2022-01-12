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
using OrganizationEntity.Core.Models.SubdivisionModels;
using OrganizationEntityContracts.ViewModels.OrgEntityViewModel.SubdivisionViewModels;

namespace OrganizationEntity.WebHost.Areas.Admin.RabbitMQ.SubdivisionConsumer
{
    public class UpdateSubdivisionConsumer : IConsumer<UpdateSubdivisionViewModel>
    {
        private readonly ISubdivisionService _subdivisionService;
        private readonly IMapper _mapper;

        public UpdateSubdivisionConsumer(ISubdivisionService subdivisionService, IMapper mapper)
        {
            _subdivisionService = subdivisionService ?? throw new ArgumentNullException(nameof(subdivisionService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task Consume(ConsumeContext<UpdateSubdivisionViewModel> context)
        {
            var existNameEntity = _subdivisionService.ExistEntityByName(context.Message.Name, context.Message.Id);
            if (!existNameEntity)
            {
                var model = new SubdivisionDTO(context.Message.Name, context.Message.Id);
                var result = await _subdivisionService.UpdateAsync(model, ClaimsPrincipal.Current);

                var response = result.Success ? new NotificationViewModel() : new NotificationViewModel(result.Errors, e: result.AspNetException);

                await context.RespondAsync<NotificationViewModel>(response);
            }
            else await context.RespondAsync<NotificationViewModel>(new NotificationViewModel(new[] { TypeOfErrors.ExistNameEntity }));
        }
    }

    public class UpdateSubdivisionConsumerDefinition : ConsumerDefinition<UpdateSubdivisionConsumer>
    {
        public UpdateSubdivisionConsumerDefinition()
        {
            EndpointName = ApiShowConstants.UpdateSubdivision;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<UpdateSubdivisionConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseRetry(x => x.Intervals(100, 500, 1000));
        }
    }
}