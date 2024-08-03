export class PamelloSpeaker implements IPamelloSpeaker {
    guildName!: string;
    voiceName!: string;
}

export interface IPamelloSpeaker {
    guildName: string;
    voiceName: string;
}
