import {  Component,  ViewEncapsulation, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AlertController, ModalController } from '@ionic/angular';
import { SessionService } from '../../services/session.service';
import {InventoryService } from '../../services/store/inventory.service';
import {UserService} from '../../services/user/user.service';
import { DataImport, ServiceResult } from '../../models/index';
import { Events } from '@ionic/angular';
import { ObjectFunctions} from '../../common/object.functions';
import {StagedDataService} from '../../services/data/stageddata.service';
import * as moment from 'moment';
import {TreeNode} from 'primeng/api';
// import { TreeNode, TreeNode } from '@angular/router/src/utils/tree';

@Component({
  selector: 'page-admin',
  templateUrl: 'admin.html',
  styleUrls: ['./admin.scss'],
  encapsulation: ViewEncapsulation.None,
  providers: [InventoryService, UserService]
})

export class AdminPage implements  OnInit {

    selectedTab = 'event';
    dataImport:  DataImport[] = [];
    selectedItem: DataImport;
    treeNodes: TreeNode[];
    files1: TreeNode[];
    showSelectedItem = false;
    processing = false;
    queuedData: any[] = []; // Items added for saving/publishing.

  constructor(
    public alertCtrl: AlertController,
    public router: Router,
    private session: SessionService,
    private stagedDataService: StagedDataService,
    public messages: Events,
    public modalCtrl: ModalController ) {
    }

  ngOnInit() {
      this.onSegmentChange(); // load data
  }

  onSegmentChange() {
      this.processing = true;
    console.log('admin.ts onSegmentChange selectedTab:', this.selectedTab);
    this.dataImport = [];
    this.treeNodes = [];

            this.stagedDataService.getStageDataWithMatches(this.selectedTab, null).subscribe(sessionResponse => {
                this.processing = false;
                const data = sessionResponse as ServiceResult;
                console.log('admin.ts onLogin onSegmentChange.data :', data);
                if (data.Code !== 200) {
                  this.messages.publish('api:err', data);
                    return false;
                }
                if (ObjectFunctions.isValid(data.Result) === false) {
                    return;
                }

                this.dataImport = data.Result;
 /*
                for (let i = 0; i < this.dataImport.length; i++ ) {
                    if (this.isStageItemQueued(this.dataImport[i].StagedItem.UUID ) === true) {
                        continue;
                    }
                    const node = {
                        data: {
                            name:  this.dataImport[i].StagedItem.Name,
                            refCount: this.dataImport[i].ActiveItems.length,
                            UUID:  this.dataImport[i].StagedItem.UUID
                        },
                        leaf: false
                       // , children: []
                    };
                    for (let j = 0; j < this.dataImport[i].ActiveItems.length; j++) {
                        const child = {
                            data: {
                                name:  this.dataImport[i].ActiveItems[j].Name,
                                refCount: 0,
                                UUID:  this.dataImport[i].ActiveItems[j].UUID
                            },
                            leaf: false
                        };
                        node.children.push(child);

                            export interface TreeNode {
                                    data?: any;
                                    children?: TreeNode[];
                                    leaf?: boolean;
                                    expanded?: boolean;
                                }
                    }
                    this.treeNodes.push(node);
                } */
            }, (err) => {
              this.processing = false;
              this.messages.publish('service:err', err);
            });

   }

   queueStagedItem( item: any, privateEvent: boolean) {
       console.log('admin.ts queueStagedItem item:', item);
       for (let i = 0; i < this.dataImport.length; i++ ) {
            if (this.dataImport[i].StagedItem.UUID === item.UUID) {
                this.dataImport.splice(i, 1 );
                break;
            }
       }
      item.private = privateEvent;
       this.queuedData.push(item);
   }

   saveStagedItems() {
    this.processing = true;
    this.stagedDataService.publishStagedItems(this.queuedData, this.selectedTab).subscribe(sessionResponse => {
        this.processing = false;
        const data = sessionResponse as ServiceResult;
        console.log('admin.ts onLogin onSegmentChange.data :', data);
        if (data.Code !== 200) {
          this.messages.publish('api:err', data);
            return false;
        }
       // this.dataImport = [];
        this.queuedData = [];

    }, (err) => {
      this.processing = false;
      this.messages.publish('service:err', err);
    });
    //
   }
   // ui show hosts cbo/search
   // show if has published matches
   // display if published or not

   formatDate(eventDate: any, format: string): string {
    const date =  moment( eventDate).local();
    return date.format(format);
    }

    isValidDate(eventDate: any): boolean {
        const date =  moment( eventDate).local();

        if (date.year() === 1) {
            // console.log('admin.ts isValidDate FALSE date:', date);
            return false;
        }
        // console.log('admin.ts isValidDate TRUE date:', date);
        return true;
    }

    isStageItemQueued(uuid: string): boolean {
        let found = false;
        for (let i = 0; i <  this.queuedData.length; i++ ) {
                if (this.queuedData[i].UUID === uuid) {
                   found = true;
                   break;
                }
        }
        return found;
    }

    selectItem(stagedUUID: string) {
        this.showSelectedItem = false;
        console.log('admin.ts selectItem uuid:', stagedUUID);
        for (let i = 0; i <  this.dataImport.length; i++ ) {
            if (this.dataImport[i].StagedItem.UUID === stagedUUID) {
                this.selectedItem = this.dataImport[i];
                console.log('admin.ts selectItem  this.selectedItem:',  this.selectedItem);
                this.showSelectedItem = true;
               break;
            }
        }

    }
}
