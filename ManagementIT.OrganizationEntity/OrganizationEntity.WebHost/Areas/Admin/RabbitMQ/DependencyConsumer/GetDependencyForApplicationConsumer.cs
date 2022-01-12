using System;
using System.Collections.Generic;
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
using OrganizationEntityContracts.ViewModels.OrgEntityViewModel.Application;
using OrganizationEntityContracts.ViewModels.OrgEntityViewModel.DepartmentViewModels;
using OrganizationEntityContracts.ViewModels.OrgEntityViewModel.EmployeeViewModels;
using OrganizationEntityContracts.ViewModels.OrgEntityViewModel.RoomViewModels;

namespace OrganizationEntity.WebHost.Areas.Admin.RabbitMQ.DependencyConsumer
{
    public class GetDependencyForApplicationConsumer : IConsumer<GetCreateForApplicationRequest>
    {
        private readonly IDependencyEntityService _dependencyEntityService;
        private readonly IMapper _mapper;

        public GetDependencyForApplicationConsumer(IDependencyEntityService dependencyEntityService, IMapper mapper)
        {
            _dependencyEntityService = dependencyEntityService ?? throw new ArgumentNullException(nameof(dependencyEntityService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task Consume(ConsumeContext<GetCreateForApplicationRequest> context)
        {
            var result = await _dependencyEntityService.GetDependencyForApplication(ClaimsPrincipal.Current);
            var response = new GetCreateForApplicationResponse();

            if (!result.Success)
                response.Notification = new NotificationViewModel(result.Errors, e: result.AspNetException);
            else
            {
                response.Notification = new NotificationViewModel();
                response.SelectDepartment = _mapper.Map<List<DepartmentViewModel>>(result.Data.SelectDepartment);
                response.SelectEmployee = _mapper.Map<List<EmployeeViewModel>>(result.Data.SelectEmployee);
                response.SelectRoom = _mapper.Map<List<RoomViewModel>>(result.Data.SelectRoom);
            }

            await context.RespondAsync<GetCreateForApplicationResponse>(response);
        }
    }

    public class GetDependencyForApplicationConsumerDefinition : ConsumerDefinition<GetDependencyForApplicationConsumer>
    {
        public GetDependencyForApplicationConsumerDefinition()
        {
            EndpointName = ApiShowConstants.GetDependencyForApplication;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<GetDependencyForApplicationConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseRetry(x => x.Intervals(100, 500, 1000));
        }
    }
}