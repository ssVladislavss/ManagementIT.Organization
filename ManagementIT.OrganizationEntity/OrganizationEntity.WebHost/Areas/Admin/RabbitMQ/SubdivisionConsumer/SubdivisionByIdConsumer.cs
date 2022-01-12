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
using OrganizationEntityContracts.ViewModels.OrgEntityViewModel.SubdivisionViewModels;

namespace OrganizationEntity.WebHost.Areas.Admin.RabbitMQ.SubdivisionConsumer
{
    public class SubdivisionByIdConsumer : IConsumer<SubdivisionByIdRequest>
    {
        private readonly ISubdivisionService _subdivisionService;
        private readonly IMapper _mapper;

        public SubdivisionByIdConsumer(ISubdivisionService subdivisionService, IMapper mapper)
        {
            _subdivisionService = subdivisionService ?? throw new ArgumentNullException(nameof(subdivisionService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task Consume(ConsumeContext<SubdivisionByIdRequest> context)
        {
            var result = await _subdivisionService.GetByIdAsync(context.Message.SubdivisionId, ClaimsPrincipal.Current);
            var response = new SubdivisionByIdResponse();

            if (!result.Success)
                response.Notification = new NotificationViewModel(result.Errors, e: result.AspNetException);
            else
            {
                response.Notification = new NotificationViewModel();
                response.Model = _mapper.Map<SubdivisionViewModel>(result.Data);
            }

            await context.RespondAsync<SubdivisionByIdResponse>(response);
        }
    }

    public class SubdivisionByIdConsumerDefinition : ConsumerDefinition<SubdivisionByIdConsumer>
    {
        public SubdivisionByIdConsumerDefinition()
        {
            EndpointName = ApiShowConstants.GetByIdSubdivision;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<SubdivisionByIdConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseRetry(x => x.Intervals(100, 500, 1000));
        }
    }
}