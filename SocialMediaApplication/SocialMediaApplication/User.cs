using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MiniSocialMedia
{
    public partial class User : IPostable, IComparable<User>
    {
        public string Username { get; init; }
        public string Email { get; init; }

        private readonly List<Post> _posts = new();
        private readonly HashSet<string> _following =
            new(StringComparer.OrdinalIgnoreCase);

        public event Action<Post>? OnNewPost;

        public User(string username, string email)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Invalid username", nameof(username));

            if (!Regex.IsMatch(email ?? "",
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                throw new SocialException("Invalid email format");

            Username = username.Trim();
            Email = email.Trim().ToLower();
        }

        public void Follow(string username)
        {
            if (string.Equals(Username, username,
                StringComparison.OrdinalIgnoreCase))
                throw new SocialException("Cannot follow yourself");

            _following.Add(username);
        }

        public bool IsFollowing(string username) =>
            _following.Any(u => string.Equals(u, username,
                StringComparison.OrdinalIgnoreCase));

        public void AddPost(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Post content cannot be empty");

            if (content.Length > 280)
                throw new SocialException("Post too long (max 280 characters)");

            var post = new Post(this, content.Trim());
            _posts.Add(post);
            OnNewPost?.Invoke(post);
        }

        public IReadOnlyList<Post> GetPosts() => _posts.AsReadOnly();

        public int CompareTo(User? other)
        {
            if (other == null) return 1;
            return string.Compare(Username, other.Username,
                StringComparison.OrdinalIgnoreCase);
        }

        public override string ToString() => $"@{Username}";
    }
}