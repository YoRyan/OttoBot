using Discord;
using Discord.Interactions;
using OttoCSharp;

public class FSharpModule : InteractionModuleBase<SocketInteractionContext>
{
    // Dependencies can be accessed through Property injection, public properties with public setters will be set by the service provider
    public InteractionService? Commands { get; set; }

    private InteractionHandler _handler;

    // Constructor injection is also a valid way to access the dependencies
    public FSharpModule(InteractionHandler handler)
    {
        _handler = handler;
    }

    // These methods must be made public so that we can use them from F#
    // computation expressions.

#pragma warning disable CS8625
    public new Task RespondAsync(string text = null, Embed[] embeds = null, bool isTTS = false, bool ephemeral = false, AllowedMentions allowedMentions = null, RequestOptions options = null, MessageComponent components = null, Embed embed = null)
    {
        return base.RespondAsync(text, embeds, isTTS, ephemeral, allowedMentions, options, components, embed);
    }

    public new Task DeferAsync(bool ephemeral = false, RequestOptions options = null)
    {
        return base.DeferAsync(ephemeral, options);
    }

    public new Task<IUserMessage> FollowupAsync(string text = null, Embed[] embeds = null, bool isTTS = false, bool ephemeral = false, AllowedMentions allowedMentions = null, RequestOptions options = null, MessageComponent components = null, Embed embed = null)
    {
        return base.FollowupAsync(text, embeds, isTTS, ephemeral, allowedMentions, options, components, embed);
    }

    public new Task<IUserMessage> FollowupWithFileAsync(Stream fileStream, string fileName, string text = null, Embed[] embeds = null, bool isTTS = false, bool ephemeral = false, AllowedMentions allowedMentions = null, MessageComponent components = null, Embed embed = null, RequestOptions options = null)
    {
        return base.FollowupWithFileAsync(fileStream, fileName, text, embeds, isTTS, ephemeral, allowedMentions, components, embed, options);
    }
}
