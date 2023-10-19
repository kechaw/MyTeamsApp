using AdaptiveCards;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Schema;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace MyTeamsApp
{
    /// <summary>
    /// An empty bot handler.
    /// You can add your customization code here to extend your bot logic if needed.
    /// </summary>
    public class TeamsBot : TeamsActivityHandler
    {
        //public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
        //{
        //    if (turnContext.Activity.Type == "message")
        //    {
        //        var reply = MessageFactory.Attachment(CreateAdaptiveCardAttachment());
        //        await turnContext.SendActivityAsync(reply, cancellationToken);
        //    }
        //    //else if (turnContext.Activity.Type == "invoke" && turnContext.Activity.Name == "adaptiveCard/action")
        //    //{
        //    //    await OnAdaptiveCardInvokeAsync(turnContext, cancellationToken);
        //    //}
        //}

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken = default)
        {
                var reply = MessageFactory.Attachment(CreateAdaptiveCardAttachment());
                await turnContext.SendActivityAsync(reply, cancellationToken);
            
        }

        private Attachment CreateAdaptiveCardAttachment()
        {
            // Read the Adaptive Card JSON from the file
            string cardJson = File.ReadAllText(Path.Combine(".", "Resources", "HelloWorldCard.json"));

            // Parse the JSON into an AdaptiveCard object
            var adaptiveCard = AdaptiveCard.FromJson(cardJson).Card;

            var attachment = new Attachment
            {
                ContentType = AdaptiveCard.ContentType,
                Content = adaptiveCard
            };

            return attachment;
        }

        protected override async Task<AdaptiveCardInvokeResponse> OnAdaptiveCardInvokeAsync(ITurnContext<IInvokeActivity> turnContext, AdaptiveCardInvokeValue invokeValue, CancellationToken cancellationToken)
        {
            if (turnContext.Activity.Value is JObject test)
            {
               
                if (test["action"]["verb"].ToString() == "qwerty")
                {
                    // Process the submission data and decide whether to hide 'text box'
                    bool shouldHideField = true;

                    var card = File.ReadAllText(Path.Combine(".", "Resources", "HelloWorldCard.json"));
                    var adaptiveCard = AdaptiveCard.FromJson(card).Card;

                    // Update the Adaptive Card JSON to hide 'hiddenField'
                    if (shouldHideField)
                    {
                        var inputField = adaptiveCard.Body.OfType<AdaptiveTextInput>().FirstOrDefault(input => input.Id == "defaultInputId");
                        if (inputField != null)
                        {
                            adaptiveCard.Body.OfType<AdaptiveTextInput>().FirstOrDefault(input => input.Id == "defaultInputId").IsVisible = false;
                        }
                    }

                    // Send the updated Adaptive Card back to the user
                    var updatedCardAttachment = new Attachment
                    {
                        ContentType = AdaptiveCard.ContentType,
                        Content = adaptiveCard,
                    };
                    var activity = MessageFactory.Attachment(updatedCardAttachment);
                    activity.Id = turnContext.Activity.ReplyToId;
                    await turnContext.UpdateActivityAsync(activity, cancellationToken);
                }
            }

            return new AdaptiveCardInvokeResponse(){StatusCode = 200};
        }
    }
}
