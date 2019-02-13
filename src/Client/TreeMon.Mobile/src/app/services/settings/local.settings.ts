import { Injectable } from '@angular/core';
import { Storage } from '@ionic/storage';

/**
 * TODO implement AppSettings and LocalSettings in SettingsService.
 * AppSettings reads from assets/data/enviroment.local.json these should be default values for resetting the app or initial install etc.
 * LocalSettings for saving locally if network isn't available or member is not logged in.
 * SettingsService loads and saves settings from the server. Settings in the service should be lower level
 * options like api url whereas what to load and theme should be in the members profile.
 * A simple settings/config class for storing key/value pairs with persistence.
 */
@Injectable({
  providedIn: 'root'
})
export class LocalSettings {

  static readonly HasSeenTutorial = 'HAS_SEEN_TUTORIAL';
  static readonly HasLoggedIn = 'HAS_LOGGED_IN';
  static readonly UserName = 'USERNAME';
  static readonly SessionToken = 'SESSION_TOKEN';
  static readonly SessionData = 'SESSION_DATA'; // stores the session.ts
  static readonly Theme = 'THEME';
  static readonly ViewType = 'VIEW_TYPE'; // Used in home page for loading the data


  private SETTINGS_KEY = 'defaults';

  settings: any;

  _defaults: any;
  _readyPromise: Promise<any>;

  constructor(
    public storage: Storage
    ) {  }

  load( settingsKey ) {
    if (settingsKey === undefined || settingsKey === '' || settingsKey === '-') {
      settingsKey = 'default';
    }
    console.log('settings.load.settingsKey:', settingsKey);

    this.SETTINGS_KEY = settingsKey;

    return this.storage.get(this.SETTINGS_KEY).then((value) => {
      if (value) {
        this.settings = value;
        return this._mergeDefaults(this._defaults);
      } else {
        return this.setAll(this._defaults).then((val) => {
          this.settings = val;
        });
      }
    });

  }

  _mergeDefaults(defaults: any) {
    for (const k in defaults) {
      if (!(k in this.settings)) {
        this.settings[k] = defaults[k];
      }
    }
    return this.setAll(this.settings);
  }

  merge(settings: any) {
    for (const k in settings) {
      if (settings[k]) {
        this.settings[k] = settings[k];
      }
    }
    return this.save();
  }

  setValue(key: string, value: any) {
    this.settings[key] = value;
    return this.storage.set(this.SETTINGS_KEY, this.settings);
  }

  setAll(value: any) {
   return this.storage.set(this.SETTINGS_KEY, value);
  }

  getValue(key: string, defaultValue: string) {
    console.log('setting.ts getValue key:', key);
    return this.storage.get(this.SETTINGS_KEY)
      .then(settings => {
        console.log('setting.ts getValue settings:', settings);
        if (settings === null) {
          return defaultValue;
        }
        return settings[key];
      });
  }

  save() {
    return this.setAll(this.settings);
  }

  get allSettings() {
    return this.settings;
  }
}
