using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Movies.Client.HttpHandler
{
    public class AuthenticationDelegatingHandler : DelegatingHandler
    {

        /*    private readonly IHttpClientFactory _httpClientFactory;
              private readonly ClientCredentialsTokenRequest _tokenRequest;

              public AuthenticationDelegatingHandler(IHttpClientFactory httpClientFactory, ClientCredentialsTokenRequest tokenRequest)
              {
                  _httpClientFactory = httpClientFactory;
                  _tokenRequest = tokenRequest;
              }
        */
        private IHttpContextAccessor _contextAccessor;

        public AuthenticationDelegatingHandler(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            /*  var httpClient = _httpClientFactory.CreateClient("IDPClient");
                var tokenResponse = await httpClient.RequestClientCredentialsTokenAsync(_tokenRequest);
                if(tokenResponse.IsError)
                {
                    throw new HttpRequestException("Something went wrong while requesting the access token");
                }
                request.SetBearerToken(tokenResponse.AccessToken);
            */
            var accessToken = await _contextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

            if(!string.IsNullOrEmpty(accessToken))
            {
                request.SetBearerToken(accessToken);
            }
            
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
