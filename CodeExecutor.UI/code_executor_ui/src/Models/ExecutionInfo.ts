import Language from "./Language";

export default interface ExecutionInfo{
    id: string;
    language?: Language|undefined;
    requestedAt?: Date|undefined;
    startedAt?: Date|undefined;
    finishedAt?: Date|undefined;
    comment?: string|undefined;
    isError: boolean;
    data?: string|undefined;
}