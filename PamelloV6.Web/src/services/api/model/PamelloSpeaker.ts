export class PamelloSpeaker implements IPamelloSpeaker {
    guildName!: string;
    vcName!: string;
}

export interface IPamelloSpeaker {
    guildName: string;
    vcName: string;
}
