import { Photo } from './Photo';

export interface User {
    id: number;
    username: string;
    age: number;
    knownAs: string;
    gender: string;
    created: Date;
    lastActive: Date;
    photoUrl: string;
    city: string;
    country: string;
    // Optional propertise must be defined after required propertise
    interests?: string;
    introduction?: string;
    lookingFor?: string;
    photos?: Photo[];
}
