export default interface ExecutionRequest{
    codeText: string;
    languageId: number; 
    priority?: number;
}