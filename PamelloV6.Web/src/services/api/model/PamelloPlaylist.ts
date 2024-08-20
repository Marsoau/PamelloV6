export class PamelloPlaylist implements IPamelloPlaylist {
    id!: number;
    name!: string;
    ownerId!: number;
    isProtected!: boolean;
    songIds!: number[];
}

export interface IPamelloPlaylist {
    id: number;
    name: string;
    ownerId: number;
    isProtected: boolean;
    songIds: number[];
}
