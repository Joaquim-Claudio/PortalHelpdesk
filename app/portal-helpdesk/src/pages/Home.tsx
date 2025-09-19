import { useMsal } from "@azure/msal-react";
import { useEffect } from "react";
import axios from "axios";
import { loginRequest } from "../auth/authConfig";

const users = axios.create({
    baseURL: import.meta.env.VITE_USERS_BASE_URL
})

function Home() {
    const {instance, accounts} = useMsal();

    function onLogoutClick() {
        instance.logoutRedirect({
            postLogoutRedirectUri: undefined
        });
    }

    async function fetchUserData() {
        if(!accounts || accounts.length === 0) return

        try {
            const request = {
                ...loginRequest,
                account: accounts[0]
            }
    
            const response = await instance.acquireTokenSilent(request);
    
            const userData = await users.get("byemail", { 
                params: {email: accounts[0].username},
                headers: {
                    Authorization: `Bearer ${response.accessToken}`
                }
            })
    
            console.log(userData);

        } catch (error) {
            console.error("Failed to fetch user data.", error)
        }

    }

    useEffect(() => {
        fetchUserData();
    }, [accounts])

    return (
        <div>
            <h1>This is the home page</h1>
            <button type="button" onClick={onLogoutClick}>
                Logout
            </button>
        </div>
    );
}

export default Home;