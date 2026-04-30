namespace SurveyBasket.Helpers;

public static class EmailBodyBuilder
{
    public static string GenerateEmailBody (string template, Dictionary<string, string> templateModel)
    {
        var templatePaht = $"{Directory.GetCurrentDirectory()}/Templates/{template}.html";
        var streamReader = new StreamReader (templatePaht);
        var body = streamReader.ReadToEnd ();
        streamReader.Close();

        foreach (var item in templateModel)
            body = body.Replace (item.Key, item.Value);


        return body;
    }
}
