import { faHouse } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import styles from "./home-link.module.css";

export default function HomeLink() {
    return <h3>
        <a href="/">
            <FontAwesomeIcon className={styles["home-link"]} icon={faHouse} /> Go Home
        </a>
    </h3>

}