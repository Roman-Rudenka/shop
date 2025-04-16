using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Shop.API.DTO;
using MediatR;
using Shop.Core.Application.Commands;
using Shop.Core.Domain.Interfaces;

namespace Shop.API.Controllers
{
    /// <summary>
    /// Контроллер для управления аккаунтом.
    /// </summary>
    [ApiController]
    [Route("api/account")]
    public class AccountController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IUserRepository _userRepository;
        private readonly IEmailRepository _emailService;

        public AccountController(IMediator mediator, IUserRepository userRepository, IEmailRepository emailService)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        }

        /// <summary>
        /// Запрос на сброс пароля.
        /// </summary>
        /// <param name="request">Запрос с email пользователя.</param>
        /// <returns>Сообщение о результате.</returns>
        [HttpPost("request-password-reset")]
        public async Task<IActionResult> RequestPasswordReset([FromBody] PasswordResetRequest request)
        {
            if (string.IsNullOrEmpty(request.Email))
            {
                return BadRequest(new { Message = "Укажите почту." });
            }

            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null)
            {
                return BadRequest(new { Message = "Пользователь не найден." });
            }

            user.ResetPasswordToken = Guid.NewGuid().ToString();
            user.ResetPasswordTokenExpireAt = DateTime.UtcNow.AddHours(1);
            await _userRepository.UpdateUserAsync(user);

            var resetLink = $"http://localhost:5059/api/account/reset-password?token={user.ResetPasswordToken}";
            await _emailService.SendEmailAsync(
                user.Email,
                "Password Reset Request",
                $"<p>Перейдите по ссылке, чтобы сбросить пароль:</p><a href='{resetLink}'>Сброс</a>"
            );

            return Ok(new { Message = "Сброс пароля отправлен на почту." });
        }

        /// <summary>
        /// Подтверждение аккаунта.
        /// </summary>
        /// <param name="token">Токен подтверждения.</param>
        /// <returns>Результат подтверждения.</returns>
        [HttpPost("confirm-account")]
        public async Task<IActionResult> ConfirmAccount([FromQuery] string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest(new { Message = "Tокен не найден." });
            }

            try
            {
                var command = new ConfirmAccountCommand(token);
                await _mediator.Send(command);

                return Ok(new { Message = "Аккаунт подтвержден" });
            }
            catch (InvalidOperationException)
            {
                return BadRequest(new { Message = "Ошибка подтверждения аккаунта." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Произошла ошибка.", Details = ex.Message });
            }
        }

        /// <summary>
        /// Отправка письма с подтверждением аккаунта.
        /// </summary>
        /// <param name="request">Запрос с email пользователя.</param>
        /// <returns>Сообщение о результате отправки.</returns>
        [HttpPost("send-confirmation-email")]
        public async Task<IActionResult> SendConfirmationEmail([FromBody] SendConfirmationRequest request)
        {
            if (string.IsNullOrEmpty(request.Email))
            {
                return BadRequest(new { Message = "Укажите почту." });
            }

            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null)
            {
                return BadRequest(new { Message = "Пользователь не найден." });
            }

            user.ConfirmationToken = Guid.NewGuid().ToString();
            user.ConfirmationTokenExpiresAt = DateTime.UtcNow.AddHours(24);
            await _userRepository.UpdateUserAsync(user);

            var confirmationLink = $"http://localhost:5059/api/account/confirm-account?token={user.ConfirmationToken}";
            await _emailService.SendEmailAsync(
                user.Email,
                "Confirm Your Account",
                $"<p>Нажмите на ссылку, чтобы подтвердить аккаунт:</p><a href='{confirmationLink}'>Подтвердить</a>"
            );

            return Ok(new { Message = "Письмо отправлено на почту." });
        }

        /// <summary>
        /// Сброс пароля.
        /// </summary>
        /// <param name="request">Запрос с токеном и новым паролем.</param>
        /// <returns>Сообщение о результате.</returns>
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            if (string.IsNullOrEmpty(request.Token) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new { Message = "Укажите токен и новый пароль." });
            }

            try
            {
                var command = new ResetPasswordCommand(request.Token, request.Password);
                await _mediator.Send(command);

                return Ok(new { Message = "Пароль успешно изменен." });
            }
            catch (InvalidOperationException)
            {
                return BadRequest(new { Message = "Ошибка сброса пароля." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Произошла ошибка.", Details = ex.Message });
            }
        }
    }
}
