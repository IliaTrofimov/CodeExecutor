import {useState} from "react";

import ExecutionInfo from "../../Models/ExecutionInfo";
import ExecutionInfoTab from "./ExecutionInfoTab";
import ExecutionOutputTab from "./ExecutionOutputTab";
import "./ExecutionView.css";
import ExecutionStatusView from "../Shared/ExecutionStatusView";


enum Tabs{
    None, Output, Info
}


export default function ExecutionResultsView(params: {result: ExecutionInfo|undefined}){
    const [selectedTab, setTab] = useState<Tabs>(Tabs.None);

    
    
    function drawTabs(){
        switch (selectedTab){
            case Tabs.Info: return <ExecutionInfoTab execution={params.result}/>;
            case Tabs.Output: return <ExecutionOutputTab execution={params.result}/>;
            default: return undefined;
        }
    }
    
    function tabClass(tab: Tabs){
        return "tab-header" + (tab == selectedTab ? " tab-selected": "");
    }
    
    function selectTab(tab: Tabs){
        setTab(selectedTab != tab ? tab : Tabs.None);
    }
    
    return (
        <div className="container-v execution-results">
            <div className="container-h">
                <button className={tabClass(Tabs.Output)}
                        onClick={_ => selectTab(Tabs.Output)}>
                    Output {params.result?.id && <ExecutionStatusView execution={params.result}/>}
                </button>
                <button className={tabClass(Tabs.Info)}
                        onClick={_ => selectTab(Tabs.Info)}>
                    Info
                </button>
                <div className="tab-filler w-100"/>
                {selectedTab != Tabs.None &&  
                    <button className="tab-header" onClick={_ => selectTab(Tabs.None)}>
                        <i className="bi bi-dash-lg"></i>
                    </button>}
            </div>
            {drawTabs()}
        </div>
    );
}