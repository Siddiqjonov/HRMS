import { Injectable } from '@angular/core';
import Keycloak, { KeycloakConfig, KeycloakInitOptions } from 'keycloak-js';
import { from, map, Observable, of, switchMap } from 'rxjs';
import { environment } from '../../environments/environment';
import { UserRole } from '../models/roles.model';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {
  private keycloak?: Keycloak;

  get isAuthenticated(): boolean {
    return this.keycloak?.authenticated ?? false;
  }

  get userEmail(): string | undefined {
    return this.keycloak?.profile?.email;
  }

  public init(): Observable<any> {
    const authorityUrl = new URL(environment.authority);
    const keycloakConfig: KeycloakConfig = {
      url: authorityUrl.origin,
      realm: authorityUrl.pathname.split('/').filter(segment => segment != '').pop() ?? '',
      clientId: environment.clientId,
    };

    this.keycloak = new Keycloak(keycloakConfig);
    return this.initKeycloak(this.keycloak)
      .pipe(
        switchMap(isAuthenticated => this.loadUserProfile(isAuthenticated)
        )
      );
  }

  public getAccessToken(): Observable<string> {
    if (!this.keycloak)
      throw Error('Keycloak authentication service not initialized');

    return from(this.keycloak.updateToken(5))
      .pipe(map(() => this.keycloak!.token ?? ''));
  }

  public login(): void {
    if (!this.keycloak) {
      throw Error('Keycloak authentication service not initialized');
    }
    this.keycloak.login();
  }

  public logout(): void {
    if (!this.keycloak) {
      throw Error('Keycloak authentication service not initialized');
    }
    this.keycloak.logout();
  }

  private initKeycloak(keycloak: Keycloak): Observable<any> {
    const initOptions: KeycloakInitOptions =
    {
      onLoad: 'login-required',
      checkLoginIframe: !environment.production, // Disable in production to avoid timeout issues
      messageReceiveTimeout: environment.production ? 10000 : 5000, // Increase timeout in production if needed
    };

    return from(keycloak.init(initOptions));
  }

  public loadUserProfile(isAuthenticated: boolean): Observable<any> {
    return isAuthenticated ? from(this.keycloak!.loadUserProfile()) : of(undefined);
  }

  public hasRole(role: UserRole): boolean {
    if (!this.keycloak?.token) {
      return false;
    }

    const token = this.keycloak.tokenParsed;
    if (!token) {
      return false;
    }
    const roles = token['roles'] as string[] | undefined;
    return roles ? roles.includes(role) : false;
  }

  public getUserRoles(): UserRole[] {
    if (!this.keycloak?.tokenParsed) {
      return [];
    }

    const token = this.keycloak.tokenParsed as any;
    const roles = token['roles'] as string[] | undefined;

    if (!roles || roles.length === 0) {
      return [];
    }

    const validRoles = Object.values(UserRole);
    return roles
      .filter(role => validRoles.includes(role as UserRole))
      .map(role => role as UserRole);
  }
}
