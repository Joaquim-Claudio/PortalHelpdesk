using MimeKit;

public static class EmailParser
{
    public static string ExtractHtmlContentWithInlineImages(MimeMessage message)
    {
        // Priorizar o HtmlBody
        var htmlContent = message.HtmlBody ?? message.TextBody ?? string.Empty;

        if (message.Body is Multipart multipart)
        {
            foreach (var part in multipart)
            {
                if (part is MimePart mimePart && !mimePart.IsAttachment && mimePart.ContentId != null)
                {
                    using var ms = new MemoryStream();
                    mimePart.Content.DecodeTo(ms);
                    var bytes = ms.ToArray();

                    // Converter para Base64
                    var base64 = Convert.ToBase64String(bytes);

                    // Substituir cid pelo inline base64
                    htmlContent = htmlContent.Replace(
                        $"cid:{mimePart.ContentId}",
                        $"data:{mimePart.ContentType.MimeType};base64,{base64}"
                    );
                }
            }
        }

        return htmlContent;
    }
}
