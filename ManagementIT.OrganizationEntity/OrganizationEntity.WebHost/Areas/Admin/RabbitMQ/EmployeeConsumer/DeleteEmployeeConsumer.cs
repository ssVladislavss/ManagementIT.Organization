using System;
using System.Security.Claims;
using System.Threading.Tasks;
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
    public class DeleteEmployeeConsumer : IConsumer<DeleteEmployeeRequest>
    {
        private readonly IEmployeeService _employeeService;

        public DeleteEmployeeConsumer(IEmployeeService employeeService)
        {
            _employeeService = employeeService ?? throw new ArgumentNullException(nameof(employeeService));
        }

        public async Task Consume(ConsumeContext<DeleteEmployeeRequest> context)
        {
            var result = await _employeeService.DeleteAsync(context.Message.EmployeeId, ClaimsPrincipal.Current);

            var response = result.Success ? new NotificationViewModel() : new NotificationViewModel(result.Errors, e: result.AspNetException);

            await context.RespondAsync<NotificationViewModel>(response);
        }
    }

    public class DeleteEmployeeConsumerDefinition : ConsumerDefinition<DeleteEmployeeConsumer>
    {
        public DeleteEmployeeConsumerDefinition()
        {
            EndpointName = ApiShowConstants.DeleteEmployee;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<DeleteEmployeeConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseRetry(x => x.Intervals(100, 500, 1000));
        }
    }
}