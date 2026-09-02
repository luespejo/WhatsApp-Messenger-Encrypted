using Microsoft.AspNetCore.Mvc;
using WhatsAppMessenger.DTOs;
using WhatsAppMessenger.Services;

namespace WhatsAppMessenger.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IMessengerService _messengerService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IMessengerService messengerService, ILogger<UsersController> logger)
        {
            _messengerService = messengerService;
            _logger = logger;
        }

        /// <summary>
        /// Registra un nuevo usuario
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserDto dto)
        {
            try
            {
                var user = await _messengerService.RegisterUserAsync(dto.Email, dto.DisplayName, dto.PhoneNumber);
                
                var response = new UserResponseDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    DisplayName = user.DisplayName,
                    PhoneNumber = user.PhoneNumber,
                    CreatedAt = user.CreatedAt,
                    IsActive = user.IsActive
                };

                _logger.LogInformation($"Usuario registrado: {user.Email}");
                return Ok(new { success = true, message = "Usuario registrado exitosamente", data = response });
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
                _logger.LogError(ex, "Error al registrar usuario");
                return StatusCode(500, new { success = false, message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene un usuario por su correo
        /// </summary>
        [HttpGet("by-email/{email}")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            try
            {
                var user = await _messengerService.GetUserByEmailAsync(email);
                
                if (user == null)
                    return NotFound(new { success = false, message = "Usuario no encontrado" });

                var response = new UserResponseDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    DisplayName = user.DisplayName,
                    PhoneNumber = user.PhoneNumber,
                    CreatedAt = user.CreatedAt,
                    IsActive = user.IsActive
                };

                return Ok(new { success = true, data = response });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuario");
                return StatusCode(500, new { success = false, message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene un usuario por su ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                var user = await _messengerService.GetUserByIdAsync(id);
                
                if (user == null)
                    return NotFound(new { success = false, message = "Usuario no encontrado" });

                var response = new UserResponseDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    DisplayName = user.DisplayName,
                    PhoneNumber = user.PhoneNumber,
                    CreatedAt = user.CreatedAt,
                    IsActive = user.IsActive
                };

                return Ok(new { success = true, data = response });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuario");
                return StatusCode(500, new { success = false, message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene todos los usuarios
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _messengerService.GetAllUsersAsync();
                
                var response = users.Select(u => new UserResponseDto
                {
                    Id = u.Id,
                    Email = u.Email,
                    DisplayName = u.DisplayName,
                    PhoneNumber = u.PhoneNumber,
                    CreatedAt = u.CreatedAt,
                    IsActive = u.IsActive
                }).ToList();

                return Ok(new { success = true, data = response });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuarios");
                return StatusCode(500, new { success = false, message = "Error interno del servidor" });
            }
        }
    }
}
