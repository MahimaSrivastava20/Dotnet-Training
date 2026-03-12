using System.Collections.Generic;
using System.Reflection;

namespace MiniSocialMedia
{
    public static class UserExtensions
    {
        public static IEnumerable<string> GetFollowingNames(this User user)
        {
            var field = typeof(User).GetField("_following",
                BindingFlags.NonPublic | BindingFlags.Instance);

            return field?.GetValue(user) as IEnumerable<string>
                   ?? new List<string>();
        }
    }
}