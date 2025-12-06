using Discord;
using Discord.Interactions;
using OttoCSharp;

// Interaction modules must be public and inherit from an IInteractionModuleBase
public class FSharpModule : InteractionModuleBase<SocketInteractionContext>
{
    // Dependencies can be accessed through Property injection, public properties with public setters will be set by the service provider
    public InteractionService? Commands { get; set; }

    protected InteractionHandler _handler;

    // Constructor injection is also a valid way to access the dependencies
    public FSharpModule(InteractionHandler handler)
    {
        _handler = handler;
    }

    // This horrible workaround is necessary to call protected members from
    // computation expressions.
    // https://github.com/dotnet/fsharp/issues/12448
#pragma warning disable CS8625

    public new Task DeferAsync(bool ephemeral = false, RequestOptions options = null)
    {
        return base.DeferAsync(ephemeral, options);
    }

    public new Task RespondAsync(string text = null, Embed[] embeds = null, bool isTTS = false, bool ephemeral = false, AllowedMentions allowedMentions = null, RequestOptions options = null, MessageComponent components = null, Embed embed = null, PollProperties poll = null, MessageFlags flags = MessageFlags.None)
    {
        return base.RespondAsync(text, embeds, isTTS, ephemeral, allowedMentions, options, components, embed, poll, flags);
    }

    public new Task<IUserMessage> FollowupAsync(string text = null, Embed[] embeds = null, bool isTTS = false, bool ephemeral = false, AllowedMentions allowedMentions = null, RequestOptions options = null, MessageComponent components = null, Embed embed = null, PollProperties poll = null, MessageFlags flags = MessageFlags.None)
    {
        return base.FollowupAsync(text, embeds, isTTS, ephemeral, allowedMentions, options, components, embed, poll, flags);
    }

    public new Task<IUserMessage> FollowupWithFilesAsync(IEnumerable<FileAttachment> attachments, string text = null, Embed[] embeds = null, bool isTTS = false, bool ephemeral = false, AllowedMentions allowedMentions = null, MessageComponent components = null, Embed embed = null, RequestOptions options = null, PollProperties poll = null, MessageFlags flags = MessageFlags.None)
    {
        return base.FollowupWithFilesAsync(attachments, text, embeds, isTTS, ephemeral, allowedMentions, components, embed, options, poll, flags);
    }
}
