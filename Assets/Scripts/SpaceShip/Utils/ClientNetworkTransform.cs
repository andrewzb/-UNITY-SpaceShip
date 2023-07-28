using Unity.Netcode.Components;

namespace SpaceShip.Utils
{
    public class ClientNetworkTransform : NetworkTransform
    {
        protected override bool OnIsServerAuthoritative()
        {
            return false;
        }
    }
}