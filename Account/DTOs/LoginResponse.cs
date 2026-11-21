using System.Text.Json.Serialization;

namespace Account.DTOs;

public record LoginResponse(
    [property: JsonConverter(typeof(JsonStringEnumConverter<LoginError>))]
    LoginError Error = LoginError.None);

public enum LoginError
{
    None,
    WrongLoginOrPassword,
    UserIsLockedOut,
}