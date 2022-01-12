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
    public class AllDepartmentConsumer : IConsumer<DepartmentViewModel>
    {
        private readonly IDepartmentService _departmentService;
        private readonly IMapper _mapper;

        public AllDepartmentConsumer(IDepartmentService departmentService, IMapper mapper)
        {
            _departmentService = departmentService ?? throw new ArgumentNullException(nameof(departmentService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task Consume(ConsumeContext<DepartmentViewModel> context)
        {
            var result = await _departmentService.GetAllAsync(ClaimsPrincipal.Current);

            var response = new AllDepartmentResponse();
            if (!result.Success)
                response.Notification = new NotificationViewModel(result.Errors, e: result.AspNetException);
            else
            {
                response.Notification = new NotificationViewModel();
                response.Model = _mapper.Map<IEnumerable<DepartmentViewModel>>(result.Data);
            }

            await context.RespondAsync<AllDepartmentResponse>(response);
        }
    }

    public class AllDepartmentConsumerDefinition : ConsumerDefinition<AllDepartmentConsumer>
    {
        public AllDepartmentConsumerDefinition()
        {
            EndpointName = ApiShowConstants.GetAllDepartment;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<AllDepartmentConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseRetry(x => x.Intervals(100, 500, 1000));
        }
    }
}