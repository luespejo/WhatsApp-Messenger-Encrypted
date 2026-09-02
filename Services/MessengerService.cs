using WhatsAppMessenger.Data;
using WhatsAppMessenger.Models;
using Microsoft.EntityFrameworkCore;

namespace WhatsAppMessenger.Services
{
    /// <summary>
    /// Implementación del servicio Messenger con encriptación
    /// </summary>
    public class MessengerService : IMessengerService
    {
        private readonly MessengerDbContext _dbContext;
        private readonly IEncryptionService _encryptionService;

        public MessengerService(MessengerDbContext dbContext, IEncryptionService encryptionService)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _encryptionService = encryptionService ?? throw new ArgumentNullException(nameof(encryptionService));
        }

        #region Métodos de Usuario

        /// <summary>
        /// Registra un nuevo usuario en el sistema
        /// </summary>
        public async Task<User> RegisterUserAsync(string email, string displayName, string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("El correo no puede estar vacío.", nameof(email));

            if (string.IsNullOrWhiteSpace(displayName))
                throw new ArgumentException("El nombre de usuario no puede estar vacío.", nameof(displayName));

            // Verificar si el usuario ya existe
            var existingUser = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Email == email);

            if (existingUser != null)
                throw new InvalidOperationException($"El usuario con correo {email} ya existe.");

            var user = new User
            {
                Email = email,
                DisplayName = displayName,
                PhoneNumber = phoneNumber,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            return user;
        }

        /// <summary>
        /// Obtiene un usuario por su correo
        /// </summary>
        public async Task<User> GetUserByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("El correo no puede estar vacío.", nameof(email));

            return await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Email == email && u.IsActive);
        }

        /// <summary>
        /// Obtiene un usuario por su ID
        /// </summary>
        public async Task<User> GetUserByIdAsync(int userId)
        {
            if (userId <= 0)
                throw new ArgumentException("El ID del usuario debe ser mayor que 0.", nameof(userId));

            return await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Id == userId && u.IsActive);
        }

        /// <summary>
        /// Obtiene todos los usuarios activos
        /// </summary>
        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _dbContext.Users
                .Where(u => u.IsActive)
                .OrderBy(u => u.DisplayName)
                .ToListAsync();
        }

        /// <summary>
        /// Actualiza la información de un usuario
        /// </summary>
        public async Task<bool> UpdateUserAsync(User user)
        {
            if (user == null || user.Id <= 0)
                throw new ArgumentException("Usuario inválido.", nameof(user));

            var existingUser = await _dbContext.Users.FindAsync(user.Id);
            if (existingUser == null)
                throw new InvalidOperationException($"El usuario con ID {user.Id} no existe.");

            existingUser.DisplayName = user.DisplayName;
            existingUser.PhoneNumber = user.PhoneNumber;
            existingUser.UpdatedAt = DateTime.UtcNow;

            _dbContext.Users.Update(existingUser);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        #endregion

        #region Métodos de Mensaje

        /// <summary>
        /// Envía un mensaje encriptado entre dos usuarios
        /// </summary>
        public async Task<Message> SendMessageAsync(int senderId, int receiverId, string plainTextMessage)
        {
            if (senderId <= 0)
                throw new ArgumentException("El ID del remitente es inválido.", nameof(senderId));

            if (receiverId <= 0)
                throw new ArgumentException("El ID del destinatario es inválido.", nameof(receiverId));

            if (string.IsNullOrWhiteSpace(plainTextMessage))
                throw new ArgumentException("El contenido del mensaje no puede estar vacío.", nameof(plainTextMessage));

            // Verificar que ambos usuarios existan
            var sender = await _dbContext.Users.FindAsync(senderId);
            var receiver = await _dbContext.Users.FindAsync(receiverId);

            if (sender == null)
                throw new InvalidOperationException($"El usuario remitente con ID {senderId} no existe.");

            if (receiver == null)
                throw new InvalidOperationException($"El usuario destinatario con ID {receiverId} no existe.");

            // Encriptar el mensaje
            string encryptedContent = _encryptionService.Encrypt(plainTextMessage);

            var message = new Message
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                EncryptedContent = encryptedContent,
                SentAt = DateTime.UtcNow,
                IsRead = false,
                MessageStatus = "Sent"
            };

            _dbContext.Messages.Add(message);
            await _dbContext.SaveChangesAsync();

            return message;
        }

        /// <summary>
        /// Obtiene un mensaje por su ID (con contenido desencriptado)
        /// </summary>
        public async Task<Message> GetMessageByIdAsync(int messageId)
        {
            if (messageId <= 0)
                throw new ArgumentException("El ID del mensaje es inválido.", nameof(messageId));

            var message = await _dbContext.Messages
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .FirstOrDefaultAsync(m => m.Id == messageId);

            if (message != null)
            {
                // Desencriptar el contenido para visualización
                message.EncryptedContent = _encryptionService.Decrypt(message.EncryptedContent);
            }

            return message;
        }

        /// <summary>
        /// Obtiene la conversación completa entre dos usuarios
        /// </summary>
        public async Task<List<Message>> GetConversationAsync(int userId1, int userId2)
        {
            if (userId1 <= 0 || userId2 <= 0)
                throw new ArgumentException("Los IDs de usuarios son inválidos.");

            var messages = await _dbContext.Messages
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .Where(m => 
                    (m.SenderId == userId1 && m.ReceiverId == userId2) ||
                    (m.SenderId == userId2 && m.ReceiverId == userId1))
                .OrderBy(m => m.SentAt)
                .ToListAsync();

            // Desencriptar todos los mensajes
            foreach (var message in messages)
            {
                message.EncryptedContent = _encryptionService.Decrypt(message.EncryptedContent);
            }

            return messages;
        }

        /// <summary>
        /// Obtiene los mensajes no leídos de un usuario
        /// </summary>
        public async Task<List<Message>> GetUnreadMessagesAsync(int userId)
        {
            if (userId <= 0)
                throw new ArgumentException("El ID del usuario es inválido.", nameof(userId));

            var messages = await _dbContext.Messages
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .Where(m => m.ReceiverId == userId && !m.IsRead)
                .OrderBy(m => m.SentAt)
                .ToListAsync();

            // Desencriptar todos los mensajes
            foreach (var message in messages)
            {
                message.EncryptedContent = _encryptionService.Decrypt(message.EncryptedContent);
            }

            return messages;
        }

        /// <summary>
        /// Marca un mensaje como leído
        /// </summary>
        public async Task<bool> MarkMessageAsReadAsync(int messageId)
        {
            if (messageId <= 0)
                throw new ArgumentException("El ID del mensaje es inválido.", nameof(messageId));

            var message = await _dbContext.Messages.FindAsync(messageId);
            if (message == null)
                throw new InvalidOperationException($"El mensaje con ID {messageId} no existe.");

            message.IsRead = true;
            message.ReadAt = DateTime.UtcNow;
            message.MessageStatus = "Read";

            _dbContext.Messages.Update(message);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Obtiene todos los mensajes de un usuario (enviados y recibidos)
        /// </summary>
        public async Task<List<Message>> GetUserMessagesAsync(int userId)
        {
            if (userId <= 0)
                throw new ArgumentException("El ID del usuario es inválido.", nameof(userId));

            var messages = await _dbContext.Messages
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .Where(m => m.SenderId == userId || m.ReceiverId == userId)
                .OrderByDescending(m => m.SentAt)
                .ToListAsync();

            // Desencriptar todos los mensajes
            foreach (var message in messages)
            {
                message.EncryptedContent = _encryptionService.Decrypt(message.EncryptedContent);
            }

            return messages;
        }

        #endregion
    }
}
