using Microsoft.AspNetCore.Mvc;
using WhatsAppMessenger.DTOs;
using WhatsAppMessenger.Services;

namespace WhatsAppMessenger.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly IMessengerService _messengerService;
        private readonly ILogger<MessagesController> _logger;

        public MessagesController(IMessengerService messengerService, ILogger<MessagesController> logger)
        {
            _messengerService = messengerService;
            _logger = logger;
        }

        /// <summary>
        /// Envía un mensaje encriptado
        /// </summary>
        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageDto dto)
        {
            try
            {
                var message = await _messengerService.SendMessageAsync(dto.SenderId, dto.ReceiverId, dto.Message);

                var response = new MessageResponseDto
                {
                    Id = message.Id,
                    SenderId = message.SenderId,
                    ReceiverId = message.ReceiverId,
                    SenderName = message.Sender?.DisplayName,
                    ReceiverName = message.Receiver?.DisplayName,
                    Content = dto.Message, // El contenido original sin encriptar
                    SentAt = message.SentAt,
                    IsRead = message.IsRead,
                    MessageStatus = message.MessageStatus
                };

                _logger.LogInformation($"Mensaje enviado de usuario {dto.SenderId} a {dto.ReceiverId}");
                return Ok(new { success = true, message = "Mensaje enviado exitosamente", data = response });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex.Message);
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex.Message);
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar mensaje");
                return StatusCode(500, new { success = false, message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene un mensaje por ID (desencriptado)
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMessageById(int id)
        {
            try
            {
                var message = await _messengerService.GetMessageByIdAsync(id);

                if (message == null)
                    return NotFound(new { success = false, message = "Mensaje no encontrado" });

                var response = new MessageResponseDto
                {
                    Id = message.Id,
                    SenderId = message.SenderId,
                    ReceiverId = message.ReceiverId,
                    SenderName = message.Sender?.DisplayName,
                    ReceiverName = message.Receiver?.DisplayName,
                    Content = message.EncryptedContent, // Ya desencriptado por el servicio
                    SentAt = message.SentAt,
                    ReadAt = message.ReadAt,
                    IsRead = message.IsRead,
                    MessageStatus = message.MessageStatus
                };

                return Ok(new { success = true, data = response });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener mensaje");
                return StatusCode(500, new { success = false, message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene la conversación entre dos usuarios
        /// </summary>
        [HttpGet("conversation")]
        public async Task<IActionResult> GetConversation(int userId1, int userId2)
        {
            try
            {
                var messages = await _messengerService.GetConversationAsync(userId1, userId2);

                var responseMessages = messages.Select(m => new MessageResponseDto
                {
                    Id = m.Id,
                    SenderId = m.SenderId,
                    ReceiverId = m.ReceiverId,
                    SenderName = m.Sender?.DisplayName,
                    ReceiverName = m.Receiver?.DisplayName,
                    Content = m.EncryptedContent, // Ya desencriptado por el servicio
                    SentAt = m.SentAt,
                    ReadAt = m.ReadAt,
                    IsRead = m.IsRead,
                    MessageStatus = m.MessageStatus
                }).ToList();

                return Ok(new { success = true, count = responseMessages.Count, data = responseMessages });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener conversación");
                return StatusCode(500, new { success = false, message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene los mensajes no leídos de un usuario
        /// </summary>
        [HttpGet("unread/{userId}")]
        public async Task<IActionResult> GetUnreadMessages(int userId)
        {
            try
            {
                var messages = await _messengerService.GetUnreadMessagesAsync(userId);

                var responseMessages = messages.Select(m => new MessageResponseDto
                {
                    Id = m.Id,
                    SenderId = m.SenderId,
                    ReceiverId = m.ReceiverId,
                    SenderName = m.Sender?.DisplayName,
                    ReceiverName = m.Receiver?.DisplayName,
                    Content = m.EncryptedContent, // Ya desencriptado por el servicio
                    SentAt = m.SentAt,
                    ReadAt = m.ReadAt,
                    IsRead = m.IsRead,
                    MessageStatus = m.MessageStatus
                }).ToList();

                return Ok(new { success = true, count = responseMessages.Count, data = responseMessages });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener mensajes no leídos");
                return StatusCode(500, new { success = false, message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Marca un mensaje como leído
        /// </summary>
        [HttpPut("mark-read/{messageId}")]
        public async Task<IActionResult> MarkMessageAsRead(int messageId)
        {
            try
            {
                await _messengerService.MarkMessageAsReadAsync(messageId);
                _logger.LogInformation($"Mensaje {messageId} marcado como leído");
                return Ok(new { success = true, message = "Mensaje marcado como leído" });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex.Message);
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al marcar mensaje como leído");
                return StatusCode(500, new { success = false, message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene todos los mensajes de un usuario
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserMessages(int userId)
        {
            try
            {
                var messages = await _messengerService.GetUserMessagesAsync(userId);

                var responseMessages = messages.Select(m => new MessageResponseDto
                {
                    Id = m.Id,
                    SenderId = m.SenderId,
                    ReceiverId = m.ReceiverId,
                    SenderName = m.Sender?.DisplayName,
                    ReceiverName = m.Receiver?.DisplayName,
                    Content = m.EncryptedContent, // Ya desencriptado por el servicio
                    SentAt = m.SentAt,
                    ReadAt = m.ReadAt,
                    IsRead = m.IsRead,
                    MessageStatus = m.MessageStatus
                }).ToList();

                return Ok(new { success = true, count = responseMessages.Count, data = responseMessages });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener mensajes del usuario");
                return StatusCode(500, new { success = false, message = "Error interno del servidor" });
            }
        }
    }
}
