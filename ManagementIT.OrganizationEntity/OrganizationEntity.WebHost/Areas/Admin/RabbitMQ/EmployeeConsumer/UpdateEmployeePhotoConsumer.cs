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
    public class UpdateEmployeePhotoConsumer : IConsumer<CreateOrEditEmployeePhotoViewModel>
    {
        private readonly IEmployeeService _employeeService;

        public UpdateEmployeePhotoConsumer(IEmployeeService employeeService)
        {
            _employeeService = employeeService ?? throw new ArgumentNullException(nameof(employeeService));
        }

        public async Task Consume(ConsumeContext<CreateOrEditEmployeePhotoViewModel> context)
        {
            var result = await _employeeService.UpdatePhoto(context.Message.EmployeeId, context.Message.BytesPhoto, ClaimsPrincipal.Current);
            var response = result.Success ? new NotificationViewModel() : new NotificationViewModel(result.Errors, e: result.AspNetException);
            await context.RespondAsync<NotificationViewModel>(response);
        }
    }

    public class UpdateEmployeePhotoConsumerDefinition : ConsumerDefinition<UpdateEmployeePhotoConsumer>
    {
        public UpdateEmployeePhotoConsumerDefinition()
        {
            EndpointName = ApiShowConstants.UpdateEmployeePhoto;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<UpdateEmployeePhotoConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseRetry(x => x.Intervals(100, 500, 1000));
        }
    }
}