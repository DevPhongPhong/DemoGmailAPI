using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MimeKit;

namespace ConsoleApp1
{
    public class ReadGmailService
    {
        private string _host { get; set; }
        private int _port { get; set; }
        private bool _useSSL { get; set; }
        private string _userName { get; set; }
        private string _password { get; set; }

        public ReadGmailService()
        {
            setDefaultValue();
        }

        public ReadGmailService(string userName, string password)
        {
            setDefaultValue();
            _userName = userName;
            _password = password;
        }

        public List<MimeMessage> GetEmailsBySubjectContainString(string containString = "TRUNG TÂM CNTT MOBIFONE gửi hóa đơn điện tử số")
        {
            List<MimeMessage> listMessage = null;
            try
            {
                using (var client = CreateClient())
                {
                    client.Inbox.Open(FolderAccess.ReadOnly);

                    var query = SearchQuery.SubjectContains(containString);
                    var uids = client.Inbox.SearchAsync(query).Result;

                    listMessage = new List<MimeMessage>();
                    foreach (var uid in uids)
                    {
                        listMessage.Add(client.Inbox.GetMessage(uid));
                    }

                    client.Disconnect(true);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {

            }
            return listMessage;
        }

        private ImapClient CreateClient()
        {
            var client = new ImapClient();
            client.Connect(_host, _port, _useSSL);
            client.Authenticate(_userName, _password);
            return client;
        }

        private void setDefaultValue()
        {
            _host = "imap.gmail.com";
            _port = 993;
            _useSSL = true;
        }
    }
}
