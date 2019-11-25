using Microsoft.AspNetCore.Components.Server;

namespace hephaestus.Services
{
    public class AuthStateService : ServerAuthenticationStateProvider
    {
        public AuthStateService(): base()
        {

        }
        public void NotifyStateChanged()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}
