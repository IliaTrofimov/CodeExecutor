import {useEffect, useState} from "react";
import ExecutionsService from "../../Services/ExecutionsService";
import AuthService from "../../Services/AuthService";
import ExecutionInfo from "../../Models/ExecutionInfo";
import ExecutionInfoView from "./ExecutionInfoView";
import LoadingSpinner from "../Shared/LoadingSpinner";


export default function ExecutionsListView(params: {onSelect?: (guid: string) => void, collapsed: boolean}){
    const api = ExecutionsService.getInstance();
    const auth = AuthService.getInstance();
    
    const [selected, setSelected] = useState<string|undefined>();
    const [executions, setExecutions] = useState<ExecutionInfo[]|undefined>();
    let refreshTimer: NodeJS.Timer|null = null;

    useEffect(() => {
        console.log("[useEffect] ExecutionsListView");
        
        resetTimer();
        auth.addOnLogin("ExecutionsListView", onLogin);
        api.addOnExecutionUpdated("ExecutionsListView", onExecute);
        
        getExecutions(false);
        return () => {
            console.log("[dismount] ExecutionsListView");
            auth.removeOnLogin("ExecutionsListView");
            api.removeOnExecutionUpdated("ExecutionsListView");
            resetTimer();
        }
    }, []);

    function resetTimer(){
        if (refreshTimer != null){
            clearInterval(refreshTimer);
            refreshTimer = null;
        }
    }
    
    function onLogin(isLoggedIn: boolean){
        console.log(`ExecutionsListView handle onLogin with isLoggedIn =`, isLoggedIn);
        isLoggedIn ? getExecutions(true) : setExecutions([]);
    }
    
    async function onExecute(info: ExecutionInfo){
        console.log(`ExecutionsListView handle onExecute with ExecutionInfo.Id =`, info.id.slice(0, 8));
        //console.log(`ExecutionsListView handle onExecute executions`, executions);
        //console.log(`ExecutionsListView handle onExecute with temp =`, temp);
        const temp = executions ? [info, ...executions] : [info];
        setRefreshTimer(temp);
    }
    
    async function getExecutions(force: boolean){
        const result = await api.getExecutions(force);
        if (result) {
            setExecutions(result);
            setRefreshTimer(result);
        }
    }
    
    function setRefreshTimer(data: ExecutionInfo[]){
        const unfinished = data.filter(ExecutionsService.isUnfinished).map(x => x.id);
        
        if (unfinished.length > 0){
            console.log(`Executions are not finished, setting timer`, unfinished);
            refreshTimer = setTimeout(() => getExecutions(true), 1000);
        }
        else{
            console.log(`All ${data.length} executions are finished, deleting timer`);
            resetTimer();
        }
    }
    
    async function deleteExecution(guid: string){
        if (await api.delete(guid)){
            const result = await api.getExecutions(true);
            if (result) {
                setExecutions(result);
                setRefreshTimer(result);
            }
        }
    }
    
    function select(guid: string){
        setSelected(guid);
        if (params.onSelect)
            params.onSelect(guid);
    }
    
    function listClass(){
        return params.collapsed 
            ? "list-group list-group-flush overflow-auto styled-scrollbars d-none"
            : "list-group list-group-flush overflow-auto styled-scrollbars";
    }
    
    return executions ?
        executions.length == 0 ?
            <div className="text-center noselect">Empty</div>
            :
            <div className={listClass()}>
                <div className="small mb-2 text-center">
                    Found <i>{executions.length} executions</i>&nbsp;
                    <a className="small-button" onClick={_ => getExecutions(true)} title="Update executions list">
                        <i className="bi bi-arrow-clockwise"></i>
                    </a>
                </div>
                {executions?.map(e => 
                    <ExecutionInfoView key={e.id} execution={e} onSelect={select} onDelete={deleteExecution} 
                                       isSelected={selected == e.id}/>
                )}
            </div>
        :
        <LoadingSpinner/>
    ;
}