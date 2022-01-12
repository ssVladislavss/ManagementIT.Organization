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
using OrganizationEntityContracts.ViewModels.OrgEntityViewModel.EmployeeViewModels;

namespace OrganizationEntity.WebHost.Areas.Admin.RabbitMQ.EmployeeConsumer
{
    public class GetUpdateEmployeeConsumer : IConsumer<GetUpdateEmployeeRequest>
    {
        private readonly IEmployeeService _employeeService;
        private readonly IMapper _mapper;

        public GetUpdateEmployeeConsumer(IEmployeeService employeeService, IMapper mapper)
        {
            _employeeService = employeeService ?? throw new ArgumentNullException(nameof(employeeService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task Consume(ConsumeContext<GetUpdateEmployeeRequest> context)
        {
            var result = await _employeeService.GetUpdateAsync(context.Message.EmployeeId, ClaimsPrincipal.Current);
            var response = new GetUpdateEmployeeResponse();

            if (!result.Success)
                response.Notification = new NotificationViewModel(result.Errors, e: result.AspNetException);
            else
            {
                response.Notification = new NotificationViewModel();
                response.Model = _mapper.Map<UpdateEmployeeViewModel>(result.Data);
            }

            await context.RespondAsync<GetUpdateEmployeeResponse>(response);
        }
    }

    public class GetUpdateEmployeeConsumerDefinition : ConsumerDefinition<GetUpdateEmployeeConsumer>
    {
        public GetUpdateEmployeeConsumerDefinition()
        {
            EndpointName = ApiShowConstants.GetUpdateEmployee;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<GetUpdateEmployeeConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseRetry(x => x.Intervals(100, 500, 1000));
        }
    }
}