using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public static class AuthenticationWrapper 
{
  public static AuthState AuthState { get; private set; } = AuthState.NotAuthenticated;

  public static async Task<AuthState>DoAuth(int maxRetries = 5) 
    {
        if(AuthState == AuthState.Authenticated) 
        {
            return AuthState;
        }

        if(AuthState == AuthState.Authenticating)
        {
            await Authenticating();
            return AuthState;
        }

        await SignInAnonymouslyAsync(maxRetries);


        return AuthState;

    }


    private static async Task<AuthState> Authenticating()
    {
        while (AuthState == AuthState.Authenticating || AuthState == AuthState.NotAuthenticated)
        {
            await Task.Delay(200);
        }

        return AuthState;
    }


    private static async Task SignInAnonymouslyAsync(int maxRetries)
    {
        AuthState = AuthState.Authenticating;

        int retries = 0;
        while (AuthState == AuthState.Authenticating && retries < maxRetries)
        {

            try 
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

                if (AuthenticationService.Instance.IsSignedIn && AuthenticationService.Instance.IsAuthorized)
                {
                    AuthState = AuthState.Authenticated;
                    break;
                }
            }
            catch(AuthenticationException authException) 
            {
                Debug.LogError($"Authentication failed: {authException.Message}");
                AuthState = AuthState.Error;
            }

            catch(RequestFailedException RequestException) 
            {
                Debug.LogError($"Request failed: {RequestException.Message}");
                AuthState = AuthState.Error;
            }
        
            retries++;
            await Task.Delay(1000);
        }

        if(AuthState != AuthState.Authenticated)
        {
            AuthState = AuthState.TimeOut;
            Debug.LogError("Authentication failed after maximum retries.");
        }


    }


}


public enum AuthState 
{
    NotAuthenticated,
    Authenticating,
    Authenticated,
    Error,
    TimeOut,
}


