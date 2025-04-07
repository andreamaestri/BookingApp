import { useState } from 'react';
import BookingList from './components/BookingList';
import './App.css';

function App() {
    const [activeView, setActiveView] = useState('bookings');

    const renderView = () => {
        switch (activeView) {
            case 'bookings':
                return <BookingList />;
            default:
                return <div>Select a view from the navigation</div>;
        }
    };

    return (
        <div className="app-container">
            <header className="app-header">
                <h1>BookingApp</h1>
                <nav className="main-nav">
                    <button 
                        className={activeView === 'bookings' ? 'active' : ''} 
                        onClick={() => setActiveView('bookings')}
                    >
                        Bookings
                    </button>
                    <button 
                        className={activeView === 'accommodations' ? 'active' : ''} 
                        onClick={() => setActiveView('accommodations')}
                    >
                        Accommodations
                    </button>
                    <button 
                        className={activeView === 'profile' ? 'active' : ''} 
                        onClick={() => setActiveView('profile')}
                    >
                        Profile
                    </button>
                </nav>
            </header>
            
            <main className="app-content">
                {renderView()}
            </main>
            
            <footer className="app-footer">
                <p>&copy; {new Date().getFullYear()} BookingApp - All rights reserved</p>
            </footer>
        </div>
    );
}

export default App;