import { Component, OnInit } from '@angular/core';
import { User } from 'src/app/_models/user';
import { AdminService } from 'src/app/_services/admin.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { BsModalRef, BsModalService } from 'ngx-bootstrap';
import { RolesModalComponent } from '../roles-modal/roles-modal.component';

@Component({
    selector: 'app-user-management',
    templateUrl: './user-management.component.html',
    styleUrls: ['./user-management.component.css']
})
export class UserManagementComponent implements OnInit {

    users: User[];
    bsModalRef: BsModalRef;

    constructor(private adminService: AdminService, private alertify: AlertifyService, private modalService: BsModalService) {}

    ngOnInit() {
        this.getUsersWithRoles();
    }

    getUsersWithRoles() {
        this.adminService.getUsersWithRoles().subscribe((users: User[]) => {
            this.users = users;
        }, error => {
            this.alertify.error(error);
        });
    }

    editRolesModal(user: User) {
        const initialState = {
            user,
            roles: this.getRolesArray(user)
        };

        this.bsModalRef = this.modalService.show(RolesModalComponent, {initialState});
        this.bsModalRef.content.updateSelectedRoles.subscribe((values) => {
            const rolesToUpdate = {
                roleNames: [...values.filter(el => el.checked === true).map(el => el.name)]
            };

            if (rolesToUpdate) {
                this.adminService.updateUserRoles(user, rolesToUpdate).subscribe(() => {
                    user.roles = [...rolesToUpdate.roleNames];
                    this.alertify.success('Updated user roles.');
                }, error => {
                    this.alertify.error(error);
                })
            }
        });
    }

    private getRolesArray(user) {
        const roles = [];
        const userRoles = user.roles;
        const availableRoles: any[] = [
            { name: 'Admin', value: 'Admin' },
            { name: 'Moderator', value: 'Moderator' },
            { name: 'Regular', value: 'Regular' },
            { name: 'VIP', value: 'VIP' }
        ];

        for (const availableRole of availableRoles) {
            let matched = false;
            for (const userRole of userRoles) {
                if (availableRole.name === userRole) {
                    matched = true;
                    availableRole.checked = true;
                    roles.push(availableRole);
                    break;
                }
            }
            if (!matched) {
                availableRole.checked = false;
                roles.push(availableRole);
            }
        }
        return roles;
    }
}
