using Discord.Interactions;
using FSharp.Control;
using OttoCompute;

namespace OttoBot.Modules
{
    public class FSharpInteropModule : InteractionModuleBase<SocketInteractionContext>
    {
        internal async Task FSharpInteractAsync(FSharp.Control.IAsyncEnumerable<Interaction> interactions)
        {
            await foreach (Interaction interact in AsyncSeq.toAsyncEnum(interactions))
            {
                switch (interact)
                {
                    case Interaction.Respond response:
                        await RespondAsync(response.text);
                        break;
                    case Interaction.RespondWithFile response:
                        await RespondWithFileAsync(response.stream, response.filename);
                        break;
                    case var x when x.IsDefer:
                        await DeferAsync();
                        break;
                    case Interaction.Followup followUp:
                        await FollowupAsync(followUp.text);
                        break;
                    case Interaction.FollowupWithFile followUp:
                        await FollowupWithFileAsync(followUp.stream, followUp.filename);
                        break;
                }
            }
        }
    }
}
