using API;
using PxStatCore.Test;
using PxStat;


namespace PxStatXUnit.Tests
{
    [Collection("PxStatXUnit")]
    public class Email_Test
    {
        [Fact]
        public void SendEmailTemplateSubstitution()
        {
            Helper.SetupTests();
            string template = GetFile(@"\Resources\eMail\template_NotifyWorkflow.html");
            var listToParse = new List<eMail_KeyValuePair>();
            listToParse.Add(new eMail_KeyValuePair() { key = "{datetime}", value = DateTime.Now.ToString("g") });
            foreach (eMail_KeyValuePair item in listToParse)
            {
                template = template.Replace(item.key, item.value);
            }
            Assert.False(template.Contains("{datetime}"));

        }

        private string GetFile(string filename)
        {
            string workingDirectory = Environment.CurrentDirectory;

            // This will get the current PROJECT directory
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
            return File.ReadAllText(projectDirectory + filename);
        }
    }
}
