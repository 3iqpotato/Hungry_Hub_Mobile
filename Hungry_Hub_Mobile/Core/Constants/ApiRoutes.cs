namespace Hungry_Hub_Mobile.Core.Constants;

public static class ApiRoutes
{
    // Базов път - ползваме AppConstants.FullBaseApiUrl
    private static string Base => AppConstants.FullBaseApiUrl;

    // ============= ACCOUNTS (Автентикация) =============
    public static class Accounts
    {
        private static string Prefix => $"{Base}accounts/";

        public static string Register => $"{Prefix}register/";
        public static string Login => $"{Prefix}login/";
        public static string Logout => $"{Prefix}logout/";
        public static string RefreshToken => $"{Prefix}token/refresh/";
    }

    // ============= USERS (Обикновени потребители) =============
    public static class Users
    {
        private static string Prefix => $"{Base}users/";

        public static string CompleteProfile => Prefix; // GET/PUT за профила
        public static string UserHome(int userId) => $"{Prefix}user_home/{userId}/";
        public static string UserCart(int userId) => $"{Prefix}user_cart/{userId}/";
        public static string UserProfile(int userId) => $"{Prefix}user_profile/{userId}/";
        public static string EditUserProfile(int userId) => $"{Prefix}user_profile/edit/{userId}/";
        public static string MyOrders => $"{Prefix}my-orders/";
    }

    // ============= SUPPLIERS (Доставчици) =============
    public static class Suppliers
    {
        private static string Prefix => $"{Base}suppliers/";

        public static string CompleteProfile => Prefix; // GET/PUT за профила
        public static string Home => $"{Prefix}supplier_home/";
        public static string AvailableOrders => $"{Prefix}supplier/available-orders/";
        public static string EditProfile => $"{Prefix}supplier/edit-profile/";
        public static string AcceptOrder(int orderId) => $"{Prefix}supplier/accept_order/{orderId}/";
        public static string ActiveOrders => $"{Prefix}supplier/active-orders/";
        public static string MarkDelivered(int orderId) => $"{Prefix}supplier/mark-delivered/{orderId}/";
        public static string DeliveredOrders => $"{Prefix}supplier/delivered-orders/";
    }

    // ============= RESTAURANTS (Ресторанти) =============
    public static class Restaurants
    {
        private static string Prefix => $"{Base}restaurants/";

        public static string CompleteProfile => Prefix; // GET/PUT за профила
        public static string Home(int restaurantId) => $"{Prefix}restaurant_home/{restaurantId}/";
        public static string Edit(int restaurantId) => $"{Prefix}restaurant/{restaurantId}/edit/";
        public static string MenuDetails(int restaurantId) => $"{Prefix}restaurant/menu/{restaurantId}/";
        public static string MenuForUsers(int restaurantId) => $"{Prefix}restaurant/menu_for_users/{restaurantId}/";
        public static string EditMenu(int restaurantId) => $"{Prefix}restaurant/menu/{restaurantId}/edit/";
        public static string Orders => $"{Prefix}restaurant/orders/";
    }

    // ============= ARTICLES (Артикули/Продукти) =============
    public static class Articles
    {
        private static string Prefix => $"{Base}articles/";

        public static string Add(int menuId) => $"{Prefix}article/add/{menuId}/";
        public static string Edit(int articleId) => $"{Prefix}article/{articleId}/edit/";
        public static string Delete => $"{Prefix}article/delete/"; // предполагам POST с ID в body
    }

    // ============= ORDERS (Поръчки) =============
    public static class Orders
    {
        private static string Prefix => $"{Base}orders/";

        public static string AddToCart(int articleId) => $"{Prefix}add-to-cart/{articleId}/";
        public static string RemoveFromCart(int articleId) => $"{Prefix}remove-from-cart/{articleId}/";
        public static string CreateOrder => $"{Prefix}orders/create_order/";
        public static string OrderDetail(int orderId) => $"{Prefix}order/{orderId}/";
        public static string MarkReady(int orderId) => $"{Prefix}order/{orderId}/ready/";
    }
}