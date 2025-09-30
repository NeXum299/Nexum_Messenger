import { useState } from 'react';
import Header from './Header/Header.jsx';
import Content from './Content/Content.jsx';
import Footer from './Footer/Footer.jsx';
import Registration from './Modals/Registration/Registration.jsx';
import Login from './Modals/Login/Login.jsx';
import '../../main.css';
import './AuthRender.css';

export default function AuthRender({ onLogin }) {
    const [registrationOpen, setRegistrationOpen] = useState(false);
    const [loginOpen, setLoginOpen] = useState(false);

    return (
        <>
            <Header />

            <div className="main-content">
                <Content
                    onOpenLogin={() => {
                        setLoginOpen(true);
                        setRegistrationOpen(false);
                    }}
                    onOpenRegistration={() => {
                        setRegistrationOpen(true);
                        setLoginOpen(false);
                    }} />

                {registrationOpen && (
                    <Registration
                        isOpen={registrationOpen}
                        onClose={() => setRegistrationOpen(false)}
                        onLogin={onLogin} />
                )}

                {loginOpen && (
                    <Login
                        isOpen={loginOpen}
                        onClose={() => setLoginOpen(false)}
                        onLogin={onLogin} />
                )}
            </div>

            <Footer />

        </>
    );
}
