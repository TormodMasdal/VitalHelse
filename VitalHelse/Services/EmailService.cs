using MimeKit;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;


namespace VitalHelse.Services;

public class EmailService 
{
    /// <summary>
    /// Sends email from the website domain
    /// </summary>
    /// <param name="to">Email of receiver</param>
    /// <param name="subject">Subject of email</param>
    /// <param name="body">Main info of email</param>
    /// <param name="htmlBody">The body nested in HTML format</param>
    public async Task SendEmailAsync(string to, string subject, string body, string htmlBody)
    {
        var message = new MimeMessage(); // Creates a new message
        message.From.Add(new MailboxAddress("VitalHelse", "tormodmasdal@gmail.com")); // Info about the sender
        message.To.Add(new MailboxAddress("Fetch First and last from db", to)); // Info about the receiver
        message.Subject = subject;

        // Bodybuilder gives an opportunity to write HTML format
        var builder = new BodyBuilder
        {
            TextBody = body,
            HtmlBody = string.Format(htmlBody)
        };

        message.Body = builder.ToMessageBody();

        // Gives information about the email sender MOVE THIS TO ENV POSSIBLY
        using var client = new SmtpClient();
        try
        {
            await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync("tormodmasdal@gmail.com", "mdru hgju qaxc ymso");

            await client.SendAsync(message);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending email: {ex.Message}");
            throw; // rethrow så du ser det i terminalen
        }
        finally
        {
            Console.WriteLine("EmailService finished");
            await client.DisconnectAsync(true);
        }

    }
}