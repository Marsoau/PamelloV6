export class PamelloEpisode implements IPamelloEpisode {
    id!: number;
    songId!: number;
    name!: string;
    start!: number;
    skip!: boolean;
}

export interface IPamelloEpisode {
    id: number;
    songId: number;
    name: string;
    start: number;
    skip: boolean;
}
