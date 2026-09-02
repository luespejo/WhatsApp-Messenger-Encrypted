namespace WhatsAppMessenger.Models
{
    /// <summary>
    /// Modelo que representa un Mensaje encriptado en el sistema
    /// </summary>
    public class Message
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        
        /// <summary>
        /// Contenido del mensaje encriptado en base64
        /// </summary>
        public string EncryptedContent { get; set; }
        
        public DateTime SentAt { get; set; }
        public DateTime? ReadAt { get; set; }
        public bool IsRead { get; set; }
        public string MessageStatus { get; set; } // Sent, Delivered, Read

        // Relaciones
        public User Sender { get; set; }
        public User Receiver { get; set; }
    }
}
