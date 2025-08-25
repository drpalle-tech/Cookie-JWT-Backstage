const webpackMerge = require('webpack-merge');
const devConfig = require('../webpack.dev.js');
const authConfig = require('../webpack.auth.js');


module.exports = webpackMerge(devConfig, authConfig);