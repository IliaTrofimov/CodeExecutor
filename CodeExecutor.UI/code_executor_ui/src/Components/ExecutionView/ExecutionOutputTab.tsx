import ExecutionInfo from "../../Models/ExecutionInfo";
import LoadingSpinner from "../Shared/LoadingSpinner";

export default function ExecutionOutputTab(props: {execution: ExecutionInfo|undefined}){
    return (
        <div className="tab-container">
            {props.execution != null 
                ? props.execution.data != null
                    ? <code><pre className="overflow-auto execution-output">{props.execution?.data}</pre></code>
                    : <div className="text-center">
                        <b>Empty</b><br/>
                        <span className="text-muted">The execution has never finished or results have been lost.</span>
                    </div>
                : <LoadingSpinner/>
            }
        </div>
    )
}
