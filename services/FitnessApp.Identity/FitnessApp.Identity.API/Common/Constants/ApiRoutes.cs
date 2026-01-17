namespace FitnessApp.Identity.API.Common.Constants
{
    public static class ApiRoutes
    {
        private const string ApiBase = "api";

        public static class Auth
        {
            public const string Base = $"{ApiBase}/auth";
            public const string Register = Base + "/register";
            public const string Login = Base + "/login";
            public const string RefreshToken = Base + "/refresh-token";
            public const string RevokeToken = Base + "/revoke-token";
            public const string ForgotPassword = Base + "/forgot-password";
            public const string ResetPassword = Base + "/reset-password";
            public const string ConfirmEmail = Base + "/confirm-email";
        }

        public static class Users
        {
            public const string Base = $"{ApiBase}/users";
            public const string GetUser = Base + "/{id:guid}";
            public const string UpdateProfile = Base + "/{id:guid}/profile";
            public const string ChangePassword = Base + "/{id:guid}/change-password";
            public const string Deactivate = Base + "/{id:guid}/deactivate";
        }

        public static class Account
        {
            public const string Base = $"{ApiBase}/account";
            public const string Profile = Base + "/profile";
            public const string UpdateProfile = Base + "/profile";
        }
    }
}
