namespace ReportApp.Data.Services;

using ReportApp.Data.Models.communication;
using ReportService.Models;

public class CommunicationRepository
    {
        private readonly DataContext _DbContext;
        
        public CommunicationRepository(DataContext context)
        {
            _DbContext = context;
            
        }

        public void CreateChat(Chat chat, Group? group, List<int> participantIds)
        {
            var existingChat = _DbContext.Chats
                .FirstOrDefault(c => c.Id == chat.Id);
            if (existingChat != null) return;
            _DbContext.Chats.Add(chat);
            if (chat.IsGroup && group != null){
                CreateGroup(group);
            }
            foreach(var userId in participantIds){
                var existingParticipant = _DbContext.ChatParticipants
                .FirstOrDefault(c => c.ChatId == chat.Id && c.UserId == userId);
                if (existingParticipant != null) continue;
                ChatParticipant participant = new ChatParticipant();
                participant.UserId = userId;
                participant.ChatId = chat.Id;
                _DbContext.ChatParticipants.Add(participant);
            }
            _DbContext.SaveChanges(); // Save changes to the database
        }

        public void UpdateChat(Chat chat, Group? group, List<int> participantIds)
        {
            var existingChat = _DbContext.Chats
                .FirstOrDefault(c => c.Id == chat.Id);
            if (existingChat == null) return;
            _DbContext.Chats.Update(chat);
            var deletedParticipants = _DbContext.ChatParticipants.Where(c=>!participantIds.Contains(c.UserId));
            foreach(var participant in deletedParticipants){
                _DbContext.ChatParticipants.Remove(participant);
            }
            foreach(var userId in participantIds){
                var existingParticipant = _DbContext.ChatParticipants
                .FirstOrDefault(c => c.ChatId == chat.Id && c.UserId == userId);
                if (existingParticipant != null) continue;
                ChatParticipant participant = new ChatParticipant();
                participant.UserId = userId;
                participant.ChatId = chat.Id;
                _DbContext.ChatParticipants.Add(participant);
            }
            if (chat.IsGroup && group != null){
                UpdateGroup(group);
            }
                
            _DbContext.SaveChanges(); // Save changes to the database
        }
        public void DeleteChat(Chat chat, Group? group) {
        {
            var existingChat = _DbContext.Chats
                .FirstOrDefault(c => c.Id == chat.Id);
            if (existingChat == null) return;
            if (chat.IsGroup && group != null){
                DeleteGroup(group);
            }
            var participants = _DbContext.ChatParticipants.Where(c=>c.ChatId == chat.Id);
            foreach (var participant in participants){
                _DbContext.ChatParticipants.Remove(participant);
            }
            _DbContext.Chats.Remove(existingChat);
            _DbContext.SaveChanges();
            }
        }
        public void CreateGroup(Group group)
        {
            var existingGroup = _DbContext.Groups
                .FirstOrDefault(g => g.Id == group.Id);
            if (existingGroup != null) return;
            _DbContext.Groups.Add(group); // Add the new instance to the context
            _DbContext.SaveChanges(); // Save changes to the database
        }

        public void UpdateGroup(Group group)
        {
            var existingGroup = _DbContext.Groups
                .FirstOrDefault(g => g.Id == group.Id);
            if (existingGroup == null) return;
            _DbContext.Groups.Update(group); // Add the new instance to the context
            _DbContext.SaveChanges(); // Save changes to the database
        }
        public void DeleteGroup(Group group)
        {
            var existingGroup = _DbContext.Groups
                .FirstOrDefault(g => g.Id == group.Id);
            if (existingGroup != null)
            {
                _DbContext.Groups.Remove(existingGroup);
                _DbContext.SaveChanges();
            }
        }
    }