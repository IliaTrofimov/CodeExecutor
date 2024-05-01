import {generateUniqueID} from "web-vitals/dist/modules/lib/generateUniqueID";
import {Dictionary} from "./Dictionary";

export default class Event<T>{
    private readonly handlers: Dictionary<string, (args: T) => void> = {};
    private readonly eventName?: string;
    
    public constructor(name: string|undefined = undefined) {
        this.eventName = name ?? generateUniqueID().slice(0, 8);
        console.debug(`[EVENT][${this.eventName}] registered`);
    }
    
    public subscribe(id: string, callback: (eventArgs: T) => void){
        if (!(id in this.handlers)){
            console.debug(`[EVENT][${this.eventName}] '${id}' subscribed`);
            this.handlers[id] = callback;
        }
    }

    public unsubscribe(id: string){
        if (id in this.handlers){
            delete this.handlers[id];
            console.debug(`[EVENT][${this.eventName}] '${id}' unsubscribed`);
        }
    }

    public unsubscribeAll(){
        console.debug(`[EVENT][${this.eventName}] all unsubscribed'`);
        for (let h in this.handlers) {
            delete this.handlers[h];
        }
    }
    
    public publish(args: T){
        console.debug(`[EVENT][${this.eventName}] publish`, args);
        for (let h in this.handlers) {
            this.handlers[h](args);
        }
    }
}