using API;
using PxStat.Security;
using System;
using System.Collections.Generic;

namespace PxStat.System.Notification
{
    internal static class Email_BSO
    {


        internal static void SendLoginTemplateEmail(string subject, List<string> addressTo, string header, string content, string footer, string subHeader, string lngIsoCode, List<string> addressCc = null, List<string> addressBcc = null)
        {

            using (eMail email = new eMail())
            {
                email.Subject = subject;
                foreach (string to in addressTo)
                    email.To.Add(to);
                if (addressCc != null)
                {
                    foreach (string cc in addressCc)
                    {
                        email.CC.Add(cc);
                    }
                }
                if (addressBcc != null)
                {
                    foreach (string bcc in addressBcc)
                    {
                        email.Bcc.Add(bcc);
                    }
                }

                var listToParse = new List<eMail_KeyValuePair>();

                string body = header + Environment.NewLine + content + Environment.NewLine + footer;

                listToParse.Add(new eMail_KeyValuePair() { key = "{header}", value = header });
                listToParse.Add(new eMail_KeyValuePair() { key = "{sub_header}", value = subHeader });
                listToParse.Add(new eMail_KeyValuePair() { key = "{content}", value = content });
                listToParse.Add(new eMail_KeyValuePair() { key = "{footer}", value = footer });
                listToParse.Add(new eMail_KeyValuePair() { key = "{subject}", value = subject });
                listToParse.Add(new eMail_KeyValuePair() { key = "{image_source}", value = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "url.logo") });
                listToParse.Add(new eMail_KeyValuePair() { key = "{datetime_label}", value = Label.Get("label.date-time", lngIsoCode) });
                listToParse.Add(new eMail_KeyValuePair() { key = "{datetime}", value = DateTime.Now.ToString("g") });
                listToParse.Add(new eMail_KeyValuePair() { key = "{date_format}", value = Label.Get("label.date-format", lngIsoCode) });
                listToParse.Add(new eMail_KeyValuePair() { key = "{timezone}", value = Label.Get("label.timezone", lngIsoCode) });


                email.Body = email.ParseTemplate(Properties.Resources.template_Login, listToParse);
                try
                {
                    email.Send();
                }
                catch (Exception ex)
                {
                    Log.Instance.Error("Email failure: " + ex.Message);
                }

            }

        }

        internal static void SendLoginTemplateBulletPointEmail(string subject, List<string> addressTo, string header, string content, string footer, List<string> bulletPoints, string subHeader, string lngIsoCode, List<string> addressCc = null, List<string> addressBcc = null)
        {

            using (eMail email = new eMail())
            {
                email.Subject = subject;
                foreach (string to in addressTo)
                    email.To.Add(to);
                if (addressCc != null)
                {
                    foreach (string cc in addressCc)
                    {
                        email.CC.Add(cc);
                    }
                }
                if (addressBcc != null)
                {
                    foreach (string bcc in addressBcc)
                    {
                        email.Bcc.Add(bcc);
                    }
                }

                var listToParse = new List<eMail_KeyValuePair>();

                string body = header + Environment.NewLine + content + Environment.NewLine + footer;

                listToParse.Add(new eMail_KeyValuePair() { key = "{header}", value = header });
                listToParse.Add(new eMail_KeyValuePair() { key = "{content}", value = content });
                listToParse.Add(new eMail_KeyValuePair() { key = "{sub_header}", value = subHeader });
                listToParse.Add(new eMail_KeyValuePair() { key = "{footer}", value = footer });
                listToParse.Add(new eMail_KeyValuePair() { key = "{subject}", value = subject });
                if (bulletPoints.Count >= 4)
                {
                    listToParse.Add(new eMail_KeyValuePair() { key = "{list_header}", value = bulletPoints[0] });
                    listToParse.Add(new eMail_KeyValuePair() { key = "{item1}", value = bulletPoints[1] });
                    listToParse.Add(new eMail_KeyValuePair() { key = "{item2}", value = bulletPoints[2] });
                    listToParse.Add(new eMail_KeyValuePair() { key = "{item3}", value = bulletPoints[3] });
                }

                listToParse.Add(new eMail_KeyValuePair() { key = "{image_source}", value = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "url.logo") });
                listToParse.Add(new eMail_KeyValuePair() { key = "{datetime_label}", value = Label.Get("label.date-time", lngIsoCode) });
                listToParse.Add(new eMail_KeyValuePair() { key = "{date_format}", value = Label.Get("label.date-format", lngIsoCode) });
                listToParse.Add(new eMail_KeyValuePair() { key = "{timezone}", value = Label.Get("label.timezone", lngIsoCode) });

                email.Body = email.ParseTemplate(Properties.Resources.template_LoginBulletPoint, listToParse);
                try
                {
                    email.Send();
                }
                catch (Exception ex)
                {
                    Log.Instance.Error("Email failure: " + ex.Message);
                }

            }

        }
    }
}
