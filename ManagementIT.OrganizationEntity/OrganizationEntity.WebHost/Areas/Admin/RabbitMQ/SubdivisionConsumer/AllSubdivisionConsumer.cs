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
using OrganizationEntityContracts.ViewModels.OrgEntityViewModel.RoomViewModels;
using OrganizationEntityContracts.ViewModels.OrgEntityViewModel.SubdivisionViewModels;

namespace OrganizationEntity.WebHost.Areas.Admin.RabbitMQ.SubdivisionConsumer
{
    public class AllSubdivisionConsumer : IConsumer<SubdivisionViewModel>
    {
        private readonly ISubdivisionService _subdivisionService;
        private readonly IMapper _mapper;

        public AllSubdivisionConsumer(ISubdivisionService subdivisionService, IMapper mapper)
        {
            _subdivisionService = subdivisionService ?? throw new ArgumentNullException(nameof(subdivisionService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task Consume(ConsumeContext<SubdivisionViewModel> context)
        {
            var result = await _subdivisionService.GetAllAsync(ClaimsPrincipal.Current);
            var response = new AllSubdivisionResponse();

            if (!result.Success)
                response.Notification = new NotificationViewModel(result.Errors, e: result.AspNetException);
            else
            {
                response.Notification = new NotificationViewModel();
                response.Model = _mapper.Map<IEnumerable<SubdivisionViewModel>>(result.Data);
            }

            await context.RespondAsync<AllSubdivisionResponse>(response);
        }
    }

    public class AllSubdivisionConsumerDefinition : ConsumerDefinition<AllSubdivisionConsumer>
    {
        public AllSubdivisionConsumerDefinition()
        {
            EndpointName = ApiShowConstants.GetAllSubdivision;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<AllSubdivisionConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseRetry(x => x.Intervals(100, 500, 1000));
        }
    }
}