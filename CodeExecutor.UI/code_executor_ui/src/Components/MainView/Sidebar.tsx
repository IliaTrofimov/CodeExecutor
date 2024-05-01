import "./MainView.css"
import {useEffect, useState} from "react";
import UserView from "./UserView";
import ExecutionsListView from "../ExecutionsListView/ExecutionsListView";

enum SidebarSelection {
    None, Account, Execution, Both
}

export default function Sidebar(params: {onSelect?: (guid: string) => void}){
    const [selection, setSelection] = useState<SidebarSelection>(SidebarSelection.None);
    
    function selectSidebar(s: SidebarSelection){
        setSelection(selection ^ s);
    }
    
    function isSelected(s: SidebarSelection){
        return (s & selection) == s;
    }
    
    function sidebarClass(s: SidebarSelection){
        return isSelected(s) ? "sidebar-button sidebar-button-selected" : "sidebar-button";
    }
    
    return (
        <div className="container-v col-sm-3 col-md-2 bg-dark text-light sidebar">
            <div className="sidebar-header sidebar-item noselect">
                <i className="bi bi-code-slash me-sm-1 me-md-2"></i>
                Code <i>E</i>xecutor
            </div>
            
            <button onClick={_ => selectSidebar(SidebarSelection.Account)} 
                    className={sidebarClass(SidebarSelection.Account)}>
                <i className="bi bi-person-badge-fill me-sm-1 me-md-2"></i>
                Account
            </button>
            {isSelected(SidebarSelection.Account) && <UserView/>}
            
            <button onClick={_ => selectSidebar(SidebarSelection.Execution)}
                    className={sidebarClass(SidebarSelection.Execution)}>
                <i className="bi bi-card-list me-sm-1 me-md-2"></i>
                Executions
            </button>
            <ExecutionsListView onSelect={params.onSelect} collapsed={!isSelected(SidebarSelection.Execution)}/>
        </div>
    );
}