using Microsoft.Graph;
using Microsoft.Graph.Models;

public static class EmailParser
{
    public static async Task<string> ExtractHtmlContentWithInlineImagesAsync(Message message, GraphServiceClient graphClient)
    {
        if (message.Body == null)
            return string.Empty;

        // HTML do email
        var htmlContent = message.Body.Content ?? string.Empty;

        // Iterar sobre attachments inline
        if (message.Attachments != null && message.Attachments.Count > 0)
        {
            foreach (var attachment in message.Attachments)
            {
                if (attachment is FileAttachment fileAttachment && fileAttachment.IsInline == true)
                {
                    // Se o ContentBytes não estiver carregado, buscar do Graph
                    byte[] contentBytes = fileAttachment.ContentBytes ?? [];
                    if (contentBytes == null || contentBytes.Length == 0)
                    {
                        var attachmentFromGraph = await graphClient.Users[message.From?.EmailAddress?.Address]
                            .Messages[message.Id]
                            .Attachments[fileAttachment.Id]
                            .GetAsync() as FileAttachment;

                        contentBytes = attachmentFromGraph?.ContentBytes ?? [];
                    }

                    var base64 = Convert.ToBase64String(contentBytes);
                    var mimeType = fileAttachment.ContentType ?? "application/octet-stream";

                    // Substituir cid pelo inline base64
                    htmlContent = htmlContent.Replace(
                        $"cid:{fileAttachment.ContentId}",
                        $"data:{mimeType};base64,{base64}");
                }
            }
        }

        return htmlContent;
    }

    public static string GenerateMessageId(string domain)
    {
        var id = Guid.NewGuid().ToString("N");
        return $"<{id}@{domain}>";
    }

}
