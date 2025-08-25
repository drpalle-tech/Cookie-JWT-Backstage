import React, { useState } from 'react';
import * as ReactDOM from 'react-dom';
var LoginState;
(function (LoginState) {
    LoginState[LoginState["Loading"] = 0] = "Loading";
    LoginState[LoginState["LoginPage"] = 1] = "LoginPage";
})(LoginState || (LoginState = {}));
const Login = () => {
    const [state, setState] = useState({
        model: LOGIN_MODEL,
        username: null,
        password: null,
        loginState: 0
    });
    return React.createElement("h2", null, "Thor");
};
ReactDOM.render(React.createElement(Login, null), document.getElementById('root') || document.createElement('div'));
//# sourceMappingURL=login.js.map