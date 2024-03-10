namespace Homies.Data.DataConstants
{
    public class Constants
    {
        //Event
        public const int EventNameMaxLength = 20;
        public const int EventNameMinLength = 5;
        public const int EventDescriptionMaxLength = 150;
        public const int EventDescriptionMinLength = 15;

        //DateTime
        public const string DataFormat = "yyyy-MM-dd H:mm";

        //Type
        public const int TypeNameMaxLength = 15;
        public const int TypeNameMinLength = 5;

        //Error message

        public const string RequireErrorMessage = "The field {0} is required";
        public const string StringLengthErrorMessage = "The field {0} must be between {2} and {1} characters long";
    }
}
