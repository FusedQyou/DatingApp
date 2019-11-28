import { Component, OnInit } from '@angular/core';
import { UserService } from 'src/app/_services/user.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { User } from 'src/app/_models/user';
import { ActivatedRoute } from '@angular/router';

@Component({
    selector: 'app-member-detail',
    templateUrl: './member-detail.component.html',
    styleUrls: ['./member-detail.component.css']
})

export class MemberDetailComponent implements OnInit {

    user: User;

    constructor(private userService: UserService, private alertify: AlertifyService, private route: ActivatedRoute) { }

    ngOnInit() {
        // Get the user
        this.route.data.subscribe(data => {
            this.user = data[`user`];
        });
    }
}
