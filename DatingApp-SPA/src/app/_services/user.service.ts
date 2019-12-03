import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from '../_models/user';
import { PaginatedResult } from '../_models/pagination';
import { map } from 'rxjs/operators';
import { Message } from '../_models/Message';

@Injectable({
  providedIn: 'root'
})

export class UserService {

    baseUrl = environment.apiUrl + 'users/';

    constructor(private http: HttpClient) {}

    getUsers(page?, itemsPerPage?, userParams?, likesParam?): Observable<PaginatedResult<User[]>> {
        const paginatedResult: PaginatedResult<User[]> = new PaginatedResult<User[]>();
        let params = new HttpParams();

        // Pagination
        if (page != null && itemsPerPage != null) {
            params = params.append('pageNumber', page);
            params = params.append('pageSize', itemsPerPage);
        }

        // Filter
        if (userParams != null) {
            params = params.append('minimumAge', userParams.minAge);
            params = params.append('maximumAge', userParams.maxAge);
            params = params.append('gender', userParams.gender);
            params = params.append('orderBy', userParams.orderBy);
        }

        if (likesParam === 'likers') {
            params = params.append('likers', 'true');
        } else if (likesParam === 'likees') {
            params = params.append('likees', 'true');
        }

        return this.http.get<User[]>(this.baseUrl, { observe: 'response', params }).pipe(
            map(response => {
                paginatedResult.result = response.body;
                if (response.headers.get('Pagination') != null) {
                    paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
                }
                return paginatedResult;
            })
        );
    }

    getUser(id: number): Observable<User> {
        return this.http.get<User>(this.baseUrl + id);
    }

    updateUser(id: number, user: User) {
        return this.http.put(this.baseUrl + id, user);
    }

    setMainPhoto(userId: number, id: number) {
        return this.http.post(this.baseUrl + userId + '/photos/' + id + '/setMain', {});
    }

    deletePhoto(userId: number, id: number) {
        return this.http.delete(this.baseUrl + userId + '/photos/' + id);
    }

    sendLike(id: number, recipientId: number) {
        return this.http.post(this.baseUrl + id + '/like/' + recipientId, {});
    }

    getMessages(id: number, page?, itemsPerPage?, messageContainer?) {
        const paginatedResult: PaginatedResult<Message[]> = new PaginatedResult<Message[]>();

        let params = new HttpParams();
        params = params.append('MessageContainer', messageContainer);

        // Pagination
        if (page != null && itemsPerPage != null) {
            params = params.append('pageNumber', page);
            params = params.append('pageSize', itemsPerPage);
        }

        return this.http.get<Message[]>(this.baseUrl + id + '/messages', { observe: 'response', params }).pipe(
            map(response => {
                paginatedResult.result = response.body;
                if (response.headers.get('Pagination') !== null) {
                    paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
                }

                return paginatedResult;
            })
        );
    }

    getMessageThread(id: number, recipientId: number) {
        return this.http.get<Message[]>(this.baseUrl + id + '/messages/thread/' + recipientId);
    }

    sendMessage(id: number, message: Message) {
        return this.http.post(this.baseUrl + id + '/messages', message);
    }

    deleteMessage(id: number, userId: number) {
        return this.http.post(this.baseUrl + userId + '/messages/' + id, {});
    }
}
