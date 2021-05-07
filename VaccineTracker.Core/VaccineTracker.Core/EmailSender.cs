using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Timers;
using VaccineTracker.Core.Domain;

namespace VaccineTracker.Core
{
    public class EmailSender : IAlertHandler
    {
        private readonly EmailSettings _emailSettings;
        private const bool EnableSsl = true;
        private const string Subject = "Vaccine Availability Status";
        private readonly Timer _timer = new Timer();
        private VaccineAvailability _latestAvailability;

        public EmailSender(EmailSettings emailSettings)
        {
            _emailSettings = emailSettings;
            _timer.Interval = 1000 * 60 * 30; // 30 minutes
            _timer.AutoReset = true;
            _timer.Enabled = true;
            _timer.Elapsed += OnTimerElapsed;
            _timer.Start();
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (_latestAvailability == null ||
                _latestAvailability.AvailableSlots.Count == 0) return;
            var message = FormatToTable(_latestAvailability.AvailableSlots);
            SendEmail(message);
        }

        public void HandleVaccineAvailability(VaccineAvailability vaccineAvailability)
        {
            _latestAvailability = vaccineAvailability;
        }

        public void HandlerError(Exception exception)
        {

        }

        private static string FormatToTable<T>(IEnumerable<T> enums)
        {
            var type = typeof(T);
            var props = type.GetProperties();
            var html = new StringBuilder("<table>");

            //Header
            html.Append("<thead><tr>");
            foreach (var p in props)
                html.Append("<th>" + p.Name + "</th>");
            html.Append("</tr></thead>");

            //Body
            html.Append("<tbody>");
            foreach (var e in enums)
            {
                html.Append("<tr>");
                props.Select(s => s.GetValue(e)).ToList().ForEach(p =>
                {
                    html.Append("<td>" + p + "</td>");
                });
                html.Append("</tr>");
            }

            html.Append("</tbody>");
            html.Append("</table>");
            return html.ToString();
        }



        private void SendEmail(string messageBody)
        {
            try
            {
                using var mail = new MailMessage
                {
                    From = new MailAddress(_emailSettings.EmailFrom)
                };
                mail.To.Add(_emailSettings.EmailTo);
                mail.Subject = Subject;
                mail.Body = messageBody;
                mail.IsBodyHtml = true;
                using var smtp = new SmtpClient(_emailSettings.SmtpAddress, _emailSettings.PortNumber)
                {
                    Credentials = new NetworkCredential(_emailSettings.EmailFrom, _emailSettings.Password),
                    EnableSsl = EnableSsl
                };
                smtp.Send(mail);
            }
            catch (Exception e)
            {
                //
            }

        }
    }
}