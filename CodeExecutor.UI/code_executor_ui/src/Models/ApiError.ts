export default interface ApiError{
    Code: number;
    Message: string;
    Data?: any|undefined;
    ErrorType?: string|undefined;
}