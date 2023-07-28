using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Authentication;

#if UNITY_EDITOR
using ParrelSync;
#endif

namespace SpaceShip.Services
{
    public static class Authentication
    {
        public static string PlayerId { get; private set; }
        public static bool  IsAuthorized => AuthenticationService.Instance?.IsAuthorized ?? false;
        
        public static async Task Login()
        {
            if (UnityServices.State == ServicesInitializationState.Uninitialized)
            {
                var options = new InitializationOptions();


#if UNITY_EDITOR
                // for parallel sync
                if (ClonesManager.IsClone())
                {
                    options.SetProfile(ClonesManager.GetArgument());
                }
                else
                {
                    options.SetProfile("Primary");
                }

#endif
                await UnityServices.InitializeAsync(options);
            }

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                PlayerId = AuthenticationService.Instance.PlayerId;
            }
        }

        public static void Logout()
        {
            AuthenticationService.Instance.SignOut();
        }
    }
}