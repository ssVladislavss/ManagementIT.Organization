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
using OrganizationEntity.Core.Models.EmployeeModels;
using OrganizationEntityContracts.ViewModels.OrgEntityViewModel.DepartmentViewModels;
using OrganizationEntityContracts.ViewModels.OrgEntityViewModel.EmployeeViewModels;

namespace OrganizationEntity.WebHost.Areas.Admin.RabbitMQ.EmployeeConsumer
{
    public class UpdateEmployeeConsumer : IConsumer<UpdateEmployeeViewModel>
    {
        private readonly IEmployeeService _employeeService;
        private readonly IMapper _mapper;

        public UpdateEmployeeConsumer(IEmployeeService employeeService, IMapper mapper)
        {
            _employeeService = employeeService ?? throw new ArgumentNullException(nameof(employeeService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task Consume(ConsumeContext<UpdateEmployeeViewModel> context)
        {
            var model = _mapper.Map<EmployeeDTO>(context.Message);
            var result = await _employeeService.UpdateAsync(model, ClaimsPrincipal.Current);

            var response = result.Success
                ? new NotificationViewModel()
                : new NotificationViewModel(result.Errors, e: result.AspNetException);

            await context.RespondAsync<NotificationViewModel>(response);
        }
    }

    public class UpdateEmployeeConsumerDefinition : ConsumerDefinition<UpdateEmployeeConsumer>
    {
        public UpdateEmployeeConsumerDefinition()
        {
            EndpointName = ApiShowConstants.UpdateEmployee;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<UpdateEmployeeConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseRetry(x => x.Intervals(100, 500, 1000));
        }
    }
}