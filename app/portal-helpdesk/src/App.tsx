import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import { MsalProvider, useIsAuthenticated, useMsalAuthentication } from "@azure/msal-react";
import { InteractionType } from "@azure/msal-browser";
import { msalInstance, loginRequest } from "./auth/authConfig";

import DefaultLayout from "./layouts/DefaultLayout";

import Home from "./pages/Home";
import Error from "./pages/Error";

function AppRoutes() {
  const { result, error } = useMsalAuthentication(InteractionType.Redirect, loginRequest);
    
  const isAuthenticated = useIsAuthenticated();

  if(error) {
    console.error("Authentication error", error);
    return <Navigate to={"/error"} replace />
  }

  if(!isAuthenticated && !result) {
    return (<div>Loading...</div>)
  }

  return (
    <Routes>
      <Route path="/" element={<DefaultLayout />}>
        <Route index element={<Home />} />
      </Route>

      <Route path="/error" element={<Error />} />
      <Route path="*" element={<Navigate to="/error" replace />} />
    </Routes>
  );
}

export default function App() {
  return (
    <MsalProvider instance={msalInstance}>
      <BrowserRouter>
        <AppRoutes />
      </BrowserRouter>
    </MsalProvider>
  );
}
