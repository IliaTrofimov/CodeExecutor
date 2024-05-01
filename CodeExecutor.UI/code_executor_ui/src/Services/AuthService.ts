import axios from "axios";
import dayjs from "dayjs";

import User from "../Models/User";
import LoginRequest from "../Models/LoginRequest";
import ApiError from "../Models/ApiError";
import AlertsService from "./AlertsService";
import Event from "../Utils/Events";

export default class AuthService{
    private static instance?: AuthService;
    private static localStorageToken: string = "code-executor-ui-token";

    private readonly baseUrl: string = "https://localhost:7155/users";
    private readonly alerts = AlertsService.getInstance();
    private readonly onLogin = new Event<boolean>("onLogin");

    private token?: string;
    private username?: string = "unknown";
    private isAdmin: boolean = false;
    private anonymousToken?: string;
    
    private constructor() {
        const token = localStorage.getItem(AuthService.localStorageToken);
        
        if (token){
            console.log("Received token from local storage");
            this.token = token;
            this.me();
            if (this.isAuthenticated()){
                this.onLogin.publish(true);
            }
        }
        this.alerts = AlertsService.getInstance();
    }

    public static getInstance(): AuthService{
        if (!AuthService.instance)
            AuthService.instance = new AuthService();
        return AuthService.instance;
    }

    public addOnLogin(id: string, callback: (hasLoggedIn: boolean) => void){
        this.onLogin.subscribe(id, callback);
    }

    public removeOnLogin(id: string){
        this.onLogin.unsubscribe(id);
    }


    public isAuthenticated(){
        return this.token != null;
    }

    public getUsername(){
        return this.username;
    }

    public isUserAdmin(){
        return this.isAdmin;
    }

    public createHeader(){
        return this.isAuthenticated() 
            ? {headers: {Authorization: `Bearer ${this.token}`}}
            : undefined;
    }

    public useAnonymous(anonymousToken: string){
        this.anonymousToken = anonymousToken;
    }

    public getAnonymousToken(){
        return this.anonymousToken;
    }

    public async me(): Promise<string|undefined> {
        if (!this.token)
            return undefined;
        
        const url = `${this.baseUrl}/me/`;
        return axios.get<User>(url, this.createHeader())
            .then(repsonse => {
                this.username = repsonse.data.username;
                this.isAdmin = repsonse.data.isSuper;
                return undefined;
            })
            .catch(error => {
                this.token = undefined;
                
                const apiError = error.response.data as ApiError;
                console.error("Cannot get current user:", apiError.Message ?? apiError.ErrorType ?? error.message)
                return apiError.Message ?? apiError.ErrorType ?? error.message;
            });
    }

    public login(username: string, password: string, remember: boolean = false): Promise<string|undefined>{
        const url = `${this.baseUrl}/login/`;
        const request: LoginRequest = {
            Username: username,
            Password: password
        };

        return axios.post<string>(url, request)
            .then(response => {
                this.token = response.data;
                this.username = username;
                this.anonymousToken = undefined;
                this.onLogin.publish(true);

                if (remember && this.token) {
                    console.debug("Saved token to local storage");
                    localStorage.setItem(AuthService.localStorageToken, this.token);
                }
                return undefined;
            })
            .catch(error => {
                const apiError = error.response.data as ApiError;
                console.error("Cannot login:", apiError.Message ?? apiError.ErrorType ?? error.message)
                return apiError.Message ?? apiError.ErrorType ?? error.message;
            });
    }

    public logout(): void{
        this.anonymousToken = undefined;
        this.token = undefined;
        this.username = undefined;
        this.isAdmin = false;
        localStorage.removeItem(AuthService.localStorageToken);
        console.debug("Removed token from local storage");
        this.onLogin.publish(false);
    }
}