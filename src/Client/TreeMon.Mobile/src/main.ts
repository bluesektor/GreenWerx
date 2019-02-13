import 'hammerjs';
import { enableProdMode } from '@angular/core';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';

import { AppModule } from './app/app.module';
import { environment } from './environments/environment';

if (environment.production) {
  enableProdMode();
  // this removes console logging..
  // if (window) { window.console.log = function() {};  }
}

platformBrowserDynamic().bootstrapModule(AppModule).then(ref => {

  // Otherwise, log the boot error
}).catch(err => console.error(err));
