import AuthService from "../../Services/AuthService";
import {useEffect, useState} from "react";

export default function UserView(){
    const auth = AuthService.getInstance();
    const [username, setUsername] = useState<string>("");
    const [password, setPassword] = useState<string>("");
    const [remember, setRemember] = useState<boolean>(false);
    
    const [message, setMessage] = useState<string>();
    const [isAuthenticated, setAuthenticated] = useState<boolean>(() => auth.isAuthenticated());

    useEffect(() => {
        getUser();
    }, []);
    
    function canLogin(){
        return username && password;
    }
    
    async function getUser(){
        const response = await auth.me();
        if (response){
            if (auth.isAuthenticated())
                setUsername(auth.getUsername() ?? ""); 
            else
                setMessage(response);
        }
    } 
    
    async function login(){
        const response = await auth.login(username, password, remember);
        if (response)
            setMessage(response);
        setAuthenticated(auth.isAuthenticated());
    } 
    
    function logout(){
        auth.logout();
        setAuthenticated(false);
        setUsername("");
        setPassword("");
    }
    
    return isAuthenticated 
        ? (
            <div className="mb-3 sidebar-item">
                <span className="me-sm-1 me-2">
                    {auth.isUserAdmin() ? <b>*</b> : undefined}{auth.getUsername()}
                </span>
                <button className="btn btn-danger badge" onClick={_ => logout()}>Logout</button>
            </div>
        )
        : (
            <div className="mb-3 sidebar-item">
                <div className="mb-2">
                    <span className="me-sm-1 me-2 noselect">Unknown user</span>
                    <button className="btn btn-primary badge" disabled={!canLogin()}
                        onClick={_ => login()}>
                        Login
                    </button>
                </div>
                <form>
                    <input type="text" className="user-input" placeholder="Username"
                           value={username} onChange={event => setUsername(event.target.value)}/>
                    <input type="password" className="user-input" placeholder="Password"
                           value={password} onChange={event => setPassword(event.target.value)} />
                    <div className="small mt-2 form-check">
                        <input className="form-check-input" type="checkbox" checked={remember} onChange={_ => setRemember(x => !x)}/>
                        <label className="form-check-label noselect">Remember me</label>
                    </div>
                    {message &&
                        <div className="small text-danger">{message}</div>
                    }
                </form>
            </div>
        );
}