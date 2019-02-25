import { HttpClient, HttpHeaders, HttpErrorResponse } from '@angular/common/http';
import {  Injectable, OnInit } from '@angular/core';
import 'rxjs/add/operator/map';
import { Observable, of as observableOf, throwError } from 'rxjs';
import { Events } from '@ionic/angular';
import { environment } from '../../../environments/environment';
import { timeoutWith } from 'rxjs/operators';
import 'rxjs/add/observable/throw';
import { catchError, tap, map } from 'rxjs/operators';
 import { AppSetting} from '../../app.settings';

@Injectable({
    providedIn: 'root'
  })
export class Api implements OnInit {
    static authToken: string;
    url =            'https://localhost:44318/'; //  'https://dev.treemon.org/'; //

  constructor(protected http: HttpClient,
    public events: Events,
    private appSettings: AppSetting
    ) {

    }

    ngOnInit() {
        this.url = environment.baseUrl;
        console.log('api.ts ngOnInit this.url:', this.url);
    }

  invokeRequest(verb: string, endPoint: string, parameters?: string  ): Observable<Object> {

    const url = this.url +  endPoint;

    console.log('invoke:', url);

    let request = null;

    switch (verb.toLowerCase()) {
        case 'get':
            request =  this.http.get(url,
                { headers: { 'Content-Type': 'application/json', 'Authorization': 'Bearer ' + Api.authToken}})
                .pipe(tap(_ =>
                        console.log('api.ts get url:', url))
                        // ,catchError(this.handleError(url))
                        );
                /*
                return this.http.get<Product>(url).pipe(
                    tap(_ => console.log(`fetched product id=${id}`)),
                    catchError(this.handleError<Product>(`getProduct id=${id}`))
                  );
*/
                break;
        case 'post':
            request =   this.http.post(url, parameters,
                { headers: {'Content-Type': 'application/json', 'Authorization': 'Bearer ' + Api.authToken}})
                .pipe(tap(_ =>
                    console.log('api.ts get url:', url))
                   // ,catchError(this.handleError(url))
                    );
                break;
        case 'patch':
            request =   this.http.patch(url, parameters,
                { headers: {'Content-Type': 'application/json', 'Authorization': 'Bearer ' + Api.authToken}});
                break;
        case 'put':
            request =   this.http.put(url, parameters,
                { headers: {'Content-Type': 'application/json', 'Authorization': 'Bearer ' + Api.authToken}});
                break;
        case 'delete':
            request =   this.http.delete(url,
                { headers: {'Content-Type': 'application/json', 'Authorization': 'Bearer ' + Api.authToken }});
                break;
    }

    if (!request) {
        this.events.publish('api:err', 401 , 'Bad request');
        return  observableOf(false);
    }
    return request;
    // .pipe(        timeoutWith(5000, Observable.throw(new Error('Failed call api.'))
    //  )).catch(this.requestTimedOut);
  }

  requestTimedOut() {
      console.log('api.ts requestTimedOut <<XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX');
  }

  uploadForm(endPoint: string, formData: FormData) {
    const url = this.url +  endPoint;

    return Observable.create(observer => {
        const xhr: XMLHttpRequest = new XMLHttpRequest();

        xhr.onreadystatechange = () => {
            if (xhr.readyState === 4) {
                if (xhr.status === 200) {
                    observer.next(JSON.parse(xhr.response));
                    observer.complete();
                } else {
                    observer.error(xhr.response);

                }
            }
        };
        xhr.open('POST', url, true);
        xhr.setRequestHeader('Authorization', 'Bearer ' + Api.authToken);
        xhr.send(formData);
    });
  }

  uploadFile(endPoint: string, files: File[]) {
    const url = this.url +  endPoint;

    return Observable.create(observer => {
        const xhr: XMLHttpRequest = new XMLHttpRequest();

        xhr.onreadystatechange = () => {
            if (xhr.readyState === 4) {
                if (xhr.status === 200) {
                    observer.next(JSON.parse(xhr.response));
                    observer.complete();
                } else {
                    observer.error(xhr.response);

                }
            }
        };

        for (let i = 0; i < files.length; i++) {
            const formData = new FormData();
            console.log('appending form data:', files[i]);
            if (i === 0) {
                formData.append('file', files[i]);
                console.log('setting defaultFile true');
                formData.append('defaultFile', 'true');
            } else {
                formData.append('file', files[i]);
                console.log('setting defaultFile false');
                formData.append('defaultFile', 'false');
            }

            xhr.open('POST', url, true);
            xhr.setRequestHeader('Authorization', 'Bearer ' + Api.authToken);
            xhr.send(formData);
        }
    });
  }
 private handleError(arg: string) {
    console.log('api.ts handlError arg:', arg);
    return Observable.create(observer => {
                    observer.error(arg); // todo make this serviceresponse
    });
    /*
        return Observable.create(observer => {
        const xhr: XMLHttpRequest = new XMLHttpRequest();

        xhr.onreadystatechange = () => {
            if (xhr.readyState === 4) {
                if (xhr.status === 200) {
                    observer.next(JSON.parse(xhr.response));
                    observer.complete();
                } else {
                    observer.error(xhr.response);

                }
            }
        };
        xhr.open('POST', url, true);
        xhr.setRequestHeader('Authorization', 'Bearer ' + Api.authToken);
        xhr.send(formData);
    });
    */
 }
/*
  private handleError<T> (operation = 'operation', result?: T) {
    return (error: any): Observable<T> => {

      // TODO: send the error to remote logging infrastructure
      console.error(error); // log to console instead

      // Let the app keep running by returning an empty result.
      return of(result as T);
    };
  }
  getProduct(id): Observable<Product> {
  const url = `${apiUrl}/${id}`;
  return this.http.get<Product>(url).pipe(
    tap(_ => console.log(`fetched product id=${id}`)),
    catchError(this.handleError<Product>(`getProduct id=${id}`))
  );
}

addProduct (product): Observable<Product> {
  return this.http.post<Product>(apiUrl, product, httpOptions).pipe(
    tap((product: Product) => console.log(`added product w/ id=${product._id}`)),
    catchError(this.handleError<Product>('addProduct'))
  );
}

updateProduct (id, product): Observable<any> {
  const url = `${apiUrl}/${id}`;
  return this.http.put(url, product, httpOptions).pipe(
    tap(_ => console.log(`updated product id=${id}`)),
    catchError(this.handleError<any>('updateProduct'))
  );
}

deleteProduct (id): Observable<Product> {
  const url = `${apiUrl}/${id}`;

  return this.http.delete<Product>(url, httpOptions).pipe(
    tap(_ => console.log(`deleted product id=${id}`)),
    catchError(this.handleError<Product>('deleteProduct'))
  );
}
  */
}


