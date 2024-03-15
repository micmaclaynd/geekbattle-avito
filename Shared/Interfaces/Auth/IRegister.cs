namespace Shared.Interfaces.Auth {
    public class IRegisterRequest {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class IRegisterResponse {
        public string Token { get; set; }
    }

    public class IRegisterHttpRequest {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class IRegisterHttpResponse {
        public string Token { get; set; }
    }
}