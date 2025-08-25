/// <binding />
const webpack = require("webpack");
const helpers = require("./helpers");
const TerserPlugin = require("terser-webpack-plugin");
const MiniCssExtractPlugin = require("mini-css-extract-plugin");
const ForkTsCheckerWebpackPlugin = require("fork-ts-checker-webpack-plugin");
const _ = require("lodash");

const DEV_MODE = process.env.NODE_ENV !== "production";

if (DEV_MODE) {
    console.info(
        helpers.suppressNotifications()
            ? "❌  Notifications Off (to turn off, run without -- -quiet or -- -no-notify)"
            : "✅  Notifications On (to turn off, run with -- -quiet)"
    );
    console.info(
        helpers.useCaching()
            ? "✅  Caching On (experimental: to turn off, run without -- -enable-cache)"
            : "❌  Caching Off (experimental: to turn on, run with -- -enable-cache)"
    );
}

//Please don't 'style-loader' and mini-css-extract-loader together
//Reference https://github.com/webpack-contrib/mini-css-extract-plugin/issues/613
let lessUseLoaders = [
    {
        loader: MiniCssExtractPlugin.loader,
        options: {
            esModule: false,
        },
    },
    {
        loader: "css-loader",
        options: {
            url: false,
            sourceMap: DEV_MODE,
            modules: false,
            esModule: false,
        },
    },
    {
        loader: "less-loader",
        options: {
            sourceMap: DEV_MODE,
            lessOptions: {
                relativeUrls: false,
            },
        },
    },
];

let cssUseLoaders = [
    {
        loader: MiniCssExtractPlugin.loader,
        options: {
            esModule: false,
        },
    },
    {
        loader: "css-loader",
        options: {
            url: false,
            sourceMap: DEV_MODE,
            modules: false,
            esModule: false,
        },
    },
];

let rules = [
    {
        test: /\.less$/,
        use: lessUseLoaders,
    },
    {
        test: /\.css$/,
        use: cssUseLoaders,
    },
    {
        test: /\.woff2?(\?v=\d+\.\d+\.\d+)?$/,
        loader: "url-loader",
        options: {
            limit: 10000,
            mimetype: "application/font-woff",
            name: "[name].[ext]",
        },
    },
    {
        test: /\.(ttf|eot|svg)(\?v=\d+\.\d+\.\d+)?$/,
        loader: "file-loader",
        options: {
            name: "[name].[ext]",
        },
    },
    {
        test: /\.js$/,
        exclude: /node_modules/,
        include: helpers.root("Standard"),
        loader: "babel-loader",
    },
    {
        test: /\.tsx?$/,
        include: helpers.root(),
        loader: "ts-loader",
        options: {
            // disable type checker - we will use it in fork plugin
            transpileOnly: true,
        },
    },
];

let plugins = [
    // Workaround for https://github.com/angular/angular/issues/11580
    new webpack.ContextReplacementPlugin(
        // The (\\|\/) piece accounts for path separators in *nix and Windows
        /angular(\\|\/)core(\\|\/)(@angular|esm5)/,
        helpers.root()
    ),
    new MiniCssExtractPlugin({
        filename: "../../Css/[name].css",
    }),
    new ForkTsCheckerWebpackPlugin(),
];

let filterUseLoaders = (useLoaders) => {
    return useLoaders.filter((loader) => {
        const loaderName = typeof loader === "string" ? loader : loader.loader;
        return !loaderName?.includes("mini-css-extract-plugin");
    });
};

if (DEV_MODE) {
    const prependStyleLoader = (loaders) => {
        const updatedLoaders = ["style-loader", ...loaders];
        return filterUseLoaders(updatedLoaders);
    };
    lessUseLoaders = prependStyleLoader(lessUseLoaders);
    cssUseLoaders = prependStyleLoader(cssUseLoaders);
}

// Configuration paths and file names are cAsE SEnSiTIvE
let conf = {
    // Common for all configurations
    resolve: {
        modules: [helpers.root(), "node_modules"],
        symlinks: false,
        extensions: [".ts", ".tsx", ".js", ".json"],
        alias: {
            "angular-animate": helpers.root(
                "node_modules/angular-animate/angular-animate"
            ),
            "kendo.all.min": helpers.root("Core/Libraries/kendo/js/kendo.all.min"),
            "esri-leaflet": helpers.root(
                "node_modules/esri-leaflet/dist/esri-leaflet"
            ),
            "leaflet-bing-layer": helpers.root(
                "Core/Libraries/Leaflet/leaflet-bing-layer"
            ),
            "leaflet-spiderfy": helpers.root(
                "Core/Libraries/Leaflet/leaflet-spiderfy"
            ),
            "leaflet-coordinates": helpers.root(
                "node_modules/leaflet.coordinates/dist/Leaflet.Coordinates-0.1.5.min"
            ),
        },
    },
    module: {
        rules: rules,
    },
    watchOptions: {
        ignored: /node_modules/,
    },
    stats: {
        colors: true,
        errorDetails: true,
    },
    node: {
        __dirname: false,
        __filename: false,
        global: true,
    },
    optimization: {
        minimizer: [
            new TerserPlugin({
                terserOptions: {
                    mangle: false,
                },
                parallel: true,
            }),
        ],
        splitChunks: {
            chunks: "async",
            minSize: 30000,
            maxSize: 0,
            minChunks: 1,
            maxAsyncRequests: 5,
            maxInitialRequests: 3,
            automaticNameDelimiter: "~",
            name: false,
            cacheGroups: {
                vendors: {
                    test: /[\\/]node_modules[\\/]/,
                    priority: -10,
                },
                default: {
                    minChunks: 2,
                    priority: -20,
                    reuseExistingChunk: true,
                },
            },
        },
    },
    plugins: plugins,
};

module.exports = conf;
