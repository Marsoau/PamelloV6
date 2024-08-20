export interface ISearchResult<T> {
    page: number;
    pagesCount: number;
    results: T[];
    query: string;
}
