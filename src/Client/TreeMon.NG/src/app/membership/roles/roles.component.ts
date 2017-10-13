// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

import { Component, ViewChild, ViewEncapsulation, OnInit, Output, EventEmitter } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { MessageBoxesComponent } from '../../common/messageboxes.component';
import { BasicValidators } from '../../common/basicValidators';
import { SessionService } from '../../services/session.service';
import { PickListComponent } from '../../common/picklist.component';
import { Filter } from '../../models/filter';
import { Screen } from '../../models/screen';

import { AccordionModule } from 'primeng/primeng';
import { CheckboxModule } from 'primeng/primeng';
import { PickListModule } from 'primeng/primeng';

import { ConfirmDialogModule, ConfirmationService, GrowlModule } from 'primeng/primeng';

import { RoleService } from '../../services/roles.service';
import { Role } from '../../models/role';

// if roleWeight > x
//            show roles pick list
// else
//  show roles this user belongs to
//  option to leave role
//  except if role owner.Must be at least one role owner
//  add RoleOperation to tables and classes

@Component({
    templateUrl: './roles.component.html',
    styles: [`
.ui-picklist-source-controls {
  display: none !important;
}

.ui-picklist-target-controls {
  display: none !important;
}

.ui-picklist-listwrapper {
  width: 45% !important;
}
  `],
   // if you want to hide the side buttons for the picklist
   // you need to include this (along with the css above).
    encapsulation: ViewEncapsulation.None,
    providers: [RoleService, ConfirmationService, SessionService]

})
export class RolesComponent implements OnInit {

    loadingData = false;
    deletingData = false;
    selectedTab = 0;

     // ===--- Top Menu Bar ---===
     newRole = false;
     roles: any[];
     msgs: any[] = [];

    // ===--- Role Detail Tab (0) ---===
    roleDetail = new Role();
    formRoleDetail: FormGroup;

     // ===--- Role Users Tab (1) ---===
     roleNonMembers: Node[];
     roleMembers: Node[];
     @Output() onMoveToSource: EventEmitter<any> = new EventEmitter();
     @Output() onMoveToTarget: EventEmitter<any> = new EventEmitter();

    // ===--- Role Permissions Tab (2) ---===
    availablePermissions: Node[];
    selectedPermissions: Node[];
    filterAvailablePermissions: Filter = new Filter();
    filterSelectedPermissions: Filter = new Filter();

    @ViewChild(MessageBoxesComponent) msgBox: MessageBoxesComponent;

    constructor(fb: FormBuilder,
        private _roleService: RoleService,
        private _confirmationService: ConfirmationService,
        private _sessionService: SessionService,
        private _router: Router,
        private _route: ActivatedRoute) {

        this.formRoleDetail = fb.group({
            Name: ['', Validators.required],
            Private: '' ,
            Active: '',
            SortOrder: 0
        });

        this.filterAvailablePermissions = new Filter();
        const screen = new Screen();
        screen.Command = 'SearchBy';
        screen.Field = 'Name';
        screen.Value = 'Name';
        this.filterAvailablePermissions.Screens.push(screen);
    }

    // ===--- General Events ---===

    ngOnInit() {

        this.loadingData = true;

        if (!this._sessionService.CurrentSession.validSession) {
            this.loadingData = false;
            return;
        }
        this.loadRoleDropDown();
    }

    onTabShow(e) {

        if (e.UUID == null) {
            e.UUID = this.roleDetail.UUID;
        }


        switch (e.index) {
            case 0:
                this.selectedTab = 0;
                this.showDetails(e.UUID);
                break;
            case 1:
                this.selectedTab = 1;
                this.showAvailableUsers(e.UUID);
                this.showSelectedUsers(e.UUID);
                break;
            case 2:
                this.selectedTab = 2;
                this.showAvailablePermissions(e.UUID);
                this.showSelectedPermissions(e.UUID);
                break;
        }
    }

    // ===--- Top Menu Bar ---===

    loadRoleDropDown() {
        const res = this._roleService.getRoles();

        res.subscribe(response => {
            this.loadingData = false;

            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                return false;
            }

            this.roles = response.Result;

            const index = 0;
            for (const role of response.Result) {

                if (role.UUID === this.roleDetail.UUID || index === 0) {
                    this.roleDetail.UUID = role.UUID;
                    this.onTabShow({ 'index': this.selectedTab, 'UUID': this.roleDetail.UUID });
                    break;
                }
            }
        }, err => {
            this.loadingData = false;
            this.msgBox.ShowResponseMessage(err.status, 10);

            if (err.status === 401) {
                this._sessionService.ClearSessionState();
                setTimeout(() => {
                    this._router.navigate(['/membership/login'], { relativeTo: this._route });
                }, 3000);
            }

        });
    }

    cboRolesChange( selectedRoleUUID ) {

        this.roleDetail.UUID = selectedRoleUUID;

        this.onTabShow({ 'index': this.selectedTab, 'UUID': selectedRoleUUID });
    }


    deleteRole(roleUUID) {
        this.msgBox.closeMessageBox();
        this.deletingData = true;
        const res = this._roleService.deleteRole(roleUUID);

        res.subscribe(response => {

           this.deletingData = false;

            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                return false;
           }

           this.msgBox.ShowMessage('info', 'Role deleted.', 10);
           this.loadRoleDropDown(); // not updating the list so reload for now.

        }, err => {
           this.deletingData = false;
           this.msgBox.ShowResponseMessage(err.status, 10);

           if (err.status === 401) {
               this._sessionService.ClearSessionState();
               setTimeout(() => {
                   this._router.navigate(['/membership/login'], { relativeTo: this._route });
               }, 3000);
           }

        });

    }

    onClickAddNewRole(event) {
        this.roleDetail = new Role();
        this.newRole = true;
    }

    onClickDeleteRoleDetail(event) {

        this._confirmationService.confirm({
            message: 'Do you want to delete this role?',
            header: 'Delete Confirmation',
            icon: 'fa fa-trash',
            accept: () => {
                this.msgs = [];
                this.msgs.push({ severity: 'info', summary: 'Confirmed', detail: 'Role deleted' });
                this.deleteRole(this.roleDetail.UUID);
                this.roleDetail = new Role();
            }
        });
    }


    // ===--- Role Detail Tab (0) ---===

    saveRoleDetail() {
        this.loadingData = true;
        this.msgBox.closeMessageBox();

        let res;
        if (this.newRole) {
            res = this._roleService.addRole(this.roleDetail);

        } else {
            res = this._roleService.updateRole(this.roleDetail);
        }

        res.subscribe(response => {

            this.loadingData = false;

            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                return false;
            }
            if (this.newRole) {
                this.msgBox.ShowMessage('info', 'Role added.', 10);
                this.roleDetail.UUID = response.Result.UUID;
                this.roles.push(this.roleDetail);
                this.newRole = false;
            } else {
                this.msgBox.ShowMessage('info', 'Role updated.', 10);
            }
            this.loadRoleDropDown(); // not updating the list so reload for now.

        }, err => {
            this.loadingData = false;
            this.msgBox.ShowResponseMessage(err.status, 10);

            if (err.status === 401) {
                this._sessionService.ClearSessionState();
                setTimeout(() => {
                    this._router.navigate(['/membership/login'], { relativeTo: this._route });
                }, 3000);
            }

        });

    }

    showDetails(roleUUID) {

        if (roleUUID == null) {
            return;
        }

        const res = this._roleService.getRole(roleUUID);

        res.subscribe(response => {

            this.loadingData = false;

            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                return false;
            }

            this.roleDetail = response.Result;

        }, err => {
            this.loadingData = false;
            this.msgBox.ShowResponseMessage(err.status, 10);

            if (err.status === 401) {
                this._sessionService.ClearSessionState();
                setTimeout(() => {
                    this._router.navigate(['/membership/login'], { relativeTo: this._route });
                }, 3000);
            }

        });

    }


    // ===--- Role Users Tab (1) ---===

    // Users not in the role
    showAvailableUsers(roleUUID) {

        if (roleUUID == null) {
            return;
        }
        this.loadingData = true;
        this.msgBox.closeMessageBox();
        let resNonMembers = this._roleService.getNonMembers(roleUUID);

        resNonMembers.subscribe(response => {

            this.loadingData = false;

            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                return false;
            }

            this.roleNonMembers = response.Result;

        }, err => {
            this.loadingData = false;
            this.msgBox.ShowResponseMessage(err.status, 10);

            if (err.status === 401) {
                this._sessionService.ClearSessionState();
                setTimeout(() => {
                    this._router.navigate(['/membership/login'], { relativeTo: this._route });
                }, 3000);
            }

        });
    }

    // Users assigned to the role.
    showSelectedUsers(roleUUID) {

        if (roleUUID == null) {
            return;
        }
        // Load role members..
        const resMembers = this._roleService.getMembers(roleUUID);

        resMembers.subscribe(response => {

            this.loadingData = false;

            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                return false;
            }

            this.roleMembers = response.Result;

        }, err => {
            this.loadingData = false;
            this.msgBox.ShowResponseMessage(err.status, 10);

            if (err.status === 401) {
                this._sessionService.ClearSessionState();
                setTimeout(() => {
                    this._router.navigate(['/membership/login'], { relativeTo: this._route });
                }, 3000);
            }
        });
    }

    addUsers(event: any) {

        this.loadingData = true;
        this.msgBox.closeMessageBox();

        const res = this._roleService.addUsersToRole(this.roleDetail.UUID,  event.items );

        res.subscribe(response => {

            this.loadingData = false;

            if (response.Code !== 200) {
                this.loadingData = false;
                this.showSelectedUsers(this.roleDetail.UUID);
                this.showAvailableUsers(this.roleDetail.UUID);
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                return false;
            }

            this.msgBox.ShowMessage('info', 'Users added.', 10);

        }, err => {

            this.loadingData = false;
            this.msgBox.ShowResponseMessage(err.status, 10);

            if (err.status === 401) {
                this._sessionService.ClearSessionState();
                setTimeout(() => {
                    this._router.navigate(['/membership/login'], { relativeTo: this._route });
                }, 3000);
            }

        });
    }

    removeUsers(event: any) {

        this.loadingData = true;
        this.msgBox.closeMessageBox();

        const res = this._roleService.removeUsersFromRole(this.roleDetail.UUID,  event.items);

        res.subscribe(response => {

            this.loadingData = false;

            if (response.Code !== 200) {
                this.showSelectedUsers(this.roleDetail.UUID);
                this.showAvailableUsers(this.roleDetail.UUID);
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                return false;
            }
            this.msgBox.ShowMessage('info', 'Users removed.', 10);

        }, err => {
            this.loadingData = false;
            this.msgBox.ShowResponseMessage(err.status, 10);

            if (err.status === 401) {
                this._sessionService.ClearSessionState();
                setTimeout(() => {
                    this._router.navigate(['/membership/login'], { relativeTo: this._route });
                }, 3000);
            }
        });
    }

    // ===--- Role Permissions Tab (2) ---===

    showAvailablePermissions(roleUUID) {

        if (roleUUID == null) {
            return;
        }

        this.loadingData = true;
        this.msgBox.closeMessageBox();

        const resNonMembers = this._roleService.getAvailablePermisssions(roleUUID, this.filterAvailablePermissions);

        resNonMembers.subscribe(response => {

            this.loadingData = false;

            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                return false;
            }

            this.availablePermissions = response.Result;

        }, err => {
            this.loadingData = false;
            this.msgBox.ShowResponseMessage(err.status, 10);

            if (err.status === 401) {
                this._sessionService.ClearSessionState();
                setTimeout(() => {
                    this._router.navigate(['/membership/login'], { relativeTo: this._route });
                }, 3000);
            }

        });
    }

    showSelectedPermissions(roleUUID) {
        if (roleUUID == null) {
            return;
        }

        this.loadingData = true;
        this.msgBox.closeMessageBox();

        const resNonMembers = this._roleService.getSelectedPermisssions(roleUUID, this.filterSelectedPermissions);

        resNonMembers.subscribe(response => {

            this.loadingData = false;

            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                return false;
            }

            this.selectedPermissions = response.Result;

        }, err => {
            this.loadingData = false;
            this.msgBox.ShowResponseMessage(err.status, 10);

            if (err.status === 401) {
                this._sessionService.ClearSessionState();
                setTimeout(() => {
                    this._router.navigate(['/membership/login'], { relativeTo: this._route });
                }, 3000);
            }

        });
    }

    addPermissions(event: any) {

        this.loadingData = true;
        this.msgBox.closeMessageBox();

        const res = this._roleService.addPermissionsToRole(this.roleDetail.UUID, event.items);

        res.subscribe(response => {

            this.loadingData = false;

            if (response.Code !== 200) {
                this.loadingData = false;
                this.showAvailablePermissions(this.roleDetail.UUID);
                this.showSelectedPermissions(this.roleDetail.UUID);
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                return false;
            }

            this.msgBox.ShowMessage('info', 'Permissions added.', 10);

        }, err => {

            this.loadingData = false;
            this.msgBox.ShowResponseMessage(err.status, 10);

            if (err.status === 401) {
                this._sessionService.ClearSessionState();
                setTimeout(() => {
                    this._router.navigate(['/membership/login'], { relativeTo: this._route });
                }, 3000);
            }

        });
    }

    removePermissions(event: any) {

        this.loadingData = true;
        this.msgBox.closeMessageBox();

        const res = this._roleService.removePermissionsFromRole(this.roleDetail.UUID, event.items);

        res.subscribe(response => {

            this.loadingData = false;

            if (response.Code !== 200) {
                this.showAvailablePermissions(this.roleDetail.UUID);
                this.showSelectedPermissions(this.roleDetail.UUID);
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                return false;
            }
            this.msgBox.ShowMessage('info', 'Permissions removed.', 10);


        }, err => {
            this.loadingData = false;
            this.msgBox.ShowResponseMessage(err.status, 10);

            if (err.status === 401) {
                this._sessionService.ClearSessionState();
                setTimeout(() => {
                    this._router.navigate(['/membership/login'], { relativeTo: this._route });
                }, 3000);
            }

        });

    }

    filterAvailableChanged(filterText) {

        if (!filterText ||  filterText.length < 2) {
            return;
        }

        this.filterAvailablePermissions.Screens = [];
        const screen = new Screen();
        screen.Command = 'SearchBy';
        screen.Field = 'Name';
        screen.Value = filterText;
        this.filterAvailablePermissions.Screens.push(screen);



        setTimeout(() => {
            this.showAvailablePermissions(this.roleDetail.UUID);
        }, 1000);
    }

    filterSelectedChanged(filterText) {

        if (!filterText || filterText.length < 2) {
            return;
        }

        this.filterSelectedPermissions.Screens = [];
        const screen = new Screen();
        screen.Command = 'SearchBy';
        screen.Field = 'Name';
        screen.Value = filterText;
        this.filterSelectedPermissions.Screens.push(screen);

        setTimeout(() => {
            this.showSelectedPermissions(this.roleDetail.UUID);
        }, 1000);
    }
}
