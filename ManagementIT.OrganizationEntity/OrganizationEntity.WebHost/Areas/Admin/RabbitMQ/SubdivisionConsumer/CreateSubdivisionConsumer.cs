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
    public class CreateSubdivisionConsumer : IConsumer<CreateSubdivisionViewModel>
    {
        private readonly ISubdivisionService _subdivisionService;
        private readonly IMapper _mapper;

        public CreateSubdivisionConsumer(ISubdivisionService subdivisionService, IMapper mapper)
        {
            _subdivisionService = subdivisionService ?? throw new ArgumentNullException(nameof(subdivisionService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task Consume(ConsumeContext<CreateSubdivisionViewModel> context)
        {
            var existNameEntity = _subdivisionService.ExistEntityByName(context.Message.Name);
            if (!existNameEntity)
            {
                var model = new SubdivisionDTO(context.Message.Name);
                var result = await _subdivisionService.AddAsync(model, ClaimsPrincipal.Current);

                var response = result.Success ? new NotificationViewModel() : new NotificationViewModel(result.Errors, e: result.AspNetException);

                await context.RespondAsync<NotificationViewModel>(response);
            }
            else await context.RespondAsync<NotificationViewModel>(new NotificationViewModel(new[] { TypeOfErrors.ExistNameEntity }));
        }
    }

    public class CreateSubdivisionConsumerDefinition : ConsumerDefinition<CreateSubdivisionConsumer>
    {
        public CreateSubdivisionConsumerDefinition()
        {
            EndpointName = ApiShowConstants.CreateSubdivision;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<CreateSubdivisionConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseRetry(x => x.Intervals(100, 500, 1000));
        }
    }
}