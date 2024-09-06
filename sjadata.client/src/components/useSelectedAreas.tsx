import { faCheck, faXmark } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { AreaLabel } from "../loaders/hours-loader";
import { useEffect, useState } from "react";

export default function useSelectedAreas() {
  const [selectedAreas, setSelectedAreas] = useState<AreaLabel[]>([]);

  function AreaCheck({ area }: { area: AreaLabel }) {
    if (selectedAreas.includes(area)) {
      return <FontAwesomeIcon fixedWidth icon={faCheck} />;
    } else {
      return <FontAwesomeIcon fixedWidth icon={faXmark} />;
    }
  }

  function toggleArea(area: AreaLabel) {
    const newAreas = selectedAreas.includes(area)
      ? selectedAreas.filter((a) => a !== area)
      : [...selectedAreas, area];

    setSelectedAreas(newAreas);
    localStorage.setItem("selectedAreas", JSON.stringify(newAreas));
  }

  useEffect(() => {
    const stored = localStorage.getItem("selectedAreas");
    if (stored) {
      setSelectedAreas(JSON.parse(stored));
    }
  }, []);

  return { selectedAreas, AreaCheck, toggleArea };
}
