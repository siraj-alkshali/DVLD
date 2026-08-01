namespace DVLD.API.Common;

public class StringUtilities
{
    public static string NormalizeUserName(string userName)
    {
        return userName.Trim().ToLower();
    }
}