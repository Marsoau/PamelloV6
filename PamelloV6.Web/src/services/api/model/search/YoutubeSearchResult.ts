import { IPamelloSong } from "../PamelloSong";
import { IYoutubeSearchVideoInfo } from "./YoutubeSearchVideoInfo";

export interface IYoutubeSearchResult {
    resultsCount: number;
    pamelloSongs: IPamelloSong[];
    youtubeVideos: IYoutubeSearchVideoInfo[];
	query: string;
}
