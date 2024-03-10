namespace SeminarHub.Data.DataConstants
{
    public class Constants
    {
        //Semirar
        public const int SeminarTopicMaxLength = 100;
        public const int SeminarTopicMinLength = 3;
        public const int SeminarLecturerMaxLength = 60;
        public const int SeminarLecturerMinLength = 5;
        public const int SeminarDetailsMaxLength = 500;
        public const int SeminarDetailsMinlength = 10;
        public const int SeminarDurationMaxValue = 180;
        public const int SeminarDurationMinValue = 30;

        //Category
        public const int CategoryNameMaxLength = 50;
        public const int CategoryNameMinLength = 3;

        //Date format
        public const string DateFormat = "dd/MM/yyyy HH:mm";

        //Error messages
        public const string TopicErrorMessage = "Topic length must be between {2} and {1} characters!";
        public const string LecturerErrorMessage = "Lecturer length must be between {2} and {1} characters!";
        public const string DetailsErrorMessage = "Details length must be between {2} and {1} characters!";
        public const string DurationErrorMessage = "Duration value must be between {1} and {2}!";
        public const string RequiredErrorMessage = "The field {0} is required";

    }
}
