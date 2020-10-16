module.exports = function (config) {
    config.set({
        frameworks: ['jasmine', 'karma-typescript'],
        files: [
            'src/**/*.ts',
            'test/**/*.ts'
        ],
        mime: {
            'text/x-typescript': ['ts', 'tsx']
        },
        preprocessors: {
            '**/*.ts': 'karma-typescript'
        },
        junitReporter: {
            outputDir: 'test_results/reports',
            suite: 'powerapps-specflow-bindings',
            useBrowserName: true,
        },
        reporters: ['progress', 'karma-typescript', 'junit'],
        browsers: ['Chrome'],
        mime: {
            'text/x-typescript': ['ts', 'tsx']
        },
        karmaTypescriptConfig: {
            reports:
            {
                html: {
                    directory: 'test_results/coverage',
                    subdirectory: 'html'
                },
                lcovonly: {
                    directory: 'test_results/coverage',
                    subdirectory: 'lcov',
                    filename: 'lcov.info',
                },
                cobertura: {
                    directory: 'test_results/coverage',
                    subdirectory: 'cobertura',
                    filename: 'cobertura.xml',
                }
            }
        }
    });
};