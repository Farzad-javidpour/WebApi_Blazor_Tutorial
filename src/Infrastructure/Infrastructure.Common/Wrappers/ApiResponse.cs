using System.Text.Json;
using Core.Application.Interfaces;

namespace Infrastructure.Common.Wrappers
{
    public class ApiResponse
    {
        //------------------------------------------------------------------------------------
        public DateTime ResponseTime { get;}
        public string Path { get; set; }
        public string Method { get; set; }
        public bool Succeeded { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
        public IPaginationInfo PaginationInfo { get; set; }
        public List<string> ExceptionErrors { get; set; }
        public List<string> ValidationErrors { get; set; }

        //------------------------------------------------------------------------------------
        public ApiResponse()
        {
            ResponseTime = DateTime.Now;
        }
        //------------------------------------------------------------------------------------
        public static ApiResponse Fail(List<string> validationErrors, string message = "One or more validation errors occurred.")
        {
            return new ApiResponse { Succeeded = false, ValidationErrors = validationErrors, Message = message };
        }
        public static ApiResponse Fail(string message)
        {
            return Fail(null, message);
        }
        public static ApiResponse Fail()
        {
            return Fail("Request failed.");
        }
        //------------------------------------------------------------------------------------
        public static Task<ApiResponse> FailAsync(List<string> validationErrors, string message = "One or more validation errors occurred.")
        {
            return Task.FromResult(Fail(validationErrors, message));
        }
        public static Task<ApiResponse> FailAsync(string message)
        {
            return Task.FromResult(Fail(message));
        }
        public static Task<ApiResponse> FailAsync()
        {
            return Task.FromResult(Fail());
        }
        //------------------------------------------------------------------------------------
        public static ApiResponse Success(object data, IPaginationInfo paginationInfo, string message = "Request successful.")
        {
            return new ApiResponse { Succeeded = true, Data = data, Message = message, PaginationInfo = paginationInfo };
        }
        public static ApiResponse Success(object data, string message = "Request successful.")
        {
            return Success(data, null, message);
        }
        public static ApiResponse Success(string message)
        {
            return Success(null, message);
        }
        public static ApiResponse Success()
        {
            return Success("Request successful.");
        }
        //------------------------------------------------------------------------------------
        public static Task<ApiResponse> SuccessAsync(object data, IPaginationInfo paginationInfo, string message = "Request successful.")
        {
            return Task.FromResult(Success(data, paginationInfo, message));
        }
        public static Task<ApiResponse> SuccessAsync(object data, string message = "Request successful.")
        {
            return Task.FromResult(Success(data, message));
        }
        public static Task<ApiResponse> SuccessAsync(string message)
        {
            return Task.FromResult(Success(message));
        }
        public static Task<ApiResponse> SuccessAsync()
        {
            return Task.FromResult(Success());
        }
        //------------------------------------------------------------------------------------
    }


}