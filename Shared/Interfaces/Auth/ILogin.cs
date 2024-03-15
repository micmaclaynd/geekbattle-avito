namespace Shared.Interfaces.Auth {
    public class ILoginRequest {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class ILoginResponse {
        public string Token { get; set; }
    }

    public class ILoginHttpRequest {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class ILoginHttpResponse {
        public string Token { get; set; }
    }
}