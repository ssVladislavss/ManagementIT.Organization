using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Contracts.Constants;
using Contracts.Enums;
using Contracts.ResponseModels;
using GreenPipes;
using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.Definition;
using OrganizationEntity.Core.Abstractions.Service;
using OrganizationEntityContracts.ViewModels.OrgEntityViewModel.DepartmentViewModels;

namespace OrganizationEntity.WebHost.Areas.Admin.RabbitMQ.DepartmentConsumer
{
    public class DeleteDepartmentConsumer : IConsumer<DeleteDepartmentRequest>
    {
        private readonly IDepartmentService _departmentService;

        public DeleteDepartmentConsumer(IDepartmentService departmentService)
        {
            _departmentService = departmentService ?? throw new ArgumentNullException(nameof(departmentService));
        }

        public async Task Consume(ConsumeContext<DeleteDepartmentRequest> context)
        {
            var result = await _departmentService.DeleteAsync(context.Message.DepartmentId, ClaimsPrincipal.Current);

            var response = result.Success
                ? new NotificationViewModel()
                : new NotificationViewModel(result.Errors, e: result.AspNetException);

            await context.RespondAsync<NotificationViewModel>(response);
        }
    }

    public class DeleteDepartmentConsumerDefinition : ConsumerDefinition<DeleteDepartmentConsumer>
    {
        public DeleteDepartmentConsumerDefinition()
        {
            EndpointName = ApiShowConstants.DeleteDepartment;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<DeleteDepartmentConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseRetry(x => x.Intervals(100, 500, 1000));
        }
    }
}