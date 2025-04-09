namespace TaskManager.Core.DTOs
{    
    public class OperationResultDto
    {
        public bool Success { get; set; }
        public required string Message { get; set; }
        public required object Data { get; set; }
    }        

}
