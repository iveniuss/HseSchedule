namespace Account.DTOs;

public record LoginRequest(string Login, string Password, bool RememberMe = false);