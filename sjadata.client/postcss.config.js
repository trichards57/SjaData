import stylelint from "stylelint";
import colorguard from "colorguard";
import autoprefixer from "autoprefixer";

export default {
    plugins: [
        stylelint(),
        colorguard(),
        autoprefixer(),
    ]
}
