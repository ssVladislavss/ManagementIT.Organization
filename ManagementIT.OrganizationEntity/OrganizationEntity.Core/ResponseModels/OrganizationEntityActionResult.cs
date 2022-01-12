using Contracts.Enums;
using System.Collections.Generic;

namespace OrganizationEntity.Core.ResponseModels
{
    public class OrganizationEntityActionResult
    {
        public bool Success { get; set; }
        public IEnumerable<TypeOfErrors> Errors { get; set; }
        public NotificationType Type { get; set; }
        public string TypeStr { get { return Type.ToString(); } }
        public string ErrorDescription { get; set; }
        public string AspNetException { get; set; }

        protected OrganizationEntityActionResult(bool success, NotificationType type)
        {
            Success = success;
            Type = type;
        }
        protected OrganizationEntityActionResult(bool success, IEnumerable<TypeOfErrors> errors, string e, string errorDescription, NotificationType type)
        {
            Success = success;
            Errors = errors;
            ErrorDescription = errorDescription;
            Type = type;
            AspNetException = e;
        }

        public static OrganizationEntityActionResult IsSuccess() { return new OrganizationEntityActionResult(true, NotificationType.Success); }
        public static OrganizationEntityActionResult Fail(IEnumerable<TypeOfErrors> errors, string e = null,
            string errorDescription = "Произошла внутренняя ошибка",
            NotificationType type = NotificationType.Error) { return new OrganizationEntityActionResult(false, errors, e, errorDescription, type); }
    }

    public class OrganizationEntityActionResult<T> : OrganizationEntityActionResult
    {
        public T Data { get; private set; }

        protected OrganizationEntityActionResult(bool success, NotificationType type, T data) : base(success, type) => Data = data;
        protected OrganizationEntityActionResult(bool success, IEnumerable<TypeOfErrors> errors, string e, string errorDescription, NotificationType type, T data)
            : base(success:success, errors:errors, e:e, errorDescription: errorDescription, type: type) => Data = data;


        public static OrganizationEntityActionResult<T> IsSuccess(T data) { return new OrganizationEntityActionResult<T>(true, NotificationType.Success, data); }
        public static OrganizationEntityActionResult<T> Fail(T data,
            IEnumerable<TypeOfErrors> errors,
            string e = null,
            string errorDescription = "Произошла внутренняя ошибка",
            NotificationType type = NotificationType.Error) { return new OrganizationEntityActionResult<T>(false, errors, e, errorDescription, type, data); }
    }
}