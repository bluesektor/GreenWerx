import { Injectable } from '@angular/core';
import 'rxjs/add/operator/map';
import { Api } from '../api/api'; // '../api/api.service';
import { SessionService } from '../session.service';
import { Filter } from '../../models/index';

@Injectable({
    providedIn: 'root'
  })
export class StagedDataService  {

    constructor( private api: Api ) {
    }


    getTestData() {
        return this.api.invokeRequest('GET', 'treelist_test.json'  );
    }

    getStageDataWithMatches( type: string, filter: Filter) {
        return this.api.invokeRequest('POST', 'api/StageData/' + type + '/MatchPublished' , JSON.stringify(filter), );
    }

    publishStagedItems(items: any[], type: string) {

        return this.api.invokeRequest('POST', 'api/StageData/Publish/' + type , JSON.stringify(items), );
    }
}
