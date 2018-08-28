using System;
using System.Threading.Tasks;

using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using System.Net.Http;
using AdaptiveCards;
using System.Web.Hosting;
using System.IO;
using System.Net;

namespace Microsoft.Bot.Sample.SimpleEchoBot
{
    [Serializable]
    public class EchoDialog : IDialog<object>
    {
        protected int count = 1;

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;

            if (message.Text == "reset")
            {
                PromptDialog.Confirm(
                    context,
                    AfterResetAsync,
                    "Are you sure you want to reset the count?",
                    "Didn't get that!",
                    promptStyle: PromptStyle.Auto);
            }
            /*
            else if (message.Text == "weather")
            {
                var returnMessage = context.MakeMessage();
                var json = await GetCardText("https://csadaptivebe3d.blob.core.windows.net/resourse/weather.json");
                var results = AdaptiveCard.FromJson(json);
                var card = results.Card;
                returnMessage.Attachments.Add(new Attachment()
                {
                    Content = card,
                    ContentType = AdaptiveCard.ContentType,
                    Name = "Card"
                });

                await context.PostAsync(returnMessage);
                context.Wait(MessageReceivedAsync);
            }
            */
            else
            {
                await context.PostAsync($"{this.count++}: You said {message.Text}");
                context.Wait(MessageReceivedAsync);
            }
        }

        public async Task AfterResetAsync(IDialogContext context, IAwaitable<bool> argument)
        {
            var confirm = await argument;
            if (confirm)
            {
                this.count = 1;
                await context.PostAsync("Reset count.");
            }
            else
            {
                await context.PostAsync("Did not reset count.");
            }
            context.Wait(MessageReceivedAsync);
        }

        public async Task<string> GetCardText(string path)
        {
            var webRequest = WebRequest.Create(path);

            using (var response = webRequest.GetResponse())
            using (var content = response.GetResponseStream())
            using (var reader = new StreamReader(content))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }
}