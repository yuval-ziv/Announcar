﻿using Announcarr.Abstractions.Contracts.Contracts;
using Announcarr.Exporters.Abstractions.Exporter.AbstractImplementations;
using Announcarr.Exporters.Telegram.Exporter.Configurations;
using Telegram.Bot;
using Telegram.Bot.Types;
using static Announcarr.Exporters.Abstractions.Exporter.Resolvers.TextMessageResolver;

namespace Announcarr.Exporters.Telegram.Exporter.Services;

public class TelegramExporterService : BaseExporterService
{
    private const string DefaultThumbnailNotAvailableUri = "https://thumbs.dreamstime.com/b/ning%C3%BAn-icono-disponible-de-la-imagen-plano-ejemplo-del-vector-132482953.jpg";
    private readonly TelegramBotClient _bot;
    private readonly List<ChatId> _chatIds;

    private readonly TelegramExporterConfiguration _configuration;

    public TelegramExporterService(TelegramExporterConfiguration configuration)
    {
        _configuration = configuration;
        _bot = new TelegramBotClient(_configuration.Bot?.Token ?? "");
        _chatIds = _configuration.Bot?.ChatIds.Select(chatId => new ChatId(chatId)).ToList() ?? [];
    }

    public override bool IsEnabled => _configuration.IsEnabled;

    public override string Name => _configuration.Name ?? "Telegram";
    public override bool? ExportOnEmptyContract { get; set; }
    public override string? CustomMessageOnEmptyContract { get; set; }
    public override bool IsTestExporterEnabled => _configuration.IsTestExporterEnabled;

    public override bool IsExportCalendarEnabled => _configuration.IsExportCalendarEnabled;

    public override bool IsExportRecentlyAddedEnabled => _configuration.IsExportRecentlyAddedEnabled;

    protected override async Task TestExporterLogicAsync(CancellationToken cancellationToken = default)
    {
        await SendToAllChatsAsync(chatId => _bot.SendTextMessageAsync(chatId, "This is a test message.", cancellationToken: cancellationToken));
    }

    protected override async Task ExportCalendarLogicAsync(CalendarContract calendarContract, DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken = default)
    {
        await SendToAllChatsAsync(chatId => _bot.SendTextMessageAsync(chatId,
            $"The calendar for {startDate.ToString(_configuration.DateTimeFormat)} to {endDate.ToString(_configuration.DateTimeFormat)} is:", cancellationToken: cancellationToken));
        await Task.WhenAll(calendarContract.CalendarItems.Select(calendarItem => SendCalendarItemToAllChatsAsync(calendarItem, cancellationToken)));
    }

    protected override async Task ExportEmptyCalendarLogicAsync(DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken = default)
    {
        string text = ResolveTextMessage(CustomMessageOnEmptyContract, AnnouncementType.Calendar);
        await SendToAllChatsAsync(chatId => _bot.SendTextMessageAsync(chatId, text, cancellationToken: cancellationToken));
    }

    protected override async Task ExportRecentlyAddedLogicAsync(RecentlyAddedContract recentlyAddedContract, DateTimeOffset startDate, DateTimeOffset endDate,
        CancellationToken cancellationToken = default)
    {
        await SendToAllChatsAsync(chatId => _bot.SendTextMessageAsync(chatId,
            $"The recently monitored items for {startDate.ToString(_configuration.DateTimeFormat)} to {endDate.ToString(_configuration.DateTimeFormat)} are:", cancellationToken: cancellationToken));
        await Task.WhenAll(recentlyAddedContract.NewlyMonitoredItems.Select(newMonitoredItem => SendNewMonitoredItemToAllChatsAsync(newMonitoredItem, cancellationToken)));
        await SendToAllChatsAsync(chatId => _bot.SendTextMessageAsync(chatId,
            $"The recently added items for {startDate.ToString(_configuration.DateTimeFormat)} to {endDate.ToString(_configuration.DateTimeFormat)} is:", cancellationToken: cancellationToken));
        await Task.WhenAll(recentlyAddedContract.NewItems.Select(calendarItem => SendCalendarItemToAllChatsAsync(calendarItem, cancellationToken)));
    }

    protected override async Task ExportEmptyRecentlyAddedLogicAsync(DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken)
    {
        string text = ResolveTextMessage(CustomMessageOnEmptyContract, AnnouncementType.RecentlyAdded);
        await SendToAllChatsAsync(chatId => _bot.SendTextMessageAsync(chatId, text, cancellationToken: cancellationToken));
    }

    private async Task SendCalendarItemToAllChatsAsync(BaseCalendarItem calendarItem, CancellationToken cancellationToken = default)
    {
        await SendToAllChatsAsync(chatId => _bot.SendPhotoAsync(chatId, new InputFileUrl(calendarItem.ThumbnailUrl ?? DefaultThumbnailNotAvailableUri),
            caption: calendarItem.GetCaption(_configuration.DateTimeFormat),
            cancellationToken: cancellationToken));
    }

    private async Task SendNewMonitoredItemToAllChatsAsync(NewlyMonitoredItem newlyMonitoredItem, CancellationToken cancellationToken = default)
    {
        await SendToAllChatsAsync(chatId => _bot.SendPhotoAsync(chatId, new InputFileUrl(newlyMonitoredItem.ThumbnailUrl ?? DefaultThumbnailNotAvailableUri),
            caption: newlyMonitoredItem.GetCaption(_configuration.DateTimeFormat),
            cancellationToken: cancellationToken));
    }

    private async Task SendToAllChatsAsync(Func<ChatId, Task<Message>> sendFunction)
    {
        await Task.WhenAll(_chatIds.Select(sendFunction));
    }
}