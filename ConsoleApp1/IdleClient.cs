using MailKit;
using MailKit.Net.Imap;
using MailKit.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class IdleClient : IDisposable
    {
        int countCurrent;
        CancellationTokenSource cancel;
        CancellationTokenSource done;
        bool messagesArrived;
        ImapClient client;
        string host; int port; string username; string password; SecureSocketOptions ssl;
        public IdleClient(string host, int port, string username, string password, SecureSocketOptions ssl)
        {
            client = new ImapClient();
            countCurrent = 0;
            cancel = new CancellationTokenSource();
            this.host = host;
            this.port = port;
            this.username = username;
            this.password = password;
            this.ssl = ssl;
        }

        async Task ReconnectAsync()
        {
            if (!client.IsConnected)
                await client.ConnectAsync(host, port, ssl, cancel.Token);

            if (!client.IsAuthenticated)
            {
                await client.AuthenticateAsync(username, password, cancel.Token);

                await client.Inbox.OpenAsync(FolderAccess.ReadOnly, cancel.Token);
            }
        }

        async Task FetchMessageSummariesAsync(bool getHoaDon)
        {
            IEnumerable<UniqueId> fetched;

            do
            {
                try
                {
                    // fetch summary information for messages that we don't already have
                    int startIndex = countCurrent;

                    fetched = client.Inbox.Fetch(startIndex, -1, MessageSummaryItems.Full | MessageSummaryItems.UniqueId, cancel.Token).Select(x => x.UniqueId);
                    break;
                }
                catch (ImapProtocolException)
                {
                    // protocol exceptions often result in the client getting disconnected
                    await ReconnectAsync();
                }
                catch (IOException)
                {
                    // I/O exceptions always result in the client getting disconnected
                    await ReconnectAsync();
                }
            } while (true);

            foreach (var uid in fetched)
            {
                countCurrent++;
                if (getHoaDon)
                {
                    var message = client.Inbox.GetMessage(uid);
                    try
                    {
                        AttachmentService.SaveXmlFile(message.Attachments);
                        //AttachmentService.GetXmlHoaDon(message.Attachments);
                        
                    }
                    catch (Exception e)
                    {

                    }
                    
                }
            }
        }

        async Task WaitForNewMessagesAsync()
        {
            do
            {
                try
                {
                    if (client.Capabilities.HasFlag(ImapCapabilities.Idle))
                    {
                        // Note: IMAP servers are only supposed to drop the connection after 30 minutes, so normally
                        // we'd IDLE for a max of, say, ~29 minutes... but GMail seems to drop idle connections after
                        // about 10 minutes, so we'll only idle for 9 minutes.
                        using (done = new CancellationTokenSource(new TimeSpan(0, 9, 0)))
                        {
                            using (var linked = CancellationTokenSource.CreateLinkedTokenSource(cancel.Token, done.Token))
                            {
                                await client.IdleAsync(linked.Token);

                                // throw OperationCanceledException if the cancel token has been canceled.
                                cancel.Token.ThrowIfCancellationRequested();
                            }
                        }
                    }
                    else
                    {
                        // Note: we don't want to spam the IMAP server with NOOP commands, so lets wait a minute
                        // between each NOOP command.
                        await Task.Delay(new TimeSpan(0, 1, 0), cancel.Token);
                        await client.NoOpAsync(cancel.Token);
                    }
                    break;
                }
                catch (ImapProtocolException)
                {
                    // protocol exceptions often result in the client getting disconnected
                    await ReconnectAsync();
                }
                catch (IOException)
                {
                    // I/O exceptions always result in the client getting disconnected
                    await ReconnectAsync();
                }
            } while (true);
        }

        async Task IdleAsync()
        {
            do
            {
                try
                {
                    await WaitForNewMessagesAsync();

                    if (messagesArrived)
                    {
                        await FetchMessageSummariesAsync(true);
                        messagesArrived = false;
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            } while (!cancel.IsCancellationRequested);
        }

        public async Task RunAsync()
        {
            // connect to the IMAP server and get our initial list of messages
            try
            {
                await ReconnectAsync();
                await FetchMessageSummariesAsync(false);
            }
            catch (OperationCanceledException)
            {
                await client.DisconnectAsync(true);
                return;
            }

            // keep track of changes to the number of messages in the folder (this is how we'll tell if new messages have arrived).
            client.Inbox.CountChanged += OnCountChanged;

            // keep track of messages being expunged so that when the CountChanged event fires, we can tell if it's
            // because new messages have arrived vs messages being removed (or some combination of the two).
            client.Inbox.MessageExpunged += OnMessageExpunged;

            // keep track of flag changes
            client.Inbox.MessageFlagsChanged += OnMessageFlagsChanged;

            await IdleAsync();

            client.Inbox.MessageFlagsChanged -= OnMessageFlagsChanged;
            client.Inbox.MessageExpunged -= OnMessageExpunged;
            client.Inbox.CountChanged -= OnCountChanged;

            await client.DisconnectAsync(true);
        }

        // Note: the CountChanged event will fire when new messages arrive in the folder and/or when messages are expunged.
        void OnCountChanged(object sender, EventArgs e)
        {
            var folder = (ImapFolder)sender;

            // Note: because we are keeping track of the MessageExpunged event and updating our
            // 'messages' list, we know that if we get a CountChanged event and folder.Count is
            // larger than messages.Count, then it means that new messages have arrived.
            //if (folder.Count > messages.Count) {
            if (folder.Count > countCurrent)
            {
                // Note: your first instict may be to fetch these new messages now, but you cannot do
                // that in this event handler (the ImapFolder is not re-entrant).
                //
                // Instead, cancel the `done` token and update our state so that we know new messages
                // have arrived. We'll fetch the summaries for these new messages later...
                messagesArrived = true;
                done?.Cancel();
            }
        }

        void OnMessageExpunged(object sender, MessageEventArgs e)
        {
            var folder = (ImapFolder)sender;

            if (e.Index < countCurrent)
            {
                // Note: If you are keeping a local cache of message information
                // (e.g. MessageSummary data) for the folder, then you'll need
                // to remove the message at e.Index.
                countCurrent--;
            }
            else
            {
            }
        }

        void OnMessageFlagsChanged(object sender, MessageFlagsChangedEventArgs e)
        {
        }

        public void Exit()
        {
            cancel.Cancel();
        }

        public void Dispose()
        {
            client.Dispose();
            cancel.Dispose();
            done?.Dispose();

            countCurrent = 0;
            host = null;
            port = 0;
            username = null;
            password = null;
        }
    }
}
