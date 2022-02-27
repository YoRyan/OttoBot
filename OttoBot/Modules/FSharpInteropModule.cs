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
                    case Interaction.Response response:
                        await RespondAsync(response.text);
                        break;
                    case Interaction.ResponseWithFile response:
                        await RespondWithFileAsync(response.stream, response.filename);
                        break;
                }
            }
        }
    }
}
