
namespace TaskManager.Core.Models
{

    public class OperationResult
    {
        public bool Success { get; set; }
        public required string Message { get; set; }
        public required object Data { get; set; }

        public static OperationResult Ok(object data, string message)
        {
            return new OperationResult
            {
                Success = true,
                Message = message,
                Data = data
            };
        }

        public static OperationResult Fail(string message, object data)
        {
            return new OperationResult
            {
                Success = false,
                Message = message,
                Data = data
            };
        }
    }


    public class OperationResult<T>
    {
        public bool Success { get; set; }
        public required string Message { get; set; }
        public required T Data { get; set; }

        public static OperationResult<T> Ok(T data, string message)
        {
            return new OperationResult<T>
            {
                Success = true,
                Message = message ?? string.Empty, // Ensure Message is not null
                Data = data
            };
        }

        public static OperationResult<T> Fail(string message, T data = default!)
        {
            return new OperationResult<T>
            {
                Success = false,
                Message = message ?? string.Empty, // Ensure Message is not null
                Data = data
            };
        }
    }
}
