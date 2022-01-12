using System;
using System.Collections.Generic;
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
using OrganizationEntityContracts.ViewModels.OrgEntityViewModel.EmployeeViewModels;

namespace OrganizationEntity.WebHost.Areas.Admin.RabbitMQ.EmployeeConsumer
{
    public class EmployeebyIdConsumer : IConsumer<EmployeeByIdRequest>
    {
        private readonly IEmployeeService _employeeService;
        private readonly IMapper _mapper;

        public EmployeebyIdConsumer(IEmployeeService employeeService, IMapper mapper)
        {
            _employeeService = employeeService ?? throw new ArgumentNullException(nameof(employeeService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task Consume(ConsumeContext<EmployeeByIdRequest> context)
        {
            var result = await _employeeService.GetByIdAsync(context.Message.EmployeeId, ClaimsPrincipal.Current);
            var response = new EmployeeByIdResponse();
            if (!result.Success)
                response.Notification = new NotificationViewModel(result.Errors, e: result.AspNetException);
            else
            {
                response.Notification = new NotificationViewModel();
                response.Model = _mapper.Map<EmployeeViewModel>(result.Data);
            }

            await context.RespondAsync<EmployeeByIdResponse>(response);
        }
    }

    public class EmployeebyIdConsumerDefinition : ConsumerDefinition<EmployeebyIdConsumer>
    {
        public EmployeebyIdConsumerDefinition()
        {
            EndpointName = ApiShowConstants.GetByIdEmployee;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<EmployeebyIdConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseRetry(x => x.Intervals(100, 500, 1000));
        }
    }
}