using System.Collections.Generic;
using System.Threading;
using System;
using System.Security;
namespace Submit_System {
    public class UserPermissions
    {
        public UserPermissions()
        {
            CourseSet = new Dictionary<string, Role>();
            LastExercise = (null, Role.Student);
            LastSubmission = (null, Role.Student);
            LastExtChat = (null, Role.Student);
            LastAppChat = (null, Role.Student);
        }
        private Dictionary<string, Role> CourseSet;
        private (string exID, Role role) LastExercise;
        private (string subID, Role role) LastSubmission;
        private string SubmissionFolder;
        private string ExerciseFolder;
        private (string chatID, Role role) LastExtChat;
        private (string chatID, Role role) LastAppChat;

        public void SetCoursePerm(string courseid, Role role)
        {
            CourseSet.TryAdd(courseid, role);
        }
        public bool CheckCoursePerm(string courseid, Role role)
        {
            Role actualRole;
            if(CourseSet.TryGetValue(courseid, out actualRole))
            {
                return (actualRole & role) == actualRole;
            }
            return false;
        }
        public bool CheckExercisePerm(string exerciseid, Role role)
        {
            return LastExercise == (exerciseid, role);
        }
        public void SetExercisePerm(string exerciseid, Role role)
        {
            LastExercise = (exerciseid, role);
        }
        /// <summary>
        /// Only teachers should have access to this.
        /// </summary>
        /// <param name="exerciseId"></param>
        /// <returns></returns>
        public string GetExerciseFolder(string exerciseId)
        {
            if(exerciseId != LastExercise.exID)
            {
                return null;
            }
            return ExerciseFolder;
        }
        public void SetTeacherExercisePerm(string exerciseid, string path)
        {
            LastExercise = (exerciseid, Role.Teacher);
            ExerciseFolder = path;
        }
        public bool CheckSubmissionPerm(string submissionid, Role role)
        {
            return LastSubmission == (submissionid, role);
        }
        public string GetSubmissionFolder(string submisionid)
        {
            return (LastSubmission.subID == submisionid) ? SubmissionFolder : null;
        }
        public void SetSubmissionPerm(string submissionid, Role role, string path)
        {
            LastExercise = (submissionid, role);
            SubmissionFolder = path;
        }
        public bool CheckChatPerm(string chatid, Role role)
        {
            return (LastExtChat == (chatid, role)) || (LastAppChat == (chatid, role));
        }
        public void SetChatPerm(string chatid, Role role, bool isExt)
        {
           var sett = (chatid, role);
           if(isExt)
           {
               LastExtChat = sett;
           }
           else
           {
               LastAppChat = sett;
           }
        }
    }
}