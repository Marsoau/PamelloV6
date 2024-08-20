export class PamelloSong implements IPamelloSong {
    id!: number;
    title!: string;
    author!: string;
    coverUrl!: string;
    youtubeId!: string;
    playCount!: number;
    isDownloaded!: boolean;
    episodeIds!: number[];
    playlistIds!: number[];
}

export interface IPamelloSong {
    id: number;
    title: string;
    author: string;
    coverUrl: string;
    youtubeId: string;
    playCount: number;
    isDownloaded: boolean;
    episodeIds: number[];
    playlistIds: number[];
}
