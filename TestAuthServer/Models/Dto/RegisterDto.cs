using System.ComponentModel.DataAnnotations;

namespace TestAuthServer.Models.Dto;

public class RegisterDto
{
    [MinLength(2), MaxLength(100)] 
    public string UserName { get; set; }
    
    [MinLength(2), MaxLength(100)] 
    public string FullName { get; set; }

    [EmailAddress] 
    public string Email { get; set; }

    [MinLength(8), DataType(DataType.Password)] 
    public string Password { get; set; }

    [MinLength(8), DataType(DataType.Password), Compare(nameof(Password))] 
    public string ConfirmPassword { get; set; }
}
