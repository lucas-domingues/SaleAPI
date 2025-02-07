namespace Sales.API.Models.Responses
{
    public class ApiErrorResponse
    {
        public string Type { get; set; }
        public string Error { get; set; }
        public string Detail { get; set; }

        public ApiErrorResponse(string type, string error, string detail)
        {
            Type = type;
            Error = error;
            Detail = detail;
        }
    }
}
