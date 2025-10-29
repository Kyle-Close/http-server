public static class Parse
{
    public static RequestLine ParseRequestLine(string line)
    {
        RequestLine rqLine = new RequestLine(line);
        return rqLine;
    }

    public static Dictionary<string, string> ParseHeaders(List<string> headerLines)
    {
        Dictionary<string, string> headerDict = new Dictionary<string, string>();

        for (int j = 1; j < headerLines.Count; j++)
        {
            var parts = headerLines[j].Split(':', 2);

            if (parts.Length != 2)
            {
                throw new Exception("Invalid header field: " + headerLines[j]);
            }

            string key = parts[0].Trim();
            string val = parts[1].Trim();

            headerDict[key] = val;
        }

        return headerDict;
    }
}
