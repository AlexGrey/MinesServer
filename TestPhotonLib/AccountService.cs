using ExitGames.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace TestPhotonLib {
    public class AccountService {
        public static readonly AccountService Instance = new AccountService();

        public List<UnityClient> Clients { get; set; }
        private readonly ReaderWriterLockSlim readWriteLock;

        public AccountService() {
            Clients = new List<UnityClient>();
            readWriteLock = new ReaderWriterLockSlim();
        }

        public bool IsContain(string name) {
            using (ReadLock.TryEnter(this.readWriteLock, 1000)) {
                return Clients.Exists(n => n.CharacterName.Equals(name));
            }
        }

        public void AddClient(UnityClient client) {
            using (WriteLock.TryEnter(this.readWriteLock, 1000)) {
                Clients.Add(client);
            }
        }

        public void RemoveClient(UnityClient client) {
            using (WriteLock.TryEnter(this.readWriteLock, 1000)) {
                Clients.Remove(client);
            }
        }

        public int GetAllClient() {
            return Clients.Count;
        }

        ~AccountService() {
            readWriteLock.Dispose();
        }
    }
}
