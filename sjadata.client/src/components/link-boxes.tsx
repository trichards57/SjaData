import { PropsWithChildren } from "react";
import styles from "./link-boxes.module.css";
import { Link, ToOptions } from "@tanstack/react-router";

interface LinkBoxesProps {
  size: "large" | "small";
}

export function LinkBoxes({
  size,
  children,
}: PropsWithChildren<LinkBoxesProps>) {
  return <div className={styles[`link-boxes-${size}`]}>{children}</div>;
}

export function LinkBox({
  children,
  color,
  ...rest
}: PropsWithChildren<
  ToOptions & {
    color: "green" | "dark-green" | "yellow" | "black" | "light-gray";
  }
>) {
  return (
    <Link className={`${styles["link-box"]} ${styles[color]}`} {...rest}>
      {children}
    </Link>
  );
}