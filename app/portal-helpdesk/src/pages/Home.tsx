import { useMsal } from "@azure/msal-react";

function Home() {
    const {instance} = useMsal();

    function onLogoutClick() {
        instance.logoutRedirect({
            postLogoutRedirectUri: undefined
        });
    }

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