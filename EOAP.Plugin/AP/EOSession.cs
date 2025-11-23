using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;

namespace EOAP.Plugin.AP
{
    public class EOSession
    {
        public string ErrorMessage { get; private set; }
        public bool Connected { get; private set; }
        public ArchipelagoSession Session { get; private set; }

        public void Start(string slotName, string hostname, int port)
        {
            if (Connected)
                return;

            ErrorMessage = string.Empty;
            Session = ArchipelagoSessionFactory.CreateSession(hostname, port);

            System.Version worldVersion = new System.Version(0, 6, 4);
            LoginResult result;
            try
            {
                result = Session.TryConnectAndLogin("Etrian Odyssey HD", slotName, ItemsHandlingFlags.AllItems, requestSlotData: true, version: worldVersion);
            }
            catch(System.Exception e)
            {
                ErrorMessage += e.Message + "\n";
                result = new LoginFailure("Exception"); // no idea why an exception message should be passed.
            }

            if (!result.Successful)
            {
                LoginFailure failure = (LoginFailure)result; // seriously wtf
                for (int i = 0; i < failure.Errors.Length; ++i)
                {
                    ErrorMessage += failure.Errors[i];
                }
                GDebug.Log(ErrorMessage);
            }
            else
            {
                LoginSuccessful success = (LoginSuccessful)result; // devs really need to study polymorphism use cases
                GDebug.Log($"Connected to AP Slot {success.Slot}");
            }

            Connected = result.Successful;
        }
    }
}
