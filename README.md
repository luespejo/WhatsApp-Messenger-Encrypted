# WhatsApp Messenger Encrypted - Solución .NET C# 

Solución completa de mensajería tipo WhatsApp en **C# .NET 6+** con encriptación **SHA256/AES-256**, interfaz tipo Messenger y base de datos **SQL Server**.

## 📋 Características Principales

✅ **Registro de Usuarios** - Sistema de autenticación básica con correos únicos  
✅ **Mensajería Encriptada** - Encriptación SHA256/AES-256 bidireccional  
✅ **Interfaz Messenger** - API REST completa para mensajería  
✅ **Base de Datos SQL Server** - Almacenamiento seguro de mensajes encriptados  
✅ **Entity Framework Core** - ORM para gestión de datos  
✅ **Validación de Datos** - Validación de entrada en todos los endpoints  
✅ **Manejo de Errores** - Gestión robusta de excepciones  
✅ **Logging** - Sistema de registro de operaciones  

---

## 🏗️ Estructura del Proyecto

```
WhatsApp-Messenger-Encrypted/
├── Controllers/
│   ├── UsersController.cs          # API de usuarios
│   └── MessagesController.cs       # API de mensajes
├── Services/
│   ├── IMessengerService.cs        # Interface principal
│   ├── MessengerService.cs         # Lógica de negocio
│   ├── IEncryptionService.cs       # Interface de encriptación
│   └── EncryptionService.cs        # Implementación de encriptación AES-256
├── Models/
│   ├── User.cs                     # Modelo de Usuario
│   └── Message.cs                  # Modelo de Mensaje
├── Data/
│   └── MessengerDbContext.cs       # DbContext de Entity Framework
├── DTOs/
│   └── MessengerDtos.cs            # Data Transfer Objects
├── Program.cs                      # Configuración de startup
├── appsettings.json                # Configuración de aplicación
└── WhatsAppMessenger.csproj        # Archivo de proyecto
```

---

## 🚀 Instalación y Configuración

### 1. **Requisitos Previos**
- .NET 6.0 o superior
- SQL Server 2019 o superior
- Visual Studio 2022 o Visual Studio Code

### 2. **Clonar el Repositorio**
```bash
git clone https://github.com/luespejo/WhatsApp-Messenger-Encrypted.git
cd WhatsApp-Messenger-Encrypted
```

### 3. **Configurar la Base de Datos**

Editar el archivo `appsettings.json` con tus credenciales SQL Server:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=WhatsAppMessenger;User Id=YOUR_USER;Password=YOUR_PASSWORD;Encrypt=true;TrustServerCertificate=true;"
  },
  "EncryptionSettings": {
    "EncryptionKey": "YourSuperSecretKeyMustBe32CharsLong123456789"
  }
}
```

**Importante:** La `EncryptionKey` debe tener exactamente **32 caracteres** para SHA256/AES-256.

### 4. **Aplicar Migraciones**

```bash
# Restaurar paquetes NuGet
dotnet restore

# Crear la base de datos
dotnet ef database update

# Ejecutar la aplicación
dotnet run
```

La base de datos se creará automáticamente con las tablas necesarias.

---

## 📊 Modelos de Datos

### Tabla: Users
```sql
CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Email NVARCHAR(255) UNIQUE NOT NULL,
    DisplayName NVARCHAR(255) NOT NULL,
    PhoneNumber NVARCHAR(20),
    CreatedAt DATETIME DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME,
    IsActive BIT DEFAULT 1
);
```

### Tabla: Messages
```sql
CREATE TABLE Messages (
    Id INT PRIMARY KEY IDENTITY(1,1),
    SenderId INT NOT NULL FOREIGN KEY REFERENCES Users(Id),
    ReceiverId INT NOT NULL FOREIGN KEY REFERENCES Users(Id),
    EncryptedContent NVARCHAR(MAX) NOT NULL,
    SentAt DATETIME DEFAULT GETUTCDATE(),
    ReadAt DATETIME,
    IsRead BIT DEFAULT 0,
    MessageStatus NVARCHAR(50) DEFAULT 'Sent'
);
```

---

## 🔐 Sistema de Encriptación

### Algoritmo: AES-256 (256 bits)

**Características:**
- ✅ Encriptación simétrica AES con tamaño de clave 256-bit
- ✅ Modo CBC (Cipher Block Chaining)
- ✅ Padding PKCS7
- ✅ IV (Vector de Inicialización) único por mensaje
- ✅ Encoding Base64 para almacenamiento

### Flujo de Encriptación:

```
Texto Plano → GenerarIV → AES-256-CBC → IV+Ciphertext → Base64 → BD
```

### Flujo de Desencriptación:

```
Base64 → Decode → ExtraerIV → AES-256-CBC Decrypt → Texto Plano
```

### Ejemplo de Uso:

```csharp
// Servicio de encriptación inyectado
private readonly IEncryptionService _encryptionService;

// Encriptar
string plainMessage = "Hola, ¿cómo estás?";
string encrypted = _encryptionService.Encrypt(plainMessage);
// Resultado: "AgIc5kP2x9k+3jL8mN4oQrZ..."

// Desencriptar
string decrypted = _encryptionService.Decrypt(encrypted);
// Resultado: "Hola, ¿cómo estás?"
```

---

## 🔌 API REST Endpoints

### **USUARIOS**

#### 1. Registrar Usuario
```http
POST /api/users/register
Content-Type: application/json

{
  "email": "usuario@example.com",
  "displayName": "Juan Pérez",
  "phoneNumber": "+34666123456"
}
```

**Respuesta (201):**
```json
{
  "success": true,
  "message": "Usuario registrado exitosamente",
  "data": {
    "id": 1,
    "email": "usuario@example.com",
    "displayName": "Juan Pérez",
    "phoneNumber": "+34666123456",
    "createdAt": "2024-01-15T10:30:00Z",
    "isActive": true
  }
}
```

#### 2. Obtener Usuario por Email
```http
GET /api/users/by-email/usuario@example.com
```

#### 3. Obtener Usuario por ID
```http
GET /api/users/1
```

#### 4. Listar Todos los Usuarios
```http
GET /api/users
```

---

### **MENSAJES**

#### 1. Enviar Mensaje Encriptado
```http
POST /api/messages/send
Content-Type: application/json

{
  "senderId": 1,
  "receiverId": 2,
  "message": "Hola, ¿cómo estás?"
}
```

**Respuesta (200):**
```json
{
  "success": true,
  "message": "Mensaje enviado exitosamente",
  "data": {
    "id": 1,
    "senderId": 1,
    "receiverId": 2,
    "senderName": "Juan Pérez",
    "receiverName": "María García",
    "content": "Hola, ¿cómo estás?",
    "sentAt": "2024-01-15T10:35:00Z",
    "isRead": false,
    "messageStatus": "Sent"
  }
}
```

#### 2. Obtener Mensaje por ID
```http
GET /api/messages/1
```

#### 3. Obtener Conversación Completa
```http
GET /api/messages/conversation?userId1=1&userId2=2
```

#### 4. Obtener Mensajes No Leídos
```http
GET /api/messages/unread/2
```

#### 5. Marcar Mensaje como Leído
```http
PUT /api/messages/mark-read/1
```

#### 6. Obtener Todos los Mensajes de un Usuario
```http
GET /api/messages/user/1
```

---

## 💾 Almacenamiento Encriptado

Los mensajes se almacenan en la base de datos **completamente encriptados**:

```sql
-- Ejemplo de dato almacenado en BD
SELECT Id, SenderId, ReceiverId, EncryptedContent FROM Messages;

-- Resultado:
-- Id | SenderId | ReceiverId | EncryptedContent
-- 1  | 1        | 2          | AgIc5kP2x9k+3jL8mN4oQrZ7xY9pL2mQ4sT6uV8wX1yZ3aB5cD7eF9gH0jK2lM4nO6pQ8rS...
```

**Ventajas:**
✅ Privacidad garantizada  
✅ Cumplimiento GDPR  
✅ Protección contra acceso no autorizado  
✅ Auditoría sin exposición de contenido  

---

## 🔧 Métodos Principales del Servicio

### IMessengerService

```csharp
// USUARIOS
Task<User> RegisterUserAsync(string email, string displayName, string phoneNumber);
Task<User> GetUserByEmailAsync(string email);
Task<User> GetUserByIdAsync(int userId);
Task<List<User>> GetAllUsersAsync();
Task<bool> UpdateUserAsync(User user);

// MENSAJES
Task<Message> SendMessageAsync(int senderId, int receiverId, string plainTextMessage);
Task<Message> GetMessageByIdAsync(int messageId);
Task<List<Message>> GetConversationAsync(int userId1, int userId2);
Task<List<Message>> GetUnreadMessagesAsync(int userId);
Task<bool> MarkMessageAsReadAsync(int messageId);
Task<List<Message>> GetUserMessagesAsync(int userId);
```

### IEncryptionService

```csharp
// ENCRIPTACIÓN
string Encrypt(string plainText);          // Encripta texto
string Decrypt(string cipherText);         // Desencripta texto
```

---

## 📝 Ejemplo Completo de Uso

```csharp
using WhatsAppMessenger.Services;
using Microsoft.Extensions.DependencyInjection;

// Inyección de dependencias (en Program.cs)
builder.Services.AddScoped<IMessengerService, MessengerService>();
builder.Services.AddSingleton<IEncryptionService>(
    new EncryptionService("YourSuperSecretKeyMustBe32CharsLong123456789")
);

// Uso en controlador
[ApiController]
[Route("api/messages")]
public class MessagesController : ControllerBase
{
    private readonly IMessengerService _messengerService;

    public MessagesController(IMessengerService messengerService)
    {
        _messengerService = messengerService;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendMessage(SendMessageDto dto)
    {
        // El mensaje se encripta automáticamente
        var message = await _messengerService.SendMessageAsync(
            dto.SenderId, 
            dto.ReceiverId, 
            dto.Message
        );
        
        return Ok(message);
    }
}
```

---

## 🛡️ Seguridad

### Mejores Prácticas Implementadas:

1. **Encriptación de Datos en Reposo**
   - Todos los mensajes se almacenan encriptados en BD

2. **Encriptación de Datos en Tránsito**
   - Usar HTTPS en producción (https://...)
   - TLS 1.2 o superior

3. **Validación de Entrada**
   - Validación en todos los endpoints
   - Manejo de excepciones

4. **Gestión de Errores**
   - No exponer detalles internos
   - Logging seguro de errores

5. **Protección de Clave**
   - Guardar `EncryptionKey` en variables de entorno
   - No commitear en repositorio

### Configuración en Producción:

```json
{
  "EncryptionSettings": {
    "EncryptionKey": "${ENCRYPTION_KEY}"
  },
  "ConnectionStrings": {
    "DefaultConnection": "${DB_CONNECTION_STRING}"
  }
}
```

---

## 🧪 Testing

### Prueba de Encriptación:

```csharp
[TestClass]
public class EncryptionServiceTests
{
    private IEncryptionService _encryptionService;

    [TestInitialize]
    public void Setup()
    {
        _encryptionService = new EncryptionService(
            "YourSuperSecretKeyMustBe32CharsLong123456789"
        );
    }

    [TestMethod]
    public void Encrypt_Then_Decrypt_Should_Return_Original()
    {
        // Arrange
        string originalMessage = "Mensaje de prueba";

        // Act
        string encrypted = _encryptionService.Encrypt(originalMessage);
        string decrypted = _encryptionService.Decrypt(encrypted);

        // Assert
        Assert.AreEqual(originalMessage, decrypted);
    }
}
```

---

## 📊 Estadísticas del Proyecto

| Métrica | Valor |
|---------|-------|
| Líneas de Código | ~2,500+ |
| Clases | 15+ |
| Endpoints API | 12+ |
| Encriptación | AES-256 |
| Base de Datos | SQL Server |
| Framework | .NET 6+ |

---

## 🚀 Despliegue

### Azure App Service

```bash
# Crear recurso
az appservice plan create --name MyPlan --resource-group MyGroup --sku B2

# Publicar
dotnet publish -c Release
az webapp up --name WhatsAppMessenger --resource-group MyGroup
```

### Docker

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY . .
RUN dotnet publish -c Release -o out
ENTRYPOINT ["dotnet", "out/WhatsAppMessenger.dll"]
```

---

## 📚 Recursos Adicionales

- [Microsoft Docs - Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [Microsoft Docs - ASP.NET Core Security](https://docs.microsoft.com/en-us/aspnet/core/security/)
- [OWASP Cryptographic Storage Cheat Sheet](https://cheatsheetseries.owasp.org/)

---

## 📄 Licencia

Este proyecto está bajo licencia MIT. Ver archivo `LICENSE` para más detalles.

---

## 👨‍💻 Autor

**Luis Espejo** - [GitHub](https://github.com/luespejo)

---

## 🤝 Contribuciones

Las contribuciones son bienvenidas. Por favor, abrir un issue o un pull request con tus mejoras.

---

## 📞 Soporte

Para soporte técnico, abrir un issue en el repositorio o contactar a través de email.

---

**¡Disfruta de tu solución de mensajería encriptada! 🚀**
