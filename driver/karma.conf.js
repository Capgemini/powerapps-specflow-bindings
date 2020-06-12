module.exports = function (config) {
    config.set({
        frameworks: ['jasmine'],
        browsers: ['ChromeDebugging'],
        files: [
            './dist/*.js',
            './tests/**/*.js',
            './node_modules/jasmine-ajax/lib/mock-ajax.js'
        ],
        preprocessors: {
            './dist/specflow.driver.js': ['coverage'],
            '**/*.js': ['sourcemap']
        },
        customLaunchers: {
            ChromeDebugging: {
                base: 'Chrome',
                flags: ['--remote-debugging-port=9333']
            }
        },
        reporters: ['progress', 'coverage', 'remap-coverage', 'junit'],
        junitReporter: {
            outputDir: './tests/reports',
            suite: 'power-apps-specflow-bindings',
            useBrowserName: true,
        },
        coverageReporter: {
            type: 'in-memory'
        },
        remapCoverageReporter: {
            html: './coverage/html',
            cobertura: './coverage/cobertura/cobertura.xml'
        }
    });
};