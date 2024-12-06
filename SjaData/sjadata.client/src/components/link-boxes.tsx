import { PropsWithChildren } from "react";
import { NavLink } from "react-router";
import styles from "./link-boxes.module.css";

type LinkColor = "dark-green" | "yellow" | "black" | "light-gray" | "green"

export function LinkBoxes({ children, size }: PropsWithChildren<{ size: "large" | "small" }>) {
    const style = styles["link-boxes"] + " " + (size === "large" ? styles["link-boxes-large"] : styles["link-boxes-small"]);

    return <div className={style}>
        {children}
    </div>
}

export function LinkBox({ children, to, colour = "green" }: PropsWithChildren<{ to?: string; colour?: LinkColor }>) {
    const style = styles["link-box"] + " " + styles["link-box-" + colour];

    if (to) {
        return <NavLink className={style} to={to}>
            {children}
        </NavLink>
    }
    else {
        <div className={style}>
            {children}
        </div>
    }
}
