import { Component } from '@angular/core';
import { OAuthService, NullValidationHandler } from 'angular-oauth2-oidc';



//Beginning with angular-oauth2-oidc version 9, the JwksValidationHandler
//has been moved to an library of its own. If you need it for implementing
//OAuth2/OIDC **implicit flow**, please install it using npm:
//import { JwksValidationHandler } from 'angular-oauth2-oidc';
import { JwksValidationHandler } from 'angular-oauth2-oidc-jwks';


import { authConfig } from './auth.config';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'angularOpenidSimple';

  constructor(private oauthService: OAuthService) {

    this.ConfigureImplicitFlowAuthentication();

    //issues/4
    //this.ConfigureWithoutDiscovery();
  }


  private ConfigureImplicitFlowAuthentication() {

    this.oauthService.configure(authConfig);
    this.oauthService.tokenValidationHandler = new JwksValidationHandler();

    this.oauthService.loadDiscoveryDocumentAndTryLogin();

    // This method just tries to parse the token(s) within the url when
    // the auth-server redirects the user back to the web-app
    // It doesn't send the user the the login page
    //this.oauthService.tryLogin();

    this.oauthService.events.subscribe(e => {
      // tslint:disable-next-line:no-console
      console.debug('oauth/oidc event', e);
    });

    /*
        this.oauthService.configure(authConfig);
      
        this.oauthService.tokenValidationHandler = new JwksValidationHandler();
      
        this.oauthService.loadDiscoveryDocument().then(doc) => {
      this.oauthService.tryLogin()
        .catch(err => {
          console.error(err);
        })
        .then(() => {
          if(!this.oauthService.hasValidAccessToken()) {
            this.oauthService.initImplicitFlow()
          }
        });
      });
      */
  }


  private ConfigureWithoutDiscovery() {

    this.oauthService.configure(authConfig);

    //TODO: No configurar validacion token
    this.oauthService.tokenValidationHandler = new NullValidationHandler();

    this.oauthService.tryLogin();


    this.oauthService.events.subscribe(e => {
      // tslint:disable-next-line:no-console
      console.debug('oauth/oidc event', e);
    });

  }


}
