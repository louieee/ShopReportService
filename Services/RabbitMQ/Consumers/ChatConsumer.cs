using Newtonsoft.Json;
using ReportApp.Data;
using ReportApp.Data.Models.communication;
using ReportApp.Data.Services;
using ReportService.Models;

namespace ReportService.Services.RabbitMQ.Consumers;

public class ChatConsumer
{

    private static ChatPayload?  FormatDataAsChatPayload(string message)
    {
        ChatPayload? chatPayload = null;
        try
        {
            chatPayload = JsonConvert.DeserializeObject<ChatPayload>(message);

        }
        catch (JsonSerializationException ex)
        {
            Console.Write(ex.Message);
            chatPayload = null;
        }

        return chatPayload;
    }


    public static void HandleChat(DataContext context, string action, string data)
    {
        switch (action)
        {
            case "create": 
                handleNewChat(context, data); break;
            case "update":
                handleChatUpdate(context, data); break;
            case "delete":
                handleChatDelete(context, data); break;
            default:
                Console.WriteLine(data); break;
                
        }
    }

    private static void handleNewChat(DataContext context, string data)
    {
        var chatPayload = FormatDataAsChatPayload(data);
        if (chatPayload == null) return;
        Console.WriteLine($"New Chat: {chatPayload.Id}");
        var chatRepository = new CommunicationRepository(context);
        var chat = new Chat
        {
            Id = chatPayload.Id,
            DateConnected = DateTime.Now,
            GroupId = chatPayload.GroupId,
            IsGroup = chatPayload.IsGroup
        };
        if (chatPayload.IsGroup && chatPayload.GroupId != null){
            var group = new Group
            {
                Id = chatPayload.GroupId,
                Name = chatPayload.GroupName ?? "" ,
                Type = chatPayload.GroupType ?? "",
                CreatorId = chatPayload.GroupCreator,
            };
            chatRepository.CreateChat(chat, group, chatPayload.Participants);
            return;
        }
        chatRepository.CreateChat(chat, null, chatPayload.Participants);
        return;

    }
    private static void handleChatUpdate(DataContext context, string data)
    {
        var chatPayload = FormatDataAsChatPayload(data);
        if (chatPayload == null) return;
        Console.WriteLine($"Updated Chat: {chatPayload.Id}");
        var chatRepository = new CommunicationRepository(context);
        var chat = new Chat
        {
            Id = chatPayload.Id,
            GroupId = chatPayload.GroupId,
            IsGroup = chatPayload.IsGroup
        };
        if (chatPayload.IsGroup && chatPayload.GroupId != null){
            var group = new Group
            {
                Id = chatPayload.GroupId,
                Name = chatPayload.GroupName ?? "" ,
                Type = chatPayload.GroupType ?? "",
                CreatorId = chatPayload.GroupCreator,
            };
            chatRepository.UpdateChat(chat, group, chatPayload.Participants);
            return;
        }
        chatRepository.UpdateChat(chat, null, chatPayload.Participants);
        return;

    }
    private static void handleChatDelete(DataContext context, string data)
    {
        var chatPayload = FormatDataAsChatPayload(data);
        if (chatPayload == null) return;
        Console.WriteLine($"Deleted Chat: {chatPayload.Id}");
        var chatRepository = new CommunicationRepository(context);
        var chat = new Chat
        {
            Id = chatPayload.Id,
            GroupId = chatPayload.GroupId,
            IsGroup = chatPayload.IsGroup
        };
        if (chatPayload.IsGroup && chatPayload.GroupId != null){
            var group = new Group
            {
                Id = chatPayload.GroupId,
                Name = chatPayload.GroupName ?? "" ,
                Type = chatPayload.GroupType ?? "",
                CreatorId = chatPayload.GroupCreator,
            };
            chatRepository.DeleteChat(chat, group);
            return;
        }
        chatRepository.DeleteChat(chat, null);
        return;
    }

}