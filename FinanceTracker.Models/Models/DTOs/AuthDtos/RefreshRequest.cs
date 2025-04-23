namespace FinanceTracker.Domain.Models.DTOs.AuthDtos
{
    public class RefreshRequest
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
