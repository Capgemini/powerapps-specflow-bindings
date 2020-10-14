module.exports = function (config) {
    config.set({
        frameworks: ["jasmine", "karma-typescript"],
        files: [
            "src/**/*.ts",
            "test/**/*.ts"
        ],
        mime: {
            'text/x-typescript': ['ts', 'tsx']
        },
        preprocessors: {
            "**/*.ts": "karma-typescript"
        },
        reporters: ["progress", "karma-typescript"],
        browsers: ["Chrome"],
        mime: {
            'text/x-typescript': ['ts', 'tsx']
        },
    });
};