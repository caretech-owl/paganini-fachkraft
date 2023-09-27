
public class ErrorData
{
    public string Message { get; set; }
    public string Content { get; set; }
    public long HTTPStatus { get; set; }

    public ErrorData() { }
    public ErrorData(string message, string content, long status)
    {
        Message = message;
        Content = content;
        HTTPStatus = status;
    }
}