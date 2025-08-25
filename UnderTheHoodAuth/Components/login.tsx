import React, { useState } from 'react';
import * as ReactDOM from 'react-dom';

declare const LOGIN_MODEL: any;

const enum LoginState {
    Loading,
    LoginPage
}

export interface State {
    model: any;
    username: string;
    password: string;
    loginState: LoginState
}

const Login = () => {
    const [state, setState] = useState<State>({
        model: LOGIN_MODEL,
        username: null,
        password: null,
        loginState: LoginState.Loading
    });

    //return state.loginState === LoginState.Loading ? <h2>Loading</h2> :
    //    <>
    //        <div>
    //            <form id="primaryForm" name="primaryForm" method="post" action="Login">
    //                <input
    //                    name={'username'}
    //                    id={'username'}
    //                    value={state.username || ''}
    //                    required={true}
    //                    onChange={value => {
    //                        setState({ ...state, username: 'HI' });
    //                    }}
    //                    autoFocus
    //                />
    //                <input
    //                    name={'password'}
    //                    id={'password'}
    //                    value={state.password || ''}
    //                    required={true}
    //                    onChange={value => {
    //                        setState({ ...state, password: 'HI' });
    //                    }}
    //                />
    //                <button title={"Log in"} />
    //            </form>
    //        </div>
    //    </>       
    return <h2>Thor</h2>
}

ReactDOM.render(<Login />, document.getElementById('root') || document.createElement('div'));
