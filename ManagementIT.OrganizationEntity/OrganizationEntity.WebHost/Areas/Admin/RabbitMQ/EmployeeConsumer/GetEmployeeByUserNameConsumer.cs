using AutoMapper;
using Contracts.ResponseModels;
using MassTransit;
using OrganizationEntity.Core.Abstractions.Service;
using OrganizationEntityContracts.ViewModels.OrgEntityViewModel.EmployeeViewModels;
using System.Threading.Tasks;

namespace OrganizationEntity.WebHost.Areas.Admin.RabbitMQ.EmployeeConsumer
{
    public class GetEmployeeByUserNameConsumer : IConsumer<GetEmployeeByUserNameRequest>
    {
        private readonly IEmployeeService _employeeService;
        private readonly IMapper _mapper;
        public GetEmployeeByUserNameConsumer(IEmployeeService employeeService, IMapper mapper)
        {
            _employeeService = employeeService ?? throw new System.ArgumentNullException(nameof(employeeService));
            _mapper = mapper ?? throw new System.ArgumentNullException(nameof(mapper));
        }

        public async Task Consume(ConsumeContext<GetEmployeeByUserNameRequest> context)
        {
            var result = await _employeeService.GetByUserNameAsync(context.Message.UserName);
            var response = new EmployeeByIdResponse();

            if (!result.Success) response.Notification = new NotificationViewModel(result.Errors, e: result.AspNetException);
            else
            {
                response.Notification = new NotificationViewModel();
                response.Model = _mapper.Map<EmployeeViewModel>(result.Data);
            }
            await context.RespondAsync<EmployeeByIdResponse>(response);
        }
    }
}
