import { HttpClient } from '@angular/common/http';

import { Injectable , Renderer2, RendererFactory2  } from '@angular/core';
import {   OnInit  } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

@Injectable({
    providedIn: 'root'
  })
export class AppSetting implements  OnInit {
    manifest: any;
    settings: any;
    private renderer: Renderer2;

    constructor(private http: HttpClient,
      private route: ActivatedRoute,
      private router: Router,
           private rendererFactory: RendererFactory2
      ) {
        console.log('APP.SETTINGS.TS initialize');
        this.renderer = rendererFactory.createRenderer(null, null);

        if (!this.manifest) {
          this.http
            .get('assets/manifest.json').subscribe((res) => {
                this.manifest = res;
                console.log('APP.SETTINGS.TS manifest:', this.manifest);
                this.loadSettings();
            });
        }

        console.log( this.getURLParameter('validate'));
        console.log( this.getURLParameter('type'));
        const type =  this.getURLParameter('type');
        console.log( this.getURLParameter('operation'));
        const op = this.getURLParameter('operation');
        console.log( this.getURLParameter('code'));
        const code =  this.getURLParameter('code');

        if (this.getURLParameter('validate') === 'membership' ) {
          this.router.navigateByUrl('/membership/validate/type/' + type +
          '/operation/' + op + '/code/' + code);
        }
    }

    ngOnInit() { }

    getURLParameter(name) {
      return decodeURIComponent((new RegExp('[?|&]' +
      name + '=' + '([^&;]+?)(&|#|;|$)').exec(location.search) || [null, ''])[1].replace(/\+/g, '%20')) || null;
    }

    protected loadSettings( ) {
      console.log('APP.SETTINGS.TS initialize');

        if (!this.settings) {
            this.http
              .get('assets/data/environment.' + this.manifest.environment +  '.json').subscribe(res => {
                if (res) {
                  this.settings = res;
                  console.log('APP.SETTINGS.TS manifest.initialize this.settings:', this.settings);
                  if (this.settings !== undefined) {
                    this.renderer.addClass(document.body, this.settings.theme);
                  }
                }
          });
        } else if (this.settings !== undefined) {
          this.renderer.addClass(document.body, this.settings.theme);
        }
    }
}
