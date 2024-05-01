import {useEffect, useState} from "react";
import CodeEditor from '@uiw/react-textarea-code-editor';

import Language from "../../Models/Language";
import ExecutionInfo from "../../Models/ExecutionInfo";
import ExecutionsService from "../../Services/ExecutionsService";
import LanguagesService from "../../Services/LanguagesService";
import ExecutionResultsView from "./ExecutionResultsView";
import "./ExecutionView.css"

export default function ExecutionView(params: {initialGuid: string|undefined}) {
    const [sourceCode, setSourceCode] = useState<string>("");
    const [language, setLanguage] = useState<number>();
    const [languages, setLanguages] = useState<Language[]>(); 
    const [result, setResult] = useState<ExecutionInfo|undefined>({id: "", isError: false});
    
    let refreshTimer: NodeJS.Timer|null = null;
    const executionsService = ExecutionsService.getInstance();
    const languagesService = LanguagesService.getInstance();
    
    useEffect(() => {
        resetTimer();
        getLanguages();

        if (params.initialGuid){
            setResult(undefined);
            getSourceCode(params.initialGuid);
            getResults(params.initialGuid);
        }
        
        return resetTimer;
    }, [params.initialGuid]);
    
    function resetTimer(){
        if (refreshTimer != null){
            clearInterval(refreshTimer);
            refreshTimer = null;
        }
    }
    
    function createBoilerplate(){
        setSourceCode(
            `using System;\nConsole.Write("Hello world!");\nfor (int i = 0; i < 10; i++) {\n\tConsole.WriteLine($"Hello {i} times!");\n}`);
    }
    
    async function getLanguages(){
        const result = await languagesService.getLanguages();
        if (result){
            setLanguages(result);
            setLanguage(result[0].id);
        }   
    }
    
    async function getSourceCode(guid: string){
        const result = await executionsService.getSourceCode(guid);
        if (result){
            setSourceCode(result);
        }
    }

    async function getResults(guid: string){
        const result = await executionsService.getResult(guid, true);
        if (result){
            setResult(result);
            
            if (result.language)
                setLanguage(result.language.id);
            
            if (!result.isError && !result.finishedAt){
                resetTimer();
                refreshTimer = setInterval(getResults, 500, guid);
            }
            else if (result.isError || result.finishedAt){
                resetTimer();
            }
        }
    }
    
    async function executeSourceCode(){
        if (!language)
            return;
        
        const result = await executionsService.execute(sourceCode, language);
        if (result){
            setResult({ 
                id: result.id, 
                requestedAt: new Date(),
                isError: false,
                language: { id: language, name: languages?.find(x => x.id == language)?.name ?? "" }
            });
            
            resetTimer();
            refreshTimer = setInterval(getResults, 500, result.id);
        }
    }
    
    function selectLanguage(value: number){
        const name = languages?.find(x => x.id == value)?.name ?? "";
        setLanguage(value);
        if (result){
            setResult({...result, language: {id: value, name: name}});
        }
    }
    
    return (
        <div className="container-v w-100">
            <div className="execution-header">
                <button onClick={_ => executeSourceCode()} className={"flex-shrink-0 btn-success btn badge mx-1"} disabled={language == 0}>
                    <i className="bi bi-play-fill"></i> Run
                </button>
                <div>
                    <select className="languages-selector" 
                            value={language} onChange={event => selectLanguage(+event.target.value)} disabled={!language}>
                        <option value="0" key="0">Select language</option>
                        {languages?.map(l =>
                            <option value={l.id} key={l.id}>{l.name}</option>
                        )}
                    </select>
                </div>
                <div className="w-100"/>
                <span className="flex-shrink-0 mx-3 font-monospace small">
                    {result?.id ?? "new execution"}
                </span>
                <button onClick={_ => createBoilerplate()} className="flex-shrink-0 btn-secondary btn badge" title="Try demo">
                    <i className="bi bi-braces-asterisk"></i>
                </button>
                <button onClick={_ => navigator.clipboard.writeText(sourceCode)} className="flex-shrink-0 btn-secondary btn badge mx-1" title="Copy to clipboard">
                    <i className="bi bi-clipboard-fill me"></i>
                </button>
                <button onClick={_ => setSourceCode("")} className="flex-shrink-0 btn-danger btn badge" title="Clear text">
                    <i className="bi bi-trash3-fill"></i>
                </button>
            </div>
            
            <CodeEditor className="overflow-auto h-100 font-monospace"
                        value={sourceCode} onChange={event => setSourceCode(event.target.value)}
                        language={result?.language?.name} data-color-mode="light" placeholder="Enter your code here..." 
            />
            
            <ExecutionResultsView result={result}/>
        </div>
    );
}