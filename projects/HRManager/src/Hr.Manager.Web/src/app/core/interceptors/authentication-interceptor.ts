import { HttpInterceptorFn, HttpRequest } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthenticationService } from '../services/authentication.service';
import { mergeMap } from 'rxjs';

export const authenticationInterceptor: HttpInterceptorFn = (req, next) => {
  const authenticationService = inject(AuthenticationService);

  return authenticationService
    .getAccessToken()
    .pipe(mergeMap(token => {
      const authenticatedRequest = authenticate(req, token);
      return next(authenticatedRequest);
    }))
};

function authenticate<T>(request: HttpRequest<T>, token: string): HttpRequest<T> {
  return request.clone(
    {
      headers: request.headers.set('Authorization', `Bearer ${token}`)
    });
}
