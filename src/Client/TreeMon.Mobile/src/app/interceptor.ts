import {
    HTTP_INTERCEPTORS, HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor,
    HttpRequest
} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {Observable} from 'rxjs/Observable';
import {_throw} from 'rxjs/observable/throw';
import {Router} from '@angular/router';
import {EmptyObservable} from 'rxjs/observable/EmptyObservable';
import { catchError } from 'rxjs/operators';

@Injectable()
export class HttpErrorInterceptor implements HttpInterceptor {
    constructor(private router: Router) {
        console.log('interceptor.ts constructor');
    }

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        console.log('interceptor.ts intercept');
        return next.handle(req)
        .pipe( catchError(error => {
            if (error instanceof HttpErrorResponse && error.status === 404 ) {
                this.router.navigateByUrl('/not-found', {replaceUrl: true});
            } else {
                return _throw(error);
            }
        }) );


    /*     request =  this.http.get(url,
            { headers: { 'Content-Type': 'application/json', 'Authorization': 'Bearer ' + Api.authToken}}
            )
            .pipe(tap(_ =>
                    console.log('api.ts get url:', url))
                    // ,catchError(this.handleError(url))
                    );


        return next.handle(req)
        .catch(error => {
            if (error instanceof HttpErrorResponse && error.status === 404) {
                this.router.navigateByUrl('/not-found', {replaceUrl: true});

                return new EmptyObservable();
            }
            else
                return _throw(error);
        });



       // original
        return next.handle(req)
            .catch(error => {
                if (error instanceof HttpErrorResponse && error.status === 404) {
                    this.router.navigateByUrl('/not-found', {replaceUrl: true});

                    return new EmptyObservable();
                }
                else
                    return _throw(error);
            });
            */
    }

}

export const HttpErrorInterceptorProvider = {
    provide: HTTP_INTERCEPTORS,
    useClass: HttpErrorInterceptor,
    multi: true,
};
