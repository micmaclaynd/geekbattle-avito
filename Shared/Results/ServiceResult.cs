namespace Shared.Results {
    public class ServiceResult<ResultType> {
        public class Generator {
            public ServiceResult<ResultType> Success(ResultType result) => new() {
                Result = result,
                IsSuccess = true,
                Error = null
            };

            public ServiceResult<ResultType> Error(string message) => new() {
                Result = default,
                IsSuccess = false,
                Error = message
            };
        }

        public ResultType? Result { get; set; }
        public bool IsSuccess { get; set; }
        public string? Error { get; set; }
    }

    public class BasicServiceResult : ServiceResult<object> { }
}
