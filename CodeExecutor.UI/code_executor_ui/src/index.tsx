import React from 'react';
import ReactDOM from 'react-dom/client';

import './index.css';
import 'bootstrap/dist/css/bootstrap.min.css';
import 'bootstrap/dist/js/bootstrap.js';
import 'bootstrap-icons/font/bootstrap-icons.css';
import MainView from "./Components/MainView/MainView";


const root = ReactDOM.createRoot(document.getElementById('root') as HTMLElement);
root.render(<MainView />);

