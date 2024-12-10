const path = require('path');

module.exports = {
    watch: false,
    entry: './src/index.ts',
    mode: 'development',
    devtool: 'inline-source-map',
    module: {
        rules: [
            {
                test: /\.tsx?$/,
                use: 'ts-loader',
                exclude: /node_modules/,
            },
        ]
    },
    resolve: {
        extensions: ['.ts', '.js'],
    },
    output: {
        filename: 'driver.js',
        path: path.resolve(__dirname, 'dist'),
        libraryTarget: 'var',
        library: 'PowerAppsSpecFlowBindings'
    },
};