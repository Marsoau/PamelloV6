import { IPamelloSpeaker } from "./PamelloSpeaker";

export class PamelloPlayer implements IPamelloPlayer {
    id!: number;
    name!: string;
    isPaused!: boolean;
    state!: PamelloPlayerState;
    speakers!: IPamelloSpeaker[];
    currentSongTimePassed!: number;
    currentSongTimeTotal!: number;
    currentSongId!: number | null;
    queueSongIds!: number[];
    queuePosition!: number;
    nextPositionRequest!: number | null;
    queueIsRandom!: boolean;
    queueIsReversed!: boolean;
    queueIsNoLeftovers!: boolean;
}

export interface IPamelloPlayer {
    id: number;
    name: string;
    isPaused: boolean;
    state: PamelloPlayerState;
    speakers: IPamelloSpeaker[];
    currentSongTimePassed: number;
    currentSongTimeTotal: number;
    currentSongId: number | null;
    queueSongIds: number[];
    queuePosition: number;
    nextPositionRequest: number | null;
    queueIsRandom: boolean;
    queueIsReversed: boolean;
    queueIsNoLeftovers: boolean;
}

export enum PamelloPlayerState {
    Ready,
    AwaitingSong,
    AwaitingSpeaker,
    AwainingSongAudio,
}
