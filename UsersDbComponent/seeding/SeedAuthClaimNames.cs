using System.Collections;
using System.Diagnostics.CodeAnalysis;
using UsersDbComponent.entities;

namespace UsersDbComponent.seeding
{
    public class SeedAuthClaimNames : IDictionary<string, AuthClaimRights>
    {
        public const string Email = "email";
        public const string Username = "username";
        public const string UserId = "userId";

        public ICollection<string> Keys => GetType()
            .GetFields()
            .Where(x => x.IsStatic && x.FieldType == typeof(string))
            .Select(f => (string)f.GetValue(null)!)
            .ToList();

        public ICollection<AuthClaimRights> Values => Enum.GetValues<AuthClaimRights>();

        public int Count => GetType()
            .GetFields()
            .Where(x => x.IsStatic && x.FieldType == typeof(string))
            .Count();

        public bool IsReadOnly => true;

        public AuthClaimRights this[string key]
        {
            get => key switch
            {
                Email => AuthClaimRights.Readable,
                Username => AuthClaimRights.Editable,
                UserId => AuthClaimRights.Invisible,
                _ => throw new NotImplementedException(),
            };

            set => throw new NotImplementedException();
        }

        public void Add(string key, AuthClaimRights value)
        {
            throw new NotImplementedException();
        }

        public bool ContainsKey(string key)
        {
            var a = this[key];
            return true;
        }

        public bool Remove(string key)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(string key, [MaybeNullWhen(false)] out AuthClaimRights value)
        {
            try
            {
                value = this[key];
            }
            catch
            {
                value = default;
                return false;
            }
            return true;
        }

        public void Add(KeyValuePair<string, AuthClaimRights> item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(KeyValuePair<string, AuthClaimRights> item)
        {
            try
            {
                var val = this[item.Key];
                return val == item.Value;
            }
            catch
            {
                return false;
            }
        }

        public void CopyTo(KeyValuePair<string, AuthClaimRights>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<string, AuthClaimRights> item)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<string, AuthClaimRights>> GetEnumerator()
            => new SeedAuthClaimNamesEnumerator(this);

        IEnumerator IEnumerable.GetEnumerator()
            => new SeedAuthClaimNamesEnumerator(this);
    }
}
