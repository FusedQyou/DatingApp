import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import { Photo } from 'src/app/_models/photo';
import { FileUploader } from 'ng2-file-upload';
import { environment } from 'src/environments/environment';
import { AuthService } from 'src/app/_services/auth.service';
import { UserService } from 'src/app/_services/user.service';
import { AlertifyService } from 'src/app/_services/alertify.service';

@Component({
    selector: 'app-photo-editor',
    templateUrl: './photo-editor.component.html',
    styleUrls: ['./photo-editor.component.css']
})

export class PhotoEditorComponent implements OnInit {
    @Input() photos: Photo[];
    uploader: FileUploader;
    hasBaseDropZoneOver = false;
    baseUrl = environment.apiUrl;
    currentMain: Photo;

    constructor(private authService: AuthService, private alertify: AlertifyService, private userService: UserService) {}

    ngOnInit() {
        this.initializeUploader();
    }

    fileOverBase(e: any): void {
        this.hasBaseDropZoneOver = e;
    }

    initializeUploader() {
        this.uploader = new FileUploader({
            url: this.baseUrl + 'users/' + this.authService.decodedToken.nameid + '/photos',
            authToken: 'Bearer ' + localStorage.getItem('token'),
            isHTML5: true,
            allowedFileType: ['image'],
            removeAfterUpload: true,
            autoUpload: false,
            maxFileSize: 10 * 1024 * 1024 // 10mb
        });

        // This fixes a problem where our CORS policy blocks the file upload to to containing credentials, which we really do not need.
        this.uploader.onAfterAddingFile = (file) => {
            file.withCredentials = false;
        };

        this.uploader.onSuccessItem = (item, response, status, headers) => {
            if (response) {
                const res: Photo = JSON.parse(response);
                const photo = {
                    id: res.id,
                    url: res.url,
                    dateAdded: res.dateAdded,
                    description: res.description,
                    asMainPhoto: res.asMainPhoto
                };

                this.photos.push(photo);
            }
        };
    }

    setMainPhoto(photo: Photo) {
        this.userService.setMainPhoto(this.authService.decodedToken.nameid, photo.id).subscribe(() => {
            // Get the new main photo
            this.currentMain = this.photos.filter(p => p.asMainPhoto === true)[0];
            this.currentMain.asMainPhoto = false;
            photo.asMainPhoto = true;

            // Update the main photo globally
            this.authService.changeMemberPhoto(photo.url);
            this.authService.currentUser.mainPhotoUrl = photo.url;

            // Update the locally saved user to display the right photo
            localStorage.setItem('user', JSON.stringify(this.authService.currentUser));

            this.alertify.success('Main photo has been updated.');
        }, error => {
            this.alertify.error(error);
        });
    }
}
