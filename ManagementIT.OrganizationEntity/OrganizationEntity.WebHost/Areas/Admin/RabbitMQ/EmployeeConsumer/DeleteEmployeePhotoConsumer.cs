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
    public class DeleteEmployeePhotoConsumer : IConsumer<DeleteEmployeePhotoRequest>
    {
        private readonly IEmployeeService _employeeService;

        public DeleteEmployeePhotoConsumer(IEmployeeService employeeService)
        {
            _employeeService = employeeService ?? throw new ArgumentNullException(nameof(employeeService));
        }

        public async Task Consume(ConsumeContext<DeleteEmployeePhotoRequest> context)
        {
            var result = await _employeeService.DeletePhotoAsync(context.Message.EmployeeId, ClaimsPrincipal.Current);

            var response = result.Success ? new NotificationViewModel() : new NotificationViewModel(result.Errors, e: result.AspNetException);

            await context.RespondAsync<NotificationViewModel>(response);
        }
    }

    public class DeleteEmployeePhotoConsumerDefinition : ConsumerDefinition<DeleteEmployeePhotoConsumer>
    {
        public DeleteEmployeePhotoConsumerDefinition()
        {
            EndpointName = ApiShowConstants.DeleteEmployeePhoto;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<DeleteEmployeePhotoConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseRetry(x => x.Intervals(100, 500, 1000));
        }
    }
}