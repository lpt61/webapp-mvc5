using GreenSol.Domain.Abstract;
using GreenSol.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace GreenSol.Domain.Concrete
{
    public class EmailSettings
    {
        //public string MailToAddress = "orders@example.com";
        //public string MailFromAddress = "user@example.com";
        public string MailToAddress = null;
        public string MailFromAddress = "greensol@mymail.com";
        public bool UseSsl = true;

        public string Username = "GreenSol";
        public string Password = "MySecret";
        public string ServerName = "mymail.com";
        public int ServerPort = 25;

        //public string Username = "MySmtpUsername";
        //public string Password = "MySmtpPassword";
        //public string ServerName = "smtp.example.com";
        //public int ServerPort = 587;

        public bool WriteAsFile = false;
        public string FileLocation = @"f:\greensol_emails";
    }

    public class EmailOrderProcessor : IOrderProcessor
    {
        private EmailSettings emailSettings;

        public EmailOrderProcessor(EmailSettings settings, IAlbumRepository repo)
        {
            emailSettings = settings;
        }

        public void ProcessOrder(ShoppingCart cart, OrderDetail orderDetail)
        {
            emailSettings.MailToAddress = orderDetail.Email;
            try
            {
                using (var smtpClient = new SmtpClient())
                {
                    //hMailServer does not support Ssl
                    //smtpClient.EnableSsl = emailSettings.UseSsl;
                    
                    smtpClient.Host = emailSettings.ServerName;
                    smtpClient.Port = emailSettings.ServerPort;
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = new NetworkCredential(emailSettings.Username,
                    emailSettings.Password);

                    //Additional line
                    //Send through a server requires Network delivery method. 
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                
                    //if (emailSettings.WriteAsFile)
                    //{
                    //    smtpClient.DeliveryMethod
                    //    = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                    //    smtpClient.PickupDirectoryLocation = emailSettings.FileLocation;
                    //    smtpClient.EnableSsl = false;
                    //}

                    StringBuilder body = new StringBuilder()
                    .AppendLine("A new order has been submitted")
                    .AppendLine("---")
                    .AppendLine("Items:");

                    foreach (var line in cart.CartLines())
                    {
                        var subtotal = line.Album.Price * line.Count;
                        body.AppendFormat(
                            "{0} x {1} (subtotal: {2:c}",
                            line.Count,
                            line.Album.Name,
                            subtotal);
                    }

                    body.AppendFormat("Total order value: {0:c}", cart.TotalPrice())
                        .AppendLine("---")
                        .AppendLine("Ship to:")
                        .AppendLine(orderDetail.Name)
                        .AppendLine(orderDetail.Adrress)
                        .AppendLine(orderDetail.City)
                        .AppendLine(orderDetail.State ?? "")
                        .AppendLine(orderDetail.Country)
                        .AppendLine(orderDetail.Zip)
                        .AppendLine("---")
                        .AppendFormat("Gift wrap: {0}",
                            orderDetail.GiftWrap ? "Yes" : "No");

                    MailMessage mailMessage = new MailMessage(
                        emailSettings.MailFromAddress, // From
                        emailSettings.MailToAddress, // To
                        "New order submitted!", // Subject
                        body.ToString()); // Body

                    if (emailSettings.WriteAsFile)
                    {
                        mailMessage.BodyEncoding = Encoding.ASCII;
                    }

                    smtpClient.Send(mailMessage);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}