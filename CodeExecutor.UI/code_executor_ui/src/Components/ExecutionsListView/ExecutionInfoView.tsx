import React from "react";

import DateView from "../Shared/DateView";
import ExecutionStatusView from "../Shared/ExecutionStatusView";
import useWindowDimensions from "../Shared/UseWindowSize";
import ExecutionInfoOptions from "./ExecutionInfoOptions";
import "./ExecutionListView.css";


export default function ExecutionInfoView(params: ExecutionInfoOptions){
    const [wWidth, wHeight] = useWindowDimensions();
    
    function drawDate(){
        if (wWidth > 200){
            if (params.execution.finishedAt)
                return <React.Fragment>finished <DateView date={params.execution.finishedAt}/></React.Fragment>
            else if (params.execution.startedAt)
                return <React.Fragment>started <DateView date={params.execution.startedAt}/></React.Fragment>
            else if (params.execution.requestedAt)
                return <React.Fragment>requested <DateView date={params.execution.requestedAt}/></React.Fragment>
        }
        else{
            if (params.execution.finishedAt)
                return <DateView date={params.execution.finishedAt}/>
            else if (params.execution.startedAt)
                return <DateView date={params.execution.startedAt}/>
            else if (params.execution.requestedAt)
                return <DateView date={params.execution.requestedAt}/>
        }
    }
    
    function deleteExecution(){
        if (params.onDelete && params.execution.id)
            params.onDelete(params.execution.id);
    }
    
    function selectExecution(){
        if (params.onSelect && params.execution.id)
            params.onSelect(params.execution.id);
    }
    
    
    return ( 
        <div className={`execution-info p-1 container-v ${params.isSelected ? "execution-selected" : ""}`}>
            <div className="container-h">
                <ExecutionStatusView execution={params.execution} className="me-1 p-1 flex-shrink-0"/>
                <a onClick={_ => selectExecution()} title={params.execution.id}>
                    {params.execution.id.slice(0, 8)}
                </a>
                <div className="w-100"/>
                <button onClick={_ => deleteExecution()} className="flex-grow-0 p-1 btn-close" title="Terminate execution"/>
            </div>
            <div className="small">
                {drawDate()}
            </div>
        </div>
    );
}
