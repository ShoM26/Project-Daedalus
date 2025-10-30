import { useNavigate } from 'react-router-dom';
import '../styles/LandingPage.css';

function LandingPage() {

    const navigate = useNavigate();
    const handleLogin = () =>{
        navigate('/login');
    };
    const handleSignup = () => {
        navigate('/signup');
    };

    return (
        <div className="landing-container">
            <div className="landing-card">
                <div className="landing-header">
                    <h1>Welcome</h1>
                </div>
                <div className="landing-body">
                    <button className="landing-button" onClick={handleLogin} type="button">Login</button>
                    <button className="landing-button" onClick={handleSignup} type="button">Sign up</button>
                </div>
            </div>
        </div>
    );
}
export default LandingPage;