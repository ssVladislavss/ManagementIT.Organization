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
using OrganizationEntity.Core.Models.SubdivisionModels;
using OrganizationEntityContracts.ViewModels.OrgEntityViewModel.SubdivisionViewModels;

namespace OrganizationEntity.WebHost.Areas.Admin.RabbitMQ.SubdivisionConsumer
{
    public class DeleteSubdivisionConsumer : IConsumer<DeleteSubdivisionRequest>
    {
        private readonly ISubdivisionService _subdivisionService;
        private readonly IMapper _mapper;

        public DeleteSubdivisionConsumer(ISubdivisionService subdivisionService, IMapper mapper)
        {
            _subdivisionService = subdivisionService ?? throw new ArgumentNullException(nameof(subdivisionService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task Consume(ConsumeContext<DeleteSubdivisionRequest> context)
        {
            var result = await _subdivisionService.DeleteAsync(context.Message.SubdivisionId, ClaimsPrincipal.Current);

            var response = result.Success ? new NotificationViewModel() : new NotificationViewModel(result.Errors, e: result.AspNetException);

            await context.RespondAsync<NotificationViewModel>(response);
        }
    }

    public class DeleteSubdivisionConsumerDefinition : ConsumerDefinition<DeleteSubdivisionConsumer>
    {
        public DeleteSubdivisionConsumerDefinition()
        {
            EndpointName = ApiShowConstants.DeleteSubdivision;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<DeleteSubdivisionConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseRetry(x => x.Intervals(100, 500, 1000));
        }
    }
}