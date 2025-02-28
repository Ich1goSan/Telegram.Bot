using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Helpers;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.Payments;
using Telegram.Bot.Types.ReplyMarkups;
using File = Telegram.Bot.Types.File;

namespace Telegram.Bot
{
    /// <summary>
    /// Extension methods that map to requests from Bot API documentation
    /// </summary>
    public static class TelegramBotClientExtensions
    {
        #region Getting updates

        /// <summary>
        /// Use this method to receive incoming updates using long polling (<see href="https://en.wikipedia.org/wiki/Push_technology#Long_polling">wiki</see>)
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="offset">
        /// Identifier of the first update to be returned. Must be greater by one than the highest among the identifiers of previously received updates. By default, updates starting with the earliest unconfirmed update are returned. An update is considered confirmed as soon as <see cref="GetUpdatesAsync"/> is called with an <paramref name="offset"/> higher than its <see cref="Update.Id"/>. The negative offset can be specified to retrieve updates starting from <paramref name="offset">-offset</paramref> update from the end of the updates queue. All previous updates will forgotten.
        /// </param>
        /// <param name="limit">
        /// Limits the number of updates to be retrieved. Values between 1-100 are accepted. Defaults to 100
        /// </param>
        /// <param name="timeout">
        /// Timeout in seconds for long polling. Defaults to 0, i.e. usual short polling. Should be positive, short polling should be used for testing purposes only.
        /// </param>
        /// <param name="allowedUpdates">
        /// A list of the update types you want your bot to receive. For example, specify [<see cref="UpdateType.Message"/>, <see cref="UpdateType.EditedChannelPost"/>, <see cref="UpdateType.CallbackQuery"/>] to only receive updates of these types. See <see cref="UpdateType"/> for a complete list of available update types. Specify an empty list to receive all update types except <see cref="UpdateType.ChatMember"/> (default). If not specified, the previous setting will be used.
        /// </param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <remarks>
        /// <list type="number">
        /// <item><description>This method will not work if an outgoing webhook is set up.</description></item>
        /// <item><description>In order to avoid getting duplicate updates, recalculate <paramref name="offset"/> after each server response.</description></item>
        /// </list>
        /// </remarks>
        /// <returns>An Array of <see cref="Update"/> objects is returned.</returns>
        public static async Task<Update[]> GetUpdatesAsync(
            this ITelegramBotClient botClient,
            int? offset = default,
            int? limit = default,
            int? timeout = default,
            IEnumerable<UpdateType>? allowedUpdates = default,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new GetUpdatesRequest
                {
                    Offset = offset,
                    Limit = limit,
                    Timeout = timeout,
                    AllowedUpdates = allowedUpdates
                }, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to specify a url and receive incoming updates via an outgoing webhook. Whenever there is an update for the bot, we will send an HTTPS POST request to the specified url, containing a JSON-serialized <see cref="Types.Update"/>. In case of an unsuccessful request, we will give up after a reasonable amount of attempts
        /// <para>
        /// If you'd like to make sure that the Webhook request comes from Telegram, we recommend using a secret path in the URL, e.g. <c>https://www.example.com/&lt;token&gt;</c>. Since nobody else knows your bot's token, you can be pretty sure it's us.
        /// </para>
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="url">HTTPS url to send updates to. Use an empty string to remove webhook integration</param>
        /// <param name="certificate">
        /// Upload your public key certificate so that the root certificate in use can be checked. See our <see href="https://core.telegram.org/bots/self-signed">self-signed guide</see> for details
        /// </param>
        /// <param name="ipAddress">The fixed IP address which will be used to send webhook requests instead of the IP address resolved through DNS</param>
        /// <param name="maxConnections">Maximum allowed number of simultaneous HTTPS connections to the webhook for update delivery, 1-100. Defaults to <i>40</i>. Use lower values to limit the load on your bot's server, and higher values to increase your bot's throughput</param>
        /// <param name="allowedUpdates">
        /// <para>A list of the update types you want your bot to receive. For example, specify [<see cref="UpdateType.Message"/>, <see cref="UpdateType.EditedChannelPost"/>, <see cref="UpdateType.CallbackQuery"/>] to only receive updates of these types. See <see cref="UpdateType"/> for a complete list of available update types. Specify an empty list to receive all update types except <see cref="UpdateType.ChatMember"/> (default). If not specified, the previous setting will be used
        /// </para>
        /// <para>
        /// Please note that this parameter doesn't affect updates created before the call to the <see cref="SetWebhookAsync"/>, so unwanted updates may be received for a short period of time.
        /// </para>
        /// </param>
        /// <param name="dropPendingUpdates">Pass True to drop all pending updates</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <remarks>
        /// 1. You will not be able to receive updates using <see cref="GetUpdatesAsync"/> for as long as an outgoing webhook is set up.<para/>
        /// 2. To use a self-signed certificate, you need to upload your <see href="https://core.telegram.org/bots/self-signed">public key certificate</see> using <paramref name="certificate"/> parameter. Please upload as <see cref="InputFileStream"/>, sending a String will not work.<para/>
        /// 3. Ports currently supported for Webhooks: <b>443, 80, 88, 8443</b>.<para/>
        /// If you're having any trouble setting up webhooks, please check out this <see href="https://core.telegram.org/bots/webhooks">amazing guide to Webhooks</see>.
        /// </remarks>
        public static async Task SetWebhookAsync(
            this ITelegramBotClient botClient,
            string url,
            InputFileStream? certificate = default,
            string? ipAddress = default,
            int? maxConnections = default,
            IEnumerable<UpdateType>? allowedUpdates = default,
            bool? dropPendingUpdates = default,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new SetWebhookRequest(url)
                {
                    Certificate = certificate,
                    IpAddress = ipAddress,
                    MaxConnections = maxConnections,
                    AllowedUpdates = allowedUpdates,
                    DropPendingUpdates = dropPendingUpdates
                }, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to remove webhook integration if you decide to switch back to <see cref="GetUpdatesAsync"/>
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="dropPendingUpdates">Pass True to drop all pending updates</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Returns true on success</returns>
        public static async Task DeleteWebhookAsync(
            this ITelegramBotClient botClient,
            bool? dropPendingUpdates = default,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new DeleteWebhookRequest()
                {
                    DropPendingUpdates = dropPendingUpdates
                }, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to get current webhook status.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>On success, returns a <see cref="WebhookInfo"/> object. If the bot is using <see cref="GetUpdatesAsync"/>, will return an object with the <see cref="WebhookInfo.Url"/> field empty.</returns>
        public static async Task<WebhookInfo> GetWebhookInfoAsync(
            this ITelegramBotClient botClient,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new GetWebhookInfoRequest(), cancellationToken).ConfigureAwait(false);

        #endregion Getting updates

        #region Available methods

        /// <summary>
        /// A simple method for testing your bot's auth token.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Returns basic information about the bot in form of a <see cref="User"/> object.</returns>
        public static async Task<User> GetMeAsync(
            this ITelegramBotClient botClient,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new GetMeRequest(), cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to log out from the cloud Bot API server before launching the bot locally. You <b>must</b> log out the bot before running it locally, otherwise there is no guarantee that the bot will receive updates. After a successful call, you can immediately log in on a local server, but will not be able to log in back to the cloud Bot API server for 10 minutes.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public static async Task LogOutAsync(
            this ITelegramBotClient botClient,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new LogOutRequest(), cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to close the bot instance before moving it from one local server to another. You need to delete the webhook before calling this method to ensure that the bot isn't launched again after server restart. The method will return error 429 in the first 10 minutes after the bot is launched.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public static async Task CloseAsync(
            this ITelegramBotClient botClient,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new CloseRequest(), cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to send text messages.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel (in the format <c>@channelusername</c>)</param>
        /// <param name="text">Text of the message to be sent, 1-4096 characters after entities parsing</param>
        /// <param name="parseMode">Mode for parsing entities in the new caption. See <see href="https://core.telegram.org/bots/api#formatting-options">formatting</see> options for more details.</param>
        /// <param name="entities">List of special entities that appear in message text, which can be specified instead of <see cref="Types.Enums.ParseMode"/></param>
        /// <param name="disableWebPagePreview">Disables link previews for links in this message</param>
        /// <param name="disableNotification">Sends the message silently. Users will receive a notification with no sound.</param>
        /// <param name="replyToMessageId">If the message is a reply, ID of the original message</param>
        /// <param name="allowSendingWithoutReply">Pass True, if the message should be sent even if the specified replied-to message is not found</param>
        /// <param name="replyMarkup">Additional interface options. An <see cref="InlineKeyboardMarkup">inline keyboard</see>, <see cref="ReplyKeyboardMarkup">custom reply keyboard</see>, instructions to <see cref="ReplyKeyboardRemove">remove reply keyboard</see> or to <see cref="ForceReplyMarkup">force a reply</see> from the user</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>On success, the sent <see cref="Message"/> is returned.</returns>
        public static async Task<Message> SendTextMessageAsync(
            this ITelegramBotClient botClient,
            ChatId chatId,
            string text,
            ParseMode? parseMode = default,
            IEnumerable<MessageEntity>? entities = default,
            bool? disableWebPagePreview = default,
            bool? disableNotification = default,
            int? replyToMessageId = default,
            bool? allowSendingWithoutReply = default,
            IReplyMarkup? replyMarkup = default,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new SendMessageRequest(chatId, text)
                {
                    ParseMode = parseMode,
                    Entities = entities,
                    DisableWebPagePreview = disableWebPagePreview,
                    DisableNotification = disableNotification,
                    ReplyToMessageId = replyToMessageId,
                    AllowSendingWithoutReply = allowSendingWithoutReply,
                    ReplyMarkup = replyMarkup
                }, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to forward messages of any kind. Service messages can't be forwarded.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel (in the format <c>@channelusername</c>)</param>
        /// <param name="fromChatId">Unique identifier for the chat where the original message was sent (or channel username in the format <c>@channelusername</c>)</param>
        /// <param name="messageId">Message identifier in the chat specified in <paramref name="fromChatId"/></param>
        /// <param name="disableNotification">Sends the message silently. Users will receive a notification with no sound.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>On success, the sent <see cref="Message"/> is returned.</returns>
        public static async Task<Message> ForwardMessageAsync(
            this ITelegramBotClient botClient,
            ChatId chatId,
            ChatId fromChatId,
            int messageId,
            bool? disableNotification = default,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new ForwardMessageRequest(chatId, fromChatId, messageId)
                {
                    DisableNotification = disableNotification
                }, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to copy messages of any kind. Service messages and invoice messages can't be copied. The method is analogous to the method <see cref="ForwardMessageAsync"/>, but the copied message doesn't have a link to the original message.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel (in the format <c>@channelusername</c>)</param>
        /// <param name="fromChatId">Unique identifier for the chat where the original message was sent (or channel username in the format <c>@channelusername</c>)</param>
        /// <param name="messageId">Message identifier in the chat specified in <paramref name="fromChatId"/></param>
        /// <param name="caption">New caption for media, 0-1024 characters after entities parsing. If not specified, the original caption is kept</param>
        /// <param name="parseMode">Mode for parsing entities in the new caption. See <see href="https://core.telegram.org/bots/api#formatting-options">formatting</see> options for more details.</param>
        /// <param name="captionEntities">List of special entities that appear in the caption, which can be specified instead of <see cref="Types.Enums.ParseMode"/></param>
        /// <param name="disableNotification">Sends the message silently. Users will receive a notification with no sound.</param>
        /// <param name="replyToMessageId">If the message is a reply, ID of the original message</param>
        /// <param name="allowSendingWithoutReply">Pass True, if the message should be sent even if the specified replied-to message is not found</param>
        /// <param name="replyMarkup">Additional interface options. An <see cref="InlineKeyboardMarkup">inline keyboard</see>, <see cref="ReplyKeyboardMarkup">custom reply keyboard</see>, instructions to <see cref="ReplyKeyboardRemove">remove reply keyboard</see> or to <see cref="ForceReplyMarkup">force a reply</see> from the user</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Returns the <see cref="Types.MessageId"/> of the sent message on success.</returns>
        public static async Task<MessageId> CopyMessageAsync(
            this ITelegramBotClient botClient,
            ChatId chatId,
            ChatId fromChatId,
            int messageId,
            string? caption = default,
            ParseMode? parseMode = default,
            IEnumerable<MessageEntity>? captionEntities = default,
            bool? disableNotification = default,
            int? replyToMessageId = default,
            bool? allowSendingWithoutReply = default,
            IReplyMarkup? replyMarkup = default,
            CancellationToken cancellationToken = default) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new CopyMessageRequest(chatId, fromChatId, messageId)
                {
                    Caption = caption,
                    ParseMode = parseMode,
                    CaptionEntities = captionEntities,
                    ReplyToMessageId = replyToMessageId,
                    DisableNotification = disableNotification,
                    AllowSendingWithoutReply = allowSendingWithoutReply,
                    ReplyMarkup = replyMarkup
                }, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to send photos.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel (in the format <c>@channelusername</c>)</param>
        /// <param name="photo">Photo to send. Pass a <see cref="InputTelegramFile.FileId"/> as String to send a photo that exists on the Telegram servers (recommended), pass an HTTP URL as a String for Telegram to get a photo from the Internet, or upload a new photo using multipart/form-data. The photo must be at most 10 MB in size. The photo's width and height must not exceed 10000 in total. Width and height ratio must be at most 20</param>
        /// <param name="caption">Photo caption (may also be used when resending photos by <see cref="InputTelegramFile.FileId"/>), 0-1024 characters after entities parsing</param>
        /// <param name="parseMode">Mode for parsing entities in the new caption. See <see href="https://core.telegram.org/bots/api#formatting-options">formatting</see> options for more details.</param>
        /// <param name="captionEntities">List of special entities that appear in the caption, which can be specified instead of <see cref="Types.Enums.ParseMode"/></param>
        /// <param name="disableNotification">Sends the message silently. Users will receive a notification with no sound.</param>
        /// <param name="replyToMessageId">If the message is a reply, ID of the original message</param>
        /// <param name="allowSendingWithoutReply">Pass True, if the message should be sent even if the specified replied-to message is not found</param>
        /// <param name="replyMarkup">Additional interface options. An <see cref="InlineKeyboardMarkup">inline keyboard</see>, <see cref="ReplyKeyboardMarkup">custom reply keyboard</see>, instructions to <see cref="ReplyKeyboardRemove">remove reply keyboard</see> or to <see cref="ForceReplyMarkup">force a reply</see> from the user</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>On success, the sent <see cref="Message"/> is returned.</returns>
        public static async Task<Message> SendPhotoAsync(
            this ITelegramBotClient botClient,
            ChatId chatId,
            InputOnlineFile photo,
            string? caption = default,
            ParseMode? parseMode = default,
            IEnumerable<MessageEntity>? captionEntities = default,
            bool? disableNotification = default,
            int? replyToMessageId = default,
            bool? allowSendingWithoutReply = default,
            IReplyMarkup? replyMarkup = default,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new SendPhotoRequest(chatId, photo)
                {
                    Caption = caption,
                    ParseMode = parseMode,
                    CaptionEntities = captionEntities,
                    DisableNotification = disableNotification,
                    ReplyToMessageId = replyToMessageId,
                    AllowSendingWithoutReply = allowSendingWithoutReply,
                    ReplyMarkup = replyMarkup
                }, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to send audio files, if you want Telegram clients to display them in the music player. Your audio must be in the .MP3 or .M4A format. Bots can currently send audio files of up to 50 MB in size, this limit may be changed in the future.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel (in the format <c>@channelusername</c>)</param>
        /// <param name="audio">Audio file to send. Pass a <see cref="InputTelegramFile.FileId"/> as String to send an audio file that exists on the Telegram servers (recommended), pass an HTTP URL as a String for Telegram to get an audio file from the Internet, or upload a new one using multipart/form-data</param>
        /// <param name="caption">Audio caption, 0-1024 characters after entities parsing</param>
        /// <param name="parseMode">Mode for parsing entities in the new caption. See <see href="https://core.telegram.org/bots/api#formatting-options">formatting</see> options for more details.</param>
        /// <param name="captionEntities">List of special entities that appear in the caption, which can be specified instead of <see cref="Types.Enums.ParseMode"/></param>
        /// <param name="duration">Duration of the audio in seconds</param>
        /// <param name="performer">Performer</param>
        /// <param name="title">Track name</param>
        /// <param name="thumb">Thumbnail of the file sent; can be ignored if thumbnail generation for the file is supported server-side. The thumbnail should be in JPEG format and less than 200 kB in size. A thumbnail's width and height should not exceed 320. Ignored if the file is not uploaded using multipart/form-data. Thumbnails can't be reused and can be only uploaded as a new file, so you can pass "attach://&lt;file_attach_name&gt;" if the thumbnail was uploaded using multipart/form-data under &lt;file_attach_name&gt;.</param>
        /// <param name="disableNotification">Sends the message silently. Users will receive a notification with no sound.</param>
        /// <param name="replyToMessageId">If the message is a reply, ID of the original message</param>
        /// <param name="allowSendingWithoutReply">Pass True, if the message should be sent even if the specified replied-to message is not found</param>
        /// <param name="replyMarkup">Additional interface options. An <see cref="InlineKeyboardMarkup">inline keyboard</see>, <see cref="ReplyKeyboardMarkup">custom reply keyboard</see>, instructions to <see cref="ReplyKeyboardRemove">remove reply keyboard</see> or to <see cref="ForceReplyMarkup">force a reply</see> from the user</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>On success, the sent <see cref="Message"/> is returned.</returns>
        public static async Task<Message> SendAudioAsync(
            this ITelegramBotClient botClient,
            ChatId chatId,
            InputOnlineFile audio,
            string? caption = default,
            ParseMode? parseMode = default,
            IEnumerable<MessageEntity>? captionEntities = default,
            int? duration = default,
            string? performer = default,
            string? title = default,
            InputMedia? thumb = default,
            bool? disableNotification = default,
            int? replyToMessageId = default,
            bool? allowSendingWithoutReply = default,
            IReplyMarkup? replyMarkup = default,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new SendAudioRequest(chatId, audio)
                {
                    Caption = caption,
                    ParseMode = parseMode,
                    CaptionEntities = captionEntities,
                    Duration = duration,
                    Performer = performer,
                    Title = title,
                    Thumb = thumb,
                    DisableNotification = disableNotification,
                    ReplyToMessageId = replyToMessageId,
                    AllowSendingWithoutReply = allowSendingWithoutReply,
                    ReplyMarkup = replyMarkup
                }, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to send general files. Bots can currently send files of any type of up to 50 MB in size, this limit may be changed in the future.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel (in the format <c>@channelusername</c>)</param>
        /// <param name="document">File to send. Pass a <see cref="InputTelegramFile.FileId"/> as String to send a file that exists on the Telegram servers (recommended), pass an HTTP URL as a String for Telegram to get a file from the Internet, or upload a new one using multipart/form-data</param>
        /// <param name="thumb">Thumbnail of the file sent; can be ignored if thumbnail generation for the file is supported server-side. The thumbnail should be in JPEG format and less than 200 kB in size. A thumbnail's width and height should not exceed 320. Ignored if the file is not uploaded using multipart/form-data. Thumbnails can't be reused and can be only uploaded as a new file, so you can pass "attach://&lt;file_attach_name&gt;" if the thumbnail was uploaded using multipart/form-data under &lt;file_attach_name&gt;</param>
        /// <param name="caption">Document caption (may also be used when resending documents by file_id), 0-1024 characters after entities parsing</param>
        /// <param name="parseMode">Mode for parsing entities in the new caption. See <see href="https://core.telegram.org/bots/api#formatting-options">formatting</see> options for more details.</param>
        /// <param name="captionEntities">List of special entities that appear in the caption, which can be specified instead of <see cref="Types.Enums.ParseMode"/></param>
        /// <param name="disableContentTypeDetection">Disables automatic server-side content type detection for files uploaded using multipart/form-data</param>
        /// <param name="disableNotification">Sends the message silently. Users will receive a notification with no sound.</param>
        /// <param name="replyToMessageId">If the message is a reply, ID of the original message</param>
        /// <param name="allowSendingWithoutReply">Pass True, if the message should be sent even if the specified replied-to message is not found</param>
        /// <param name="replyMarkup">Additional interface options. An <see cref="InlineKeyboardMarkup">inline keyboard</see>, <see cref="ReplyKeyboardMarkup">custom reply keyboard</see>, instructions to <see cref="ReplyKeyboardRemove">remove reply keyboard</see> or to <see cref="ForceReplyMarkup">force a reply</see> from the user</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>On success, the sent <see cref="Message"/> is returned.</returns>
        public static async Task<Message> SendDocumentAsync(
            this ITelegramBotClient botClient,
            ChatId chatId,
            InputOnlineFile document,
            InputMedia? thumb = default,
            string? caption = default,
            ParseMode? parseMode = default,
            IEnumerable<MessageEntity>? captionEntities = default,
            bool? disableContentTypeDetection = default,
            bool? disableNotification = default,
            int? replyToMessageId = default,
            bool? allowSendingWithoutReply = default,
            IReplyMarkup? replyMarkup = default,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new SendDocumentRequest(chatId, document)
                {
                    Thumb = thumb,
                    Caption = caption,
                    ParseMode = parseMode,
                    CaptionEntities = captionEntities,
                    DisableContentTypeDetection = disableContentTypeDetection,
                    DisableNotification = disableNotification,
                    ReplyToMessageId = replyToMessageId,
                    AllowSendingWithoutReply = allowSendingWithoutReply,
                    ReplyMarkup = replyMarkup
                }, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to send video files, Telegram clients support mp4 videos (other formats may be sent as <see cref="Document"/>). Bots can currently send video files of up to 50 MB in size, this limit may be changed in the future.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel (in the format <c>@channelusername</c>)</param>
        /// <param name="video">Video to send. Pass a <see cref="InputTelegramFile.FileId"/> as String to send a video that exists on the Telegram servers (recommended), pass an HTTP URL as a String for Telegram to get a video from the Internet, or upload a new video using multipart/form-data</param>
        /// <param name="duration">Duration of sent video in seconds</param>
        /// <param name="width">Video width</param>
        /// <param name="height">Video height</param>
        /// <param name="thumb">Thumbnail of the file sent; can be ignored if thumbnail generation for the file is supported server-side. The thumbnail should be in JPEG format and less than 200 kB in size. A thumbnail's width and height should not exceed 320. Ignored if the file is not uploaded using multipart/form-data. Thumbnails can't be reused and can be only uploaded as a new file, so you can pass "attach://&lt;file_attach_name&gt;" if the thumbnail was uploaded using multipart/form-data under &lt;file_attach_name&gt;</param>
        /// <param name="caption">Video caption (may also be used when resending videos by file_id), 0-1024 characters after entities parsing</param>
        /// <param name="parseMode">Mode for parsing entities in the new caption. See <see href="https://core.telegram.org/bots/api#formatting-options">formatting</see> options for more details.</param>
        /// <param name="captionEntities">List of special entities that appear in the caption, which can be specified instead of <see cref="Types.Enums.ParseMode"/></param>
        /// <param name="supportsStreaming">Pass True, if the uploaded video is suitable for streaming</param>
        /// <param name="disableNotification">Sends the message silently. Users will receive a notification with no sound.</param>
        /// <param name="replyToMessageId">If the message is a reply, ID of the original message</param>
        /// <param name="allowSendingWithoutReply">Pass True, if the message should be sent even if the specified replied-to message is not found</param>
        /// <param name="replyMarkup">Additional interface options. An <see cref="InlineKeyboardMarkup">inline keyboard</see>, <see cref="ReplyKeyboardMarkup">custom reply keyboard</see>, instructions to <see cref="ReplyKeyboardRemove">remove reply keyboard</see> or to <see cref="ForceReplyMarkup">force a reply</see> from the user</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>On success, the sent <see cref="Message"/> is returned.</returns>
        public static async Task<Message> SendVideoAsync(
            this ITelegramBotClient botClient,
            ChatId chatId,
            InputOnlineFile video,
            int? duration = default,
            int? width = default,
            int? height = default,
            InputMedia? thumb = default,
            string? caption = default,
            ParseMode? parseMode = default,
            IEnumerable<MessageEntity>? captionEntities = default,
            bool? supportsStreaming = default,
            bool? disableNotification = default,
            int? replyToMessageId = default,
            bool? allowSendingWithoutReply = default,
            IReplyMarkup? replyMarkup = default,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new SendVideoRequest(chatId, video)
                {
                    Duration = duration,
                    Width = width,
                    Height = height,
                    Thumb = thumb,
                    Caption = caption,
                    ParseMode = parseMode,
                    CaptionEntities = captionEntities,
                    SupportsStreaming = supportsStreaming,
                    DisableNotification = disableNotification,
                    ReplyToMessageId = replyToMessageId,
                    AllowSendingWithoutReply = allowSendingWithoutReply,
                    ReplyMarkup = replyMarkup
                }, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to send animation files (GIF or H.264/MPEG-4 AVC video without sound). Bots can currently send animation files of up to 50 MB in size, this limit may be changed in the future.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel (in the format <c>@channelusername</c>)</param>
        /// <param name="animation">Animation to send. Pass a <see cref="InputTelegramFile.FileId"/> as String to send an animation that exists on the Telegram servers (recommended), pass an HTTP URL as a String for Telegram to get an animation from the Internet, or upload a new animation using multipart/form-data</param>
        /// <param name="duration">Duration of sent animation in seconds</param>
        /// <param name="width">Animation width</param>
        /// <param name="height">Animation height</param>
        /// <param name="thumb">Thumbnail of the file sent; can be ignored if thumbnail generation for the file is supported server-side. The thumbnail should be in JPEG format and less than 200 kB in size. A thumbnail's width and height should not exceed 320. Ignored if the file is not uploaded using multipart/form-data. Thumbnails can't be reused and can be only uploaded as a new file, so you can pass "attach://&lt;file_attach_name&gt;" if the thumbnail was uploaded using multipart/form-data under &lt;file_attach_name&gt;</param>
        /// <param name="caption">Animation caption (may also be used when resending animation by <see cref="InputTelegramFile.FileId"/>), 0-1024 characters after entities parsing</param>
        /// <param name="parseMode">Mode for parsing entities in the new caption. See <see href="https://core.telegram.org/bots/api#formatting-options">formatting</see> options for more details.</param>
        /// <param name="captionEntities">List of special entities that appear in the caption, which can be specified instead of parse_mode</param>
        /// <param name="disableNotification">Sends the message silently. Users will receive a notification with no sound.</param>
        /// <param name="replyToMessageId">If the message is a reply, ID of the original message</param>
        /// <param name="allowSendingWithoutReply">Pass True, if the message should be sent even if the specified replied-to message is not found</param>
        /// <param name="replyMarkup">Additional interface options. An <see cref="InlineKeyboardMarkup">inline keyboard</see>, <see cref="ReplyKeyboardMarkup">custom reply keyboard</see>, instructions to <see cref="ReplyKeyboardRemove">remove reply keyboard</see> or to <see cref="ForceReplyMarkup">force a reply</see> from the user</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>On success, the sent <see cref="Message"/> is returned.</returns>
        public static async Task<Message> SendAnimationAsync(
            this ITelegramBotClient botClient,
            ChatId chatId,
            InputOnlineFile animation,
            int? duration = default,
            int? width = default,
            int? height = default,
            InputMedia? thumb = default,
            string? caption = default,
            ParseMode? parseMode = default,
            IEnumerable<MessageEntity>? captionEntities = default,
            bool? disableNotification = default,
            int? replyToMessageId = default,
            bool? allowSendingWithoutReply = default,
            IReplyMarkup? replyMarkup = default,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new SendAnimationRequest(chatId, animation)
                {
                    Duration = duration,
                    Width = width,
                    Height = height,
                    Thumb = thumb,
                    Caption = caption,
                    ParseMode = parseMode,
                    CaptionEntities = captionEntities,
                    DisableNotification = disableNotification,
                    ReplyToMessageId = replyToMessageId,
                    AllowSendingWithoutReply = allowSendingWithoutReply,
                    ReplyMarkup = replyMarkup,
                }, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to send audio files, if you want Telegram clients to display the file as a playable voice message. For this to work, your audio must be in an .OGG file encoded with OPUS (other formats may be sent as <see cref="Audio"/> or <see cref="Document"/>). Bots can currently send voice messages of up to 50 MB in size, this limit may be changed in the future.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel (in the format <c>@channelusername</c>)</param>
        /// <param name="voice">Audio file to send. Pass a <see cref="InputTelegramFile.FileId"/> as String to send a file that exists on the Telegram servers (recommended), pass an HTTP URL as a String for Telegram to get a file from the Internet, or upload a new one using multipart/form-data</param>
        /// <param name="caption">Voice message caption, 0-1024 characters after entities parsing</param>
        /// <param name="parseMode">Mode for parsing entities in the new caption. See <see href="https://core.telegram.org/bots/api#formatting-options">formatting</see> options for more details.</param>
        /// <param name="captionEntities">List of special entities that appear in the caption, which can be specified instead of parse_mode</param>
        /// <param name="duration">Duration of the voice message in seconds</param>
        /// <param name="disableNotification">Sends the message silently. Users will receive a notification with no sound.</param>
        /// <param name="replyToMessageId">If the message is a reply, ID of the original message</param>
        /// <param name="allowSendingWithoutReply">Pass True, if the message should be sent even if the specified replied-to message is not found</param>
        /// <param name="replyMarkup">Additional interface options. An <see cref="InlineKeyboardMarkup">inline keyboard</see>, <see cref="ReplyKeyboardMarkup">custom reply keyboard</see>, instructions to <see cref="ReplyKeyboardRemove">remove reply keyboard</see> or to <see cref="ForceReplyMarkup">force a reply</see> from the user</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>On success, the sent <see cref="Message"/> is returned.</returns>
        public static async Task<Message> SendVoiceAsync(
            this ITelegramBotClient botClient,
            ChatId chatId,
            InputOnlineFile voice,
            string? caption = default,
            ParseMode? parseMode = default,
            IEnumerable<MessageEntity>? captionEntities = default,
            int? duration = default,
            bool? disableNotification = default,
            int? replyToMessageId = default,
            bool? allowSendingWithoutReply = default,
            IReplyMarkup? replyMarkup = default,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new SendVoiceRequest(chatId, voice)
                {
                    Caption = caption,
                    ParseMode = parseMode,
                    CaptionEntities = captionEntities,
                    Duration = duration,
                    DisableNotification = disableNotification,
                    ReplyToMessageId = replyToMessageId,
                    AllowSendingWithoutReply = allowSendingWithoutReply,
                    ReplyMarkup = replyMarkup
                }, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// As of <see href="https://telegram.org/blog/video-messages-and-telescope">v.4.0</see>, Telegram clients support rounded square mp4 videos of up to 1 minute long. Use this method to send video messages.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel (in the format <c>@channelusername</c>)</param>
        /// <param name="videoNote">Video note to send. Pass a <see cref="InputTelegramFile.FileId"/> as String to send a video note that exists on the Telegram servers (recommended) or upload a new video using multipart/form-data. Sending video notes by a URL is currently unsupported</param>
        /// <param name="duration">Duration of sent video in seconds</param>
        /// <param name="length">Video width and height, i.e. diameter of the video message</param>
        /// <param name="thumb">Thumbnail of the file sent; can be ignored if thumbnail generation for the file is supported server-side. The thumbnail should be in JPEG format and less than 200 kB in size. A thumbnail's width and height should not exceed 320. Ignored if the file is not uploaded using multipart/form-data. Thumbnails can't be reused and can be only uploaded as a new file, so you can pass "attach://&lt;file_attach_name&gt;" if the thumbnail was uploaded using multipart/form-data under &lt;file_attach_name&gt;</param>
        /// <param name="disableNotification">Sends the message silently. Users will receive a notification with no sound.</param>
        /// <param name="replyToMessageId">If the message is a reply, ID of the original message</param>
        /// <param name="allowSendingWithoutReply">Pass True, if the message should be sent even if the specified replied-to message is not found</param>
        /// <param name="replyMarkup">Additional interface options. An <see cref="InlineKeyboardMarkup">inline keyboard</see>, <see cref="ReplyKeyboardMarkup">custom reply keyboard</see>, instructions to <see cref="ReplyKeyboardRemove">remove reply keyboard</see> or to <see cref="ForceReplyMarkup">force a reply</see> from the user</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>On success, the sent <see cref="Message"/> is returned.</returns>
        public static async Task<Message> SendVideoNoteAsync(
            this ITelegramBotClient botClient,
            ChatId chatId,
            InputTelegramFile videoNote,
            int? duration = default,
            int? length = default,
            InputMedia? thumb = default,
            bool? disableNotification = default,
            int? replyToMessageId = default,
            bool? allowSendingWithoutReply = default,
            IReplyMarkup? replyMarkup = default,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new SendVideoNoteRequest(chatId, videoNote)
                {
                    Duration = duration,
                    Length = length,
                    Thumb = thumb,
                    DisableNotification = disableNotification,
                    ReplyToMessageId = replyToMessageId,
                    AllowSendingWithoutReply = allowSendingWithoutReply,
                    ReplyMarkup = replyMarkup
                }, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to send a group of photos, videos, documents or audios as an album. Documents and audio files can be only grouped in an album with messages of the same type.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel (in the format <c>@channelusername</c>)</param>
        /// <param name="media">An array describing messages to be sent, must include 2-10 items</param>
        /// <param name="disableNotification">Sends the message silently. Users will receive a notification with no sound.</param>
        /// <param name="replyToMessageId">If the message is a reply, ID of the original message</param>
        /// <param name="allowSendingWithoutReply">Pass True, if the message should be sent even if the specified replied-to message is not found</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>On success, an array of <see cref="Message"/>s that were sent is returned.</returns>
        public static async Task<Message[]> SendMediaGroupAsync(
            this ITelegramBotClient botClient,
            ChatId chatId,
            IEnumerable<IAlbumInputMedia> media,
            bool? disableNotification = default,
            int? replyToMessageId = default,
            bool? allowSendingWithoutReply = default,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new SendMediaGroupRequest(chatId, media)
                {
                    DisableNotification = disableNotification,
                    ReplyToMessageId = replyToMessageId,
                    AllowSendingWithoutReply = allowSendingWithoutReply,
                }, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to send point on the map.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel (in the format <c>@channelusername</c>)</param>
        /// <param name="latitude">Latitude of location</param>
        /// <param name="longitude">Longitude of location</param>
        /// <param name="livePeriod">Period in seconds for which the location will be updated, should be between 60 and 86400</param>
        /// <param name="heading">For live locations, a direction in which the user is moving, in degrees. Must be between 1 and 360 if specified.</param>
        /// <param name="proximityAlertRadius">For live locations, a maximum distance for proximity alerts about approaching another chat member, in meters. Must be between 1 and 100000 if specified.</param>
        /// <param name="disableNotification">Sends the message silently. Users will receive a notification with no sound.</param>
        /// <param name="replyToMessageId">If the message is a reply, ID of the original message</param>
        /// <param name="allowSendingWithoutReply">Pass True, if the message should be sent even if the specified replied-to message is not found</param>
        /// <param name="replyMarkup">Additional interface options. An <see cref="InlineKeyboardMarkup">inline keyboard</see>, <see cref="ReplyKeyboardMarkup">custom reply keyboard</see>, instructions to <see cref="ReplyKeyboardRemove">remove reply keyboard</see> or to <see cref="ForceReplyMarkup">force a reply</see> from the user</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>On success, the sent <see cref="Message"/> is returned.</returns>
        public static async Task<Message> SendLocationAsync(
            this ITelegramBotClient botClient,
            ChatId chatId,
            double latitude,
            double longitude,
            int? livePeriod = default,
            int? heading = default,
            int? proximityAlertRadius = default,
            bool? disableNotification = default,
            int? replyToMessageId = default,
            bool? allowSendingWithoutReply = default,
            IReplyMarkup? replyMarkup = default,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new SendLocationRequest(chatId, latitude, longitude)
                {
                    LivePeriod = livePeriod,
                    Heading = heading,
                    ProximityAlertRadius = proximityAlertRadius,
                    DisableNotification = disableNotification,
                    ReplyToMessageId = replyToMessageId,
                    AllowSendingWithoutReply = allowSendingWithoutReply,
                    ReplyMarkup = replyMarkup,
                }, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to edit live location messages. A location can be edited until its <see cref="Types.Location.LivePeriod"/> expires or editing is explicitly disabled by a call to <see cref="StopMessageLiveLocationAsync(ITelegramBotClient, ChatId, int, InlineKeyboardMarkup?, CancellationToken)"/>.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel (in the format <c>@channelusername</c>)</param>
        /// <param name="messageId">Identifier of the message to edit</param>
        /// <param name="latitude">Latitude of new location</param>
        /// <param name="longitude">Longitude of new location</param>
        /// <param name="horizontalAccuracy">The radius of uncertainty for the location, measured in meters; 0-1500</param>
        /// <param name="heading">Direction in which the user is moving, in degrees. Must be between 1 and 360 if specified.</param>
        /// <param name="proximityAlertRadius">Maximum distance for proximity alerts about approaching another chat member, in meters. Must be between 1 and 100000 if specified.</param>
        /// <param name="replyMarkup">Additional interface options. An <see cref="InlineKeyboardMarkup">inline keyboard</see>, <see cref="ReplyKeyboardMarkup">custom reply keyboard</see>, instructions to <see cref="ReplyKeyboardRemove">remove reply keyboard</see> or to <see cref="ForceReplyMarkup">force a reply</see> from the user</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>On success the edited <see cref="Message"/> is returned.</returns>
        public static async Task<Message> EditMessageLiveLocationAsync(
            this ITelegramBotClient botClient,
            ChatId chatId,
            int messageId,
            double latitude,
            double longitude,
            float? horizontalAccuracy = default,
            int? heading = default,
            int? proximityAlertRadius = default,
            InlineKeyboardMarkup? replyMarkup = default,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new EditMessageLiveLocationRequest(chatId, messageId, latitude, longitude)
                {
                    HorizontalAccuracy = horizontalAccuracy,
                    Heading = heading,
                    ProximityAlertRadius = proximityAlertRadius,
                    ReplyMarkup = replyMarkup
                }, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to edit live location messages. A location can be edited until its <see cref="Types.Location.LivePeriod"/> expires or editing is explicitly disabled by a call to <see cref="StopMessageLiveLocationAsync(ITelegramBotClient, string, InlineKeyboardMarkup?, CancellationToken)"/>.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="inlineMessageId">Identifier of the inline message</param>
        /// <param name="latitude">Latitude of new location</param>
        /// <param name="longitude">Longitude of new location</param>
        /// <param name="horizontalAccuracy">The radius of uncertainty for the location, measured in meters; 0-1500</param>
        /// <param name="heading">Direction in which the user is moving, in degrees. Must be between 1 and 360 if specified.</param>
        /// <param name="proximityAlertRadius">Maximum distance for proximity alerts about approaching another chat member, in meters. Must be between 1 and 100000 if specified.</param>
        /// <param name="replyMarkup">Additional interface options. An <see cref="InlineKeyboardMarkup">inline keyboard</see>, <see cref="ReplyKeyboardMarkup">custom reply keyboard</see>, instructions to <see cref="ReplyKeyboardRemove">remove reply keyboard</see> or to <see cref="ForceReplyMarkup">force a reply</see> from the user</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public static async Task EditMessageLiveLocationAsync(
            this ITelegramBotClient botClient,
            string inlineMessageId,
            double latitude,
            double longitude,
            float? horizontalAccuracy = default,
            int? heading = default,
            int? proximityAlertRadius = default,
            InlineKeyboardMarkup? replyMarkup = default,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new EditInlineMessageLiveLocationRequest(inlineMessageId, latitude, longitude)
                {
                    HorizontalAccuracy = horizontalAccuracy,
                    Heading = heading,
                    ProximityAlertRadius = proximityAlertRadius,
                    ReplyMarkup = replyMarkup
                }, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to stop updating a live location message before <see cref="Types.Location.LivePeriod"/> expires.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel (in the format <c>@channelusername</c>)</param>
        /// <param name="messageId">Identifier of the sent message</param>
        /// <param name="replyMarkup">Additional interface options. An <see cref="InlineKeyboardMarkup">inline keyboard</see>, <see cref="ReplyKeyboardMarkup">custom reply keyboard</see>, instructions to <see cref="ReplyKeyboardRemove">remove reply keyboard</see> or to <see cref="ForceReplyMarkup">force a reply</see> from the user</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>On success the sent <see cref="Message"/> is returned.</returns>
        public static async Task<Message> StopMessageLiveLocationAsync(
            this ITelegramBotClient botClient,
            ChatId chatId,
            int messageId,
            InlineKeyboardMarkup? replyMarkup = default,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new StopMessageLiveLocationRequest(chatId, messageId)
                {
                    ReplyMarkup = replyMarkup
                }, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to stop updating a live location message before <see cref="Types.Location.LivePeriod"/> expires.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="inlineMessageId">Identifier of the inline message</param>
        /// <param name="replyMarkup">Additional interface options. An <see cref="InlineKeyboardMarkup">inline keyboard</see>, <see cref="ReplyKeyboardMarkup">custom reply keyboard</see>, instructions to <see cref="ReplyKeyboardRemove">remove reply keyboard</see> or to <see cref="ForceReplyMarkup">force a reply</see> from the user</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public static async Task StopMessageLiveLocationAsync(
            this ITelegramBotClient botClient,
            string inlineMessageId,
            InlineKeyboardMarkup? replyMarkup = default,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new StopInlineMessageLiveLocationRequest(inlineMessageId)
                {
                    ReplyMarkup = replyMarkup
                }, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to send information about a venue.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel (in the format <c>@channelusername</c>)</param>
        /// <param name="latitude">Latitude of the venue</param>
        /// <param name="longitude">Longitude of the venue</param>
        /// <param name="title">Name of the venue</param>
        /// <param name="address">Address of the venue</param>
        /// <param name="foursquareId">Foursquare identifier of the venue</param>
        /// <param name="foursquareType">Foursquare type of the venue, if known. (For example, “arts_entertainment/default”, “arts_entertainment/aquarium” or “food/icecream”.)</param>
        /// <param name="googlePlaceId">Google Places identifier of the venue</param>
        /// <param name="googlePlaceType">Google Places type of the venue. (See <see href="https://developers.google.com/places/web-service/supported_types">supported types</see>.)</param>
        /// <param name="disableNotification">Sends the message silently. Users will receive a notification with no sound.</param>
        /// <param name="replyToMessageId">If the message is a reply, ID of the original message</param>
        /// <param name="allowSendingWithoutReply">Pass True, if the message should be sent even if the specified replied-to message is not found</param>
        /// <param name="replyMarkup">Additional interface options. An <see cref="InlineKeyboardMarkup">inline keyboard</see>, <see cref="ReplyKeyboardMarkup">custom reply keyboard</see>, instructions to <see cref="ReplyKeyboardRemove">remove reply keyboard</see> or to <see cref="ForceReplyMarkup">force a reply</see> from the user</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>On success, the sent <see cref="Message"/> is returned.</returns>
        /// <see href="https://core.telegram.org/bots/api#sendvenue"/>
        public static async Task<Message> SendVenueAsync(
            this ITelegramBotClient botClient,
            ChatId chatId,
            double latitude,
            double longitude,
            string title,
            string address,
            string? foursquareId = default,
            string? foursquareType = default,
            string? googlePlaceId = default,
            string? googlePlaceType = default,
            bool? disableNotification = default,
            int? replyToMessageId = default,
            bool? allowSendingWithoutReply = default,
            IReplyMarkup? replyMarkup = default,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new SendVenueRequest(chatId, latitude, longitude, title, address)
                {
                    FoursquareId = foursquareId,
                    FoursquareType = foursquareType,
                    GooglePlaceId = googlePlaceId,
                    GooglePlaceType = googlePlaceType,
                    DisableNotification = disableNotification,
                    ReplyToMessageId = replyToMessageId,
                    AllowSendingWithoutReply = allowSendingWithoutReply,
                    ReplyMarkup = replyMarkup
                }, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to send phone contacts.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel (in the format <c>@channelusername</c>)</param>
        /// <param name="phoneNumber">Contact's phone number</param>
        /// <param name="firstName">Contact's first name</param>
        /// <param name="lastName">Contact's last name</param>
        /// <param name="vCard">Additional data about the contact in the form of a vCard, 0-2048 bytes</param>
        /// <param name="disableNotification">Sends the message silently. Users will receive a notification with no sound.</param>
        /// <param name="replyToMessageId">If the message is a reply, ID of the original message</param>
        /// <param name="allowSendingWithoutReply">Pass True, if the message should be sent even if the specified replied-to message is not found</param>
        /// <param name="replyMarkup">Additional interface options. An <see cref="InlineKeyboardMarkup">inline keyboard</see>, <see cref="ReplyKeyboardMarkup">custom reply keyboard</see>, instructions to <see cref="ReplyKeyboardRemove">remove reply keyboard</see> or to <see cref="ForceReplyMarkup">force a reply</see> from the user</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>On success, the sent <see cref="Message"/> is returned.</returns>
        public static async Task<Message> SendContactAsync(
            this ITelegramBotClient botClient,
            ChatId chatId,
            string phoneNumber,
            string firstName,
            string? lastName = default,
            string? vCard = default,
            bool? disableNotification = default,
            int? replyToMessageId = default,
            bool? allowSendingWithoutReply = default,
            IReplyMarkup? replyMarkup = default,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new SendContactRequest(chatId, phoneNumber, firstName)
                {
                    LastName = lastName,
                    Vcard = vCard,
                    DisableNotification = disableNotification,
                    ReplyToMessageId = replyToMessageId,
                    AllowSendingWithoutReply = allowSendingWithoutReply,
                    ReplyMarkup = replyMarkup
                }, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to send a native poll.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel (in the format <c>@channelusername</c>)</param>
        /// <param name="question">Poll question, 1-300 characters</param>
        /// <param name="options">A list of answer options, 2-10 strings 1-100 characters each</param>
        /// <param name="isAnonymous">True, if the poll needs to be anonymous, defaults to True</param>
        /// <param name="type">Poll type, <see cref="PollType.Quiz"/> or <see cref="PollType.Regular"/>, defaults to <see cref="PollType.Regular"/></param>
        /// <param name="allowsMultipleAnswers">True, if the poll allows multiple answers, ignored for polls in quiz mode, defaults to False</param>
        /// <param name="correctOptionId">0-based identifier of the correct answer option, required for polls in quiz mode</param>
        /// <param name="explanation">Text that is shown when a user chooses an incorrect answer or taps on the lamp icon in a quiz-style poll, 0-200 characters with at most 2 line feeds after entities parsing</param>
        /// <param name="explanationParseMode">Mode for parsing entities in the explanation. See <see href="https://core.telegram.org/bots/api#formatting-options">formatting options</see> for more details.</param>
        /// <param name="explanationEntities">List of special entities that appear in the poll explanation, which can be specified instead of <see cref="ParseMode"/></param>
        /// <param name="openPeriod">Amount of time in seconds the poll will be active after creation, 5-600. Can't be used together with <paramref name="closeDate"/>.</param>
        /// <param name="closeDate">Point in time when the poll will be automatically closed. Must be at least 5 and no more than 600 seconds in the future. Can't be used together with <paramref name="openPeriod"/>.</param>
        /// <param name="isClosed">Pass True, if the poll needs to be immediately closed. This can be useful for poll preview.</param>
        /// <param name="disableNotification">Sends the message silently. Users will receive a notification with no sound.</param>
        /// <param name="replyToMessageId">If the message is a reply, ID of the original message</param>
        /// <param name="allowSendingWithoutReply">Pass True, if the message should be sent even if the specified replied-to message is not found</param>
        /// <param name="replyMarkup">Additional interface options. An <see cref="InlineKeyboardMarkup">inline keyboard</see>, <see cref="ReplyKeyboardMarkup">custom reply keyboard</see>, instructions to <see cref="ReplyKeyboardRemove">remove reply keyboard</see> or to <see cref="ForceReplyMarkup">force a reply</see> from the user</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>On success, the sent <see cref="Message"/> is returned.</returns>
        public static async Task<Message> SendPollAsync(
            this ITelegramBotClient botClient,
            ChatId chatId,
            string question,
            IEnumerable<string> options,
            bool? isAnonymous = default,
            PollType? type = default,
            bool? allowsMultipleAnswers = default,
            int? correctOptionId = default,
            string? explanation = default,
            ParseMode? explanationParseMode = default,
            IEnumerable<MessageEntity>? explanationEntities = default,
            int? openPeriod = default,
            DateTime? closeDate = default,
            bool? isClosed = default,
            bool? disableNotification = default,
            int? replyToMessageId = default,
            bool? allowSendingWithoutReply = default,
            IReplyMarkup? replyMarkup = default,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new SendPollRequest(chatId, question, options)
                {
                    IsAnonymous = isAnonymous,
                    Type = type,
                    AllowsMultipleAnswers = allowsMultipleAnswers,
                    CorrectOptionId = correctOptionId,
                    Explanation = explanation,
                    ExplanationParseMode = explanationParseMode,
                    ExplanationEntities = explanationEntities,
                    OpenPeriod = openPeriod,
                    CloseDate = closeDate,
                    IsClosed = isClosed,
                    DisableNotification = disableNotification,
                    ReplyToMessageId = replyToMessageId,
                    AllowSendingWithoutReply = allowSendingWithoutReply,
                    ReplyMarkup = replyMarkup
                }, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to send an animated emoji that will display a random value.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel (in the format <c>@channelusername</c>)</param>
        /// <param name="emoji">Emoji on which the dice throw animation is based. Currently, must be one of <see cref="Emoji.Dice"/>, <see cref="Emoji.Darts"/>, <see cref="Emoji.Basketball"/>, <see cref="Emoji.Football"/>, <see cref="Emoji.Bowling"/> or <see cref="Emoji.SlotMachine"/>. Dice can have values 1-6 for <see cref="Emoji.Dice"/>, <see cref="Emoji.Darts"/> and <see cref="Emoji.Bowling"/>, values 1-5 for <see cref="Emoji.Basketball"/> and <see cref="Emoji.Football"/>, and values 1-64 for <see cref="Emoji.SlotMachine"/>. Defauts to <see cref="Emoji.Dice"/></param>
        /// <param name="disableNotification">Sends the message silently. Users will receive a notification with no sound.</param>
        /// <param name="replyToMessageId">If the message is a reply, ID of the original message</param>
        /// <param name="allowSendingWithoutReply">Pass True, if the message should be sent even if the specified replied-to message is not found</param>
        /// <param name="replyMarkup">Additional interface options. An <see cref="InlineKeyboardMarkup">inline keyboard</see>, <see cref="ReplyKeyboardMarkup">custom reply keyboard</see>, instructions to <see cref="ReplyKeyboardRemove">remove reply keyboard</see> or to <see cref="ForceReplyMarkup">force a reply</see> from the user</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>On success, the sent <see cref="Message"/> is returned.</returns>
        public static async Task<Message> SendDiceAsync(
            this ITelegramBotClient botClient,
            ChatId chatId,
            Emoji? emoji = default,
            bool? disableNotification = default,
            int? replyToMessageId = default,
            bool? allowSendingWithoutReply = default,
            IReplyMarkup? replyMarkup = default,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new SendDiceRequest(chatId)
                {
                    Emoji = emoji,
                    DisableNotification = disableNotification,
                    ReplyToMessageId = replyToMessageId,
                    AllowSendingWithoutReply = allowSendingWithoutReply,
                    ReplyMarkup = replyMarkup,
                }, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method when you need to tell the user that something is happening on the bot's side. The status is set for 5 seconds or less (when a message arrives from your bot, Telegram clients clear its typing status).
        /// </summary>
        /// <remarks>
        /// Example: The <see href="https://t.me/imagebot">ImageBot</see> needs some time to process a request and upload the image. Instead of sending a text message along the lines of “Retrieving image, please wait…”, the bot may use <see cref="SendChatActionAsync"/> with <see cref="Action"/> = <see cref="ChatAction.UploadPhoto"/>.The user will see a “sending photo” status for the bot.
        /// <para>We only recommend using this method when a response from the bot will take a <b>noticeable</b> amount of time to arrive.</para>
        /// </remarks>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel (in the format <c>@channelusername</c>)</param>
        /// <param name="chatAction">Type of action to broadcast. Choose one, depending on what the user is about to receive: <see cref="ChatAction.Typing"/> for <see cref="SendTextMessageAsync">text messages</see>, <see cref="ChatAction.UploadPhoto"/> for <see cref="SendPhotoAsync">photos</see>, <see cref="ChatAction.RecordVideo"/> or <see cref="ChatAction.UploadVideo"/> for <see cref="SendVideoAsync">videos</see>, <see cref="ChatAction.RecordVoice"/> or <see cref="ChatAction.UploadVoice"/> for <see cref="SendVoiceAsync">voice notes</see>, <see cref="ChatAction.UploadDocument"/> for <see cref="SendDocumentAsync">general files</see>, <see cref="ChatAction.FindLocation"/> for <see cref="SendLocationAsync">location data</see>, <see cref="ChatAction.RecordVideoNote"/> or <see cref="ChatAction.UploadVideoNote"/> for <see cref="SendVideoNoteAsync">video notes</see></param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public static async Task SendChatActionAsync(
            this ITelegramBotClient botClient,
            ChatId chatId,
            ChatAction chatAction,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new SendChatActionRequest(chatId, chatAction), cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to get a list of profile pictures for a user.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="userId">Unique identifier of the target user</param>
        /// <param name="offset">Sequential number of the first photo to be returned. By default, all photos are returned</param>
        /// <param name="limit">Limits the number of photos to be retrieved. Values between 1-100 are accepted. Defaults to 100</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Returns a <see cref="UserProfilePhotos"/> object.</returns>
        public static async Task<UserProfilePhotos> GetUserProfilePhotosAsync(
            this ITelegramBotClient botClient,
            long userId,
            int? offset = default,
            int? limit = default,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new GetUserProfilePhotosRequest(userId)
                {
                    Offset = offset,
                    Limit = limit
                }, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to get basic info about a file and prepare it for downloading. For the moment, bots can download files of up to 20MB in size. The file can then be downloaded via the link <c>https://api.telegram.org/file/bot&lt;token&gt;/&lt;file_path&gt;</c>, where <c>&lt;file_path&gt;</c> is taken from the response. It is guaranteed that the link will be valid for at least 1 hour. When the link expires, a new one can be requested by calling <see cref="GetFileAsync"/> again.
        /// </summary>
        /// <remarks>
        /// You can use <see cref="ITelegramBotClient.DownloadFileAsync"/> or <see cref="TelegramBotClientExtensions.GetInfoAndDownloadFileAsync"/> methods to download the file
        /// </remarks>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="fileId">File identifier to get info about</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>On success, a <see cref="File"/> object is returned.</returns>
        public static async Task<File> GetFileAsync(
            this ITelegramBotClient botClient,
            string fileId,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new GetFileRequest(fileId), cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to get basic info about a file download it. For the moment, bots can download files of up to 20MB in size.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="fileId">File identifier to get info about</param>
        /// <param name="destination">Destination stream to write file to</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>On success, a <see cref="File"/> object is returned.</returns>
        public static async Task<File> GetInfoAndDownloadFileAsync(
            this ITelegramBotClient botClient,
            string fileId,
            Stream destination,
            CancellationToken cancellationToken = default)
        {
            var file = await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(new GetFileRequest(fileId), cancellationToken)
                .ConfigureAwait(false);

            await botClient.DownloadFileAsync(file.FilePath!, destination, cancellationToken)
                .ConfigureAwait(false);

            return file;
        } 

        /// <summary>
        /// Use this method to ban a user in a group, a supergroup or a channel. In the case of supergroups and channels, the user will not be able to return to the chat on their own using invite links, etc., unless <see cref="UnbanChatMemberAsync(ITelegramBotClient, ChatId, long, bool?, CancellationToken)">unbanned</see> first. The bot must be an administrator in the chat for this to work and must have the appropriate admin rights.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel (in the format <c>@channelusername</c>)</param>
        /// <param name="userId">Unique identifier of the target user</param>
        /// <param name="untilDate">Date when the user will be unbanned. If user is banned for more than 366 days or less than 30 seconds from the current time they are considered to be banned forever. Applied for supergroups and channels only.</param>
        /// <param name="revokeMessages">Pass True to delete all messages from the chat for the user that is being removed. If False, the user will be able to see messages in the group that were sent before the user was removed. Always True for supergroups and channels.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        [Obsolete("Use BanChatMemberAsync instead")]
        public static async Task KickChatMemberAsync(
            this ITelegramBotClient botClient,
            ChatId chatId,
            long userId,
            DateTime? untilDate = default,
            bool? revokeMessages = default,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new KickChatMemberRequest(chatId, userId)
                {
                    UntilDate = untilDate,
                    RevokeMessages = revokeMessages
                }, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to ban a user in a group, a supergroup or a channel. In the case of supergroups and channels, the user will not be able to return to the chat on their own using invite links, etc., unless <see cref="UnbanChatMemberAsync(ITelegramBotClient, ChatId, long, bool?, CancellationToken)">unbanned</see> first. The bot must be an administrator in the chat for this to work and must have the appropriate admin rights.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="chatId">Unique identifier for the target group or username of the target supergroup or channel (in the format <c>@channelusername</c>)</param>
        /// <param name="userId">Unique identifier of the target user</param>
        /// <param name="untilDate">Date when the user will be unbanned. If user is banned for more than 366 days or less than 30 seconds from the current time they are considered to be banned forever. Applied for supergroups and channels only.</param>
        /// <param name="revokeMessages">Pass True to delete all messages from the chat for the user that is being removed. If False, the user will be able to see messages in the group that were sent before the user was removed. Always True for supergroups and channels.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public static async Task BanChatMemberAsync(
            this ITelegramBotClient botClient,
            ChatId chatId,
            long userId,
            DateTime? untilDate = default,
            bool? revokeMessages = default,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new BanChatMemberRequest(chatId, userId)
                {
                    UntilDate = untilDate,
                    RevokeMessages = revokeMessages
                }, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to unban a previously banned user in a supergroup or channel. The user will <b>not</b> return to the group or channel automatically, but will be able to join via link, etc. The bot must be an administrator for this to work. By default, this method guarantees that after the call the user is not a member of the chat, but will be able to join it. So if the user is a member of the chat they will also be <b>removed</b> from the chat. If you don't want this, use the parameter <paramref name="onlyIfBanned"/>.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="chatId">Unique identifier for the target group or username of the target supergroup or channel (in the format <c>@username</c>)</param>
        /// <param name="userId">Unique identifier of the target user</param>
        /// <param name="onlyIfBanned">Do nothing if the user is not banned</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public static async Task UnbanChatMemberAsync(
            this ITelegramBotClient botClient,
            ChatId chatId,
            long userId,
            bool? onlyIfBanned = default,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new UnbanChatMemberRequest(chatId, userId)
                {
                    OnlyIfBanned = onlyIfBanned
                }, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to restrict a user in a supergroup. The bot must be an administrator in the supergroup for this to work and must have the appropriate admin rights. Pass True for all permissions to lift restrictions from a user.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="chatId">Unique identifier for the target chat or username of the target supergroup (in the format <c>@supergroupusername</c>)</param>
        /// <param name="userId">Unique identifier of the target user</param>
        /// <param name="permissions">New user permissions</param>
        /// <param name="untilDate">Date when restrictions will be lifted for the user, unix time. If user is restricted for more than 366 days or less than 30 seconds from the current time, they are considered to be restricted forever.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public static async Task RestrictChatMemberAsync(
            this ITelegramBotClient botClient,
            ChatId chatId,
            long userId,
            ChatPermissions permissions,
            DateTime? untilDate = default,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new RestrictChatMemberRequest(chatId, userId, permissions)
                {
                    UntilDate = untilDate
                },
                cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to promote or demote a user in a supergroup or a channel. The bot must be an administrator in the chat for this to work and must have the appropriate admin rights. Pass <c>False</c> for all boolean parameters to demote a user.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel (in the format <c>@channelusername</c>)</param>
        /// <param name="userId">Unique identifier of the target user</param>
        /// <param name="isAnonymous">Pass True, if the administrator's presence in the chat is hidden</param>
        /// <param name="canManageChat">Pass True, if the administrator can access the chat event log, chat statistics, message statistics in channels, see channel members, see anonymous administrators in supergroups and ignore slow mode. Implied by any other administrator privilege</param>
        /// <param name="canPostMessages">Pass True, if the administrator can create channel posts, channels only</param>
        /// <param name="canEditMessages">Pass True, if the administrator can edit messages of other users, channels only</param>
        /// <param name="canDeleteMessages">Pass True, if the administrator can delete messages of other users</param>
        /// <param name="canManageVoiceChats">Pass True, if the administrator can manage voice chats, supergroups only</param>
        /// <param name="canRestrictMembers">Pass True, if the administrator can restrict, ban or unban chat members</param>
        /// <param name="canPromoteMembers">Pass True, if the administrator can add new administrators with a subset of his own privileges or demote administrators that he has promoted, directly or indirectly (promoted by administrators that were appointed by him)</param>
        /// <param name="canChangeInfo">Pass True, if the administrator can change chat title, photo and other settings</param>
        /// <param name="canInviteUsers">Pass True, if the administrator can invite new users to the chat</param>
        /// <param name="canPinMessages">Pass True, if the administrator can pin messages, supergroups only</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public static async Task PromoteChatMemberAsync(
            this ITelegramBotClient botClient,
            ChatId chatId,
            long userId,
            bool? isAnonymous = default,
            bool? canManageChat = default,
            bool? canPostMessages = default,
            bool? canEditMessages = default,
            bool? canDeleteMessages = default,
            bool? canManageVoiceChats = default,
            bool? canRestrictMembers = default,
            bool? canPromoteMembers = default,
            bool? canChangeInfo = default,
            bool? canInviteUsers = default,
            bool? canPinMessages = default,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new PromoteChatMemberRequest(chatId, userId)
                {
                    IsAnonymous = isAnonymous,
                    CanManageChat = canManageChat,
                    CanPostMessages = canPostMessages,
                    CanEditMessages = canEditMessages,
                    CanDeleteMessages = canDeleteMessages,
                    CanManageVoiceChat = canManageVoiceChats,
                    CanRestrictMembers = canRestrictMembers,
                    CanPromoteMembers = canPromoteMembers,
                    CanChangeInfo = canChangeInfo,
                    CanInviteUsers = canInviteUsers,
                    CanPinMessages = canPinMessages,
                }, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to set a custom title for an administrator in a supergroup promoted by the bot.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="chatId">Unique identifier for the target chat or username of the target supergroup (in the format <c>@supergroupusername</c>)</param>
        /// <param name="userId">Unique identifier of the target user</param>
        /// <param name="customTitle">New custom title for the administrator; 0-16 characters, emoji are not allowed</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public static async Task SetChatAdministratorCustomTitleAsync(
            this ITelegramBotClient botClient,
            ChatId chatId,
            long userId,
            string customTitle,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new SetChatAdministratorCustomTitleRequest(chatId, userId, customTitle),
                cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to set default chat permissions for all members. The bot must be an administrator in the group or a supergroup for this to work and must have the can_restrict_members admin rights.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="chatId">Unique identifier for the target chat or username of the target supergroup (in the format <c>@supergroupusername</c>)</param>
        /// <param name="permissions">New default chat permissions</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public static async Task SetChatPermissionsAsync(
            this ITelegramBotClient botClient,
            ChatId chatId,
            ChatPermissions permissions,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new SetChatPermissionsRequest(chatId, permissions), cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to generate a new primary invite link for a chat; any previously generated primary link is revoked. The bot must be an administrator in the chat for this to work and must have the appropriate admin rights.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel (in the format <c>@channelusername</c>)</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public static async Task<string> ExportChatInviteLinkAsync(
            this ITelegramBotClient botClient,
            ChatId chatId,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new ExportChatInviteLinkRequest(chatId), cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to create an additional invite link for a chat. The bot must be an administrator in the chat for this to work and must have the appropriate admin rights. The link can be revoked using the method <see cref="RevokeChatInviteLinkAsync"/>.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel (in the format <c>@channelusername</c>)</param>
        /// <param name="expireDate">Point in time when the link will expire</param>
        /// <param name="memberLimit">Maximum number of users that can be members of the chat simultaneously after joining the chat via this invite link; 1-99999</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Returns the new invite link as <see cref="Types.ChatInviteLink"/> object.</returns>
        public static async Task<ChatInviteLink> CreateChatInviteLinkAsync(
            this ITelegramBotClient botClient,
            ChatId chatId,
            DateTime? expireDate = default,
            int? memberLimit = default,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new CreateChatInviteLinkRequest(chatId)
                {
                    ExpireDate = expireDate,
                    MemberLimit = memberLimit
                }, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to edit a non-primary invite link created by the bot. The bot must be an administrator in the chat for this to work and must have the appropriate admin rights.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel (in the format <c>@channelusername</c>)</param>
        /// <param name="inviteLink">The invite link to edit</param>
        /// <param name="expireDate">Point in time when the link will expire</param>
        /// <param name="memberLimit">Maximum number of users that can be members of the chat simultaneously after joining the chat via this invite link; 1-99999</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Returns the edited invite link as a <see cref="Types.ChatInviteLink"/> object.</returns>
        public static async Task<ChatInviteLink> EditChatInviteLinkAsync(
            this ITelegramBotClient botClient,
            ChatId chatId,
            string inviteLink,
            DateTime? expireDate = default,
            int? memberLimit = default,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new EditChatInviteLinkRequest(chatId, inviteLink)
                {
                    ExpireDate = expireDate,
                    MemberLimit = memberLimit
                }, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to revoke an invite link created by the bot. If the primary link is revoked, a new link is automatically generated. The bot must be an administrator in the chat for this to work and must have the appropriate admin rights.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel (in the format <c>@channelusername</c>)</param>
        /// <param name="inviteLink">The invite link to revoke</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Returns the revoked invite link as <see cref="ChatInviteLink"/> object.</returns>
        public static async Task<ChatInviteLink> RevokeChatInviteLinkAsync(
            this ITelegramBotClient botClient,
            ChatId chatId,
            string inviteLink,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new RevokeChatInviteLinkRequest(chatId, inviteLink), cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to set a new profile photo for the chat. Photos can't be changed for private chats. The bot must be an administrator in the chat for this to work and must have the appropriate admin rights.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel (in the format <c>@channelusername</c>)</param>
        /// <param name="photo">New chat photo, uploaded using multipart/form-data</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public static async Task SetChatPhotoAsync(
            this ITelegramBotClient botClient,
            ChatId chatId,
            InputFileStream photo,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new SetChatPhotoRequest(chatId, photo), cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to delete a chat photo. Photos can't be changed for private chats. The bot must be an administrator in the chat for this to work and must have the appropriate admin rights.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel (in the format @channelusername)</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public static async Task DeleteChatPhotoAsync(
            this ITelegramBotClient botClient,
            ChatId chatId,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync
            (new DeleteChatPhotoRequest(chatId), cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to change the title of a chat. Titles can't be changed for private chats. The bot must be an administrator in the chat for this to work and must have the appropriate admin rights.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel (in the format <c>@channelusername</c>)</param>
        /// <param name="title">New chat title, 1-255 characters</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public static async Task SetChatTitleAsync(
            this ITelegramBotClient botClient,
            ChatId chatId,
            string title,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new SetChatTitleRequest(chatId, title), cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to change the description of a group, a supergroup or a channel. The bot must be an administrator in the chat for this to work and must have the appropriate admin rights.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel (in the format <c>@channelusername</c>)</param>
        /// <param name="description">New chat Description, 0-255 characters</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public static async Task SetChatDescriptionAsync(
            this ITelegramBotClient botClient,
            ChatId chatId,
            string? description = default,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new SetChatDescriptionRequest(chatId)
                {
                    Description = description
                }, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to add a message to the list of pinned messages in a chat. If the chat is not a private chat, the bot must be an administrator in the chat for this to work and must have the '<see cref="ChatPermissions.CanPinMessages"/>' admin right in a supergroup or '<see cref="ChatMemberAdministrator.CanEditMessages"/>' admin right in a channel.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel (in the format <c>@channelusername</c>)</param>
        /// <param name="messageId">Identifier of a message to pin</param>
        /// <param name="disableNotification">Pass <c>True</c>, if it is not necessary to send a notification to all chat members about the new pinned message. Notifications are always disabled in channels and private chats.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public static async Task PinChatMessageAsync(
            this ITelegramBotClient botClient,
            ChatId chatId,
            int messageId,
            bool? disableNotification = default,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new PinChatMessageRequest(chatId, messageId)
                {
                    DisableNotification = disableNotification
                }, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to remove a message from the list of pinned messages in a chat. If the chat is not a private chat, the bot must be an administrator in the chat for this to work and must have the '<see cref="ChatMemberAdministrator.CanPinMessages"/>' admin right in a supergroup or '<see cref="ChatMemberAdministrator.CanEditMessages"/>' admin right in a channel.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel (in the format <c>@channelusername</c>)</param>
        /// <param name="messageId">Identifier of a message to unpin. If not specified, the most recent pinned message (by sending date) will be unpinned.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public static async Task UnpinChatMessageAsync(
            this ITelegramBotClient botClient,
            ChatId chatId,
            int? messageId = default,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new UnpinChatMessageRequest(chatId)
                {
                    MessageId = messageId
                }, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to clear the list of pinned messages in a chat. If the chat is not a private chat, the bot must be an administrator in the chat for this to work and must have the '<see cref="ChatMemberAdministrator.CanPinMessages"/>' admin right in a supergroup or '<see cref="ChatMemberAdministrator.CanEditMessages"/>' admin right in a channel. Returns True on success.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel (in the format <c>@channelusername</c>)</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public static async Task UnpinAllChatMessages(
            this ITelegramBotClient botClient,
            ChatId chatId,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new UnpinAllChatMessagesRequest(chatId), cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method for your bot to leave a group, supergroup or channel.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="chatId">Unique identifier for the target chat or username of the target supergroup or channel (in the format <c>@channelusername</c>)</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public static async Task LeaveChatAsync(
            this ITelegramBotClient botClient,
            ChatId chatId,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new LeaveChatRequest(chatId), cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to get up to date information about the chat (current name of the user for one-on-one conversations, current username of a user, group or channel, etc.).
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="chatId">Unique identifier for the target chat or username of the target supergroup or channel (in the format <c>@channelusername</c>)</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Returns a <see cref="Chat"/> object on success.</returns>
        public static async Task<Chat> GetChatAsync(
            this ITelegramBotClient botClient,
            ChatId chatId,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new GetChatRequest(chatId), cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to get a list of administrators in a chat.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="chatId">Unique identifier for the target chat or username of the target supergroup or channel (in the format <c>@channelusername</c>)</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>On success, returns an Array of <see cref="ChatMember"/> objects that contains information about all chat administrators except other bots. If the chat is a group or a supergroup and no administrators were appointed, only the creator will be returned.</returns>
        public static async Task<ChatMember[]> GetChatAdministratorsAsync(
            this ITelegramBotClient botClient,
            ChatId chatId,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new GetChatAdministratorsRequest(chatId), cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to get the number of members in a chat.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="chatId">Unique identifier for the target chat or username of the target supergroup or channel (in the format <c>@channelusername</c>)</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Returns <see cref="int"/> on success..</returns>
        [Obsolete("Use GetChatMemberCountAsync")]
        public static async Task<int> GetChatMembersCountAsync(
            this ITelegramBotClient botClient,
            ChatId chatId,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new GetChatMembersCountRequest(chatId), cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to get the number of members in a chat.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="chatId">Unique identifier for the target chat or username of the target supergroup or channel (in the format <c>@channelusername</c>)</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Returns <see cref="int"/> on success..</returns>
        public static async Task<int> GetChatMemberCountAsync(
            this ITelegramBotClient botClient,
            ChatId chatId,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new GetChatMemberCountRequest(chatId), cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to get information about a member of a chat.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="chatId">Unique identifier for the target chat or username of the target supergroup or channel (in the format <c>@channelusername</c>)</param>
        /// <param name="userId">Unique identifier of the target user</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Returns a <see cref="ChatMember"/> object on success.</returns>
        public static async Task<ChatMember> GetChatMemberAsync(
            this ITelegramBotClient botClient,
            ChatId chatId,
            long userId,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new GetChatMemberRequest(chatId, userId), cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to set a new group sticker set for a supergroup. The bot must be an administrator in the chat for this to work and must have the appropriate admin rights. Use the field <see cref="Chat.CanSetStickerSet"/> optionally returned in <see cref="GetChatAsync"/> requests to check if the bot can use this method.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel (in the format <c>@channelusername</c>)</param>
        /// <param name="stickerSetName">Name of the sticker set to be set as the group sticker set</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public static async Task SetChatStickerSetAsync(
            this ITelegramBotClient botClient,
            ChatId chatId,
            string stickerSetName,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new SetChatStickerSetRequest(chatId, stickerSetName), cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to delete a group sticker set from a supergroup. The bot must be an administrator in the chat for this to work and must have the appropriate admin rights. Use the field <see cref="Types.Chat.CanSetStickerSet"/> optionally returned in <see cref="GetChatAsync"/> requests to check if the bot can use this method.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel (in the format <c>@channelusername</c>)</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public static async Task DeleteChatStickerSetAsync(
            this ITelegramBotClient botClient,
            ChatId chatId,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new DeleteChatStickerSetRequest(chatId), cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to send answers to callback queries sent from <see cref="Types.ReplyMarkups.InlineKeyboardMarkup">inline keyboards</see>. The answer will be displayed to the user as a notification at the top of the chat screen or as an alert.
        /// </summary>
        /// <remarks>
        /// Alternatively, the user can be redirected to the specified Game URL.For this option to work, you must first create a game for your bot via <c>@Botfather</c> and accept the terms. Otherwise, you may use links like <c>t.me/your_bot? start = XXXX</c> that open your bot with a parameter.
        /// </remarks>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="callbackQueryId">Unique identifier for the query to be answered</param>
        /// <param name="text">Text of the notification. If not specified, nothing will be shown to the user, 0-200 characters</param>
        /// <param name="showAlert">If true, an alert will be shown by the client instead of a notification at the top of the chat screen. Defaults to false</param>
        /// <param name="url">
        /// URL that will be opened by the user's client. If you have created a <see href="https://core.telegram.org/bots/api#game">Game</see> and accepted the conditions via <c>@Botfather</c>, specify the URL that opens your game — note that this will only work if the query comes from a callback_game button.
        /// <para>Otherwise, you may use links like <c>t.me/your_bot? start = XXXX</c> that open your bot with a parameter</para>
        /// </param>
        /// <param name="cacheTime">The maximum amount of time in seconds that the result of the callback query may be cached client-side. Telegram apps will support caching starting in version 3.14.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public static async Task AnswerCallbackQueryAsync(
            this ITelegramBotClient botClient,
            string callbackQueryId,
            string? text = default,
            bool? showAlert = default,
            string? url = default,
            int? cacheTime = default,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new AnswerCallbackQueryRequest(callbackQueryId)
                {
                    Text = text,
                    ShowAlert = showAlert,
                    Url = url,
                    CacheTime = cacheTime
                }, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to change the list of the bot's commands. See <see href="https://core.telegram.org/bots#commands"/> for more details about bot commands. Returns True on success.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="commands">A list of bot commands to be set as the list of the bot's commands. At most 100 commands can be specified.</param>
        /// <param name="scope">
        /// An object, describing scope of users for which the commands are relevant.
        /// Defaults to <see cref="BotCommandScopeDefault"/>.
        /// </param>
        /// <param name="languageCode">A two-letter ISO 639-1 language code. If empty, commands will be applied to all users from the given
        /// <paramref name="scope"/>, for whose language there are no dedicated commands</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public static async Task SetMyCommandsAsync(
            this ITelegramBotClient botClient,
            IEnumerable<BotCommand> commands,
            BotCommandScope? scope = default,
            string? languageCode = default,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new SetMyCommandsRequest(commands)
                {
                    Scope = scope,
                    LanguageCode = languageCode
                }, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to delete the list of the bot's commands for the given <paramref name="scope"/> and <paramref name="languageCode">user language</paramref>. After deletion, <see href="https://core.telegram.org/bots/api#determining-list-of-commands">higher level commands</see> will be shown to affected users. Returns True on success.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="scope">
        /// An object, describing scope of users for which the commands are relevant.
        /// Defaults to <see cref="BotCommandScopeDefault"/>.
        /// </param>
        /// <param name="languageCode">
        /// A two-letter ISO 639-1 language code. If empty, commands will be applied to all users from the given
        /// <paramref name="scope"/>, for whose language there are no dedicated commands
        /// </param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public static async Task DeleteMyCommandsAsync(
            this ITelegramBotClient botClient,
            BotCommandScope? scope = default,
            string? languageCode = default,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new DeleteMyCommandsRequest
                {
                    Scope = scope,
                    LanguageCode = languageCode
                }, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to get the current list of the bot's commands for the given <paramref name="scope"/> and <paramref name="languageCode">user language</paramref>.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="scope">
        /// An object, describing scope of users. Defaults to <see cref="BotCommandScopeDefault"/>.
        /// </param>
        /// <param name="languageCode">
        /// A two-letter ISO 639-1 language code or an empty string
        /// </param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Returns Array of <see cref="BotCommand"/> on success. If commands aren't set, an empty list is returned.</returns>
        public static async Task<BotCommand[]> GetMyCommandsAsync(
            this ITelegramBotClient botClient,
            BotCommandScope? scope = default,
            string? languageCode = default,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new GetMyCommandsRequest
                {
                    Scope = scope,
                    LanguageCode = languageCode
                }, cancellationToken).ConfigureAwait(false);

        #endregion Available methods

        #region Updating messages

        /// <summary>
        /// Use this method to edit text and game messages.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel (in the format <c>@channelusername</c>)</param>
        /// <param name="messageId">Identifier of the message to edit</param>
        /// <param name="text">New text of the message, 1-4096 characters after entities parsing</param>
        /// <param name="parseMode">Mode for parsing entities in the new caption. See <see href="https://core.telegram.org/bots/api#formatting-options">formatting</see> options for more details.</param>
        /// <param name="entities">List of special entities that appear in message text, which can be specified instead of parseMode</param>
        /// <param name="disableWebPagePreview">Disables link previews for links in this message</param>
        /// <param name="replyMarkup">Additional interface options. An <see cref="InlineKeyboardMarkup">inline keyboard</see>, <see cref="ReplyKeyboardMarkup">custom reply keyboard</see>, instructions to <see cref="ReplyKeyboardRemove">remove reply keyboard</see> or to <see cref="ForceReplyMarkup">force a reply</see> from the user</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>On success the edited <see cref="Message"/> is returned.</returns>
        public static async Task<Message> EditMessageTextAsync(
            this ITelegramBotClient botClient,
            ChatId chatId,
            int messageId,
            string text,
            ParseMode? parseMode = default,
            IEnumerable<MessageEntity>? entities = default,
            bool? disableWebPagePreview = default,
            InlineKeyboardMarkup? replyMarkup = default,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new EditMessageTextRequest(chatId, messageId, text)
                {
                    ParseMode = parseMode,
                    Entities = entities,
                    DisableWebPagePreview = disableWebPagePreview,
                    ReplyMarkup = replyMarkup
                }, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to edit text and game messages.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="inlineMessageId">Identifier of the inline message</param>
        /// <param name="text">New text of the message, 1-4096 characters after entities parsing</param>
        /// <param name="parseMode">Mode for parsing entities in the new caption. See <see href="https://core.telegram.org/bots/api#formatting-options">formatting</see> options for more details.</param>
        /// <param name="entities">List of special entities that appear in message text, which can be specified instead of parseMode</param>
        /// <param name="disableWebPagePreview">Disables link previews for links in this message</param>
        /// <param name="replyMarkup">Additional interface options. An <see cref="InlineKeyboardMarkup">inline keyboard</see>, <see cref="ReplyKeyboardMarkup">custom reply keyboard</see>, instructions to <see cref="ReplyKeyboardRemove">remove reply keyboard</see> or to <see cref="ForceReplyMarkup">force a reply</see> from the user</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public static async Task EditMessageTextAsync(
            this ITelegramBotClient botClient,
            string inlineMessageId,
            string text,
            ParseMode? parseMode = default,
            IEnumerable<MessageEntity>? entities = default,
            bool? disableWebPagePreview = default,
            InlineKeyboardMarkup? replyMarkup = default,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new EditInlineMessageTextRequest(inlineMessageId, text)
                {
                    ParseMode = parseMode,
                    Entities = entities,
                    DisableWebPagePreview = disableWebPagePreview,
                    ReplyMarkup = replyMarkup
                }, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to edit captions of messages.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel (in the format <c>@channelusername</c>)</param>
        /// <param name="messageId">dentifier of the message to edit</param>
        /// <param name="caption">New caption of the message, 0-1024 characters after entities parsing</param>
        /// <param name="parseMode">Mode for parsing entities in the new caption. See <see href="https://core.telegram.org/bots/api#formatting-options">formatting</see> options for more details.</param>
        /// <param name="captionEntities">List of special entities that appear in the caption, which can be specified instead of parse_mode</param>
        /// <param name="replyMarkup">Additional interface options. An <see cref="InlineKeyboardMarkup">inline keyboard</see>, <see cref="ReplyKeyboardMarkup">custom reply keyboard</see>, instructions to <see cref="ReplyKeyboardRemove">remove reply keyboard</see> or to <see cref="ForceReplyMarkup">force a reply</see> from the user</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>On success the edited <see cref="Message"/> is returned.</returns>
        public static async Task<Message> EditMessageCaptionAsync(
            this ITelegramBotClient botClient,
            ChatId chatId,
            int messageId,
            string? caption,
            ParseMode? parseMode = default,
            IEnumerable<MessageEntity>? captionEntities = default,
            InlineKeyboardMarkup? replyMarkup = default,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new EditMessageCaptionRequest(chatId, messageId)
                {
                    Caption = caption,
                    ParseMode = parseMode,
                    CaptionEntities = captionEntities,
                    ReplyMarkup = replyMarkup
                }, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to edit captions of messages.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="inlineMessageId">Identifier of the inline message</param>
        /// <param name="caption">New caption of the message, 0-1024 characters after entities parsing</param>
        /// <param name="parseMode">Mode for parsing entities in the new caption. See <see href="https://core.telegram.org/bots/api#formatting-options">formatting</see> options for more details.</param>
        /// <param name="captionEntities">List of special entities that appear in the caption, which can be specified instead of parse_mode</param>
        /// <param name="replyMarkup">Additional interface options. An <see cref="InlineKeyboardMarkup">inline keyboard</see>, <see cref="ReplyKeyboardMarkup">custom reply keyboard</see>, instructions to <see cref="ReplyKeyboardRemove">remove reply keyboard</see> or to <see cref="ForceReplyMarkup">force a reply</see> from the user</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public static async Task EditMessageCaptionAsync(
            this ITelegramBotClient botClient,
            string inlineMessageId,
            string? caption,
            ParseMode? parseMode = default,
            IEnumerable<MessageEntity>? captionEntities = default,
            InlineKeyboardMarkup? replyMarkup = default,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new EditInlineMessageCaptionRequest(inlineMessageId)
                {
                    Caption = caption,
                    ParseMode = parseMode,
                    CaptionEntities = captionEntities,
                    ReplyMarkup = replyMarkup
                }, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to edit animation, audio, document, photo, or video messages. If a message is part of a message album, then it can be edited only to an audio for audio albums, only to a document for document albums and to a photo or a video otherwise. Use a previously uploaded file via its <see cref="Types.InputFiles.InputTelegramFile.FileId"/> or specify a URL.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel (in the format <c>@channelusername</c>)</param>
        /// <param name="messageId">Identifier of the message to edit</param>
        /// <param name="media">A new media content of the message</param>
        /// <param name="replyMarkup">Additional interface options. An <see cref="InlineKeyboardMarkup">inline keyboard</see>, <see cref="ReplyKeyboardMarkup">custom reply keyboard</see>, instructions to <see cref="ReplyKeyboardRemove">remove reply keyboard</see> or to <see cref="ForceReplyMarkup">force a reply</see> from the user</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>On success the edited <see cref="Message"/> is returned.</returns>
        public static async Task<Message> EditMessageMediaAsync(
            this ITelegramBotClient botClient,
            ChatId chatId,
            int messageId,
            InputMediaBase media,
            InlineKeyboardMarkup? replyMarkup = default,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new EditMessageMediaRequest(chatId, messageId, media)
                {
                    ReplyMarkup = replyMarkup
                }, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to edit animation, audio, document, photo, or video messages. If a message is part of a message album, then it can be edited only to an audio for audio albums, only to a document for document albums and to a photo or a video otherwise. Use a previously uploaded file via its <see cref="Types.InputFiles.InputTelegramFile.FileId"/> or specify a URL.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="inlineMessageId">Identifier of the inline message</param>
        /// <param name="media">A new media content of the message</param>
        /// <param name="replyMarkup">Additional interface options. An <see cref="InlineKeyboardMarkup">inline keyboard</see>, <see cref="ReplyKeyboardMarkup">custom reply keyboard</see>, instructions to <see cref="ReplyKeyboardRemove">remove reply keyboard</see> or to <see cref="ForceReplyMarkup">force a reply</see> from the user</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public static async Task EditMessageMediaAsync(
            this ITelegramBotClient botClient,
            string inlineMessageId,
            InputMediaBase media,
            InlineKeyboardMarkup? replyMarkup = default,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new EditInlineMessageMediaRequest(inlineMessageId, media)
                {
                    ReplyMarkup = replyMarkup
                }, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to edit only the reply markup of messages.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel (in the format <c>@channelusername</c>)</param>
        /// <param name="messageId">Identifier of the message to edit</param>
        /// <param name="replyMarkup">Additional interface options. An <see cref="InlineKeyboardMarkup">inline keyboard</see>, <see cref="ReplyKeyboardMarkup">custom reply keyboard</see>, instructions to <see cref="ReplyKeyboardRemove">remove reply keyboard</see> or to <see cref="ForceReplyMarkup">force a reply</see> from the user</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>On success the edited <see cref="Message"/> is returned.</returns>
        public static async Task<Message> EditMessageReplyMarkupAsync(
            this ITelegramBotClient botClient,
            ChatId chatId,
            int messageId,
            InlineKeyboardMarkup? replyMarkup = default,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new EditMessageReplyMarkupRequest(chatId, messageId)
                {
                    ReplyMarkup = replyMarkup
                }, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to edit only the reply markup of messages.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="inlineMessageId">Identifier of the inline message</param>
        /// <param name="replyMarkup">Additional interface options. An <see cref="InlineKeyboardMarkup">inline keyboard</see>, <see cref="ReplyKeyboardMarkup">custom reply keyboard</see>, instructions to <see cref="ReplyKeyboardRemove">remove reply keyboard</see> or to <see cref="ForceReplyMarkup">force a reply</see> from the user</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public static async Task EditMessageReplyMarkupAsync(
            this ITelegramBotClient botClient,
            string inlineMessageId,
            InlineKeyboardMarkup? replyMarkup = default,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new EditInlineMessageReplyMarkupRequest(inlineMessageId)
                {
                    ReplyMarkup = replyMarkup
                }, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to stop a poll which was sent by the bot.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel (in the format <c>@channelusername</c>)</param>
        /// <param name="messageId">Identifier of the original message with the poll</param>
        /// <param name="replyMarkup">Additional interface options. An <see cref="InlineKeyboardMarkup">inline keyboard</see>, <see cref="ReplyKeyboardMarkup">custom reply keyboard</see>, instructions to <see cref="ReplyKeyboardRemove">remove reply keyboard</see> or to <see cref="ForceReplyMarkup">force a reply</see> from the user</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>On success, the stopped <see cref="Poll"/> with the final results is returned.</returns>
        public static async Task<Poll> StopPollAsync(
            this ITelegramBotClient botClient,
            ChatId chatId,
            int messageId,
            InlineKeyboardMarkup? replyMarkup = default,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new StopPollRequest(chatId, messageId)
                {
                    ReplyMarkup = replyMarkup
                }, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to delete a message, including service messages, with the following limitations:
        /// <list type="bullet">
        /// <item><description>A message can only be deleted if it was sent less than 48 hours ago.</description></item>
        /// <item><description>A dice message in a private chat can only be deleted if it was sent more than 24 hours ago.</description></item>
        /// <item><description>Bots can delete outgoing messages in private chats, groups, and supergroups.</description></item>
        /// <item><description>Bots can delete incoming messages in private chats.</description></item>
        /// <item><description>Bots granted can_post_messages permissions can delete outgoing messages in channels.</description></item>
        /// <item><description>If the bot is an administrator of a group, it can delete any message there.</description></item>
        /// <item><description>If the bot has can_delete_messages permission in a supergroup or a channel, it can delete any message there.</description></item>
        /// </list>
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel (in the format <c>@channelusername</c>)</param>
        /// <param name="messageId">Identifier of the message to delete</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public static async Task DeleteMessageAsync(
            this ITelegramBotClient botClient,
            ChatId chatId,
            int messageId,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new DeleteMessageRequest(chatId, messageId), cancellationToken).ConfigureAwait(false);

        #endregion Updating messages

        #region Stickers

        /// <summary>
        /// Use this method to send static .WEBP or animated .TGS stickers.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel (in the format <c>@channelusername</c>)</param>
        /// <param name="sticker">Sticker to send. Pass a <see cref="InputTelegramFile.FileId"/> as String to send a file that exists on the Telegram servers (recommended), pass an HTTP URL as a String for Telegram to get a .WEBP file from the Internet, or upload a new one using multipart/form-data</param>
        /// <param name="disableNotification">Sends the message silently. Users will receive a notification with no sound.</param>
        /// <param name="replyToMessageId">If the message is a reply, ID of the original message</param>
        /// <param name="allowSendingWithoutReply">Pass True, if the message should be sent even if the specified replied-to message is not found</param>
        /// <param name="replyMarkup">Additional interface options. An <see cref="InlineKeyboardMarkup">inline keyboard</see>, <see cref="ReplyKeyboardMarkup">custom reply keyboard</see>, instructions to <see cref="ReplyKeyboardRemove">remove reply keyboard</see> or to <see cref="ForceReplyMarkup">force a reply</see> from the user</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>On success, the sent <see cref="Message"/> is returned.</returns>
        public static async Task<Message> SendStickerAsync(
            this ITelegramBotClient botClient,
            ChatId chatId,
            InputOnlineFile sticker,
            bool? disableNotification = default,
            int? replyToMessageId = default,
            bool? allowSendingWithoutReply = default,
            IReplyMarkup? replyMarkup = default,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new SendStickerRequest(chatId, sticker)
                {
                    DisableNotification = disableNotification,
                    ReplyToMessageId = replyToMessageId,
                    AllowSendingWithoutReply = allowSendingWithoutReply,
                    ReplyMarkup = replyMarkup
                }, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to get a sticker set.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="name">Name of the sticker set</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>On success, a <see cref="StickerSet"/> object is returned.</returns>
        public static async Task<StickerSet> GetStickerSetAsync(
            this ITelegramBotClient botClient,
            string name,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new GetStickerSetRequest(name), cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to upload a .PNG file with a sticker for later use in <see cref="CreateNewStickerSetAsync"/>/<see cref="CreateNewAnimatedStickerSetAsync"/> and <see cref="AddStickerToSetAsync"/>/<see cref="AddAnimatedStickerToSetAsync"/> methods (can be used multiple times).
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="userId">User identifier of sticker file owner</param>
        /// <param name="pngSticker"><b>PNG</b> image with the sticker, must be up to 512 kilobytes in size, dimensions must not exceed 512px, and either width or height must be exactly 512px</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Returns the uploaded <see cref="File"/> on success.</returns>
        public static async Task<File> UploadStickerFileAsync(
            this ITelegramBotClient botClient,
            long userId,
            InputFileStream pngSticker,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new UploadStickerFileRequest(userId, pngSticker), cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to create a new static sticker set owned by a user. The bot will be able to edit the sticker set thus created.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="userId">User identifier of created sticker set owner</param>
        /// <param name="name">Short name of sticker set, to be used in <c>t.me/addstickers/</c> URLs (e.g., <i>animals</i>). Can contain only english letters, digits and underscores. Must begin with a letter, can't contain consecutive underscores and must end in <i>"_by_&lt;bot username&gt;"</i>. <i>&lt;bot_username&gt;</i> is case insensitive. 1-64 characters</param>
        /// <param name="title">Sticker set title, 1-64 characters</param>
        /// <param name="pngSticker"><b>PNG</b> image with the sticker, must be up to 512 kilobytes in size, dimensions must not exceed 512px, and either width or height must be exactly 512px. Pass a <see cref="Types.InputFiles.InputTelegramFile.FileId"/> as a String to send a file that already exists on the Telegram servers, pass an HTTP URL as a String for Telegram to get a file from the Internet, or upload a new one using multipart/form-data</param>
        /// <param name="emojis">One or more emoji corresponding to the sticker</param>
        /// <param name="containsMasks">Pass True, if a set of mask stickers should be created</param>
        /// <param name="maskPosition">An object for position where the mask should be placed on faces</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public static async Task CreateNewStickerSetAsync(
            this ITelegramBotClient botClient,
            long userId,
            string name,
            string title,
            InputOnlineFile pngSticker,
            string emojis,
            bool? containsMasks = default,
            MaskPosition? maskPosition = default,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new CreateNewStickerSetRequest(userId, name, title, pngSticker, emojis)
                {
                    ContainsMasks = containsMasks,
                    MaskPosition = maskPosition
                }, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to create a new animated sticker set owned by a user. The bot will be able to edit the sticker set thus created.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="userId">User identifier of created sticker set owner</param>
        /// <param name="name">Short name of sticker set, to be used in <c>t.me/addstickers/</c> URLs (e.g., <i>animals</i>). Can contain only english letters, digits and underscores. Must begin with a letter, can't contain consecutive underscores and must end in <i>"_by_&lt;bot username&gt;"</i>. <i>&lt;bot_username&gt;</i> is case insensitive. 1-64 characters</param>
        /// <param name="title">Sticker set title, 1-64 characters</param>
        /// <param name="tgsSticker"><b>TGS</b> animation with the sticker, uploaded using multipart/form-data. See <see href="https://core.telegram.org/animated_stickers#technical-requirements"/> for technical requirements</param>
        /// <param name="emojis">One or more emoji corresponding to the sticker</param>
        /// <param name="containsMasks">Pass True, if a set of mask stickers should be created</param>
        /// <param name="maskPosition">An object for position where the mask should be placed on faces</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public static async Task CreateNewAnimatedStickerSetAsync(
            this ITelegramBotClient botClient,
            long userId,
            string name,
            string title,
            InputFileStream tgsSticker,
            string emojis,
            bool? containsMasks = default,
            MaskPosition? maskPosition = default,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new CreateNewAnimatedStickerSetRequest(userId, name, title, tgsSticker, emojis)
                {
                    ContainsMasks = containsMasks,
                    MaskPosition = maskPosition
                }, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to add a new sticker to a set created by the bot. Static sticker sets can have up to 120 stickers.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="userId">User identifier of sticker set owner</param>
        /// <param name="name">Sticker set name</param>
        /// <param name="pngSticker"><b>PNG</b> image with the sticker, must be up to 512 kilobytes in size, dimensions must not exceed 512px, and either width or height must be exactly 512px. Pass a <see cref="Types.InputFiles.InputTelegramFile.FileId"/> as a String to send a file that already exists on the Telegram servers, pass an HTTP URL as a String for Telegram to get a file from the Internet, or upload a new one using multipart/form-data</param>
        /// <param name="emojis">One or more emoji corresponding to the sticker</param>
        /// <param name="maskPosition">An object for position where the mask should be placed on faces</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public static async Task AddStickerToSetAsync(
            this ITelegramBotClient botClient,
            long userId,
            string name,
            InputOnlineFile pngSticker,
            string emojis,
            MaskPosition? maskPosition = default,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new AddStickerToSetRequest(userId, name, pngSticker, emojis)
                {
                    MaskPosition = maskPosition
                }, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to add a new sticker to a set created by the bot. Animated stickers can be added to animated sticker sets and only to them. Animated sticker sets can have up to 50 stickers. Returns True on success.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="userId">User identifier of sticker set owner</param>
        /// <param name="name">Sticker set name</param>
        /// <param name="tgsSticker"><b>TGS</b> animation with the sticker, uploaded using multipart/form-data. See <see href="https://core.telegram.org/animated_stickers#technical-requirements"/> for technical requirements</param>
        /// <param name="emojis">One or more emoji corresponding to the sticker</param>
        /// <param name="maskPosition">An object for position where the mask should be placed on faces</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public static async Task AddAnimatedStickerToSetAsync(
            this ITelegramBotClient botClient,
            long userId,
            string name,
            InputFileStream tgsSticker,
            string emojis,
            MaskPosition? maskPosition = default,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new AddAnimatedStickerToSetRequest(userId, name, tgsSticker, emojis)
                {
                    MaskPosition = maskPosition
                }, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to move a sticker in a set created by the bot to a specific position.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="sticker">File identifier of the sticker</param>
        /// <param name="position">New sticker position in the set, zero-based</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public static async Task SetStickerPositionInSetAsync(
            this ITelegramBotClient botClient,
            string sticker,
            int position,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new SetStickerPositionInSetRequest(sticker, position),
                cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to delete a sticker from a set created by the bot.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="sticker">File identifier of the sticker</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public static async Task DeleteStickerFromSetAsync(
            this ITelegramBotClient botClient,
            string sticker,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new DeleteStickerFromSetRequest(sticker), cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to set the thumbnail of a sticker set. Animated thumbnails can be set for animated sticker sets only.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="name">Sticker set name</param>
        /// <param name="userId">User identifier of the sticker set owner</param>
        /// <param name="thumb">A <b>PNG</b> image with the thumbnail, must be up to 128 kilobytes in size and have width and height exactly 100px, or a <b>TGS</b> animation with the thumbnail up to 32 kilobytes in size; see <see href="https://core.telegram.org/animated_stickers#technical-requirements"/> for animated sticker technical requirements. Pass a <see cref="InputTelegramFile.FileId"/> as a String to send a file that already exists on the Telegram servers, pass an HTTP URL as a String for Telegram to get a file from the Internet, or upload a new one using multipart/form-data. Animated sticker set thumbnail can't be uploaded via HTTP URL</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public static async Task SetStickerSetThumbAsync(
            this ITelegramBotClient botClient,
            string name,
            long userId,
            InputOnlineFile? thumb = default,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new SetStickerSetThumbRequest(name, userId)
                {
                    Thumb = thumb
                }, cancellationToken).ConfigureAwait(false);

        #endregion

        #region Inline mode

        /// <summary>
        /// Use this method to send answers to an inline query.
        /// </summary>
        /// <remarks>
        /// No more than <b>50</b> results per query are allowed.
        /// </remarks>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="inlineQueryId">Unique identifier for the answered query</param>
        /// <param name="results">An array of results for the inline query</param>
        /// <param name="cacheTime">The maximum amount of time in seconds that the result of the inline query may be cached on the server. Defaults to 300</param>
        /// <param name="isPersonal">Pass True, if results may be cached on the server side only for the user that sent the query. By default, results may be returned to any user who sends the same query</param>
        /// <param name="nextOffset">Pass the offset that a client should send in the next query with the same text to receive more results. Pass an empty string if there are no more results or if you don't support pagination. Offset length can't exceed 64 bytes</param>
        /// <param name="switchPmText">If passed, clients will display a button with specified text that switches the user to a private chat with the bot and sends the bot a start message with the parameter <paramref name="switchPmParameter"/></param>
        /// <param name="switchPmParameter">
        /// <see href="https://core.telegram.org/bots#deep-linking">Deep-linking</see> parameter for the <c>/start</c> message sent to the bot when user presses the switch button. 1-64 characters, only <c>A-Z</c>, <c>a-z</c>, <c>0-9</c>, <c>_</c> and <c>-</c> are allowed.
        /// <para>
        /// <i>Example</i>: An inline bot that sends YouTube videos can ask the user to connect the bot to their YouTube account to adapt search results accordingly. To do this, it displays a 'Connect your YouTube account' button above the results, or even before showing any. The user presses the button, switches to a private chat with the bot and, in doing so, passes a start parameter that instructs the bot to return an oauth link. Once done, the bot can offer a <see cref="Types.ReplyMarkups.InlineKeyboardButton.SwitchInlineQuery"/> button so that the user can easily return to the chat where they wanted to use the bot's inline capabilities.
        /// </para>
        /// </param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public static async Task AnswerInlineQueryAsync(
            this ITelegramBotClient botClient,
            string inlineQueryId,
            IEnumerable<InlineQueryResult> results,
            int? cacheTime = default,
            bool? isPersonal = default,
            string? nextOffset = default,
            string? switchPmText = default,
            string? switchPmParameter = default,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new AnswerInlineQueryRequest(inlineQueryId, results)
                {
                    CacheTime = cacheTime,
                    IsPersonal = isPersonal,
                    NextOffset = nextOffset,
                    SwitchPmText = switchPmText,
                    SwitchPmParameter = switchPmParameter
                }, cancellationToken).ConfigureAwait(false);

        # endregion Inline mode

        #region Payments

        /// <summary>
        /// Use this method to send invoices.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel (in the format <c>@channelusername</c>)</param>
        /// <param name="title">Product name, 1-32 characters</param>
        /// <param name="description">Product description, 1-255 characters</param>
        /// <param name="payload">Bot-defined invoice payload, 1-128 bytes. This will not be displayed to the user, use for your internal processes.</param>
        /// <param name="providerToken">Payments provider token, obtained via <see href="https://t.me/botfather">@Botfather</see></param>
        /// <param name="currency">Three-letter ISO 4217 currency code, see <see href="https://core.telegram.org/bots/payments#supported-currencies">more on currencies</see></param>
        /// <param name="prices">Price breakdown, a list of components (e.g. product price, tax, discount, delivery cost, delivery tax, bonus, etc.)</param>
        /// <param name="maxTipAmount">The maximum accepted amount for tips in the smallest units of the currency (integer, not float/double). For example, for a maximum tip of <c>US$ 1.45</c> pass <c><paramref name="maxTipAmount"/> = 145</c>. See the <i>exp</i> parameter in <see href="https://core.telegram.org/bots/payments/currencies.json">currencies.json</see>, it shows the number of digits past the decimal point for each currency (2 for the majority of currencies). Defaults to 0</param>
        /// <param name="suggestedTipAmounts">An array of suggested amounts of tips in the <i>smallest units</i> of the currency (integer, <b>not</b> float/double). At most 4 suggested tip amounts can be specified. The suggested tip amounts must be positive, passed in a strictly increased order and must not exceed <paramref name="maxTipAmount"/></param>
        /// <param name="startParameter">Unique deep-linking parameter. If left empty, <b>forwarded copies</b> of the sent message will have a <i>Pay</i> button, allowing multiple users to pay directly from the forwarded message, using the same invoice. If non-empty, forwarded copies of the sent message will have a <i>URL</i> button with a deep link to the bot (instead of a <i>Pay</i> button), with the value used as the start parameter</param>
        /// <param name="providerData">A JSON-serialized data about the invoice, which will be shared with the payment provider. A detailed description of required fields should be provided by the payment provider.</param>
        /// <param name="photoUrl">URL of the product photo for the invoice. Can be a photo of the goods or a marketing image for a service. People like it better when they see what they are paying for.</param>
        /// <param name="photoSize">Photo size</param>
        /// <param name="photoWidth">Photo width</param>
        /// <param name="photoHeight">Photo height</param>
        /// <param name="needName">Pass True, if you require the user's full name to complete the order</param>
        /// <param name="needPhoneNumber">Pass True, if you require the user's phone number to complete the order</param>
        /// <param name="needEmail">Pass True, if you require the user's email to complete the order</param>
        /// <param name="needShippingAddress">Pass True, if you require the user's shipping address to complete the order</param>
        /// <param name="sendPhoneNumberToProvider">Pass True, if user's phone number should be sent to provider</param>
        /// <param name="sendEmailToProvider">Pass True, if user's email address should be sent to provider</param>
        /// <param name="isFlexible">Pass True, if the final price depends on the shipping method</param>
        /// <param name="disableNotification">Sends the message silently. Users will receive a notification with no sound</param>
        /// <param name="replyToMessageId">If the message is a reply, ID of the original message</param>
        /// <param name="allowSendingWithoutReply">Pass True, if the message should be sent even if the specified replied-to message is not found</param>
        /// <param name="replyMarkup">Additional interface options. An <see cref="InlineKeyboardMarkup">inline keyboard</see>, <see cref="ReplyKeyboardMarkup">custom reply keyboard</see>, instructions to <see cref="ReplyKeyboardRemove">remove reply keyboard</see> or to <see cref="ForceReplyMarkup">force a reply</see> from the user</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>On success, the sent <see cref="Message"/> is returned.</returns>
        public static async Task<Message> SendInvoiceAsync(
            this ITelegramBotClient botClient,
            long chatId,
            string title,
            string description,
            string payload,
            string providerToken,
            string currency,
            IEnumerable<LabeledPrice> prices,
            int? maxTipAmount = default,
            IEnumerable<int>? suggestedTipAmounts = default,
            string? startParameter = default,
            string? providerData = default,
            string? photoUrl = default,
            int? photoSize = default,
            int? photoWidth = default,
            int? photoHeight = default,
            bool? needName = default,
            bool? needPhoneNumber = default,
            bool? needEmail = default,
            bool? needShippingAddress = default,
            bool? sendPhoneNumberToProvider = default,
            bool? sendEmailToProvider = default,
            bool? isFlexible = default,
            bool? disableNotification = default,
            int? replyToMessageId = default,
            bool? allowSendingWithoutReply = default,
            InlineKeyboardMarkup? replyMarkup = default,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new SendInvoiceRequest(
                chatId,
                title,
                description,
                payload,
                providerToken,
                currency,
                // ReSharper disable once PossibleMultipleEnumeration
                prices
            )
                {
                    MaxTipAmount = maxTipAmount,
                    SuggestedTipAmounts = suggestedTipAmounts,
                    StartParameter = startParameter,
                    ProviderData = providerData,
                    PhotoUrl = photoUrl,
                    PhotoSize = photoSize,
                    PhotoWidth = photoWidth,
                    PhotoHeight = photoHeight,
                    NeedName = needName,
                    NeedPhoneNumber = needPhoneNumber,
                    NeedEmail = needEmail,
                    NeedShippingAddress = needShippingAddress,
                    SendPhoneNumberToProvider = sendPhoneNumberToProvider,
                    SendEmailToProvider = sendEmailToProvider,
                    IsFlexible = isFlexible,
                    DisableNotification = disableNotification,
                    ReplyToMessageId = replyToMessageId,
                    AllowSendingWithoutReply = allowSendingWithoutReply,
                    ReplyMarkup = replyMarkup
                }, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// If you sent an invoice requesting a shipping address and the parameter <c>isFlexible"</c> was specified, the Bot API will send an <see cref="Types.Update"/> with a <see cref="Types.Update.ShippingQuery"/> field to the bot. Use this method to reply to shipping queries.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="shippingQueryId">Unique identifier for the query to be answered</param>
        /// <param name="shippingOptions">Required if ok is True. An array of available shipping options.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public static async Task AnswerShippingQueryAsync(
            this ITelegramBotClient botClient,
            string shippingQueryId,
            IEnumerable<ShippingOption> shippingOptions,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new AnswerShippingQueryRequest(shippingQueryId, shippingOptions), cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// If you sent an invoice requesting a shipping address and the parameter <c>isFlexible"</c> was specified, the Bot API will send an <see cref="Types.Update"/> with a <see cref="Types.Update.ShippingQuery"/> field to the bot. Use this method to indicate failed shipping query.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="shippingQueryId">Unique identifier for the query to be answered</param>
        /// <param name="errorMessage">Required if <see cref="Requests.AnswerShippingQueryRequest.Ok"/> is False. Error message in human readable form that explains why it is impossible to complete the order (e.g. "Sorry, delivery to your desired address is unavailable'). Telegram will display this message to the user.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public static async Task AnswerShippingQueryAsync(
            this ITelegramBotClient botClient,
            string shippingQueryId,
            string errorMessage,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new AnswerShippingQueryRequest(shippingQueryId, errorMessage), cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Once the user has confirmed their payment and shipping details, the Bot API sends the final confirmation in the form of an <see cref="Types.Update"/> with the field <see cref="Types.Update.PreCheckoutQuery"/>. Use this method to respond to such pre-checkout queries.
        /// </summary>
        /// <remarks>
        /// <b>Note</b>: The Bot API must receive an answer within 10 seconds after the pre-checkout query was sent.
        /// </remarks>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="preCheckoutQueryId">Unique identifier for the query to be answered</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public static async Task AnswerPreCheckoutQueryAsync(
            this ITelegramBotClient botClient,
            string preCheckoutQueryId,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new AnswerPreCheckoutQueryRequest(preCheckoutQueryId), cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Once the user has confirmed their payment and shipping details, the Bot API sends the final confirmation in the form of an <see cref="Types.Update"/> with the field <see cref="Types.Update.PreCheckoutQuery"/>. Use this method to respond to indicate failed pre-checkout query.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="preCheckoutQueryId">Unique identifier for the query to be answered</param>
        /// <param name="errorMessage">Required if <see cref="Requests.AnswerPreCheckoutQueryRequest.Ok"/> is False. Error message in human readable form that explains the reason for failure to proceed with the checkout (e.g. "Sorry, somebody just bought the last of our amazing black T-shirts while you were busy filling out your payment details. Please choose a different color or garment!"). Telegram will display this message to the user.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public static async Task AnswerPreCheckoutQueryAsync(
            this ITelegramBotClient botClient,
            string preCheckoutQueryId,
            string errorMessage,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new AnswerPreCheckoutQueryRequest(preCheckoutQueryId, errorMessage), cancellationToken).ConfigureAwait(false);

        #endregion Payments

        #region Games

        /// <summary>
        /// Use this method to send a game.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="chatId">Unique identifier for the target chat</param>
        /// <param name="gameShortName">Short name of the game, serves as the unique identifier for the game. Set up your games via <see href="https://t.me/botfather">@Botfather</see></param>
        /// <param name="disableNotification">Sends the message silently. Users will receive a notification with no sound.</param>
        /// <param name="replyToMessageId">If the message is a reply, ID of the original message</param>
        /// <param name="allowSendingWithoutReply">Pass True, if the message should be sent even if the specified replied-to message is not found</param>
        /// <param name="replyMarkup">Additional interface options. An <see cref="InlineKeyboardMarkup">inline keyboard</see>, <see cref="ReplyKeyboardMarkup">custom reply keyboard</see>, instructions to <see cref="ReplyKeyboardRemove">remove reply keyboard</see> or to <see cref="ForceReplyMarkup">force a reply</see> from the user</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>On success, the sent <see cref="Message"/> is returned.</returns>
        public static async Task<Message> SendGameAsync(
            this ITelegramBotClient botClient,
            long chatId,
            string gameShortName,
            bool? disableNotification = default,
            int? replyToMessageId = default,
            bool? allowSendingWithoutReply = default,
            InlineKeyboardMarkup? replyMarkup = default,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new SendGameRequest(chatId, gameShortName)
                {
                    DisableNotification = disableNotification,
                    ReplyToMessageId = replyToMessageId,
                    AllowSendingWithoutReply = allowSendingWithoutReply,
                    ReplyMarkup = replyMarkup
                }, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to set the score of the specified user in a game.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="userId">User identifier</param>
        /// <param name="score">New score, must be non-negative</param>
        /// <param name="chatId">Unique identifier for the target chat</param>
        /// <param name="messageId">Identifier of the sent message</param>
        /// <param name="force">Pass True, if the high score is allowed to decrease. This can be useful when fixing mistakes or banning cheaters.</param>
        /// <param name="disableEditMessage">Pass True, if the game message should not be automatically edited to include the current scoreboard</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>On success returns the edited <see cref="Message"/>. Returns an error, if the new score is not greater than the user's current score in the chat and <paramref name="force"/> is False.</returns>
        public static async Task<Message> SetGameScoreAsync(
            this ITelegramBotClient botClient,
            long userId,
            int score,
            long chatId,
            int messageId,
            bool? force = default,
            bool? disableEditMessage = default,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new SetGameScoreRequest(userId, score, chatId, messageId)
                {
                    Force = force,
                    DisableEditMessage = disableEditMessage
                }, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to set the score of the specified user in a game.
        /// </summary>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="userId">User identifier</param>
        /// <param name="score">New score, must be non-negative</param>
        /// <param name="inlineMessageId">Identifier of the inline message.</param>
        /// <param name="force">Pass True, if the high score is allowed to decrease. This can be useful when fixing mistakes or banning cheaters.</param>
        /// <param name="disableEditMessage">Pass True, if the game message should not be automatically edited to include the current scoreboard</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Returns an error, if the new score is not greater than the user's current score in the chat and <paramref name="force"/> is False.</returns>
        public static async Task SetGameScoreAsync(
            this ITelegramBotClient botClient,
            long userId,
            int score,
            string inlineMessageId,
            bool? force = default,
            bool? disableEditMessage = default,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new SetInlineGameScoreRequest(userId, score, inlineMessageId)
                {
                    Force = force,
                    DisableEditMessage = disableEditMessage
                }, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to get data for high score tables. Will return the score of the specified user and several of their neighbors in a game.
        /// </summary>
        /// <remarks>
        /// This method will currently return scores for the target user, plus two of their closest neighbors on each side. Will also return the top three users if the user and his neighbors are not among them. Please note that this behavior is subject to change.
        /// </remarks>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="userId">Target user id</param>
        /// <param name="chatId">Unique identifier for the target chat</param>
        /// <param name="messageId">Identifier of the sent message</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>On success, returns an Array of <see cref="GameHighScore"/> objects.</returns>
        public static async Task<GameHighScore[]> GetGameHighScoresAsync(
            this ITelegramBotClient botClient,
            long userId,
            long chatId,
            int messageId,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new GetGameHighScoresRequest(userId, chatId, messageId),
                cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Use this method to get data for high score tables. Will return the score of the specified user and several of their neighbors in a game.
        /// </summary>
        /// <remarks>
        /// This method will currently return scores for the target user, plus two of their closest neighbors on each side. Will also return the top three users if the user and his neighbors are not among them. Please note that this behavior is subject to change.
        /// </remarks>
        /// <param name="botClient">An instance of <see cref="ITelegramBotClient"/>.</param>
        /// <param name="userId">User identifier</param>
        /// <param name="inlineMessageId">Identifier of the inline message</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>On success, returns an Array of <see cref="GameHighScore"/> objects.</returns>
        public static async Task<GameHighScore[]> GetGameHighScoresAsync(
            this ITelegramBotClient botClient,
            long userId,
            string inlineMessageId,
            CancellationToken cancellationToken = default
        ) =>
            await botClient.ThrowIfNull(nameof(botClient)).MakeRequestAsync(
                new GetInlineGameHighScoresRequest(userId, inlineMessageId),
                cancellationToken).ConfigureAwait(false);

        #endregion Games
    }
}
