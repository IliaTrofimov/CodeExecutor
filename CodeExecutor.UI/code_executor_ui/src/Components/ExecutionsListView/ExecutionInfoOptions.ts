import ExecutionInfo from "../../Models/ExecutionInfo";

export default interface ExecutionInfoOptions{
    execution: ExecutionInfo, 
    onSelect?: (guid: string) => void,
    onDelete?: (guid: string) => void,
    isSelected: boolean
}