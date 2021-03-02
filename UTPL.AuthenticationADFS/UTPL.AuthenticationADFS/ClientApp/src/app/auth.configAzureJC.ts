import { AuthConfig } from 'angular-oauth2-oidc';

export const authConfig: AuthConfig = {
  // Url of the Identity Provider
  issuer: 'https://login.microsoftonline.com/6eeb49aa-436d-43e6-becd-bbdf79e5077d/v2.0',
  // URL of the SPA to redirect the user to after login
  redirectUri: window.location.origin,
  // The SPA's id. The SPA is registerd with this id at the auth-server
  clientId: 'bd1009ef-b2fb-4821-898d-63a1a3d9de33',
  // set the scope for the permissions the client should request
  scope: 'openid profile email',
  showDebugInformation: true,
  // turn off validation that discovery document endpoints start with the issuer url defined above
  strictDiscoveryDocumentValidation: false
}
