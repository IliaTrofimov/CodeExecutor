import ExecutionInfo from "../../Models/ExecutionInfo";
import DateView from "../Shared/DateView";

export default function ExecutionInfoTab(props: {execution: ExecutionInfo|undefined}){
    return (
        <div className="container-h tab-container">
            <div className="col w-100 container-h">
                <div className="col-sm-5 col-3">
                    <div><b>Requested at</b></div>
                    <div><b>Started at</b></div>
                    <div><b>Finished at</b></div>
                </div>
                <div className="col">
                    <div>
                        {props.execution?.requestedAt
                            ? <DateView date={props.execution.requestedAt}/> 
                            : <span className="text-muted">new execution</span>}
                    </div>
                    <div>
                        {props.execution?.startedAt
                            ? <DateView date={props.execution.startedAt}/>
                            : <span className="text-muted"><span className="spinner-grow spinner-grow-sm me-1"/>pending...</span>}
                    </div>
                    <div>
                        {props.execution?.finishedAt
                            ? <DateView date={props.execution.finishedAt}/>
                            : <span className="text-muted"><span className="spinner-grow spinner-grow-sm me-1"/>pending...</span>}
                    </div>
                </div>
            </div>
            <div className="col w-100">
                <div><b>Comment:</b></div>
                <code><pre className="overflow-auto">{props.execution?.comment}</pre></code>
            </div>
        </div>  
    );
}