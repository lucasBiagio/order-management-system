import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { catchError, throwError } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (request, next) => {
  const snackBar = inject(MatSnackBar);

  return next(request).pipe(
    catchError((error: HttpErrorResponse) => {
      const message = getErrorMessage(error);

      snackBar.open(message, 'Fechar', {
        duration: 5000,
        horizontalPosition: 'right',
        verticalPosition: 'top',
        panelClass: ['error-snackbar']
    });

      return throwError(() => error);
    })
  );
};

function getErrorMessage(error: HttpErrorResponse): string {
  if (error.status === 0) {
    return 'Não foi possível conectar à API.';
  }

  if (typeof error.error === 'string' && error.error.trim()) {
    return error.error;
  }

  if (error.error?.message) {
    return error.error.message;
  }

  if (error.status === 400) {
    return 'Os dados informados são inválidos.';
  }

  if (error.status === 404) {
    return 'Recurso não encontrado.';
  }

  if (error.status >= 500) {
    return 'Ocorreu um erro interno no servidor.';
  }

  return 'Não foi possível concluir a operação.';
}