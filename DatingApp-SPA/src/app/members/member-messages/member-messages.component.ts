import { Component, OnInit, Input } from '@angular/core';
import { Message } from 'src/app/_models/Message';
import { UserService } from 'src/app/_services/user.service';
import { AuthService } from 'src/app/_services/auth.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { tap } from 'rxjs/operators';

@Component({
    selector: 'app-member-messages',
    templateUrl: './member-messages.component.html',
    styleUrls: ['./member-messages.component.css']
})

export class MemberMessagesComponent implements OnInit {

    @Input() recipientId: number;
    messages: Message[];
    newMessage: any = {};

    constructor(
        private userService: UserService,
        private authService: AuthService,
        private alertify: AlertifyService) {}

    ngOnInit() {
        this.loadMessages();
    }

    loadMessages() {
        const currentUserId: number = this.authService.decodedToken.nameid;
        this.userService.getMessageThread(this.authService.decodedToken.nameid, this.recipientId)

            // Mark all messages that are not read as read.
            .pipe(
                tap(messages => {
                    for (const message of messages) {
                        // tslint:disable-next-line: triple-equals
                        if (message.isRead === false && message.recipientId == currentUserId) {
                            this.userService.markAsRead(currentUserId, message.id);
                        }
                    }
                })
            )

            // Get the resulting messages
            .subscribe(messages => {
                this.messages = messages;
            }, error => {
                this.alertify.error(error);
        });
    }

    sendMessage() {
        this.newMessage.recipientId = this.recipientId;
        this.userService.sendMessage(this.authService.decodedToken.nameid, this.newMessage)
            .subscribe((message: Message) => {
                this.messages.unshift(message);
                this.newMessage.content = '';
        }, error => {
            this.alertify.error(error);
        });
    }
}
