namespace Users.Domain.Constants;

public class UserConstants
{
    public static readonly int UserUsernameMinLength = 6;
    public static readonly int UserUsernameMaxLength = 16;
    
    public static readonly int UserFirstNameMinLength = 2;
    public static readonly int UserFirstNameMaxLength = 50;
    
    public static readonly int UserLastNameMinLength = 2;
    public static readonly int UserLastNameMaxLength = 50;
    
    public static readonly int UserEmailMaxLength = 255;
    
    public static readonly int UserPasswordMinLength = 8;
    public static readonly int UserPasswordMaxLength = 24;
}