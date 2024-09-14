import { faHouse } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { createFileRoute, Link } from "@tanstack/react-router";
import styles from "./update.module.css";
import { useState } from "react";
import { usePeopleUploader } from "../loaders/people-uploader";
import { useHoursUploader } from "../loaders/hours-uploader";

function dragOver(event: React.DragEvent<HTMLDivElement>) {
  event.preventDefault();
  event.stopPropagation();

  event.currentTarget.classList.add(styles["drag-over"]);

  event.dataTransfer.dropEffect =
    event.dataTransfer.items.length === 1 ? "copy" : "none";
}

function dragLeave(event: React.DragEvent<HTMLDivElement>) {
  event.preventDefault();
  event.stopPropagation();

  event.currentTarget.classList.remove(styles["drag-over"]);
}

export function UpdatePage() {
  const [showInvalidFile, setShowInvalidFile] = useState(false);
  const [showUploading, setShowUploading] = useState(false);
  const [showUploadFailed, setShowUploadFailed] = useState(false);
  const [showUploadSuccess, setShowUploadSuccess] = useState(false);
  const [uploadCount, setUploadCount] = useState(0);
  const [uploadItem, setUploadItem] = useState("");

  const uploadPeople = usePeopleUploader();
  const uploadHours = useHoursUploader();

  function fileDrop(event: React.DragEvent<HTMLDivElement>) {
    event.stopPropagation();
    event.preventDefault();

    event.currentTarget.classList.remove(styles["drag-over"]);

    if (event.dataTransfer.files.length !== 1) {
      return;
    }

    handleFile(event.dataTransfer.files[0]);
  }

  async function pickFile() {
    const [fileHandle] = await window.showOpenFilePicker({
      types: [
        {
          accept: { "text/csv": [".csv"] },
          description: "CSV Files",
        },
      ],
    });
    const file = await fileHandle.getFile();
    handleFile(file);
  }

  async function handleFile(file: File) {
    const firstBytes = await file.slice(0, 6, "text/plain").text();

    if (firstBytes.toUpperCase() === "MYDATA") {
      setShowUploading(true);
      try {
        const result = await uploadPeople(file);

        if (result) {
          setShowUploadSuccess(true);
          setUploadCount(result.count);
          setUploadItem("people");
          setTimeout(() => setShowUploadSuccess(false), 3000);
        } else {
          setShowUploadFailed(true);
          setTimeout(() => setShowUploadFailed(false), 3000);
        }
      } catch {
        setShowUploadFailed(true);
        setTimeout(() => setShowUploadFailed(false), 3000);
      } finally {
        setShowUploading(false);
      }
    } else if (firstBytes.toUpperCase() === "LOCATI") {
      setShowUploading(true);
      try {
        const result = await uploadHours(file);

        if (result) {
          setShowUploadSuccess(true);
          setUploadCount(result.count);
          setUploadItem("hours");
          setTimeout(() => setShowUploadSuccess(false), 3000);
        } else {
          setShowUploadFailed(true);
          setTimeout(() => setShowUploadFailed(false), 3000);
        }
      } catch {
        setShowUploadFailed(true);
        setTimeout(() => setShowUploadFailed(false), 3000);
      } finally {
        setShowUploading(false);
      }
    } else {
      setShowInvalidFile(true);
      setTimeout(() => setShowInvalidFile(false), 3000);
    }
  }

  return (
    <section>
      <h2>Update</h2>
      <h3>
        <Link to="/">
          <FontAwesomeIcon icon={faHouse} /> Go Home
        </Link>
      </h3>
      {showUploading && (
        <div className={styles["working-message"]}>One moment please...</div>
      )}
      {showInvalidFile && (
        <div className={styles["error-message"]}>
          That is not a recognised file.
        </div>
      )}
      {showUploadFailed && (
        <div className={styles["error-message"]}>
          Unable to upload that file.
        </div>
      )}
      {showUploadSuccess && (
        <div className={styles["success-message"]}>
          Uploaded {uploadCount} {uploadItem}.
        </div>
      )}
      <div
        className={styles["drop-site"]}
        onDragOver={dragOver}
        onDragLeave={dragLeave}
        onDrop={fileDrop}
      >
        <div className={styles["drop-message"]}>Drop Your File Here</div>
        <button onClick={pickFile} className={styles["file-button"]}>
          Choose File
        </button>
      </div>
    </section>
  );
}

export const Route = createFileRoute("/update")({
  component: UpdatePage,
});
