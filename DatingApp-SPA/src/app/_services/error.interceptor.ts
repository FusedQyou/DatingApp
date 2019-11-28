import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpErrorResponse, HTTP_INTERCEPTORS } from '@angular/common/http';
import { catchError } from 'rxjs/operators';
import { throwError } from 'rxjs';

// Due to the nature of .NET Core's API if it comes to validation with EntityFramework combined,
// it is possible to have many different types of errors returned.
//
// This service makes sure that the errors are caught when the response is received,
// and handles these to a proper return element which the application can use.

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
    intercept(
        req: import('@angular/common/http').HttpRequest<any>,
        next: import('@angular/common/http').HttpHandler
    ): import('rxjs').Observable<import('@angular/common/http').HttpEvent<any>> {
        return next.handle(req).pipe(
            catchError(error => {

                // Handle 401 error
                if (error.status === 401) {
                    return throwError(error.statusText);
                }

                // Handle instance of HttpErrorResponse (should be the rest)
                if (error instanceof HttpErrorResponse) {

                    // Application error (api returned exception)
                    const applicationError = error.headers.get('Application-Error');
                    if (applicationError) {
                        return throwError(applicationError);
                    }

                    // Server error
                    const serverError = error.error;

                    // Validation error
                    let modalStateErrors = '';
                    if (serverError.errors && typeof serverError.errors === 'object') {

                        // Loop through all validation errors
                        for (const key in serverError.errors) {
                            if (serverError.errors[key]) {
                                modalStateErrors += serverError.errors[key] + '\n';
                            }
                        }
                    }

                    // Try to throw either the server error or the validation error
                    return throwError(modalStateErrors || error.statusText || 'Server encountered an error.');
                }
            })
        );
    }
}

export const ErrorInterceptorProvider = {
    provide: HTTP_INTERCEPTORS,
    useClass: ErrorInterceptor,
    multi: true
};
