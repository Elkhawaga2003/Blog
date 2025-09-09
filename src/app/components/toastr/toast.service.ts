import {
  Injectable,
  ApplicationRef,
  ComponentRef,
  Injector,
} from '@angular/core';
import { ToastrComponent } from '../toastr/toastr.component';
import { createComponent } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ToastService {
  constructor(private injector: Injector, private appRef: ApplicationRef) {}

  show(message: string, type: 'success' | 'error' | 'info' = 'success') {
    const toastRef: ComponentRef<ToastrComponent> = createComponent(
      ToastrComponent,
      {
        environmentInjector: this.appRef.injector,
      }
    );

    toastRef.instance.message = message;
    toastRef.instance.type = type;

    this.appRef.attachView(toastRef.hostView);
    const domElem = (toastRef.hostView as any).rootNodes[0] as HTMLElement;
    document.body.appendChild(domElem);

    setTimeout(() => {
      this.appRef.detachView(toastRef.hostView);
      toastRef.destroy();
    }, 3000);
  }
}
