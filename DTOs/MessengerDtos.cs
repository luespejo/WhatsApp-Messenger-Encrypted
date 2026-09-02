using WhatsAppMessenger.Models;

namespace WhatsAppMessenger.DTOs
{
    /// <summary>
    /// DTO para registrar un nuevo usuario
    /// </summary>
    public class RegisterUserDto
    {
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string PhoneNumber { get; set; }
    }

    /// <summary>
    /// DTO para la respuesta de usuario
    /// </summary>
    public class UserResponseDto
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// DTO para enviar un mensaje
    /// </summary>
    public class SendMessageDto
    {
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string Message { get; set; }
    }

    /// <summary>
    /// DTO para la respuesta de mensaje (con contenido desencriptado)
    /// </summary>
    public class MessageResponseDto
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string SenderName { get; set; }
        public string ReceiverName { get; set; }
        public string Content { get; set; } // Contenido desencriptado
        public DateTime SentAt { get; set; }
        public DateTime? ReadAt { get; set; }
        public bool IsRead { get; set; }
        public string MessageStatus { get; set; }
    }

    /// <summary>
    /// DTO para la respuesta de conversación
    /// </summary>
    public class ConversationResponseDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public List<MessageResponseDto> Messages { get; set; }
    }
}
