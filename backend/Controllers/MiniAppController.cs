using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using backend.Models.DTOs.MiniApp;

namespace backend.Controllers
{
    public class MiniAppController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<MiniAppController> _logger;
        private readonly string _botToken;

        public MiniAppController(IConfiguration configuration, ILogger<MiniAppController> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _botToken = configuration["Telegram:BotToken"] ?? throw new ArgumentException("BotToken not configured");
        }

        [HttpGet("/miniapp")]
        public IActionResult ServeMiniApp()
        {
            var validationResult = ValidateTelegramWebAppData();
            
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Blocked unauthorized access: {Reason}", validationResult.Error);
                return BadRequest(new { 
                    error = validationResult.Error,
                    instruction = "Откройте приложение через Telegram бота"
                });
            }

            var frontendUrl = _configuration["App:FrontendUrl"] ?? "http://localhost:3000";
            
            var redirectUrl = $"{frontendUrl}?{Request.QueryString}";
            
            _logger.LogInformation("Redirecting authenticated user to frontend");
            return Redirect(redirectUrl);
        }

        private TelegramValidationResult ValidateTelegramWebAppData()
        {
            try
            {
                var initData = Request.Query["tgWebAppData"].ToString();
                
                if (string.IsNullOrEmpty(initData))
                {
                    return new TelegramValidationResult 
                    { 
                        IsValid = false, 
                        Error = "Missing Telegram Web App data" 
                    };
                }
                
                var parsedData = HttpUtility.ParseQueryString(initData);
                var hash = parsedData["hash"];
                var checkString = new List<string>();
                
                foreach (string key in parsedData)
                {
                    if (key == "hash") continue;
                    checkString.Add($"{key}={parsedData[key]}");
                }

                checkString.Sort();
                var checkStringStr = string.Join("\n", checkString);
                
                var secretKey = HMACSHA256.HashData(
                    Encoding.UTF8.GetBytes("WebAppData"), 
                    Encoding.UTF8.GetBytes(_botToken)
                );
                
                var computedHash = BytesToHex(HMACSHA256.HashData(
                    secretKey, 
                    Encoding.UTF8.GetBytes(checkStringStr)
                ));

                if (computedHash != hash)
                {
                    return new TelegramValidationResult 
                    { 
                        IsValid = false, 
                        Error = "Invalid Telegram signature" 
                    };
                }
                
                var authDateStr = parsedData["auth_date"];
                if (long.TryParse(authDateStr, out var authDate))
                {
                    var dataAge = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - authDate;
                    if (dataAge > 3600)
                    {
                        return new TelegramValidationResult 
                        { 
                            IsValid = false, 
                            Error = "Telegram data expired" 
                        };
                    }
                }

                return new TelegramValidationResult 
                { 
                    IsValid = true,
                    UserId = parsedData["id"],
                    FirstName = parsedData["first_name"],
                    LastName = parsedData["last_name"]
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating Telegram data");
                return new TelegramValidationResult 
                { 
                    IsValid = false, 
                    Error = "Validation error" 
                };
            }
        }

        private static string BytesToHex(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
    }


}