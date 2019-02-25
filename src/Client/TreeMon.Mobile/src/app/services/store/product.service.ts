import {  Injectable, OnInit } from '@angular/core';
import { Observable, of, throwError } from 'rxjs';
import { HttpClient, HttpHeaders, HttpErrorResponse } from '@angular/common/http';
import { catchError, tap, map } from 'rxjs/operators';
import { Product } from './product';

const httpOptions = {
    headers: new HttpHeaders({'Content-Type': 'application/json'})
  };
  const apiUrl = 'https://localhost:44318/api/v1/products';

@Injectable({
    providedIn: 'root'
  })
export class ProductService {

    constructor(private http: HttpClient) { }

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

      addProduct (newProduct): Observable<Product> {
        return this.http.post<Product>(apiUrl, newProduct, httpOptions).pipe(
          tap((product: Product) => console.log(`added product w/ id=${product.id}`)),
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
}
