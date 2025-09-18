import { PublicClientApplication } from "@azure/msal-browser";

const msalConfig = {
    auth: {
        clientId: "d4b36c10-5b12-48d7-af65-5953d5be7097", // ClientId da SPA
        authority: "https://login.microsoftonline.com/a972590c-a46c-46e1-b077-bdbfbe97b233",
        redirectUri: "http://localhost:5173/auth" // ou localhost em dev
    }
};

export const msalInstance = new PublicClientApplication(msalConfig);

export const loginRequest = {
    scopes: ["api://b4253df8-cfe5-4ff4-b919-b7610749724c/Access.AsUser"]
};
