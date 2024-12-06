import { PropsWithChildren } from "react";
import styles from "./main-layout.module.css";

export default function MainLayout({ children }: PropsWithChildren) {
    return <div className={styles["container"]}>
        <h1>SJA In Numbers</h1>
        {children}
    </div>
}
