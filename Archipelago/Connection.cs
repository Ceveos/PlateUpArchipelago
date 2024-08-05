using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using KitchenArchipelago.Persistence;
using System;

namespace KitchenArchipelago.Archipelago
{
    public class Connection
    {
        public bool Connected { get; private set; } = false;
        public ArchipelagoSession Session { get; private set; }
        public event EventHandler OnConnected;
        public event EventHandler OnDisconnected;
        public event EventHandler OnCheck;
        public event EventHandler OnItemReceived;

        private static Connection _instance;
        public static Connection Instance
        {
            get
            {
                return _instance ??= new Connection();
            }
        }

        public async void Connect()
        {
            if (!Settings.Enabled)
            {
                KitchenArchipelago.Logger.LogError("Attempted to connect to Archipelago with non-enabled profile");
                return;
            }

            LoginResult result;
            string server = Settings.Get<string>(ProfileConfig.Host);
            string user = Settings.Get<string>(ProfileConfig.User);
            string password = Settings.Get<string>(ProfileConfig.Password);

            Session = ArchipelagoSessionFactory.CreateSession(server);

            try
            {
                result = await Session.LoginAsync("Plate-Up", user, ItemsHandlingFlags.AllItems, password: password);
            }
            catch (Exception e)
            {
                result = new LoginFailure(e.GetBaseException().Message);
            }

            if (!result.Successful)
            {
                LoginFailure failure = (LoginFailure)result;
                string errorMessage = $"Failed to Connect to {server} as {user}:";
                foreach (string error in failure.Errors)
                {
                    errorMessage += $"\n    {error}";
                }
                foreach (ConnectionRefusedError error in failure.ErrorCodes)
                {
                    errorMessage += $"\n    {error}";
                }

                KitchenArchipelago.Logger.LogError(errorMessage);

                OnDisconnected?.Invoke(this, null);
                return; // Did not connect, show the user the contents of `errorMessage`
            }

            // Successfully connected, `ArchipelagoSession` (assume statically defined as `session` from now on) can now be used to interact with the server and the returned `LoginSuccessful` contains some useful information about the initial connection (e.g. a copy of the slot data as `loginSuccess.SlotData`)
            var loginSuccess = (LoginSuccessful)result;
            OnConnected?.Invoke(this, null);
        }

        public async void Disconnect()
        {
            await Session.Socket.DisconnectAsync();
            Connected = false;
            OnDisconnected?.Invoke(this, null);
        }

    }
}
