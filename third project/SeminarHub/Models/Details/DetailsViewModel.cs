namespace SeminarHub.Models.Details
{
    public class DetailsViewModel
    {
        public int Id { get; set; }
        public string Topic { get; set; } = null!;

        public string Lecturer { get; set; } = null!;

        public string Details { get; set; } = null!;

        public int? Duration { get; set; } = 0!;

        public string Category { get; set; } = null!;

        public string DateAndTime { get; set; } = null!;

        public string Organizer { get; set; } = null!;
    }
}
