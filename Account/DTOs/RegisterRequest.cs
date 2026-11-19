namespace Account.DTOs;

public record RegisterRequest(string Login, string Password, bool RememberMe = false);