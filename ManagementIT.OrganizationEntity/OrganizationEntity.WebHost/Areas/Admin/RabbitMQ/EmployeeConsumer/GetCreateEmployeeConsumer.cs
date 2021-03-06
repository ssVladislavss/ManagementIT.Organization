using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
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
    public class GetCreateEmployeeConsumer : IConsumer<GetCreateEmployeeRequest>
    {
        private readonly IEmployeeService _employeeService;
        private readonly IMapper _mapper;

        public GetCreateEmployeeConsumer(IEmployeeService employeeService, IMapper mapper)
        {
            _employeeService = employeeService ?? throw new ArgumentNullException(nameof(employeeService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task Consume(ConsumeContext<GetCreateEmployeeRequest> context)
        {
            var result = await _employeeService.GetCreateAsync(ClaimsPrincipal.Current);
            var response = new GetCreateEmployeeResponse();

            if (!result.Success)
                response.Notification = new NotificationViewModel(result.Errors, e: result.AspNetException);
            else
            {
                response.Notification = new NotificationViewModel();
                response.Model = _mapper.Map<CreateEmployeeViewModel>(result.Data);
            }

            await context.RespondAsync<GetCreateEmployeeResponse>(response);
        }
    }

    public class GetCreateEmployeeConsumerDefinition : ConsumerDefinition<GetCreateEmployeeConsumer>
    {
        public GetCreateEmployeeConsumerDefinition()
        {
            EndpointName = ApiShowConstants.GetCreateEmployee;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<GetCreateEmployeeConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseRetry(x => x.Intervals(100, 500, 1000));
        }
    }
}