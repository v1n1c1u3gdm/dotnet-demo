import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { NavigationExtras, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { catchError } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {

  const router = inject(Router)
  const toastr = inject(ToastrService);

  return next(req)
    .pipe(
      catchError(e => {
        if (e) { //event of error exists
          switch (e.status) {
            case 400: //bad request
              if (e.error.errors) { //it's a list of validation errors, that must be presented to the user
                const modalStateErrors = [];

                for (const key in e.error.errors) {
                  if (e.error.errors[key])
                    modalStateErrors.push(e.error.errors[key]);
                }

                throw modalStateErrors.flat();
              }
              else
                toastr.error(e.error, e.status); //Something a bit simpler
              break;

            case 401: //Unauthorized
              toastr.error("Ação não autorizada", e.status);
              break;
            case 403: //Forbiden
            case 404: //Not found
              var navExtras: NavigationExtras = { state: e.error };
              router.navigateByUrl("/not-found", navExtras);
              break;
            case 405: //Method not allowed
              toastr.info("Aparentemente você está com uma versão antiga do nosso sistema. Por favor limpe seu cache e cookies e volte novamente.", "Atualizamos!");
              break;
            case 407: //Proxy authentication required
              toastr.warning("Essa funcionalidade ainda não está disponível para você. Entre em contato conosco via...", "Ops! :-/");
              break;
            case 408: //Request timeout
              toastr.info("Estamos demorando um pouco mais que o normal. Por favor tente novamente em breve", "Ops! ⏲");
              break;
            case 429: //Too many requests
              toastr.warning("Muitas solicitações disparadas a partir do seu computador. Estamos sendo recusados", "Calma robozinho!");
              break;
            case 500: //internal server error
            default: //WHAAAAAAAAAT?
              toastr.error("Diaxo! Uma falha inesperada ocorreu. Acabamos de alertar nossa equipe!", "Poxa vida! :-{");
              router.navigateByUrl("/unexpected")
              console.log(e);
              break;
          }
        }
        throw e;
      }
      ));
}
