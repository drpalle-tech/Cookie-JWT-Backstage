const webpackMerge = require("webpack-merge");
const webpack = require("webpack");
const helpers = require("./helpers");
const commonConfig = require("./webpack.common.js");
const HardSourceWebpackPlugin = require("hard-source-webpack-plugin");
const localhost = "localhost:55441"; // used in dev mode

const DEV_MODE = process.env.NODE_ENV !== "production";
let plugins = [new webpack.HotModuleReplacementPlugin()];

if (DEV_MODE && helpers.useCaching()) {
    plugins.push(new HardSourceWebpackPlugin());
}

module.exports = webpackMerge(commonConfig, {
    // Dev Configuration
    output: {
        filename: "[name].js",
        chunkFilename: "[id].chunk.js",
        hotUpdateChunkFilename: "hot/hot-update.js",
        hotUpdateMainFilename: "hot/hot-update.json",
    },
    mode: "development",
    devtool: "eval-source-map",
    plugins: plugins,
    devServer: {
        hot: true,
        proxy: {
            "*": {
                target: "http://" + localhost,
                secure: false,
            },
        },
        port: 8082,
        host: "0.0.0.0",
        stats: "minimal",
    },
});
