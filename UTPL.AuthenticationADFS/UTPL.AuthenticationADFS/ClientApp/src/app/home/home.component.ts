import { Component, OnInit } from '@angular/core';
import { OAuthService } from 'angular-oauth2-oidc';

import { HttpClient, HttpHeaders, HttpErrorResponse, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';
import { Router } from '@angular/router';
import { throwError } from 'rxjs';

import 'rxjs/add/operator/do';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/switchMap';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent {

  constructor(private http: HttpClient, private oauthService: OAuthService, private router: Router) { }

  public message: string;
  public token: string;
  public idtoken: string;
  public Token = "";
  protected UrlService = window.location.origin + '/api/';

  public login() {
    this.oauthService.initLoginFlow();
  }

  public logoff() {
    this.oauthService.logOut();
//    window.location.href = 'http://login.microsoftonline.com/6eeb49aa-436d-43e6-becd-bbdf79e5077d/oauth2/v2.0/logout?wa=wsignoutcleanup1.0';
  }

  public get name() {
    const claims = this.oauthService.getIdentityClaims();
    if (!claims) {
      console.log('No claims - getIdentityClaims()');
      return null;
    }
    return claims['name'];
  }

  getPublicAreaMessage() {
    this.getPublicMessage()
      .subscribe(
        msg => this.message = msg,
        error => { console.log(error); alert("Error occured"); });
  }

  getPrivateAreaMessage() {
    this.getPrivateMessage()
      .subscribe(
        msg => this.message = msg,
        error => { console.log(error); alert("Error occured"); });
  }


  getPrivateMessage(): Observable<string> {
    const options = this.getAuthHeader();

    return this.http.get(this.UrlService + "Auth/private", { headers: options })
      .map((res): string => this.extractData(res))
      .catch(err => this.serviceError(err, true));
  }

  getPublicMessage(): Observable<string> {
    const options = this.getAuthHeader();
    console.log("options");
    console.log(options);
    //let headers = new HttpHeaders({ 'Content-Type': 'application/json' });

    return this.http.get(this.UrlService + "Auth/public", { headers: options })
      .map((res: string) => this.extractData(res))
      .catch(err => this.serviceError(err, true));

  }

  protected getAuthHeader(): HttpHeaders {

    this.Token = this.oauthService.getAccessToken();
    let headers = new HttpHeaders({ 'Content-Type': 'application/json', 'Authorization': `Bearer ${this.Token}` });

    return headers;
  }

  protected serviceError(error: HttpErrorResponse | any, redirectToErrorPageOn500: boolean) {
    let errMsg: string;

    if (error instanceof HttpErrorResponse && error.status !== 401 && error.status !== 403 && error.status !== 404) {
      const body = error.error.json() || '';
      const err = body.error || JSON.stringify(body);
      errMsg = `${error.status} - ${error.statusText || ''} ${err}`;
    }
    else {
      errMsg = error.message ? error.message : error.toString();
    }

    console.error(error);

    //return Observable.throw(error);
    return throwError(error);
  }

  protected extractData(response: any) {
    return response.message || {};
  }
}
