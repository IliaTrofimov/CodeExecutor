import React, { useState} from "react";
import ExecutionView from "../../Components/ExecutionView/ExecutionView";
import Sidebar from "./Sidebar";
import {Toaster} from "react-hot-toast";

export default function MainView() {
    const [selectedGuid, setSelectedGuid] = useState<string>();
    
    return (
        <div className="container-v">
            <Toaster position={"top-right"}/>
            <div className="container-h">
                <Sidebar onSelect={guid => setSelectedGuid(guid)}/>
                <ExecutionView initialGuid={selectedGuid}/>
            </div>
        </div>
    );
}
