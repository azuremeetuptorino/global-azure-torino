const AssetsPlugin = require("assets-webpack-plugin");
module.exports = {
  // ...
  webpack: {
    alias: {
      /* ... */
    },
    output: {
      uniqueName: "istantquote-app",
      publicPath: "",
    },
    plugins: {
      add: [
        new AssetsPlugin({
          fullPath: false,
          removeFullPathAutoPrefix: true,
          useCompilerPath: true,
        }),
      ],
      remove: [
        /* ... */
      ],
    },
    configure: {
      /* ... */
    },
    configure: (webpackConfig, { env, paths }) => {
      /* ... */
      return webpackConfig;
    },
  },
};
