namespace Hungry_Hub_Mobile.Core.Constants;

public static class AppConstants
{
    // Базов API URL - различен според средата
#if DEBUG
    public const string BaseApiUrl = "http://10.0.2.2:8000/";
#else
    public const string BaseApiUrl = "https://hungryhub.azurewebsites.net/";
#endif

    // API версия (ако я ползваш)
    public const string ApiVersion = "api/";

    // Пълен базов API път
    public static string FullBaseApiUrl => BaseApiUrl + ApiVersion;

    public const string AccessTokenKey = "access_token";
    public const string RefreshTokenKey = "refresh_token";
    public const string UserTypeKey = "user_type"; //

    public static class UserTypes
    {
        public const string RegularUser = "user";
        public const string Supplier = "supplier";
        public const string Restaurant = "restaurant";
    }

    public static class Validation
    {
        public const int MinPasswordLength = 8;
        public const int MaxNameLength = 100;
        public const int PhoneNumberLength = 10;
    }

    public static class StorageKeys
    {
        public const string UserProfile = "user_profile";
        public const string CartItems = "cart_items";
        public const string LastLoggedInUser = "last_user";
    }

    public const long MaxImageSizeBytes = 5 * 1024 * 1024; // 5 MB
    public const string MaxImageSizeLabel = "5 MB";
}