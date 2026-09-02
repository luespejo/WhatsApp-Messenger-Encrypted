using WhatsAppMessenger.Models;

namespace WhatsAppMessenger.Services
{
    /// <summary>
    /// Interface del servicio Messenger
    /// </summary>
    public interface IMessengerService
    {
        // Métodos de Usuario
        Task<User> RegisterUserAsync(string email, string displayName, string phoneNumber);
        Task<User> GetUserByEmailAsync(string email);
        Task<User> GetUserByIdAsync(int userId);
        Task<List<User>> GetAllUsersAsync();
        Task<bool> UpdateUserAsync(User user);

        // Métodos de Mensaje
        Task<Message> SendMessageAsync(int senderId, int receiverId, string plainTextMessage);
        Task<Message> GetMessageByIdAsync(int messageId);
        Task<List<Message>> GetConversationAsync(int userId1, int userId2);
        Task<List<Message>> GetUnreadMessagesAsync(int userId);
        Task<bool> MarkMessageAsReadAsync(int messageId);
        Task<List<Message>> GetUserMessagesAsync(int userId);
    }
}
