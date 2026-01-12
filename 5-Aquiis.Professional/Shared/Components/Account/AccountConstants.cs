using Aquiis.Professional.Entities;
namespace Aquiis.Professional.Shared.Components.Account
{
    public static class AccountConstants
    {
        public static string LoginPath { get; } = "/Account/Login";
        public static string RegisterPath { get; } = "/Account/Register";
        public static string ForgotPasswordPath { get; } = "/Account/ForgotPassword";
        public static string ResetPasswordPath { get; } = "/Account/ResetPassword";
        public static string LogoutPath { get; } = "/Account/Logout";
        public static string LockoutPath { get; } = "/Account/Lockout";
        public static string ProfilePath { get; } = "/Account/Profile";
    }
}