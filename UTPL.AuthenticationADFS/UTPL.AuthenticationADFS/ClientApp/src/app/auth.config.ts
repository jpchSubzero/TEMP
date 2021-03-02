import { AuthConfig } from 'angular-oauth2-oidc';
import { AppConfiguration } from "read-appsettings-json";

export const authConfig: AuthConfig = {
  redirectUri: window.location.origin,

  clientId: AppConfiguration.Setting().JwtConfig.clientId,                  //'fb5706d4-a03d-48b6-8294-cbb05e16bf07',
  loginUrl: AppConfiguration.Setting().JwtConfig.loginUrl,                  //'https://login.microsoftonline.com/6eeb49aa-436d-43e6-becd-bbdf79e5077d/oauth2/v2.0/authorize',
  issuer: AppConfiguration.Setting().JwtConfig.issuer,                      //'https://login.microsoftonline.com/6eeb49aa-436d-43e6-becd-bbdf79e5077d/v2.0',
  tokenEndpoint: AppConfiguration.Setting().JwtConfig.tokenEndpoint,        //'https://login.microsoftonline.com/6eeb49aa-436d-43e6-becd-bbdf79e5077d/oauth2/token',
  userinfoEndpoint: AppConfiguration.Setting().JwtConfig.userinfoEndpoint,  //'https://login.microsoftonline.com/6eeb49aa-436d-43e6-becd-bbdf79e5077d/oauth2/userinfo',
  logoutUrl: AppConfiguration.Setting().JwtConfig.logoutUrl,                //'https://login.microsoftonline.com/6eeb49aa-436d-43e6-becd-bbdf79e5077d/oauth2/logout',
  jwks: AppConfiguration.Setting().JwtConfig.jwks,
  scope: AppConfiguration.Setting().JwtConfig.scope,                        //'openid profile api://fb5706d4-a03d-48b6-8294-cbb05e16bf07/Users.Read.All',
  responseType: AppConfiguration.Setting().JwtConfig.responseType,          //'id_token token',
  showDebugInformation: AppConfiguration.Setting().JwtConfig.showDebugInformation,//true,
  strictDiscoveryDocumentValidation: AppConfiguration.Setting().JwtConfig.strictDiscoveryDocumentValidation,//false
}
