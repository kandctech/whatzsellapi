

namespace XYZ.WShop.Application.Constants
{
    public static class ApplicationContants
    {
        public static readonly int MaxBatchSMS = 100;
        public const string SupportNumber = "support@wshop.com";
        public const string AppName = "WShop";

        public static object SupportEmail { get; set; }
        public static string CallbackUrl { get; set; } = "https://wakawithus.com";
        public static double TrialDay { get; set; } = 14;

        public static class Messages
        {
            public static readonly string SuccessRetrieval = "{0} retrieved successfully.";
            public static readonly string PasswordResetMessage = "Password reset successfully.";
            public static readonly string AddedSuccessfulMessage = "{0} added successfully.";
            public static readonly string ChangedSuccessfulMessage = "{0} changed successfully.";
            public static readonly string CreatedSuccessfulMessage = "{0} created successfully.";
            public static readonly string NotFound = "{0} not found.";

            public const string RetrievedSuccessfully = "{0} retrieved successfully.";
            public const string CreatedSuccessfully = "{0} created successfully.";
            public const string Success = "Success.";
            public const string Failed = "Failed.";
            public const string ActivatedSuccessfully = "{0} activated successfully.";
            public const string DeactivatedSuccessfully = "{0} deactivated successfully.";
            public const string ApprovedSuccessfully = "{0} approved successfully.";
            public const string DisapprovedSuccessfully = "{0} disapproved successfully.";
            public const string DeletedSuccessfully = "{0} deleted successfully.";
            public const string UpdatedSuccessfully = "{0} updated successfully.";
            public const string LoginSuccessfully = "Login in successfully.";
            public const string BadRequest = "Bad request.";
            public const string InvalidCredentials = "Invalid credentials.";
            public const string RoleExisitMessage = "Role already exist.";
        }
    }
}
