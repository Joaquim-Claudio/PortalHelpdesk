import { Outlet } from "react-router-dom";

function DefaultLayout() {
    return (
        <div>
            <nav>My App navigation</nav>
            <header>My App header</header>
            <main>
                <Outlet />
            </main>
            <footer>An awesome footer</footer>
        </div>
    );
}

export default DefaultLayout;