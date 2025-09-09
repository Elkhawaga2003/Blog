using MailKit.Net.Smtp;
using MimeKit;
using System.Threading.Tasks;

public class EmailSender
{
    private string smtpServer = "smtp.gmail.com";  
    private int smtpPort = 587;                    
    private string smtpUser = "elkhawaga1900@gmail.com";  
    private string smtpPass = "ebifeebjqfekhmig";    

    public async Task SendVerificationEmail(string toEmail, string code)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Your App", smtpUser));  
        message.To.Add(new MailboxAddress("", toEmail));            
        message.Subject = "كود التحقق الخاص بك";

        message.Body = new TextPart("plain")
        {
            Text = $"كود التحقق الخاص بك هو: {code}"
        };

        using (var client = new SmtpClient())
        {
            await client.ConnectAsync(smtpServer, smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(smtpUser, smtpPass);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
