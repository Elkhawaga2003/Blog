public class VerificationService
{
    public class EmailVerification
    {
        public string Email { get; set; }
        public string Code { get; set; }
        public DateTime ExpiryTime { get; set; }
    }

    private readonly Dictionary<string, EmailVerification> _codes = new();

    public string GenerateCode(string email)
    {
        var code = new Random().Next(100000, 999999).ToString();
        _codes[email] = new EmailVerification
        {
            Email = email,
            Code = code,
            ExpiryTime = DateTime.UtcNow.AddMinutes(5)
        };
        return code;
    }

    public bool VerifyCode(string email, string code)
    {
        if (_codes.TryGetValue(email, out var verification))
        {
            if (verification.Code == code && verification.ExpiryTime > DateTime.UtcNow)
            {
                _codes.Remove(email); 
                return true;
            }
        }
        return false;
    }
}
