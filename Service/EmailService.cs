using MimeKit;
using MailKit.Net.Smtp;
using System.Threading.Tasks;
namespace MovieWebsite.Service
{
    public class EmailService
    {
        private readonly string _fromEmail = "trankhietloi2004@gmail.com";         
        private readonly string _appPassword = "rvwijokoqnzimxvm";            

        public async Task SendVerificationEmail(string toEmail, string token)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Movie Website", _fromEmail));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = "Xác minh tài khoản";

            /*            var link = $"http://localhost:5029/Auth/Verify?token={token}";
            */
            var link = $"http://localhost:5029/Account/Verify?token={token}";

            message.Body = new TextPart("plain")
            {
                Text = $"Chào bạn,\n\nVui lòng nhấn vào liên kết sau để xác minh tài khoản của bạn:{link}\n\n\nLiên kết này có hiệu lực trong 24 giờ."
            };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync("smtp.gmail.com", 587, false);
            await smtp.AuthenticateAsync(_fromEmail, _appPassword);
            await smtp.SendAsync(message);
            await smtp.DisconnectAsync(true);
        }

        public async Task SendResetPasswordEmail(string toEmail, string resetLink)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Movie Website", _fromEmail));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = "Khôi phục mật khẩu";

            message.Body = new TextPart("plain")
            {
                Text = $"Chào bạn,\n\nVui lòng nhấn vào liên kết sau để đặt lại mật khẩu:\n{resetLink}\n\nLiên kết sẽ hết hạn sau 1 giờ."
            };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync("smtp.gmail.com", 587, false);
            await smtp.AuthenticateAsync(_fromEmail, _appPassword);
            await smtp.SendAsync(message);
            await smtp.DisconnectAsync(true);
        }

    }
}
