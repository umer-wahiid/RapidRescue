using RapidResuce.Enums;

namespace RapidResuce.Models
{

    public class Session
    {
        private static ISession _session => new HttpContextAccessor().HttpContext.Session;

        public static int UserId
        {
            get
            {
                return _session.GetInt32("RRUserId") ?? 0;
            }
            set
            {
                _session.SetInt32("RRUserId", value);
            }
        }

        public static string Name
        {
            get
            {
                return _session.GetString("RRUserName") ?? string.Empty;
            }
            set
            {
                _session.SetString("RRUserName", value);
            }
        }

        public static UserRole Role
        {
            get
            {
                var roleString = _session.GetString("UserRole");
                return Enum.TryParse(roleString, out UserRole role) ? role : UserRole.Patient;
            }
            set
            {
                _session.SetString("UserRole", value.ToString());
            }
        }

        public static string Email
        {
            get
            {
                return _session.GetString("RRUserEmail") ?? string.Empty;
            }
            set
            {
                _session.SetString("RRUserEmail", value);
            }
        }

        public static string Image
        {
            get
            {
                return _session.GetString("RRUserImage") ?? string.Empty;
            }
            set
            {
                _session.SetString("RRUserImage", value);
            }
        }


    }
}
