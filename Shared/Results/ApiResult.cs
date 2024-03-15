namespace Shared.Results {
    public class ApiResult<ResultType> {
        public class Generator {
            public ApiResult<ResultType> Success(ResultType value) => new() {
                IsSuccess = true,
                Error = null,
                Result = value
            };

            public ApiResult<ResultType> Error(string message) => new() {
                IsSuccess = false,
                Error = message,
                Result = default
            };
        }

        public bool IsSuccess { get; set; }
        public ResultType? Result { get; set; }
        public string? Error { get; set; }
    }

    public class BasicApiResult : ApiResult<object> { }
}