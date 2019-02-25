import { Injectable } from '@angular/core';
import { Events } from 'ionic-angular';
// import { Network } from '@ionic-native/network';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';
import { Observable } from 'rxjs/Observable';

export enum ConnectionStatus {
    Online,
    Offline
}

@Injectable()
export class NetworkService {

    public status: ConnectionStatus;
    private _status: BehaviorSubject<ConnectionStatus> = new BehaviorSubject(null);

    constructor(
        //   public network: Network, <= this was causing app to stop working.
        public events: Events
    ) {
        this.status = ConnectionStatus.Online;
    }

    public initializeNetworkEvents(): void {

        // OFFLINE
        // this.network.onDisconnect().subscribe(() => {
        //     if (this.status === ConnectionStatus.Online) {
        //         this.setStatus(ConnectionStatus.Offline);
        //      }
        //  });

        // ONLINE
        //   this.network.onConnect().subscribe(() => {
        //       if (this.status === ConnectionStatus.Offline) {
        //           this.setStatus(ConnectionStatus.Online);
        //       }
        //   });

    }

  //  public getNetworkType(): string {
  //      return this.network.type;
  //  }

    public getNetworkStatus(): Observable<ConnectionStatus> {
        return this._status.asObservable();
    }

    private setStatus(status: ConnectionStatus) {
        this.status = status;
        this._status.next(this.status);
    }
}


