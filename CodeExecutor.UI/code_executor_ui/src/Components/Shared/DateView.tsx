import dayjs from "dayjs";
import relativeTime from 'dayjs/plugin/relativeTime';
dayjs.extend(relativeTime);

export default function DateView(params: {date: Date, fullDate?: boolean, withTime?: boolean, badgeClass?: string}){
    const date = dayjs(params.date);
    
    if (params.fullDate == true){
        return <span className={params.badgeClass && ("badge " + params.badgeClass)}>
            {date.format(`dd.MM.yyyy${params.withTime == true && " HH:mm"}`)}
        </span>
    } 
    else{
        return <span className={params.badgeClass && ("badge " + params.badgeClass)} title={date.format("D MMM YYYY HH:mm:ss")}>
            {date.fromNow()}
        </span>
    }
}