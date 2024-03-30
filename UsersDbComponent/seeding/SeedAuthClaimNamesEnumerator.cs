using AuthFrontend.seeds;
using System.Collections;
using UsersDbComponent.entities;

namespace UsersDbComponent.seeding
{
    public class SeedAuthClaimNamesEnumerator : IEnumerator<KeyValuePair<string, AuthClaimRights>>
    {
        private readonly SeedAuthClaimNames _dict;
        private IEnumerator<string> _keysEnumerator;

        public SeedAuthClaimNamesEnumerator(SeedAuthClaimNames dict)
        {
            _dict = dict;
            _keysEnumerator = _dict.Keys.GetEnumerator();
        }

        public KeyValuePair<string, AuthClaimRights> Current => new KeyValuePair<string, AuthClaimRights>(_keysEnumerator.Current, _dict[_keysEnumerator.Current]);

        object IEnumerator.Current => new KeyValuePair<string, AuthClaimRights>(_keysEnumerator.Current, _dict[_keysEnumerator.Current]);

        public void Dispose() => _keysEnumerator.Dispose();

        public bool MoveNext() => _keysEnumerator.MoveNext();

        public void Reset() => _keysEnumerator.Reset();
    }
}
