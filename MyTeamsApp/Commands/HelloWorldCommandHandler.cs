using MyTeamsApp.Models;
using AdaptiveCards.Templating;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.TeamsFx.Conversation;
using Newtonsoft.Json;
using AdaptiveCards;

namespace MyTeamsApp.Commands
{
    /// <summary>
    /// The <see cref="HelloWorldCommandHandler"/> registers a pattern with the <see cref="ITeamsCommandHandler"/> and
    /// responds with an Adaptive Card if the user types the <see cref="TriggerPatterns"/>.
    /// </summary>
    public class HelloWorldCommandHandler : ITeamsCommandHandler
    {
        private readonly ILogger<HelloWorldCommandHandler> _logger;
        private readonly string _adaptiveCardFilePath = Path.Combine(".", "Resources", "HelloWorldCard.json");

        public IEnumerable<ITriggerPattern> TriggerPatterns => new List<ITriggerPattern>
        {
            // Used to trigger the command handler if the command text contains 'helloWorld'
            new RegExpTrigger("helloWorld")
        };

        public HelloWorldCommandHandler(ILogger<HelloWorldCommandHandler> logger)
        {
            _logger = logger;
        }

        public async Task<ICommandResponse> HandleCommandAsync(ITurnContext turnContext, CommandMessage message, CancellationToken cancellationToken = default)
        {
            _logger?.LogInformation($"App received message: {message.Text}");

            // Read the Adaptive Card JSON from the file
            string cardJson = File.ReadAllText(Path.Combine(".", "Resources", "HelloWorldCard.json"));

            // Parse the JSON into an AdaptiveCard object
            var adaptiveCard = AdaptiveCard.FromJson(cardJson).Card;

            var attachment = new Attachment
            {
                ContentType = AdaptiveCard.ContentType,
                Content = adaptiveCard
            };
            var reply = MessageFactory.Attachment(attachment);
            // send response
            return new ActivityCommandResponse(reply);
        }
    }
}
