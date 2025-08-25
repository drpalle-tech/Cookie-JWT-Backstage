var path = require('path');

var _root = path.resolve(__dirname, '..');

function root(args) {
    args = Array.prototype.slice.call(arguments, 0);
    return path.join.apply(path, [_root].concat(args));
}

function suppressNotifications() {
    return process.argv.some(arg => arg.indexOf('no-notify') !== -1 || arg.indexOf('quiet') !== -1);
}

function useCaching() {
    return process.argv.some(arg => arg.indexOf('enable-cache') !== -1);
}

module.exports = {
    root,
    suppressNotifications,
    useCaching
};
