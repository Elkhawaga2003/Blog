namespace Blog.Helpers
{
    public class JwtOptions
    {

        
            public string Key { get; set; }
            public string Issuer { get; set; }
            public string Audience { get; set; }
            public double Expiration { get; set; } 

    }
}
