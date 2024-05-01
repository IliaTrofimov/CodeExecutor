import axios from "axios";
import dayjs from "dayjs";

import ExecutionInfo from "../Models/ExecutionInfo";
import ExecutionStartResponse from "../Models/ExecutionStartResponse";
import ExecutionRequest from "../Models/ExecutionRequest";
import SourceCode from "../Models/SourceCode";

import AuthService from "./AuthService";
import AlertsService from "./AlertsService";
import Event from "../Utils/Events";
import Cache from "../Utils/Cache";

export default class ExecutionsService{
    private static instance?: ExecutionsService;
    private readonly baseUrl = "https://localhost:7155/codeExecutions";
    private readonly auth = AuthService.getInstance();
    private readonly alerts = AlertsService.getInstance();
    
    private onExecutionUpdated = new Event<ExecutionInfo>("onExecutionUpdated");

    private executionsCache = new Cache<ExecutionInfo>(50, 5, "executions");
    private sourceCodeCache = new Cache<string>(10, undefined, "sourceCode");
    private guidsCache = new Cache<string[]>(2, 5, "execGuids");
    
    private constructor() { }
    
    static getInstance(): ExecutionsService{
        if (!ExecutionsService.instance)
            ExecutionsService.instance = new ExecutionsService();
        return ExecutionsService.instance;
    }
    
    public addOnExecutionUpdated(id: string, callback: (args: ExecutionInfo) => void){
        this.onExecutionUpdated.subscribe(id, callback);
    }

    public removeOnExecutionUpdated(id: string){
        this.onExecutionUpdated.unsubscribe(id);
    }
    
    
    public async getExecutionsByIds(guids: string[], force: boolean = false){
        if (!this.auth.isAuthenticated())
            return [];
        
        let cached: ExecutionInfo[] = [];
        if (!force){
            cached = guids.map(guid => this.executionsCache.tryGet(guid))
                .filter(ExecutionsService.notEmpty)
                .filter(x => !ExecutionsService.isUnfinished(x));
            
            
            console.log(`getExecutionsByIds using ${cached.length} cached values`);
        }
        
        const url = `${this.baseUrl}/executionsByIds`;
        return axios.post<ExecutionInfo[]>(url, {ids: guids}, this.auth.createHeader())
            .then(response => {
                for (let e of response.data){
                    this.executionsCache.set(e.id, e);
                }
                return response.data.sort(ExecutionsService.sortExecutions);
            })
            .catch(error => {
                this.alerts.addError("Cannot load executions list",
                    error.response?.data?.Message ?? error.response?.data?.ErrorType ?? error.message);
                console.error("Cannot load executions list", error.response?.data ?? error);
                return undefined;
            });
    }
    
    public async getExecutions(force: boolean = false) : Promise<ExecutionInfo[]|undefined> {
        if (!this.auth.isAuthenticated())
            return [];
        
        const guids = this.guidsCache.tryGet(this.auth.getUsername() ?? "");
        if (!force && guids){
            console.debug(`Using cached executions for user '${this.auth.getUsername()}'`);
            return guids.map(g => this.executionsCache.tryGet(g)).filter(ExecutionsService.notEmpty);
        }
        else{
            console.debug(`Loading executions for user '${this.auth.getUsername()}'`);
            
            const url = `${this.baseUrl}/executions`;
            return axios.get<ExecutionInfo[]>(url, this.auth.createHeader())
                .then(response => {
                    for (let e of response.data){
                        this.executionsCache.set(e.id, e);
                    }
                    this.guidsCache.set(this.auth.getUsername() ?? "", response.data.map(e => e.id));
                    return response.data.sort(ExecutionsService.sortExecutions);
                })
                .catch(error => {
                    this.alerts.addError("Cannot load executions list",
                        error.response?.data?.Message ?? error.response?.data?.ErrorType ?? error.message);
                    console.error("Cannot load executions list", error.response?.data ?? error);
                    return undefined;
                });
        }
    }

    public async getResult(guid: string, force: boolean = false): Promise<ExecutionInfo|undefined> {
        const anonymousUserGuid = this.auth.getAnonymousToken();
        const url = anonymousUserGuid
            ? `${this.baseUrl}/result/${guid}?userGuid=${anonymousUserGuid}`
            : `${this.baseUrl}/result/${guid}`;

        return axios.get<ExecutionInfo>(url, this.auth.createHeader())
            .then(response => {
                this.addNewExecutionToCache(response.data);
                return response.data;
            })
            .catch(error => {
                this.alerts.addError("Cannot load execution",
                    error.response?.data?.Message ?? error.response?.data?.ErrorType ?? error.message);
                console.log("Cannot load execution", error.response?.data ?? error);
                return undefined;
            })
    }

    public getSourceCode(guid: string): Promise<string|undefined> {
        const anonymousUserGuid = this.auth.getAnonymousToken();
        const url = anonymousUserGuid
            ? `${this.baseUrl}/sourceCode/${guid}?userGuid=${anonymousUserGuid}`
            : `${this.baseUrl}/sourceCode/${guid}`;
        
        return this.sourceCodeCache.tryGetAsync(guid, () => 
            axios.get<SourceCode>(url, this.auth.createHeader())
                .then(respone => respone.data.text)
                .catch(error => {
                    this.alerts.addError("Cannot load source code for execution",
                        error.response?.data?.Message ?? error.response?.data?.ErrorType ?? error.message);
                    console.log("Cannot load source code for execution", error.response?.data ?? error);
                    return undefined;
                }));
    }

    public execute(sourceCode: string, languageId: number): Promise<ExecutionStartResponse|undefined>{
        const url = `${this.baseUrl}/execute`;
        const request: ExecutionRequest = {
            codeText: sourceCode,
            languageId: languageId
        };
        
        return axios.post<ExecutionStartResponse>(url, request, this.auth.createHeader())
            .then(response => {
                if (response.data.anonymousUserId){
                    this.auth.useAnonymous(response.data.anonymousUserId);
                }
                
                this.addNewExecutionToCache(response.data);
                this.onExecutionUpdated.publish({
                    id: response.data.id,
                    comment: response.data.comment,
                    language: {id: languageId, name: ""},
                    isError: false,
                    requestedAt: new Date()
                });
                
                response.data.comment 
                    ? this.alerts.addMessage(`Execution started with comment: ${response.data.comment}`)
                    : this.alerts.addMessage("Execution started");
                
                return response.data;
            })
            .catch(error => {
                this.alerts.addError("Cannot start code execution",
                    error.response?.data?.Message ?? error.response?.data?.ErrorType ?? error.message);
                console.error("Cannot start code execution", error.response?.data ?? error);
                return undefined;
            });
    }

    public async delete(guid: string) : Promise<boolean> {
        const url = `${this.baseUrl}/delete/${guid}`;
        return axios.delete(url, this.auth.createHeader())
            .then(response => {
                this.alerts.addMessage("Code execution was deleted");
                return true;
            })
            .catch(error => {
                this.alerts.addError("Cannot delete code execution",
                    error.response?.data?.Message ?? error.response?.data?.ErrorType ?? error.message);
                console.error("Cannot delete code execution", error.response?.data ?? error);
                return false;
            });
    }
    
    private addNewExecutionToCache(info: { id: string }){
        const guids = this.guidsCache.tryGet(this.auth.getUsername() ?? "");
        if (guids)
            this.guidsCache.set(this.auth.getUsername() ?? "", [info.id, ...guids]);
        else
            this.guidsCache.set(this.auth.getUsername() ?? "", [info.id]);
    }


    public static sortExecutions(x: ExecutionInfo, y: ExecutionInfo) {
        return dayjs(y.requestedAt).diff(x.requestedAt);
    }

    public static isUnfinished(info: ExecutionInfo) {
        return !info.isError &&
            (!info.finishedAt || dayjs().diff(info.requestedAt, "hour", true) > 6);
    }

    private static notEmpty<TValue>(value: TValue | null | undefined): value is TValue {
        return !(value === null || value === undefined);
    }
}