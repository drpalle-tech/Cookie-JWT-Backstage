const webpack = require('webpack');
const helpers = require('./helpers');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const WebpackNotifierPlugin = require('webpack-notifier');

const DEV_MODE = process.env.NODE_ENV !== 'production';

let plugins = [
    new MiniCssExtractPlugin({
        filename: '[name].css'
    }),
    new webpack.DefinePlugin({ ADMIN: false, DEV_MODE })
];

if (!helpers.suppressNotifications()) {
    plugins.push(
        new WebpackNotifierPlugin({
            title: 'auth',
            alwaysNotify: true
        })
    );
}

// File names and paths are cAsE SEnSiTIvE!
module.exports =
// auth webpack
{
    name: 'auth',
    resolve: {
        extensions: ['.ts', '.tsx', '.js', '.json']
    },
    module: {
        rules: [
            {
                test: /\.tsx?$/,
                include: helpers.root('../UnderTheHoodAuth/'),
                loader: 'ts-loader',
                options: {
                    // disable type checker - we will use it in fork plugin
                    transpileOnly: true
                }
            }
        ]
    },
    entry: {
        'login-bundle': helpers.root('../UnderTheHoodAuth/Components/login.tsx')
    },
    output: {
        path: helpers.root('../UnderTheHoodAuth/wwwroot')
    },
    plugins: plugins
};
