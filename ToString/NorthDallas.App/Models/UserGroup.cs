using NorthDallas.Generators;

namespace NorthDallas.App.Models
{
    [ToString]
    public partial class UserGroup
    {
        public string Name { get; set; } = "North Dallas Developers";

        public string StateMeeting { get; set; } = "First Wednesdays 6pm";

        public string MeetingLocation { get; set; } = "Improving";

    }
}
