using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Configuration;
namespace Restaurant_QKA.Services
{
    public class EmailService
    {
        public void SendEmail(string toEmail, string subject, string body)
        {
            try
            {
                // Địa chỉ email gửi
                var fromAddress = new MailAddress("2224801030312@student.tdmu.edu.vn", "QKA Restaurant");
                var toAddress = new MailAddress(toEmail);
                const string fromPassword = "asuj xqtd ntyv kkly"; // Sử dụng mật khẩu ứng dụng Gmail

                // Cấu hình SMTP
                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com", // SMTP server Gmail
                    Port = 587, // Cổng sử dụng TLS
                    EnableSsl = true, // Kích hoạt bảo mật
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword) // Xác thực
                };

                // Tạo nội dung email
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true // Nếu muốn gửi email định dạng HTML
                })
                {
                    smtp.Send(message); // Gửi email
                }
            }
            catch (Exception ex)
            {
                // Ghi log lỗi hoặc thông báo lỗi
                Console.WriteLine("Lỗi gửi email: " + ex.Message);
            }
        }
    }
}