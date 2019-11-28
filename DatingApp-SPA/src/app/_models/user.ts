import { Photo } from './photo';

export interface User {
    id: number;
    username: string;
    firstName: string;
    insertion?: string;
    lastName?: string;
    gender: string;
    age: number;
    created: Date;
    lastActive: Date;
    mainPhotoUrl: string;
    introduction?: string;
    interests?: string;
    photos?: Photo[];
}
