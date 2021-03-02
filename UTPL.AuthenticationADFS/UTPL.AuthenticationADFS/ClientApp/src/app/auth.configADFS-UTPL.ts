import { AuthConfig } from 'angular-oauth2-oidc';
import { AppConfiguration } from "read-appsettings-json";

export const authConfig: AuthConfig = {

  //ADFS UTPL
  // Url of the Identity Provider
  //issuer: 'https://sts.utpl.edu.ec/adfs',
  issuer: AppConfiguration.Setting().JwtConfig.issuer,

  // The SPA's id. The SPA is registerd with this id at the auth-server
  //clientId: 'aadc7236-2851-4499-b24f-04983f2c8eae',
  clientId: AppConfiguration.Setting().JwtConfig.clientId,

  /**
  * The auth server's endpoint that allows to log
  * the user in when using implicit flow.
  */
  //loginUrl: 'https://sts.utpl.edu.ec/adfs/oauth2/authorize/',
  loginUrl: AppConfiguration.Setting().JwtConfig.loginUrl,

  /**
   * Url of the token endpoint as defined by OpenId Connect and OAuth 2.
   */
  //tokenEndpoint: 'https://sts.utpl.edu.ec/adfs/oauth2/token/',
  tokenEndpoint: AppConfiguration.Setting().JwtConfig.tokenEndpoint,

  /**
  * Url of the userinfo endpoint as defined by OpenId Connect.
  */
  //userinfoEndpoint: 'https://sts.utpl.edu.ec/adfs/userinfo',
  userinfoEndpoint: AppConfiguration.Setting().JwtConfig.userinfoEndpoint,

  /**
   * The logout url.
   */
  //logoutUrl: 'https://sts.utpl.edu.ec/adfs/oauth2/logout',
  logoutUrl: AppConfiguration.Setting().JwtConfig.logoutUrl,

  // URL of the SPA to redirect the user to after login
  //redirectUri: window.location.origin + '/index.html',
  redirectUri: window.location.origin,



  /**
 * JSON Web Key Set (https://tools.ietf.org/html/rfc7517)
 * with keys used to validate received id_tokens.
 * This is taken out of the disovery document. Can be set manually too.
 */
  // public jwks?: object = null;
  jwks: {
    keys: [
      {
        "kty": "RSA",
        "use": "sig",
        "alg": "RS256",
        "kid": "EKeI_CD5KCS6U6NrQLTw6GkspeI",
        "x5t": "EKeI_CD5KCS6U6NrQLTw6GkspeI",
        "n": "i12RuZULRpoUAk6PsJ31dGxy4cPhr8vQ-bPolvh9oVzhb2A3C_pucpionCBUEBrFc9jAvgrcr2UEq3PaMWHcQcQrS97Ps2ZSUYQby63WVfS9i_Dn52mqXaq2m-d1NOpWEpPeaTqqWwvO0x2xrh9qauWGTPx4UA0voLoc58ygf8p5e7CxgoI-ZmJ15It4CadqcOwHH7lHsWBK48IONXXpaiYVp1cCXJ3PuA80fvzTCxCNsaJ0PlNN4DyKSwfwp9jR4ZXEOh7fUAMRLU7J1QH3sEfQ8uU0PAyAOZQevJzzH3mS_Nwen1b_ZCKjLX8ZXF5FF9wDaBqzMptcoHoH2Jhm2w",
        "e": "AQAB",
        "x5c": [
          "MIIC2jCCAcKgAwIBAgIQY+gRqjzM0K5FFSGcZGhrMjANBgkqhkiG9w0BAQsFADApMScwJQYDVQQDEx5BREZTIFNpZ25pbmcgLSBzdHMudXRwbC5lZHUuZWMwHhcNMjAwMjE2MDQwNzEwWhcNMjUwMjE2MDQwNzEwWjApMScwJQYDVQQDEx5BREZTIFNpZ25pbmcgLSBzdHMudXRwbC5lZHUuZWMwggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIBAQCLXZG5lQtGmhQCTo+wnfV0bHLhw+Gvy9D5s+iW+H2hXOFvYDcL+m5ymKicIFQQGsVz2MC+CtyvZQSrc9oxYdxBxCtL3s+zZlJRhBvLrdZV9L2L8Ofnaapdqrab53U06lYSk95pOqpbC87THbGuH2pq5YZM/HhQDS+guhznzKB/ynl7sLGCgj5mYnXki3gJp2pw7AcfuUexYErjwg41delqJhWnVwJcnc+4DzR+/NMLEI2xonQ+U03gPIpLB/Cn2NHhlcQ6Ht9QAxEtTsnVAfewR9Dy5TQ8DIA5lB68nPMfeZL83B6fVv9kIqMtfxlcXkUX3ANoGrMym1ygegfYmGbbAgMBAAEwDQYJKoZIhvcNAQELBQADggEBAExvhOB08sOQHCT5EA9z6bOpdgZqWN5r/VXdlu/rP5F9Tk/vq7mGcB3CSaMnWzUBI6/NDGpd6f8AjlbVUn1/4i/7E416juq2WTC1KjMkx0fbQ3R5q38pBF4xuGvsbg9ophzVh/cdetv7/LH9G7xlfQOm74noyujCgWgCeiTpAfld5CKV21fT8y3ahvRZDT+Vdm1FneC5eVOfkV8NyN3E6rzQ38X+5FmDi+M6ST7M5sPb+r5TqGqNjF7Ue0rr/Y97bfxJmzWEUUqsBmB0lL+eiOVRDkQXEcJCFadld4DNNRP/3VvT2IpTsbNTf0SDuTDwTuiMwTr+iSUa76kWbnZr+n8="
        ]
      }
    ]
  },

  // set the scope for the permissions the client should request
  // The first four are defined by OIDC.
  // Important: Request offline_access to get a refresh token
  // The api scope is a usecase specific one
  //AADSTS650053: The application 'Verificar-Proveedor-OpenId' asked for scope 'api' that doesn't exist on the resource '00000003-0000-0000-c000-000000000000'. Contact the app vendor.
  //scope: 'openid profile email API_Get',
  scope: AppConfiguration.Setting().JwtConfig.scope,


  /**
  * Defines whether additional debug information should
  * be shown at the console. Note that in certain browsers
  * the verbosity of the console needs to be explicitly set
  * to include Debug level messages.
  */
  //showDebugInformation: true,
  showDebugInformation: AppConfiguration.Setting().JwtConfig.showDebugInformation,

  /**
  * Defines whether every url provided by the discovery
  * document has to start with the issuer's url.
  */
  // turn off validation that discovery document endpoints start with the issuer url defined above
  //strictDiscoveryDocumentValidation: false
  strictDiscoveryDocumentValidation: AppConfiguration.Setting().JwtConfig.strictDiscoveryDocumentValidation
}
