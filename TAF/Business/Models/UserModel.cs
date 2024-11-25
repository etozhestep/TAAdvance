using Newtonsoft.Json;

namespace TAF.Business.Models;

public class UserModel
{
    [JsonProperty("email")] public string? Email { get; set; }

    [JsonProperty("password")] public string? Pass { get; set; }

    public class UserBuilder
    {
        private readonly UserModel _user = new();

        public UserBuilder WithEmail(string email)
        {
            _user.Email = email;
            return this;
        }

        public UserBuilder WithPassword(string? pass)
        {
            _user.Pass = pass;
            return this;
        }

        public UserModel Build()
        {
            return _user;
        }
    }
}