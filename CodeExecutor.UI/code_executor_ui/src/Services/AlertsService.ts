import toast from 'react-hot-toast';

export default class AlertsService{
    private static instance?: AlertsService;
    
    private constructor() {}

    public static getInstance(){
        if (!AlertsService.instance){
            AlertsService.instance = new AlertsService();
        }
        return AlertsService.instance;
    }

    public addMessage(message: string){
        toast(message, {
            duration: 4000,
            style: {fontSize: "10pt"},
            position: "bottom-right"
        });
    }

    public addError(message: string, details: string|undefined = undefined) {
        if (details){
            toast.error(`${message}\n${details}`, {
                duration: 5000,
                style: {fontSize: "10pt"},
                position: "bottom-right"
            });
        }
        else{
            toast.error(message, {
                duration: 5000,
                style: {fontSize: "10pt"},
                position: "bottom-right"
            });
        }
    }
}