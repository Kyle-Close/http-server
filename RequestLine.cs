public enum MethodToken
{
    GET,
    POST,
    PUT,
    DELETE
}

public class RequestLine
{
    public MethodToken Method { get; set; }
    public string RequestTarget { get; set; }
    public string ProtocolVersion { get; set; }

    public RequestLine(string rqLine)
    {
        var parts = rqLine.Split(' ');
        if (parts.Length != 3)
        {
            throw new Exception("Malformed request line: " + rqLine);
        }

        Method = ConvertMethodToken(parts[0]);
        RequestTarget = parts[1];
        ProtocolVersion = parts[2];
    }

    private MethodToken ConvertMethodToken(string token)
    {
        switch (token)
        {
            case "GET":
                return MethodToken.GET;
            case "POST":
                return MethodToken.POST;
            case "PUT":
                return MethodToken.PUT;
            case "DELETE":
                return MethodToken.DELETE;
            default:
                throw new Exception("Invalid method passed in request line: " + token);
        }
    }
}
