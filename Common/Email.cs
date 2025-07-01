using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;

namespace WebForm.Common
{
 
    public class EmailInfo  
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Name { get; set; }
        public string EmailCC { get; set; }
        public string PassWord { get; set; }
        public string DisplayName { get; set; }
        public bool IsSsl { get; set; }
        public string MailTo { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public List<string> LstAttachment { get; set; }
    }

    public class EmailHelper
    {
        public static bool SendMail(EmailInfo pEmail, out string oMsg)
        {
            oMsg = string.Empty;
            try
            {
                using (var mail = new MailMessage())
                {
                    mail.From = new MailAddress(pEmail.Name, pEmail.DisplayName);
                    foreach (var emailTo in pEmail.MailTo.Split(new[] {";"}, StringSplitOptions.RemoveEmptyEntries))
                        mail.To.Add(emailTo);
                    mail.Subject = pEmail.Subject;
                    mail.Body = pEmail.Content;
                    mail.IsBodyHtml = true;
                    if (!string.IsNullOrEmpty(pEmail.EmailCC)) mail.CC.Add(pEmail.EmailCC);

                    if (pEmail.LstAttachment != null)
                    {
                        var strAttachMent = pEmail.LstAttachment.ToArray();
                        if (strAttachMent.Length > 0)
                            foreach (var vAttach in strAttachMent)
                                mail.Attachments.Add(new Attachment(vAttach));
                    }

                    using (var smtp = new SmtpClient(pEmail.Host, pEmail.Port))
                    {
                        smtp.Credentials = new NetworkCredential(pEmail.Name, pEmail.PassWord);
                        smtp.EnableSsl = pEmail.IsSsl;

                        try
                        {
                            smtp.Send(mail);
                        }
                        catch (Exception ex)
                        {
                            oMsg = "Gửi email thất bại";
                            Logger.Log.Error(ex.ToString());
                            return false;
                        }
                    }
                }

                oMsg = "Gửi email thành công";
                return true;
            }
            catch (Exception ex)
            {
                oMsg = "Gửi email thất bại";
                Logger.Log.Error(ex.ToString());
                return false;
            }
        }
    }
}