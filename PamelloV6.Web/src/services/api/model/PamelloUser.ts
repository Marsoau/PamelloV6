export class PamelloUser implements IPamelloUser {
    id!: number;
    name!: string;
    coverUrl!: string;
    discordId!: number;
    selectedPlayerId!: number | null;
    isAdministrator!: boolean;
    ownedPlaylistIds!: number[];
}

export interface IPamelloUser {
    id: number;
    name: string;
    coverUrl: string;
    discordId: number;
    selectedPlayerId: number | null;
    isAdministrator: boolean;
    ownedPlaylistIds: number[];
}
