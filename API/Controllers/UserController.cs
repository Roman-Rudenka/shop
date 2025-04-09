using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.Core.Entities;
using Shop.Core.UseCases.Commands;
using Shop.Core.UseCases.Queries;
using Shop.API.DTO;
using Shop.Core.Interfaces;

namespace Shop.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly UserCommands _userCommands;
        private readonly UserQueries _userQueries;
        private readonly IProductRepository _productRepository;
        public UsersController(UserCommands userCommands, UserQueries userQueries, IProductRepository productRepository)
        {
            _userCommands = userCommands ?? throw new ArgumentNullException(nameof(userCommands));
            _userQueries = userQueries ?? throw new ArgumentNullException(nameof(userQueries));
            _productRepository = productRepository;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
        {
            if (string.IsNullOrEmpty(request.Name) || string.IsNullOrEmpty(request.Email) ||
            string.IsNullOrEmpty(request.Password) || string.IsNullOrEmpty(request.Role))
            {
                return BadRequest(new { message = "Все поля обязательны для заполнения." });
            }

            if (!Enum.TryParse<UserRole>(request.Role, true, out var userRole))
            {
                return BadRequest(new { message = "Некорректная роль пользователя." });
            }

            await _userCommands.RegisterUserAsync(request.Name, request.Email, request.Password, userRole);

            return Ok(new { message = "Пользователь успешно зарегистрирован." });
        }


        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var token = await _userCommands.LoginAsync(request.Email, request.Password);
            return Ok(new { Token = token });
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userQueries.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var currentUserId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;

            if (currentUserId == null)
                return Unauthorized(new { Message = "Не удалось идентифицировать пользователя." });

            if (id.ToString() != currentUserId && !User.IsInRole("Admin"))
                return Forbid("Недостаточно прав для доступа к данным другого пользователя.");

            var user = await _userQueries.GetUserByIdAsync(id);
            if (user == null)
                return NotFound(new { Message = "Пользователь не найден." });

            return Ok(user);
        }


        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserRequest request)
        {
            var currentUserId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;

            if (currentUserId == null)
            {
                return Unauthorized(new { Message = "Не удалось идентифицировать пользователя." });
            }

            if (id.ToString() != currentUserId && !User.IsInRole("Admin"))
            {
                return Forbid("Недостаточно прав для обновления другого пользователя.");
            }

            if (!Enum.IsDefined(typeof(UserRole), request.Role))
            {
                return BadRequest(new { Message = "Некорректная роль пользователя." });
            }

            await _userCommands.UpdateUserAsync(id, request.Name, request.Email, request.Password, request.Role);

            return Ok(new { Message = "Данные пользователя успешно обновлены." });
        }



        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var currentUserId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;

            if (currentUserId == null)
                return Unauthorized(new { Message = "Не удалось идентифицировать пользователя." });

            if (id.ToString() != currentUserId && !User.IsInRole("Admin"))
                return Forbid("Недостаточно прав для удаления другого пользователя.");

            await _userCommands.DeleteUserAsync(id);
            return Ok(new { Message = "Пользователь успешно удален." });
        }

        [HttpPost("{id}/activate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ActivateUser(Guid id)
        {
            try
            {
                await _userCommands.ActivateUserAsync(id);


                await _productRepository.ShowProductsByPublisherAsync(id);

                return Ok(new { Message = $"Пользователь с ID {id} успешно активирован, а его продукты отображены." });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { Message = $"Пользователь с ID {id} не найден." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка активации пользователя с ID {id}: {ex.Message}");
                return StatusCode(500, new { Message = $"Произошла ошибка при активации пользователя с ID {id}.", Details = ex.Message });
            }
        }


        [HttpPost("{id}/deactivate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeactivateUser(Guid id)
        {
            try
            {
                await _userCommands.DeactivateUserAsync(id);

                await _productRepository.HideProductsByPublisherAsync(id);

                return Ok(new { Message = $"Пользователь с ID {id} успешно деактивирован, а его продукты скрыты." });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { Message = $"Пользователь с ID {id} не найден." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при деактивации пользователя с ID {id}: {ex.Message}");
                return StatusCode(500, new { Message = $"Произошла ошибка при деактивации пользователя с ID {id}.", Details = ex.Message });
            }
        }

    }
}
