using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Shop.Core.UseCases.Commands;
using Shop.Core.Interfaces;
using Shop.Core.Services;
using Shop.API.DTO;

namespace Shop.API.Controllers
{
    [ApiController]
    [Route("api/account")]
    public class AccountController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly PasswordResetService _passwordResetService;
        private readonly IEmailRepository _emailService;
        private readonly ConfirmAccountCommand _confirmAccountCommand;
        private readonly ResetPasswordCommand _resetPasswordCommand;

        public AccountController(
            IUserRepository userRepository,
            PasswordResetService passwordResetService,
            IEmailRepository emailService,
            ConfirmAccountCommand confirmAccountCommand,
            ResetPasswordCommand resetPasswordCommand)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _passwordResetService = passwordResetService ?? throw new ArgumentNullException(nameof(passwordResetService));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _confirmAccountCommand = confirmAccountCommand ?? throw new ArgumentNullException(nameof(confirmAccountCommand));
            _resetPasswordCommand = resetPasswordCommand ?? throw new ArgumentNullException(nameof(resetPasswordCommand));
        }

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



        [HttpPost("confirm-account")]
        public async Task<IActionResult> ConfirmAccount([FromQuery] string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest(new { Message = "Tокен не найден." });
            }

            try
            {
                await _confirmAccountCommand.ExecuteAsync(token);
                return Ok(new { Message = "Аккаунт подтвержден" });
            }
            catch (InvalidOperationException)
            {
                return BadRequest("Ошибка запроса");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred.", Details = ex.Message });
            }
        }

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
            await _emailService.SendEmailAsync(user.Email, "Confirm Your Account",
                $"<p>Нажмите на ссылку, чтобы подтвердить аккаунт:</p><a href='{confirmationLink}'>Подтвердить</a>");

            return Ok(new { Message = "Письмо отправлено на почту." });
        }


        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            if (string.IsNullOrEmpty(request.Token) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new { Message = "Укажите новый пароль" });
            }

            try
            {
                await _resetPasswordCommand.ExecuteAsync(request.Token, request.Password);
                return Ok(new { Message = "Пароль успешно изменен." });
            }
            catch (InvalidOperationException)
            {
                return BadRequest("Ошибка запроса");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred.", Details = ex.Message });
            }
        }
    }
}
