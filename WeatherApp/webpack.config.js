const ClosureCompilerPlugin = require('webpack-closure-compiler');

const path = require("path");
const webpack = require('webpack');
const BROWSER = process.env.BROWSER;
// const features = require('./src/features/' + BROWSER);

const config = {
    context: __dirname,
    entry: {
        index: [
            "./FrontEnd/index.ts",
        ]
       ,viewHistory: [
            "./FrontEnd/viewHistory.ts",
        ]
    },
    output: {
        path: __dirname + '/wwwroot/js',
        pathinfo: true,
        filename: '[name].js',
        // publicPath: '',
    },
    resolve: {
        extensions: ['.tsx', '.ts', '.js', '.json'],
        modules: ['node_modules']
    },
    externals: {
    },
    resolveLoader: { 
        alias: {
            'copy': 'file-loader?name=[path][name].[ext]&context=./assets/',
        }
    },
    plugins: [
        new webpack.NoEmitOnErrorsPlugin(), // don't reload if there is an error
        new webpack.DefinePlugin(Object.assign({}, /* features, */ {TARGET: JSON.stringify(process.env.TARGET)}))
    ],
    module: {
        rules: [
            {enforce: 'pre', test: /\.ts(x?)$/, loader: "tslint-loader" /*, does not work - there is an issue - https://github.com/wbuchwalter/tslint-loader/issues/57 options: {typeCheck: true} */},
            {test: /\.ts(x?)$/, exclude: /node_modules/, loader: 'ts-loader'},
        ]
    }
};

    config.plugins.push(

        new ClosureCompilerPlugin({
            compiler: {
/*                externs: "", */
                language_in: 'ECMASCRIPT6',
                language_out: 'ECMASCRIPT5',
                compilation_level: 'SIMPLE'
            },
            concurrency: 3
        })
    );

if(process.env.TARGET !== 'production'){
    config.devtool = 'source-map';
}

module.exports = config;
