import axios from "axios";

import Cache from "../Utils/Cache";
import Language from "../Models/Language";
import AuthService from "./AuthService";
import AlertsService from "./AlertsService";

export default class LanguagesService{
    private static instance?: LanguagesService;
    
    private readonly baseUrl = "https://localhost:7155/languages";
    private static cache = new Cache<Language[]>(1, 20, "languages");
    private readonly auth = AuthService.getInstance();
    private readonly alerts = AlertsService.getInstance();
    
    private constructor() {}

    static getInstance(): LanguagesService{
        if (!LanguagesService.instance)
            LanguagesService.instance = new LanguagesService();
        return LanguagesService.instance;
    }
    
    
    public async getLanguages(){
        const url = `${this.baseUrl}/list`;
        return LanguagesService.cache.tryGetAsync("languages", () =>
            axios.get<Language[]>(url, this.auth.createHeader())
                .then(response => response.data.sort(x => x.id))
                .catch(error => {
                    this.alerts.addError("Cannot load languages");
                    console.error("Cannot load languages", error);
                    return undefined;
                }));
    }
    
}