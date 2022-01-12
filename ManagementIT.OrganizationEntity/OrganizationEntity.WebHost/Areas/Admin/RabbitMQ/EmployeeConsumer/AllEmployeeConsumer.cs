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
    public class AllEmployeeConsumer : IConsumer<EmployeeViewModel>
    {
        private readonly IEmployeeService _employeeService;
        private readonly IMapper _mapper;

        public AllEmployeeConsumer(IEmployeeService employeeService, IMapper mapper)
        {
            _employeeService = employeeService ?? throw new ArgumentNullException(nameof(employeeService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task Consume(ConsumeContext<EmployeeViewModel> context)
        {
            var result = await _employeeService.GetAllAsync(ClaimsPrincipal.Current);
            var response = new AllEmployeeResponse();
            if (!result.Success)
                response.Notification = new NotificationViewModel(result.Errors, e: result.AspNetException);
            else
            {
                response.Notification = new NotificationViewModel();
                response.Model = _mapper.Map<IEnumerable<EmployeeViewModel>>(result.Data);
            }

            await context.RespondAsync<AllEmployeeResponse>(response);
        }
    }

    public class AllEmployeeConsumerDefinition : ConsumerDefinition<AllEmployeeConsumer>
    {
        public AllEmployeeConsumerDefinition() => EndpointName = ApiShowConstants.GetAllEmployee;
        
        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<AllEmployeeConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseRetry(x => x.Intervals(100, 500, 1000));
        }
    }
}