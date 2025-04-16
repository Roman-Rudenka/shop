using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Shop.Core.Application.Commands.Users;
using Shop.Core.Application.Queries.Users;
using Shop.Core.Domain.Entities;
using Shop.API.DTO;


namespace Shop.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
        {
            if (string.IsNullOrEmpty(request.Name) || string.IsNullOrEmpty(request.Email) ||
                string.IsNullOrEmpty(request.Password) || string.IsNullOrEmpty(request.Role))
            {
                return BadRequest(new { Message = "Все поля обязательны для заполнения." });
            }

            if (!Enum.TryParse<UserRole>(request.Role, true, out var userRole))
            {
                return BadRequest(new { Message = "Некорректная роль пользователя." });
            }

            var command = new RegisterUserCommand(request.Name, request.Email, request.Password, userRole);
            await _mediator.Send(command);

            return Ok(new { Message = "Пользователь успешно зарегистрирован." });
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var command = new LoginCommand(request.Email, request.Password);
            var token = await _mediator.Send(command);

            return Ok(new { Token = token });
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var query = new GetAllUsersQuery();
            var users = await _mediator.Send(query);

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

            var query = new GetUserByIdQuery(id);
            var user = await _mediator.Send(query);

            if (user == null)
                return NotFound(new { Message = "Пользователь не найден." });

            return Ok(user);
        }

        [HttpGet("email/{email}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            var query = new GetUserByEmailQuery(email);
            var user = await _mediator.Send(query);

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

            var command = new UpdateUserCommand(id, request.Name, request.Email, request.Password, request.Role);
            await _mediator.Send(command);

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

            var command = new DeleteUserCommand(id);
            await _mediator.Send(command);

            return Ok(new { Message = "Пользователь успешно удален." });
        }

        [HttpPost("{id}/activate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ActivateUser(Guid id)
        {
            try
            {
                var command = new ActivateUserCommand(id);
                await _mediator.Send(command);

                return Ok(new { Message = $"Пользователь с ID {id} успешно активирован." });
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
                var command = new DeactivateUserCommand(id);
                await _mediator.Send(command);

                return Ok(new { Message = $"Пользователь с ID {id} успешно деактивирован." });
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
