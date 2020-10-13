module.exports = function (config) {
    config.set({
        frameworks: ['jasmine'],
        browsers: ['ChromeDebugging'],
        files: [
            './dist/**/*.js',
        ],
        preprocessors: {
            './dist/src/**/*.js': ['coverage'],
            './dist/**/*.js': ['sourcemap']
        },
        customLaunchers: {
            ChromeDebugging: {
                base: 'Chrome',
                flags: ['--remote-debugging-port=9333']
            }
        },
        reporters: ['progress', 'coverage', 'remap-coverage', 'junit'],
        junitReporter: {
            outputDir: './dist/test/reports',
            suite: 'powerapps-specflow-bindings',
            useBrowserName: true,
        },
        coverageReporter: {
            type: 'in-memory'
        },
        remapCoverageReporter: {
            html: './dist/test/reports/coverage/html',
            cobertura: './dist/test/reports/coverage/cobertura.xml'
        }
    });
};