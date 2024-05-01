import {Dictionary} from "./Dictionary";
import {generateUniqueID} from "web-vitals/dist/modules/lib/generateUniqueID";
import dayjs from "dayjs";

class CacheItem<T>{
    public readonly expireAt?: dayjs.Dayjs;
    public readonly value: T;
    
    public constructor(value: T, expireInMinutes: number|undefined = undefined) {
        this.value = value;
        this.expireAt = expireInMinutes ? dayjs().add(expireInMinutes, "minute") : undefined;
    }
    
    public isExpired(){
        return this.expireAt 
            ? dayjs().diff(this.expireAt) <= 0
            : false;
    }
}

export default class Cache<T>{
    private readonly items: Dictionary<string, CacheItem<T>> = {};
    private readonly cacheName: string;
    private readonly maxItems: number;
    private readonly expireInMinutes: number|undefined;
    
    public constructor(maxItems: number, expireInMinutes: number|undefined, name: string|undefined = undefined) {
        this.maxItems = maxItems;
        this.expireInMinutes = expireInMinutes;
        this.cacheName = name ?? generateUniqueID().slice(0, 8);
        console.debug(`[CACHE][${this.cacheName}] registered`)
    }
    
    public tryGet(id: string, setter: (() => T|undefined)|undefined = undefined): T|undefined{
        if (id in this.items){
            console.debug(`[CACHE][${this.cacheName}] '${id.slice(0, 8)}' was found`);
            return this.items[id].value;
        }
        else if (setter){
            console.debug(`[CACHE][${this.cacheName}] '${id.slice(0, 8)}' was not found and will be evaluated`);
            this.set(id, setter());
            return this.items[id].value;
        }
    }

    public async tryGetAsync(id: string, setter: (() => Promise<T|undefined>|undefined)|undefined = undefined){
        if (id in this.items){
            console.debug(`[CACHE][${this.cacheName}] '${id.slice(0, 8)}' was found`);
            return this.items[id].value;
        }
        else if (setter){
            console.debug(`[CACHE][${this.cacheName}] '${id.slice(0, 8)}' was not found and will be evaluated`);
            this.set(id, await setter());
            return this.items[id].value;
        }
    }
    
    public set(id: string, value: T|undefined){
        if (value === undefined){
            delete this.items[id];
            console.debug(`[CACHE][${this.cacheName}] '${id.slice(0, 8)}' was deleted`);
        }
        else{
            if (Object.keys(this.items).length >= this.maxItems){
                delete this.items[Object.keys(this.items)[0]];
                console.debug(`[CACHE][${this.cacheName}] shrink 1 item`);
            }
            this.items[id] = new CacheItem<T>(value, this.expireInMinutes);
            console.debug(`[CACHE][${this.cacheName}] '${id.slice(0, 8)}' was added`, this.items[id].value);
        }
    }
    
    public contains(id: string){
        return id in this.items && !this.items[id].isExpired();
    }
}