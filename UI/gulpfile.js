const gulp = require('gulp');
const esbuild = require('esbuild');
const fs = require('fs-extra'); // fs-extra is an extended version of Node's fs module

gulp.task('build-jsx', function (done) {
    return esbuild.build({
        entryPoints: ['src/ui.jsx'],
        outdir: '../Code/Resources',
        bundle: true,
        platform: 'browser',
        loader: {
            '.js': 'jsx',
            '.jsx': 'jsx'
        }
        // Add other esbuild options as needed
    }).then(() => {
        console.log("UI built and copied to ../Code/Resources")
    }).catch((error) => {
        console.error(error);
        done(new Error('Build failed'));
    });
});

gulp.task('default', gulp.series('build-jsx'));
