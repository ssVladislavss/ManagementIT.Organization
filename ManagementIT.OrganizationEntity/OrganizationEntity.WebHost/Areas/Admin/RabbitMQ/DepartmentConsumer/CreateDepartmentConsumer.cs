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
using OrganizationEntity.Core.Models.DepartmentModels;
using OrganizationEntityContracts.ViewModels.OrgEntityViewModel.DepartmentViewModels;

namespace OrganizationEntity.WebHost.Areas.Admin.RabbitMQ.DepartmentConsumer
{
    public class CreateDepartmentConsumer : IConsumer<CreateDepartmentViewModel>
    {
        private readonly IDepartmentService _departmentService;

        public CreateDepartmentConsumer(IDepartmentService departmentService)
        {
            _departmentService = departmentService ?? throw new ArgumentNullException(nameof(departmentService));
        }

        public async Task Consume(ConsumeContext<CreateDepartmentViewModel> context)
        {
            var existName = _departmentService.ExistEntityByName(context.Message.Name);
            if (!existName)
            {
                var model = new DepartmentDTO(context.Message.Name, context.Message.SubdivisionId);
                var result = await _departmentService.AddAsync(model, ClaimsPrincipal.Current);

                var response = result.Success
                    ? new NotificationViewModel()
                    : new NotificationViewModel(result.Errors, e: result.AspNetException);

                await context.RespondAsync<NotificationViewModel>(response);
            }
            else
                await context.RespondAsync<NotificationViewModel>(
                    new NotificationViewModel(new[] { TypeOfErrors.ExistNameEntity }));
        }
    }

    public class CreateDepartmentConsumerDefinition : ConsumerDefinition<CreateDepartmentConsumer>
    {
        public CreateDepartmentConsumerDefinition()
        {
            EndpointName = ApiShowConstants.CreateDepartment;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<CreateDepartmentConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseRetry(x => x.Intervals(100, 500, 1000));
        }
    }
}