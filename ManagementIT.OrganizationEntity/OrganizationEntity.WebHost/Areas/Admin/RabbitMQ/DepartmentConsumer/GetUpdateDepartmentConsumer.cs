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
using OrganizationEntityContracts.ViewModels.OrgEntityViewModel.DepartmentViewModels;

namespace OrganizationEntity.WebHost.Areas.Admin.RabbitMQ.DepartmentConsumer
{
    public class GetUpdateDepartmentConsumer : IConsumer<GetUpdateDepartmentRequest>
    {
        private readonly IDepartmentService _departmentService;
        private readonly IMapper _mapper;

        public GetUpdateDepartmentConsumer(IDepartmentService departmentService, IMapper mapper)
        {
            _departmentService = departmentService ?? throw new ArgumentNullException(nameof(departmentService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task Consume(ConsumeContext<GetUpdateDepartmentRequest> context)
        {
            var result = await _departmentService.GetUpdateDeptAsync(context.Message.DepartmentId ,ClaimsPrincipal.Current);
            var response = new GetUpdateDepartmentResponse();

            if (!result.Success)
                response.Notification = new NotificationViewModel(result.Errors, e: result.AspNetException);
            else
            {
                response.Notification = new NotificationViewModel();
                response.Model = _mapper.Map<UpdateDepartmentViewModel>(result.Data);
            }

            await context.RespondAsync<GetUpdateDepartmentResponse>(response);
        }
    }

    public class GetUpdateDepartmentConsumerDefinition : ConsumerDefinition<GetUpdateDepartmentConsumer>
    {
        public GetUpdateDepartmentConsumerDefinition()
        {
            EndpointName = ApiShowConstants.GetUpdateDepartment;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<GetUpdateDepartmentConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseRetry(x => x.Intervals(100, 500, 1000));
        }
    }
}