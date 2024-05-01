import ExecutionInfo from "../../Models/ExecutionInfo";

export default function ExecutionStatusView(params: {execution?: ExecutionInfo, className?: string}){
    if (params.execution?.isError)
        return <span className={`badge bg-danger execution-status ${params.className}`} title="Failed"><i className="bi bi-exclamation-triangle-fill"></i></span>
    else if (params.execution?.finishedAt != null)
        return <span className={`badge bg-success execution-status ${params.className}`} title="Done"><i className="bi bi-check-circle-fill"></i></span>
    else if (params.execution?.startedAt != null)
        return <span className={`badge bg-primary execution-status ${params.className}`} title="Active"><i className="bi bi-gear-fill"></i></span>
    else if (params.execution?.requestedAt != null)
        return <span className={`badge bg-secondary execution-status ${params.className}`} title="Pending"><i className="bi bi-stopwatch-fill"></i></span>
    else if (!params.execution)
        return <span className="ms-1 spinner-grow spinner-grow-sm"/>;
    return null;
}