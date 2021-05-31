using System.Collections.Concurrent;
namespace Submit_System {
    class UserPermissions
    {
        public UserPermissions()
        {
            CourseSet = new ConcurrentDictionary<string, Role>();
        }
        public ConcurrentDictionary<string, Role> CourseSet {get; set; }
        public (string, Role) LastExercise {get; set;}
        public (string, Role) LastSubmission {get; set; }
        public (string, Role) LastChat { get; set; }

    }
}